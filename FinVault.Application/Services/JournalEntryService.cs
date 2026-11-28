
using AutoMapper;
using FinVault.Application.DTOs.JournalEntryDtos;
using FinVault.Application.DTOs.JournalEntryLineDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class JournalEntryService : IJournalEntryService
{
    private static readonly string[] ValidJournalTypes =
    {
        "Transaction", "Interest", "Fee", "Loan", "Adjustment", "Reversal"
    };

    private static readonly string[] ValidStatuses =
    {
        "Draft", "Posted", "Reversed"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public JournalEntryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. JOURNAL ENTRY IDENTIFICATION

    public async Task<IEnumerable<JournalEntryDto>> GetAllAsync()
    {
        var entries = await _unitOfWork.JournalEntry.GetAllAsync();
        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<JournalEntryDto?> GetByIdAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        var entry = await _unitOfWork.JournalEntry.GetByIdAsync(journalEntryId);
        return _mapper.Map<JournalEntryDto?>(entry);
    }

    public async Task<JournalEntryDto?> GetByJournalNumberAsync(string journalNumber)
    {
        ValidateRequiredString(journalNumber, nameof(journalNumber), 30);

        var entry = await _unitOfWork.JournalEntry
            .GetByJournalNumberAsync(journalNumber.Trim());

        return _mapper.Map<JournalEntryDto?>(entry);
    }

    public async Task<JournalEntryDto?> GetJournalEntryDetailsAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        var entry = await _unitOfWork.JournalEntry
            .GetJournalEntryDetailsAsync(journalEntryId);

        return _mapper.Map<JournalEntryDto?>(entry);
    }

    public async Task<bool> ExistsByJournalNumberAsync(string journalNumber)
    {
        ValidateRequiredString(journalNumber, nameof(journalNumber), 30);

        return await _unitOfWork.JournalEntry
            .ExistsByJournalNumberAsync(journalNumber.Trim());
    }

    public async Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateJournal(dto.JournalNumber, dto.JournalType,
            dto.Description, dto.Status, dto.EntryDate);

        ValidateId(dto.CurrencyId, nameof(dto.CurrencyId));

        if (dto.Status != "Draft")
            throw new InvalidOperationException(
                "Journal entries must be created as Draft. Posting requires validation of journal lines.");

        var journalNumber = dto.JournalNumber.Trim();

        if (await _unitOfWork.JournalEntry
            .ExistsByJournalNumberAsync(journalNumber))
        {
            throw new InvalidOperationException(
                "A journal entry with this journal number already exists.");
        }

        if (await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId) == null)
            throw new InvalidOperationException("Currency does not exist.");

        if (dto.TransactionId.HasValue &&
            await _unitOfWork.Transaction.GetByIdAsync(dto.TransactionId.Value) == null)
            throw new InvalidOperationException("Transaction does not exist.");

        if (dto.PostedByUserId.HasValue &&
            await _unitOfWork.User.GetByIdAsync(dto.PostedByUserId.Value) == null)
            throw new InvalidOperationException("Posting user does not exist.");

        if (dto.ReversalOfJournalEntryId.HasValue)
        {
            ValidateId(dto.ReversalOfJournalEntryId.Value,
                nameof(dto.ReversalOfJournalEntryId));

            var original = await _unitOfWork.JournalEntry
                .GetByIdAsync(dto.ReversalOfJournalEntryId.Value);

            if (original == null || original.Status != "Posted")
                throw new InvalidOperationException(
                    "A reversal must reference an existing posted journal entry.");

            if (dto.JournalType != "Reversal")
                throw new InvalidOperationException(
                    "A journal referencing an original entry must have Reversal type.");
        }
        else if (dto.JournalType == "Reversal")
        {
            throw new ArgumentException(
                "Reversal journal requires an original journal entry.");
        }

        var entry = _mapper.Map<JournalEntry>(dto);

        entry.JournalNumber = journalNumber;
        entry.Description = dto.Description.Trim();
        entry.JournalType = dto.JournalType.Trim();
        entry.Status = "Draft";
        entry.TotalDebit = 0m;
        entry.TotalCredit = 0m;
        entry.PostedAt = null;
        entry.PostedByUserId = null;
        entry.EntryDate = dto.EntryDate == default
            ? DateTime.UtcNow
            : dto.EntryDate;

        entry.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.JournalEntry.AddAsync(entry);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<JournalEntryDto>(entry);
    }

    public async Task<JournalEntryDto?> UpdateAsync(UpdateJournalEntryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.JournalEntryId, nameof(dto.JournalEntryId));
        ValidateJournal(dto.JournalNumber, dto.JournalType,
            dto.Description, dto.Status, dto.EntryDate);

        ValidateId(dto.CurrencyId, nameof(dto.CurrencyId));

        if (dto.RowVersion == null || dto.RowVersion.Length == 0)
            throw new ArgumentException("RowVersion is required.");

        var entry = await _unitOfWork.JournalEntry
            .GetByIdAsync(dto.JournalEntryId);

        if (entry == null)
            return null;

        if (entry.Status != "Draft")
            throw new InvalidOperationException(
                "Posted or reversed journal entries cannot be modified.");

        if (!entry.RowVersion.SequenceEqual(dto.RowVersion))
            throw new InvalidOperationException(
                "Journal entry was modified by another user. Reload before updating.");

        var journalNumber = dto.JournalNumber.Trim();

        if (await _unitOfWork.JournalEntry
            .ExistsByJournalNumberAsync(journalNumber) &&
            !string.Equals(entry.JournalNumber, journalNumber,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Journal number already exists.");
        }

        if (await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId) == null)
            throw new InvalidOperationException("Currency does not exist.");

        if (dto.TransactionId.HasValue &&
            await _unitOfWork.Transaction.GetByIdAsync(dto.TransactionId.Value) == null)
            throw new InvalidOperationException("Transaction does not exist.");

        if (dto.PostedByUserId.HasValue &&
            await _unitOfWork.User.GetByIdAsync(dto.PostedByUserId.Value) == null)
            throw new InvalidOperationException("Posting user does not exist.");

        if (dto.Status == "Reversed")
            throw new InvalidOperationException(
                "Create a separate reversal journal instead of changing the original journal status.");

        if (dto.JournalType == "Reversal" &&
            !dto.ReversalOfJournalEntryId.HasValue)
            throw new ArgumentException(
                "Reversal journal requires an original journal entry.");

        if (dto.ReversalOfJournalEntryId.HasValue)
        {
            var original = await _unitOfWork.JournalEntry
                .GetByIdAsync(dto.ReversalOfJournalEntryId.Value);

            if (original == null || original.Status != "Posted")
                throw new InvalidOperationException(
                    "Original journal must exist and be posted.");
        }

        _mapper.Map(dto, entry);

        entry.JournalNumber = journalNumber;
        entry.Description = dto.Description.Trim();
        entry.JournalType = dto.JournalType.Trim();
        entry.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == "Posted")
        {
            if (!dto.PostedByUserId.HasValue)
                throw new ArgumentException(
                    "PostedByUserId is required when posting a journal.");

            await ValidatePostingAsync(entry.JournalEntryId);

            entry.TotalDebit = await _unitOfWork.JournalEntryLine
                .GetTotalDebitByJournalEntryIdAsync(entry.JournalEntryId);

            entry.TotalCredit = await _unitOfWork.JournalEntryLine
                .GetTotalCreditByJournalEntryIdAsync(entry.JournalEntryId);

            entry.PostedAt = DateTime.UtcNow;
            entry.PostedByUserId = dto.PostedByUserId;
        }

        _unitOfWork.JournalEntry.Update(entry);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<JournalEntryDto>(entry);
    }

    public async Task<bool> DeleteAsync(DeleteJournalEntryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.JournalEntryId, nameof(dto.JournalEntryId));

        var entry = await _unitOfWork.JournalEntry
            .GetJournalEntryDetailsAsync(dto.JournalEntryId);

        if (entry == null)
            return false;

        if (entry.Status != "Draft")
            throw new InvalidOperationException(
                "Only draft journal entries can be deleted.");

        if (entry.InterestCalculations.Any())
            throw new InvalidOperationException(
                "Journal entries associated with interest calculations cannot be deleted.");

        foreach (var line in entry.Lines.ToList())
            _unitOfWork.JournalEntryLine.Delete(line);

        _unitOfWork.JournalEntry.Delete(entry);

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. JOURNAL CLASSIFICATION AND STATUS

    public async Task<IEnumerable<JournalEntryDto>> GetByJournalTypeAsync(string journalType)
    {
        ValidateJournalType(journalType);

        var entries = await _unitOfWork.JournalEntry
            .GetByJournalTypeAsync(journalType.Trim());

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetByStatusAsync(string status)
    {
        ValidateStatus(status);

        var entries = await _unitOfWork.JournalEntry
            .GetByStatusAsync(status.Trim());

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetDraftEntriesAsync()
    {
        var entries = await _unitOfWork.JournalEntry.GetDraftEntriesAsync();
        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetPostedEntriesAsync()
    {
        var entries = await _unitOfWork.JournalEntry.GetPostedEntriesAsync();
        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetReversedEntriesAsync()
    {
        var entries = await _unitOfWork.JournalEntry.GetReversedEntriesAsync();
        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    // 3. TRANSACTION AND CURRENCY ASSOCIATIONS

    public async Task<IEnumerable<JournalEntryDto>> GetByTransactionIdAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var entries = await _unitOfWork.JournalEntry
            .GetByTransactionIdAsync(transactionId);

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetByCurrencyIdAsync(int currencyId)
    {
        ValidateId(currencyId, nameof(currencyId));

        var entries = await _unitOfWork.JournalEntry
            .GetByCurrencyIdAsync(currencyId);

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetByPostedByUserIdAsync(int userId)
    {
        ValidateId(userId, nameof(userId));

        var entries = await _unitOfWork.JournalEntry
            .GetByPostedByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    // 4. REVERSAL MANAGEMENT

    public async Task<JournalEntryDto?> GetOriginalEntryAsync(int reversalEntryId)
    {
        ValidateId(reversalEntryId, nameof(reversalEntryId));

        var entry = await _unitOfWork.JournalEntry
            .GetOriginalEntryAsync(reversalEntryId);

        return _mapper.Map<JournalEntryDto?>(entry);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetReversalEntriesAsync(int originalJournalEntryId)
    {
        ValidateId(originalJournalEntryId, nameof(originalJournalEntryId));

        var entries = await _unitOfWork.JournalEntry
            .GetReversalEntriesAsync(originalJournalEntryId);

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetEntriesWithReversalsAsync()
    {
        var entries = await _unitOfWork.JournalEntry.GetEntriesWithReversalsAsync();
        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    // 5. JOURNAL DATE AND PERIOD MANAGEMENT

    public async Task<IEnumerable<JournalEntryDto>> GetByEntryDateRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var entries = await _unitOfWork.JournalEntry
            .GetByEntryDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetPostedWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var entries = await _unitOfWork.JournalEntry
            .GetPostedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    // 6. JOURNAL LINE MANAGEMENT

    public async Task<IEnumerable<JournalEntryLineDto>> GetLinesByJournalEntryIdAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetByJournalEntryIdAsync(journalEntryId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    public async Task<JournalEntryLineDto?> GetJournalEntryLineDetailsAsync(int journalEntryLineId)
    {
        ValidateId(journalEntryLineId, nameof(journalEntryLineId));

        var line = await _unitOfWork.JournalEntryLine
            .GetJournalEntryLineDetailsAsync(journalEntryLineId);

        return _mapper.Map<JournalEntryLineDto?>(line);
    }

    public async Task<JournalEntryLineDto?> GetLineByJournalEntryAndLineNumberAsync(
        int journalEntryId, int lineNumber)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        if (lineNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(lineNumber));

        var line = await _unitOfWork.JournalEntryLine
            .GetByJournalEntryAndLineNumberAsync(journalEntryId, lineNumber);

        return _mapper.Map<JournalEntryLineDto?>(line);
    }

    public async Task<JournalEntryLineDto> CreateLineAsync(CreateJournalEntryLineDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.JournalEntryId, nameof(dto.JournalEntryId));
        ValidateId(dto.AccountId, nameof(dto.AccountId));

        if (dto.LineNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.LineNumber));

        ValidateDebitCredit(dto.DebitAmount, dto.CreditAmount);

        if (dto.Narration?.Length > 250)
            throw new ArgumentException("Narration cannot exceed 250 characters.");

        var entry = await _unitOfWork.JournalEntry
            .GetByIdAsync(dto.JournalEntryId);

        if (entry == null)
            throw new InvalidOperationException("Journal entry does not exist.");

        if (entry.Status != "Draft")
            throw new InvalidOperationException(
                "Lines can only be added to draft journal entries.");

        if (await _unitOfWork.Account.GetByIdAsync(dto.AccountId) == null)
            throw new InvalidOperationException("Account does not exist.");

        if (await _unitOfWork.JournalEntryLine
            .ExistsByJournalEntryAndLineNumberAsync(
                dto.JournalEntryId, dto.LineNumber))
        {
            throw new InvalidOperationException(
                "Line number already exists in this journal entry.");
        }

        var line = _mapper.Map<JournalEntryLine>(dto);

        line.Narration = dto.Narration?.Trim();
        line.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.JournalEntryLine.AddAsync(line);

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<JournalEntryLineDto>(line);
    }

    public async Task<bool> ExistsByJournalEntryAndLineNumberAsync(
        int journalEntryId, int lineNumber)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        if (lineNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(lineNumber));

        return await _unitOfWork.JournalEntryLine
            .ExistsByJournalEntryAndLineNumberAsync(journalEntryId, lineNumber);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> GetDebitLinesByJournalEntryIdAsync(
        int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetDebitLinesByJournalEntryIdAsync(journalEntryId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> GetCreditLinesByJournalEntryIdAsync(
        int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetCreditLinesByJournalEntryIdAsync(journalEntryId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> GetJournalLinesWithAccountsAsync(
        int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetJournalLinesWithAccountsAsync(journalEntryId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    // 7. ACCOUNT ASSOCIATIONS

    public async Task<IEnumerable<JournalEntryLineDto>> GetLinesByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> GetDebitLinesByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetDebitLinesByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> GetCreditLinesByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetCreditLinesByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> GetAccountLinesByDateRangeAsync(
        int accountId, DateTime startDate, DateTime endDate)
    {
        ValidateId(accountId, nameof(accountId));
        ValidateDateRange(startDate, endDate);

        var lines = await _unitOfWork.JournalEntryLine
            .GetAccountLinesByDateRangeAsync(accountId, startDate, endDate);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    // 8. ACCOUNTING VALIDATION

    public async Task<decimal> GetTotalDebitByJournalEntryIdAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        return await _unitOfWork.JournalEntryLine
            .GetTotalDebitByJournalEntryIdAsync(journalEntryId);
    }

    public async Task<decimal> GetTotalCreditByJournalEntryIdAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        return await _unitOfWork.JournalEntryLine
            .GetTotalCreditByJournalEntryIdAsync(journalEntryId);
    }

    public async Task<bool> IsJournalEntryBalancedAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        return await _unitOfWork.JournalEntryLine
            .IsJournalEntryBalancedAsync(journalEntryId);
    }

    public async Task<int> CountLinesByJournalEntryIdAsync(int journalEntryId)
    {
        ValidateId(journalEntryId, nameof(journalEntryId));

        return await _unitOfWork.JournalEntryLine
            .CountLinesByJournalEntryIdAsync(journalEntryId);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetUnbalancedDraftEntriesAsync()
    {
        var entries = await _unitOfWork.JournalEntry
            .GetUnbalancedDraftEntriesAsync();

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryDto>> GetEntriesWithoutLinesAsync()
    {
        var entries = await _unitOfWork.JournalEntry
            .GetEntriesWithoutLinesAsync();

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    // 9. INTEREST AND FINANCIAL REPORTING

    public async Task<IEnumerable<JournalEntryDto>> GetEntriesWithInterestCalculationsAsync()
    {
        var entries = await _unitOfWork.JournalEntry
            .GetEntriesWithInterestCalculationsAsync();

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<decimal> GetTotalPostedDebitsAsync(
        int currencyId, DateTime startDate, DateTime endDate)
    {
        ValidateId(currencyId, nameof(currencyId));
        ValidateDateRange(startDate, endDate);

        return await _unitOfWork.JournalEntry
            .GetTotalPostedDebitsAsync(currencyId, startDate, endDate);
    }

    public async Task<decimal> GetTotalPostedCreditsAsync(
        int currencyId, DateTime startDate, DateTime endDate)
    {
        ValidateId(currencyId, nameof(currencyId));
        ValidateDateRange(startDate, endDate);

        return await _unitOfWork.JournalEntry
            .GetTotalPostedCreditsAsync(currencyId, startDate, endDate);
    }

    // 10. AUDIT AND SEARCH

    public async Task<IEnumerable<JournalEntryLineDto>> GetLinesByCreatedByUserIdAsync(int userId)
    {
        ValidateId(userId, nameof(userId));

        var lines = await _unitOfWork.JournalEntryLine
            .GetByCreatedByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    public async Task<IEnumerable<JournalEntryDto>> SearchJournalEntriesAsync(string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm), 200);

        var entries = await _unitOfWork.JournalEntry
            .SearchJournalEntriesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<JournalEntryDto>>(entries);
    }

    public async Task<IEnumerable<JournalEntryLineDto>> SearchJournalEntryLinesAsync(string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm), 200);

        var lines = await _unitOfWork.JournalEntryLine
            .SearchJournalEntryLinesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<JournalEntryLineDto>>(lines);
    }

    // PRIVATE BUSINESS VALIDATION

    private async Task ValidatePostingAsync(int journalEntryId)
    {
        var lineCount = await _unitOfWork.JournalEntryLine
            .CountLinesByJournalEntryIdAsync(journalEntryId);

        if (lineCount < 2)
            throw new InvalidOperationException(
                "A journal requires at least two lines before posting.");

        var balanced = await _unitOfWork.JournalEntryLine
            .IsJournalEntryBalancedAsync(journalEntryId);

        if (!balanced)
            throw new InvalidOperationException(
                "Journal cannot be posted because debit and credit totals are not balanced.");

        var debit = await _unitOfWork.JournalEntryLine
            .GetTotalDebitByJournalEntryIdAsync(journalEntryId);

        var credit = await _unitOfWork.JournalEntryLine
            .GetTotalCreditByJournalEntryIdAsync(journalEntryId);

        if (debit <= 0 || debit != credit)
            throw new InvalidOperationException(
                "Journal must have equal positive debit and credit totals.");
    }

    private static void ValidateDebitCredit(decimal debit, decimal credit)
    {
        if (debit < 0 || credit < 0)
            throw new ArgumentOutOfRangeException(
                "Debit and credit amounts cannot be negative.");

        bool hasDebit = debit > 0;
        bool hasCredit = credit > 0;

        if (hasDebit == hasCredit)
            throw new ArgumentException(
                "Each journal line must contain either a debit or a credit, never both or neither.");
    }

    private static void ValidateJournal(
        string journalNumber,
        string journalType,
        string description,
        string status,
        DateTime entryDate)
    {
        ValidateRequiredString(journalNumber, nameof(journalNumber), 30);
        ValidateRequiredString(description, nameof(description), 250);
        ValidateJournalType(journalType);
        ValidateStatus(status);

        if (entryDate != default && entryDate > DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException(
                "Journal entry date cannot be in the future.");
    }

    private static void ValidateJournalType(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!ValidJournalTypes.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid journal type.", nameof(value));
    }

    private static void ValidateStatus(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!ValidStatuses.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid journal status.", nameof(value));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName, "ID must be greater than zero.");
    }

    private static void ValidateRequiredString(
        string? value, string parameterName, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", parameterName);

        if (value.Trim().Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Valid start and end dates are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }
}
