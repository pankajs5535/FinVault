
using AutoMapper;
using FinVault.Application.DTOs.ReconciliationBatchDtos;
using FinVault.Application.DTOs.ReconciliationRecordDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class ReconciliationService : IReconciliationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly string[] ValidTypes =
    {
        "Upi", "Neft", "Rtgs", "Imps", "CardSettlement", "AtmSwitch", "GeneralLedger"
    };

    private static readonly string[] ValidBatchStatuses =
    {
        "Pending", "InProgress", "Completed", "CompletedWithExceptions", "Failed"
    };

    private static readonly string[] ValidMatchStatuses =
    {
        "Matched", "Unmatched", "AmountMismatch",
        "MissingInBank", "MissingInExternal", "Manual"
    };

    public ReconciliationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. RECONCILIATION BATCH CRUD

    public async Task<IEnumerable<ReconciliationBatchDto>> GetAllBatchesAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch.GetAllAsync();
        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<ReconciliationBatchDto?> GetBatchByIdAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return null;

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByIdAsync(reconciliationBatchId);

        return batch == null ? null : _mapper.Map<ReconciliationBatchDto>(batch);
    }

    public async Task<ReconciliationBatchDto?> GetByBatchNumberAsync(string batchNumber)
    {
        ValidateRequiredText(batchNumber, nameof(batchNumber), 30);

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByBatchNumberAsync(batchNumber.Trim());

        return batch == null ? null : _mapper.Map<ReconciliationBatchDto>(batch);
    }

    public async Task<ReconciliationBatchDto?> GetByFileHashAsync(string fileHash)
    {
        ValidateRequiredText(fileHash, nameof(fileHash), 64);

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByFileHashAsync(fileHash.Trim());

        return batch == null ? null : _mapper.Map<ReconciliationBatchDto>(batch);
    }

    public async Task<ReconciliationBatchDto?> GetBatchDetailsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return null;

        var batch = await _unitOfWork.ReconciliationBatch
            .GetBatchDetailsAsync(reconciliationBatchId);

        return batch == null ? null : _mapper.Map<ReconciliationBatchDto>(batch);
    }

    public Task<bool> ExistsByBatchNumberAsync(string batchNumber)
    {
        ValidateRequiredText(batchNumber, nameof(batchNumber), 30);

        return _unitOfWork.ReconciliationBatch
            .ExistsByBatchNumberAsync(batchNumber.Trim());
    }

    public Task<bool> ExistsByFileHashAsync(string fileHash)
    {
        ValidateRequiredText(fileHash, nameof(fileHash), 64);

        return _unitOfWork.ReconciliationBatch
            .ExistsByFileHashAsync(fileHash.Trim());
    }

    public async Task<ReconciliationBatchDto> CreateBatchAsync(
        CreateReconciliationBatchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateBatch(dto);

        var batchNumber = dto.BatchNumber.Trim();

        if (await _unitOfWork.ReconciliationBatch
            .ExistsByBatchNumberAsync(batchNumber))
        {
            throw new InvalidOperationException(
                "A reconciliation batch with this batch number already exists.");
        }

        var fileHash = NormalizeOptionalText(dto.FileHash);

        if (fileHash != null &&
            await _unitOfWork.ReconciliationBatch.ExistsByFileHashAsync(fileHash))
        {
            throw new InvalidOperationException(
                "This settlement file has already been imported.");
        }

        await ValidateBatchReferencesAsync(
            dto.PaymentMethodId,
            dto.InitiatedByUserId);

        var batch = _mapper.Map<ReconciliationBatch>(dto);

        batch.BatchNumber = batchNumber;
        batch.ReconciliationType = dto.ReconciliationType.Trim();
        batch.SourceName = dto.SourceName.Trim();
        batch.FileName = NormalizeOptionalText(dto.FileName);
        batch.FileHash = fileHash;
        batch.Status = dto.Status.Trim();
        batch.CreatedAt = DateTime.UtcNow;
        batch.UpdatedAt = null;
        batch.CreatedBy = null;
        batch.UpdatedBy = null;

        await _unitOfWork.ReconciliationBatch.AddAsync(batch);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReconciliationBatchDto>(batch);
    }

    public async Task<ReconciliationBatchDto?> UpdateBatchAsync(
        UpdateReconciliationBatchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.ReconciliationBatchId <= 0)
            return null;

        ValidateBatch(dto);

        if (dto.RowVersion == null || dto.RowVersion.Length == 0)
            throw new ArgumentException("RowVersion is required.");

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByIdAsync(dto.ReconciliationBatchId);

        if (batch == null)
            return null;

        if (batch.RowVersion == null ||
            !batch.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "The reconciliation batch was modified by another operation. " +
                "Reload the batch before updating it.");
        }

        if (batch.Status == "Completed" ||
            batch.Status == "CompletedWithExceptions")
        {
            throw new InvalidOperationException(
                "A completed reconciliation batch cannot be modified.");
        }

        var batchNumber = dto.BatchNumber.Trim();

        var existingNumber = await _unitOfWork.ReconciliationBatch
            .GetByBatchNumberAsync(batchNumber);

        if (existingNumber != null &&
            existingNumber.ReconciliationBatchId != dto.ReconciliationBatchId)
        {
            throw new InvalidOperationException(
                "Another batch already uses this batch number.");
        }

        var fileHash = NormalizeOptionalText(dto.FileHash);

        if (fileHash != null)
        {
            var existingHash = await _unitOfWork.ReconciliationBatch
                .GetByFileHashAsync(fileHash);

            if (existingHash != null &&
                existingHash.ReconciliationBatchId != dto.ReconciliationBatchId)
            {
                throw new InvalidOperationException(
                    "This settlement file has already been imported.");
            }
        }

        await ValidateBatchReferencesAsync(
            dto.PaymentMethodId,
            dto.InitiatedByUserId);

        var records = (await _unitOfWork.ReconciliationRecord
            .GetByBatchIdAsync(dto.ReconciliationBatchId)).ToList();

        if (dto.Status == "Completed" ||
            dto.Status == "CompletedWithExceptions")
        {
            ValidateBatchTotalsAgainstRecords(dto, records);

            var unresolvedExceptions = records.Any(
                x => x.MatchStatus != "Matched" &&
                     !(x.MatchStatus == "Manual" && x.ResolvedAt.HasValue));

            if (dto.Status == "Completed" && unresolvedExceptions)
            {
                throw new InvalidOperationException(
                    "The batch cannot be marked Completed while unresolved exceptions remain.");
            }

            if (dto.Status == "CompletedWithExceptions" && !unresolvedExceptions)
            {
                throw new InvalidOperationException(
                    "CompletedWithExceptions requires at least one unresolved exception.");
            }

            if (!dto.CompletedAt.HasValue)
                throw new ArgumentException("CompletedAt is required for a completed batch.");
        }

        _mapper.Map(dto, batch);

        batch.BatchNumber = batchNumber;
        batch.ReconciliationType = dto.ReconciliationType.Trim();
        batch.SourceName = dto.SourceName.Trim();
        batch.FileName = NormalizeOptionalText(dto.FileName);
        batch.FileHash = fileHash;
        batch.Status = dto.Status.Trim();
        batch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ReconciliationBatch.Update(batch);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReconciliationBatchDto>(batch);
    }

    public async Task<bool> DeleteBatchAsync(DeleteReconciliationBatchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.ReconciliationBatchId <= 0)
            return false;

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByIdAsync(dto.ReconciliationBatchId);

        if (batch == null)
            return false;

        if (batch.Status != "Pending")
        {
            throw new InvalidOperationException(
                "Only Pending reconciliation batches can be deleted.");
        }

        var records = await _unitOfWork.ReconciliationRecord
            .GetByBatchIdAsync(dto.ReconciliationBatchId);

        if (records.Any())
        {
            throw new InvalidOperationException(
                "A batch containing reconciliation records cannot be deleted.");
        }

        _unitOfWork.ReconciliationBatch.Delete(batch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. BATCH TYPE AND BUSINESS DATE

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetBatchesByReconciliationTypeAsync(string reconciliationType)
    {
        ValidateAllowedValue(reconciliationType, ValidTypes, nameof(reconciliationType));

        var batches = await _unitOfWork.ReconciliationBatch
            .GetByReconciliationTypeAsync(reconciliationType.Trim());

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetBatchesByBusinessDateAsync(DateTime businessDate)
    {
        ValidateDate(businessDate, nameof(businessDate));

        var batches = await _unitOfWork.ReconciliationBatch
            .GetByBusinessDateAsync(businessDate);

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetBatchesByTypeAndBusinessDateAsync(
            string reconciliationType,
            DateTime businessDate)
    {
        ValidateAllowedValue(reconciliationType, ValidTypes, nameof(reconciliationType));
        ValidateDate(businessDate, nameof(businessDate));

        var batches = await _unitOfWork.ReconciliationBatch
            .GetByTypeAndBusinessDateAsync(reconciliationType.Trim(), businessDate);

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetBatchesByBusinessDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var batches = await _unitOfWork.ReconciliationBatch
            .GetByBusinessDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    // 3. BATCH STATUS

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetBatchesByStatusAsync(string status)
    {
        ValidateAllowedValue(status, ValidBatchStatuses, nameof(status));

        var batches = await _unitOfWork.ReconciliationBatch
            .GetByStatusAsync(status.Trim());

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>> GetPendingBatchesAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch.GetPendingBatchesAsync();
        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>> GetInProgressBatchesAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch.GetInProgressBatchesAsync();
        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>> GetCompletedBatchesAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch.GetCompletedBatchesAsync();
        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>> GetCompletedWithExceptionsAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch
            .GetCompletedWithExceptionsAsync();

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>> GetFailedBatchesAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch.GetFailedBatchesAsync();
        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    // 4. BATCH INITIATION AND RESULTS

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetBatchesByPaymentMethodIdAsync(int paymentMethodId)
    {
        if (paymentMethodId <= 0)
            return Enumerable.Empty<ReconciliationBatchDto>();

        var batches = await _unitOfWork.ReconciliationBatch
            .GetByPaymentMethodIdAsync(paymentMethodId);

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetBatchesByInitiatedByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<ReconciliationBatchDto>();

        var batches = await _unitOfWork.ReconciliationBatch
            .GetByInitiatedByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>> GetBatchesWithDifferencesAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch
            .GetBatchesWithDifferencesAsync();

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>> GetBatchesWithRecordsAsync()
    {
        var batches = await _unitOfWork.ReconciliationBatch
            .GetBatchesWithRecordsAsync();

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationBatchDto>>
        GetCompletedBatchesWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var batches = await _unitOfWork.ReconciliationBatch
            .GetCompletedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    // 5. RECONCILIATION RECORD CRUD

    public async Task<IEnumerable<ReconciliationRecordDto>> GetAllRecordsAsync()
    {
        var records = await _unitOfWork.ReconciliationRecord.GetAllAsync();
        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<ReconciliationRecordDto?> GetRecordByIdAsync(int reconciliationRecordId)
    {
        if (reconciliationRecordId <= 0)
            return null;

        var record = await _unitOfWork.ReconciliationRecord
            .GetByIdAsync(reconciliationRecordId);

        return record == null ? null : _mapper.Map<ReconciliationRecordDto>(record);
    }

    public async Task<ReconciliationRecordDto?> GetRecordDetailsAsync(
        int reconciliationRecordId)
    {
        if (reconciliationRecordId <= 0)
            return null;

        var record = await _unitOfWork.ReconciliationRecord
            .GetRecordDetailsAsync(reconciliationRecordId);

        return record == null ? null : _mapper.Map<ReconciliationRecordDto>(record);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetByExternalReferenceAsync(string externalReference)
    {
        ValidateRequiredText(externalReference, nameof(externalReference), 50);

        var records = await _unitOfWork.ReconciliationRecord
            .GetByExternalReferenceAsync(externalReference.Trim());

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public Task<bool> ExistsByExternalReferenceAsync(string externalReference)
    {
        ValidateRequiredText(externalReference, nameof(externalReference), 50);

        return _unitOfWork.ReconciliationRecord
            .ExistsByExternalReferenceAsync(externalReference.Trim());
    }

    public async Task<ReconciliationRecordDto> CreateRecordAsync(
        CreateReconciliationRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateRecord(dto);

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByIdAsync(dto.ReconciliationBatchId);

        if (batch == null)
            throw new InvalidOperationException("The reconciliation batch does not exist.");

        EnsureBatchAcceptsRecords(batch);

        if (await _unitOfWork.ReconciliationRecord
            .ExistsByExternalReferenceAsync(dto.ExternalReference.Trim()))
        {
            throw new InvalidOperationException(
                "This external settlement reference already exists.");
        }

        await ValidateRecordReferencesAsync(
            dto.TransactionId,
            dto.ResolvedByUserId);

        var record = _mapper.Map<ReconciliationRecord>(dto);

        record.ExternalReference = dto.ExternalReference.Trim();
        record.MatchStatus = dto.MatchStatus.Trim();
        record.ExceptionReason = NormalizeOptionalText(dto.ExceptionReason);
        record.ResolutionNotes = NormalizeOptionalText(dto.ResolutionNotes);
        record.CreatedAt = DateTime.UtcNow;
        record.UpdatedAt = null;
        record.CreatedBy = null;
        record.UpdatedBy = null;

        await _unitOfWork.ReconciliationRecord.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();

        await RefreshBatchTotalsAsync(dto.ReconciliationBatchId);

        return _mapper.Map<ReconciliationRecordDto>(record);
    }

    public async Task<ReconciliationRecordDto?> UpdateRecordAsync(
        UpdateReconciliationRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.ReconciliationRecordId <= 0)
            return null;

        ValidateRecord(dto);

        var record = await _unitOfWork.ReconciliationRecord
            .GetByIdAsync(dto.ReconciliationRecordId);

        if (record == null)
            return null;

        if (record.ReconciliationBatchId != dto.ReconciliationBatchId)
        {
            throw new InvalidOperationException(
                "A reconciliation record cannot be moved to another batch.");
        }

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByIdAsync(record.ReconciliationBatchId);

        if (batch == null)
            throw new InvalidOperationException("The associated batch does not exist.");

        EnsureBatchAcceptsRecords(batch);

        var existingRecords = await _unitOfWork.ReconciliationRecord
            .GetByExternalReferenceAsync(dto.ExternalReference.Trim());

        if (existingRecords.Any(x =>
                x.ReconciliationRecordId != dto.ReconciliationRecordId))
        {
            throw new InvalidOperationException(
                "This external settlement reference already exists.");
        }

        await ValidateRecordReferencesAsync(
            dto.TransactionId,
            dto.ResolvedByUserId);

        _mapper.Map(dto, record);

        record.ExternalReference = dto.ExternalReference.Trim();
        record.MatchStatus = dto.MatchStatus.Trim();
        record.ExceptionReason = NormalizeOptionalText(dto.ExceptionReason);
        record.ResolutionNotes = NormalizeOptionalText(dto.ResolutionNotes);
        record.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ReconciliationRecord.Update(record);
        await _unitOfWork.SaveChangesAsync();

        await RefreshBatchTotalsAsync(record.ReconciliationBatchId);

        return _mapper.Map<ReconciliationRecordDto>(record);
    }

    public async Task<bool> DeleteRecordAsync(DeleteReconciliationRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.ReconciliationRecordId <= 0)
            return false;

        var record = await _unitOfWork.ReconciliationRecord
            .GetByIdAsync(dto.ReconciliationRecordId);

        if (record == null)
            return false;

        var batch = await _unitOfWork.ReconciliationBatch
            .GetByIdAsync(record.ReconciliationBatchId);

        if (batch == null || batch.Status != "Pending")
        {
            throw new InvalidOperationException(
                "A reconciliation record can only be deleted while its batch is Pending.");
        }

        _unitOfWork.ReconciliationRecord.Delete(record);
        await _unitOfWork.SaveChangesAsync();

        await RefreshBatchTotalsAsync(record.ReconciliationBatchId);

        return true;
    }

    // 6. RECORD BATCH AND TRANSACTION RELATIONSHIPS

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsByBatchIdAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetByBatchIdAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetBatchRecordsWithTransactionsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetBatchRecordsWithTransactionsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsByTransactionIdAsync(int transactionId)
    {
        if (transactionId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetByTransactionIdAsync(transactionId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsWithoutTransactionAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetWithoutTransactionAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    // 7. RECORD MATCH STATUS

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsByMatchStatusAsync(string matchStatus)
    {
        ValidateAllowedValue(matchStatus, ValidMatchStatuses, nameof(matchStatus));

        var records = await _unitOfWork.ReconciliationRecord
            .GetByMatchStatusAsync(matchStatus.Trim());

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetMatchedRecordsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetMatchedRecordsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetUnmatchedRecordsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetUnmatchedRecordsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetAmountMismatchRecordsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetAmountMismatchRecordsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetMissingInBankRecordsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetMissingInBankRecordsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetMissingInExternalRecordsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetMissingInExternalRecordsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetManuallyResolvedRecordsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetManuallyResolvedRecordsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsWithDifferencesAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetRecordsWithDifferencesAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    // 8. RECORD AMOUNT AND DATE ANALYSIS

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsByExternalDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var records = await _unitOfWork.ReconciliationRecord
            .GetByExternalDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsByExternalAmountRangeAsync(
            decimal minimumAmount,
            decimal maximumAmount)
    {
        if (minimumAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumAmount));

        if (maximumAmount < minimumAmount)
            throw new ArgumentException(
                "Maximum amount cannot be less than minimum amount.");

        var records = await _unitOfWork.ReconciliationRecord
            .GetByExternalAmountRangeAsync(minimumAmount, maximumAmount);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    // 9. EXCEPTION RESOLUTION AND AUDIT

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsByResolvedByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetByResolvedByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetRecordsResolvedWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var records = await _unitOfWork.ReconciliationRecord
            .GetResolvedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        GetUnresolvedRecordsAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            return Enumerable.Empty<ReconciliationRecordDto>();

        var records = await _unitOfWork.ReconciliationRecord
            .GetUnresolvedRecordsAsync(reconciliationBatchId);

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    // 10. SEARCH AND REPORTING

    public async Task<IEnumerable<ReconciliationBatchDto>>
        SearchBatchesAsync(string searchTerm)
    {
        ValidateRequiredText(searchTerm, nameof(searchTerm));

        var batches = await _unitOfWork.ReconciliationBatch
            .SearchBatchesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<ReconciliationBatchDto>>(batches);
    }

    public async Task<IEnumerable<ReconciliationRecordDto>>
        SearchRecordsAsync(string searchTerm)
    {
        ValidateRequiredText(searchTerm, nameof(searchTerm));

        var records = await _unitOfWork.ReconciliationRecord
            .SearchRecordsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<ReconciliationRecordDto>>(records);
    }

    public Task<int> CountBatchesByStatusAndBusinessDateAsync(
        string status,
        DateTime businessDate)
    {
        ValidateAllowedValue(status, ValidBatchStatuses, nameof(status));
        ValidateDate(businessDate, nameof(businessDate));

        return _unitOfWork.ReconciliationBatch
            .CountByStatusAndBusinessDateAsync(status.Trim(), businessDate);
    }

    public Task<int> CountRecordsByBatchAndMatchStatusAsync(
        int reconciliationBatchId,
        string matchStatus)
    {
        if (reconciliationBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(reconciliationBatchId));

        ValidateAllowedValue(matchStatus, ValidMatchStatuses, nameof(matchStatus));

        return _unitOfWork.ReconciliationRecord
            .CountByBatchAndMatchStatusAsync(
                reconciliationBatchId,
                matchStatus.Trim());
    }

    public Task<decimal> GetTotalExternalAmountByBatchAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(reconciliationBatchId));

        return _unitOfWork.ReconciliationRecord
            .GetTotalExternalAmountByBatchAsync(reconciliationBatchId);
    }

    public Task<decimal> GetTotalInternalAmountByBatchAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(reconciliationBatchId));

        return _unitOfWork.ReconciliationRecord
            .GetTotalInternalAmountByBatchAsync(reconciliationBatchId);
    }

    public Task<decimal> GetTotalDifferenceAmountByBatchAsync(int reconciliationBatchId)
    {
        if (reconciliationBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(reconciliationBatchId));

        return _unitOfWork.ReconciliationRecord
            .GetTotalDifferenceAmountByBatchAsync(reconciliationBatchId);
    }

    // PRIVATE BUSINESS RULES

    private async Task ValidateBatchReferencesAsync(
        int? paymentMethodId,
        int? initiatedByUserId)
    {
        if (paymentMethodId.HasValue &&
            await _unitOfWork.PaymentMethod.GetByIdAsync(paymentMethodId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified payment method does not exist.");
        }

        if (initiatedByUserId.HasValue &&
            await _unitOfWork.User.GetByIdAsync(initiatedByUserId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified initiating user does not exist.");
        }
    }

    private async Task ValidateRecordReferencesAsync(
        int? transactionId,
        int? resolvedByUserId)
    {
        if (transactionId.HasValue &&
            await _unitOfWork.Transaction.GetByIdAsync(transactionId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified transaction does not exist.");
        }

        if (resolvedByUserId.HasValue &&
            await _unitOfWork.User.GetByIdAsync(resolvedByUserId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified resolving user does not exist.");
        }
    }

    private static void EnsureBatchAcceptsRecords(ReconciliationBatch batch)
    {
        if (batch.Status != "Pending" && batch.Status != "InProgress")
        {
            throw new InvalidOperationException(
                "Records can only be added or modified in Pending or InProgress batches.");
        }
    }

    private async Task RefreshBatchTotalsAsync(int reconciliationBatchId)
    {
        var batch = await _unitOfWork.ReconciliationBatch
            .GetByIdAsync(reconciliationBatchId);

        if (batch == null)
            throw new InvalidOperationException("The reconciliation batch does not exist.");

        var records = (await _unitOfWork.ReconciliationRecord
            .GetByBatchIdAsync(reconciliationBatchId)).ToList();

        batch.TotalRecords = records.Count;

        batch.MatchedRecords = records.Count(x =>
            x.MatchStatus == "Matched" ||
            (x.MatchStatus == "Manual" && x.ResolvedAt.HasValue));

        batch.UnmatchedRecords = records.Count(x =>
            x.MatchStatus != "Matched" &&
            !(x.MatchStatus == "Manual" && x.ResolvedAt.HasValue));

        batch.TotalAmount = records.Sum(x => x.ExternalAmount);

        batch.MatchedAmount = records
            .Where(x => x.MatchStatus == "Matched" ||
                        (x.MatchStatus == "Manual" && x.ResolvedAt.HasValue))
            .Sum(x => x.ExternalAmount);

        batch.DifferenceAmount = records.Sum(x => x.DifferenceAmount);
        batch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ReconciliationBatch.Update(batch);
        await _unitOfWork.SaveChangesAsync();
    }

    private static void ValidateBatchTotalsAgainstRecords(
        UpdateReconciliationBatchDto dto,
        IReadOnlyCollection<ReconciliationRecord> records)
    {
        var matchedRecords = records.Count(x =>
            x.MatchStatus == "Matched" ||
            (x.MatchStatus == "Manual" && x.ResolvedAt.HasValue));

        var unmatchedRecords = records.Count(x =>
            x.MatchStatus != "Matched" &&
            !(x.MatchStatus == "Manual" && x.ResolvedAt.HasValue));

        var totalAmount = records.Sum(x => x.ExternalAmount);

        var matchedAmount = records
            .Where(x => x.MatchStatus == "Matched" ||
                        (x.MatchStatus == "Manual" && x.ResolvedAt.HasValue))
            .Sum(x => x.ExternalAmount);

        var differenceAmount = records.Sum(x => x.DifferenceAmount);

        if (dto.TotalRecords != records.Count ||
            dto.MatchedRecords != matchedRecords ||
            dto.UnmatchedRecords != unmatchedRecords ||
            dto.TotalAmount != totalAmount ||
            dto.MatchedAmount != matchedAmount ||
            dto.DifferenceAmount != differenceAmount)
        {
            throw new InvalidOperationException(
                "Batch totals do not match the imported reconciliation records.");
        }
    }

    private static void ValidateBatch(CreateReconciliationBatchDto dto)
    {
        ValidateRequiredText(dto.BatchNumber, nameof(dto.BatchNumber), 30);
        ValidateAllowedValue(dto.ReconciliationType, ValidTypes, nameof(dto.ReconciliationType));
        ValidateRequiredText(dto.SourceName, nameof(dto.SourceName), 100);
        ValidateAllowedValue(dto.Status, ValidBatchStatuses, nameof(dto.Status));
        ValidateOptionalTextLength(dto.FileName, nameof(dto.FileName), 260);
        ValidateOptionalTextLength(dto.FileHash, nameof(dto.FileHash), 64);

        if (dto.BusinessDate == default)
            throw new ArgumentException("BusinessDate is required.");

        if (dto.TotalRecords < 0 || dto.MatchedRecords < 0 || dto.UnmatchedRecords < 0)
            throw new ArgumentException("Batch record counts cannot be negative.");

        if (dto.MatchedRecords + dto.UnmatchedRecords > dto.TotalRecords)
            throw new ArgumentException(
                "MatchedRecords and UnmatchedRecords cannot exceed TotalRecords.");

        if (dto.TotalAmount < 0 || dto.MatchedAmount < 0)
            throw new ArgumentException("Batch amounts cannot be negative.");

        if (dto.MatchedAmount > dto.TotalAmount)
            throw new ArgumentException(
                "MatchedAmount cannot exceed TotalAmount.");

        if (dto.StartedAt.HasValue && dto.StartedAt.Value == default)
            throw new ArgumentException("StartedAt is invalid.");

        if (dto.CompletedAt.HasValue && dto.CompletedAt.Value == default)
            throw new ArgumentException("CompletedAt is invalid.");

        if (dto.CompletedAt.HasValue && dto.StartedAt.HasValue &&
            dto.CompletedAt.Value < dto.StartedAt.Value)
        {
            throw new ArgumentException(
                "CompletedAt cannot be earlier than StartedAt.");
        }

        if (dto.PaymentMethodId.HasValue && dto.PaymentMethodId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.PaymentMethodId));

        if (dto.InitiatedByUserId.HasValue && dto.InitiatedByUserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.InitiatedByUserId));
    }

    private static void ValidateBatch(UpdateReconciliationBatchDto dto)
    {
        ValidateRequiredText(dto.BatchNumber, nameof(dto.BatchNumber), 30);
        ValidateAllowedValue(dto.ReconciliationType, ValidTypes, nameof(dto.ReconciliationType));
        ValidateRequiredText(dto.SourceName, nameof(dto.SourceName), 100);
        ValidateAllowedValue(dto.Status, ValidBatchStatuses, nameof(dto.Status));
        ValidateOptionalTextLength(dto.FileName, nameof(dto.FileName), 260);
        ValidateOptionalTextLength(dto.FileHash, nameof(dto.FileHash), 64);

        if (dto.BusinessDate == default)
            throw new ArgumentException("BusinessDate is required.");

        if (dto.TotalRecords < 0 || dto.MatchedRecords < 0 || dto.UnmatchedRecords < 0)
            throw new ArgumentException("Batch record counts cannot be negative.");

        if (dto.MatchedRecords + dto.UnmatchedRecords > dto.TotalRecords)
            throw new ArgumentException(
                "MatchedRecords and UnmatchedRecords cannot exceed TotalRecords.");

        if (dto.TotalAmount < 0 || dto.MatchedAmount < 0)
            throw new ArgumentException("Batch amounts cannot be negative.");

        if (dto.MatchedAmount > dto.TotalAmount)
            throw new ArgumentException(
                "MatchedAmount cannot exceed TotalAmount.");

        if (dto.StartedAt.HasValue && dto.StartedAt.Value == default)
            throw new ArgumentException("StartedAt is invalid.");

        if (dto.CompletedAt.HasValue && dto.CompletedAt.Value == default)
            throw new ArgumentException("CompletedAt is invalid.");

        if (dto.CompletedAt.HasValue && dto.StartedAt.HasValue &&
            dto.CompletedAt.Value < dto.StartedAt.Value)
        {
            throw new ArgumentException(
                "CompletedAt cannot be earlier than StartedAt.");
        }

        if (dto.PaymentMethodId.HasValue && dto.PaymentMethodId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.PaymentMethodId));

        if (dto.InitiatedByUserId.HasValue && dto.InitiatedByUserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.InitiatedByUserId));
    }

    private static void ValidateRecord(CreateReconciliationRecordDto dto)
    {
        ValidateRequiredText(dto.ExternalReference, nameof(dto.ExternalReference), 50);
        ValidateAllowedValue(dto.MatchStatus, ValidMatchStatuses, nameof(dto.MatchStatus));
        ValidateOptionalTextLength(dto.ExceptionReason, nameof(dto.ExceptionReason), 250);
        ValidateOptionalTextLength(dto.ResolutionNotes, nameof(dto.ResolutionNotes), 500);

        if (dto.ExternalAmount < 0 || dto.InternalAmount < 0)
            throw new ArgumentException("Record amounts cannot be negative.");

        if (dto.ExternalDate == default)
            throw new ArgumentException("ExternalDate is required.");

        if (dto.ReconciliationBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.ReconciliationBatchId));

        if (dto.TransactionId.HasValue && dto.TransactionId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.TransactionId));

        if (dto.ResolvedByUserId.HasValue && dto.ResolvedByUserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.ResolvedByUserId));

        ValidateManualResolution(
            dto.MatchStatus,
            dto.ResolvedAt,
            dto.ResolvedByUserId,
            dto.ResolutionNotes);
    }

    private static void ValidateRecord(UpdateReconciliationRecordDto dto)
    {
        ValidateRequiredText(dto.ExternalReference, nameof(dto.ExternalReference), 50);
        ValidateAllowedValue(dto.MatchStatus, ValidMatchStatuses, nameof(dto.MatchStatus));
        ValidateOptionalTextLength(dto.ExceptionReason, nameof(dto.ExceptionReason), 250);
        ValidateOptionalTextLength(dto.ResolutionNotes, nameof(dto.ResolutionNotes), 500);

        if (dto.ExternalAmount < 0 || dto.InternalAmount < 0)
            throw new ArgumentException("Record amounts cannot be negative.");

        if (dto.ExternalDate == default)
            throw new ArgumentException("ExternalDate is required.");

        if (dto.ReconciliationBatchId <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.ReconciliationBatchId));

        if (dto.TransactionId.HasValue && dto.TransactionId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.TransactionId));

        if (dto.ResolvedByUserId.HasValue && dto.ResolvedByUserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.ResolvedByUserId));

        ValidateManualResolution(
            dto.MatchStatus,
            dto.ResolvedAt,
            dto.ResolvedByUserId,
            dto.ResolutionNotes);
    }

    private static void ValidateManualResolution(
        string matchStatus,
        DateTime? resolvedAt,
        int? resolvedByUserId,
        string? resolutionNotes)
    {
        if (matchStatus == "Manual" &&
            (!resolvedAt.HasValue ||
             !resolvedByUserId.HasValue ||
             string.IsNullOrWhiteSpace(resolutionNotes)))
        {
            throw new ArgumentException(
                "Manual resolution requires ResolvedAt, ResolvedByUserId, and ResolutionNotes.");
        }

        if (resolvedAt.HasValue && resolvedAt.Value == default)
            throw new ArgumentException("ResolvedAt is invalid.");
    }

    private static void ValidateAllowedValue(
        string? value,
        IEnumerable<string> allowedValues,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !allowedValues.Contains(value.Trim(), StringComparer.Ordinal))
        {
            throw new ArgumentException(
                $"Invalid {parameterName}. Allowed values: {string.Join(", ", allowedValues)}.",
                parameterName);
        }
    }

    private static void ValidateRequiredText(
        string? value,
        string parameterName,
        int? maximumLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", parameterName);

        if (maximumLength.HasValue &&
            value.Trim().Length > maximumLength.Value)
        {
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength.Value} characters.",
                parameterName);
        }
    }

    private static void ValidateOptionalTextLength(
        string? value,
        string parameterName,
        int maximumLength)
    {
        if (value?.Trim().Length > maximumLength)
        {
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
        }
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException("Date is required.", parameterName);
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
