using AutoMapper;
using FinVault.Application.DTOs.ReconciliationBatchDtos;
using FinVault.Application.DTOs.ReconciliationRecordDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public sealed class ReconciliationService : IReconciliationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReconciliationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieve a batch with its records and calculated status.
    public async Task<ReconciliationBatchDto?> GetBatchAsync(int batchId)
    {
        var batch = await _unitOfWork.ReconciliationBatch.GetBatchDetailsAsync(batchId);
        return batch == null ? null : _mapper.Map<ReconciliationBatchDto>(batch);
    }

    // 2. Start a new batch and prevent duplicate batch numbers or file hashes.
    public async Task<ReconciliationBatchDto> StartBatchAsync(CreateReconciliationBatchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.ReconciliationBatch.ExistsByBatchNumberAsync(dto.BatchNumber.Trim()))
            throw new InvalidOperationException("A batch with this number already exists.");
        if (!string.IsNullOrWhiteSpace(dto.FileHash) &&
            await _unitOfWork.ReconciliationBatch.ExistsByFileHashAsync(dto.FileHash.Trim()))
            throw new InvalidOperationException("This source file has already been processed.");
        var batch = _mapper.Map<ReconciliationBatch>(dto);
        batch.BatchNumber = dto.BatchNumber.Trim();
        batch.Status = "InProgress";
        batch.StartedAt = DateTime.UtcNow;
        batch.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.ReconciliationBatch.AddAsync(batch);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ReconciliationBatchDto>(batch);
    }

    // 3. Add one imported external record to an existing batch.
    public async Task<ReconciliationRecordDto> AddRecordAsync(CreateReconciliationRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var batch = await _unitOfWork.ReconciliationBatch.GetByIdAsync(dto.ReconciliationBatchId)
            ?? throw new KeyNotFoundException("Reconciliation batch was not found.");
        if (!string.Equals(batch.Status, "InProgress", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Records can only be added to an InProgress batch.");
        if (await _unitOfWork.ReconciliationRecord.ExistsByExternalReferenceAsync(dto.ExternalReference.Trim()))
            throw new InvalidOperationException("This external reference already exists.");
        var record = _mapper.Map<ReconciliationRecord>(dto);
        record.ExternalReference = dto.ExternalReference.Trim();
        record.DifferenceAmount = dto.ExternalAmount - (dto.InternalAmount ?? 0m);
        record.MatchStatus = dto.InternalAmount.HasValue && record.DifferenceAmount == 0m ? "Matched" : "Unmatched";
        record.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.ReconciliationRecord.AddAsync(record);
        batch.TotalRecords++;
        if (record.MatchStatus == "Matched")
        {
            batch.MatchedRecords++;
            batch.MatchedAmount += record.ExternalAmount;
        }
        else
        {
            batch.UnmatchedRecords++;
            batch.DifferenceAmount += record.DifferenceAmount;
        }
        batch.TotalAmount += record.ExternalAmount;
        batch.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ReconciliationBatch.Update(batch);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ReconciliationRecordDto>(record);
    }

    // 4. Complete a batch only when no unresolved records remain.
    public async Task<bool> CompleteBatchAsync(int batchId)
    {
        var batch = await _unitOfWork.ReconciliationBatch.GetBatchDetailsAsync(batchId);
        if (batch == null) return false;
        if (!string.Equals(batch.Status, "InProgress", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Only an InProgress batch can be completed.");
        var unresolved = await _unitOfWork.ReconciliationRecord.GetUnresolvedRecordsAsync(batchId);
        if (unresolved.Any())
            throw new InvalidOperationException("Resolve all outstanding reconciliation exceptions before completing the batch.");
        batch.Status = batch.UnmatchedRecords > 0 ? "CompletedWithExceptions" : "Completed";
        batch.CompletedAt = DateTime.UtcNow;
        batch.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.ReconciliationBatch.Update(batch);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 5. Return records that need attention.
    public async Task<IReadOnlyList<ReconciliationRecordDto>> GetExceptionsAsync(int batchId)
    {
        var records = await _unitOfWork.ReconciliationRecord.GetUnresolvedRecordsAsync(batchId);
        return _mapper.Map<IReadOnlyList<ReconciliationRecordDto>>(records);
    }
}
