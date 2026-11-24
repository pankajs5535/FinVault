using AutoMapper;
using FinVault.Application.DTOs.JournalEntryDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Maintains journal headers and permits posting only when journal lines balance.</summary>
public sealed class JournalService : IJournalService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JournalService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves a journal entry together with its lines and related details.
    public async Task<JournalEntryDto?> GetEntryByIdAsync(int journalEntryId)
    {
        if (journalEntryId <= 0) throw new ArgumentOutOfRangeException(nameof(journalEntryId));
        var entry = await _unitOfWork.JournalEntry.GetJournalEntryDetailsAsync(journalEntryId);
        return entry is null ? null : _mapper.Map<JournalEntryDto>(entry);
    }

    // 2. Retrieves journal entries created within an inclusive date range.
    public async Task<IReadOnlyList<JournalEntryDto>> GetEntriesAsync(DateTime from, DateTime to)
    {
        if (from > to) throw new ArgumentException("Start date cannot be after end date.");
        var entries = await _unitOfWork.JournalEntry.GetByEntryDateRangeAsync(from, to);
        return _mapper.Map<IReadOnlyList<JournalEntryDto>>(entries);
    }

    // 3. Creates a draft journal header; debit and credit lines are added through the journal-line workflow.
    public async Task<JournalEntryDto> CreateEntryAsync(CreateJournalEntryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(dto.JournalNumber)) throw new ArgumentException("Journal number is required.");
        if (await _unitOfWork.JournalEntry.ExistsByJournalNumberAsync(dto.JournalNumber.Trim()))
            throw new InvalidOperationException("Journal number already exists.");

        var entry = _mapper.Map<JournalEntry>(dto);
        entry.JournalNumber = dto.JournalNumber.Trim();
        entry.Status = "Draft";
        entry.TotalDebit = 0;
        entry.TotalCredit = 0;
        entry.PostedAt = null;
        entry.PostedByUserId = null;
        entry.EntryDate = dto.EntryDate == default ? DateTime.UtcNow.Date : dto.EntryDate;
        entry.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.JournalEntry.AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<JournalEntryDto>(entry);
    }

    // 4. Posts a draft only when it contains at least two lines and debit equals credit.
    public async Task<bool> PostEntryAsync(int journalEntryId)
    {
        if (journalEntryId <= 0) throw new ArgumentOutOfRangeException(nameof(journalEntryId));
        var entry = await _unitOfWork.JournalEntry.GetJournalEntryDetailsAsync(journalEntryId);
        if (entry is null) return false;
        if (entry.Status != "Draft") throw new InvalidOperationException("Only draft journal entries can be posted.");
        var lineCount = await _unitOfWork.JournalEntryLine.CountLinesByJournalEntryIdAsync(journalEntryId);
        if (lineCount < 2) throw new InvalidOperationException("A journal entry must contain at least two lines before posting.");
        if (!await _unitOfWork.JournalEntryLine.IsJournalEntryBalancedAsync(journalEntryId))
            throw new InvalidOperationException("Journal debits and credits must balance before posting.");

        entry.TotalDebit = await _unitOfWork.JournalEntryLine.GetTotalDebitByJournalEntryIdAsync(journalEntryId);
        entry.TotalCredit = await _unitOfWork.JournalEntryLine.GetTotalCreditByJournalEntryIdAsync(journalEntryId);
        entry.Status = "Posted";
        entry.PostedAt = DateTime.UtcNow;
        entry.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.JournalEntry.Update(entry);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
