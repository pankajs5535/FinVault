
using AutoMapper;
using FinVault.Application.DTOs.RefundDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;


namespace FinVault.Application.Services;

public class RefundService : IRefundService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RefundService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<RefundDto>> GetAllAsync()
    {
        var refunds = await _unitOfWork.Refund.GetAllAsync();
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<RefundDto?> GetByIdAsync(int refundId)
    {
        if (refundId <= 0) return null;

        var refund = await _unitOfWork.Refund.GetByIdAsync(refundId);
        return refund == null ? null : _mapper.Map<RefundDto>(refund);
    }

    public async Task<RefundDto?> GetByRefundReferenceAsync(string refundReference)
    {
        if (string.IsNullOrWhiteSpace(refundReference)) return null;

        var refund = await _unitOfWork.Refund.GetByRefundReferenceAsync(refundReference.Trim());
        return refund == null ? null : _mapper.Map<RefundDto>(refund);
    }

    public async Task<RefundDto?> GetRefundDetailsAsync(int refundId)
    {
        if (refundId <= 0) return null;

        var refund = await _unitOfWork.Refund.GetRefundDetailsAsync(refundId);
        return refund == null ? null : _mapper.Map<RefundDto>(refund);
    }

    public async Task<RefundDto> CreateAsync(CreateRefundDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRequestDetails(dto.RefundReference, dto.RefundType, dto.Amount, dto.Reason);

        if (dto.Status != "Requested")
            throw new InvalidOperationException("A refund must initially be created with Requested status.");

        if (dto.ProcessedAt.HasValue ||
            dto.RefundTransactionId.HasValue ||
            dto.ApprovedByUserId.HasValue ||
            !string.IsNullOrWhiteSpace(dto.RejectionReason))
            throw new InvalidOperationException("Approval, rejection and processing details cannot be supplied when creating a refund.");

        if (!dto.RequestedByUserId.HasValue)
            throw new InvalidOperationException("The requesting user is required for maker-checker control.");

        if (await _unitOfWork.Refund.ExistsByRefundReferenceAsync(dto.RefundReference.Trim()))
            throw new InvalidOperationException("The refund reference already exists.");

        var originalTransaction = await _unitOfWork.Transaction.GetByIdAsync(dto.OriginalTransactionId);

        if (originalTransaction == null)
            throw new InvalidOperationException("The original transaction does not exist.");

        if (originalTransaction.Status != "Posted")
            throw new InvalidOperationException("Only posted transactions can be refunded.");

        if (await _unitOfWork.User.GetByIdAsync(dto.RequestedByUserId.Value) == null)
            throw new InvalidOperationException("The requesting user does not exist.");

        if (dto.DisputeId.HasValue &&
            await _unitOfWork.Dispute.GetByIdAsync(dto.DisputeId.Value) == null)
            throw new InvalidOperationException("The specified dispute does not exist.");

        var alreadyProcessedAmount =
            await _unitOfWork.Refund.GetTotalProcessedAmountByOriginalTransactionIdAsync(
                dto.OriginalTransactionId);

        ValidateRefundAmount(
            dto.RefundType,
            dto.Amount,
            originalTransaction.Amount,
            alreadyProcessedAmount);

        var refund = new Refund
        {
            RefundReference = dto.RefundReference.Trim(),
            RefundType = dto.RefundType,
            Amount = dto.Amount,
            Reason = dto.Reason.Trim(),
            Status = "Requested",
            RequestedAt = DateTime.UtcNow,
            ProcessedAt = null,
            RejectionReason = null,
            OriginalTransactionId = dto.OriginalTransactionId,
            RefundTransactionId = null,
            DisputeId = dto.DisputeId,
            RequestedByUserId = dto.RequestedByUserId,
            ApprovedByUserId = null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = dto.RequestedByUserId
        };

        await _unitOfWork.Refund.AddAsync(refund);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RefundDto>(refund);
    }

    public async Task<RefundDto?> UpdateAsync(UpdateRefundDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRequestDetails(dto.RefundReference, dto.RefundType, dto.Amount, dto.Reason);

        var refund = await _unitOfWork.Refund.GetByIdAsync(dto.RefundId);
        if (refund == null) return null;

        if (dto.RowVersion == null || dto.RowVersion.Length == 0 ||
            refund.RowVersion == null ||
            !refund.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                    "The refund was modified by another user. Reload the refund before updating it.");
        }

        if (refund.Status is "Processed" or "Rejected" or "Failed")
            throw new InvalidOperationException("A refund in a final status cannot be modified.");

        if (refund.OriginalTransactionId != dto.OriginalTransactionId)
            throw new InvalidOperationException("The original transaction cannot be changed.");

        if (refund.RefundReference != dto.RefundReference.Trim() &&
            await _unitOfWork.Refund.ExistsByRefundReferenceAsync(dto.RefundReference.Trim()))
        {
            throw new InvalidOperationException("The refund reference already exists.");
        }

        var originalTransaction = await _unitOfWork.Transaction.GetByIdAsync(refund.OriginalTransactionId);

        if (originalTransaction == null)
            throw new InvalidOperationException("The original transaction does not exist.");

        if (dto.DisputeId.HasValue &&
            await _unitOfWork.Dispute.GetByIdAsync(dto.DisputeId.Value) == null)
        {
            throw new InvalidOperationException("The specified dispute does not exist.");
        }

        if (!dto.RequestedByUserId.HasValue ||
            dto.RequestedByUserId != refund.RequestedByUserId)
        {
            throw new InvalidOperationException("The original requesting user cannot be changed.");
        }

        if (refund.Status == "Requested")
        {
            if (dto.Status == "Requested")
            {
                if (dto.ApprovedByUserId.HasValue ||
                    dto.RefundTransactionId.HasValue ||
                    dto.ProcessedAt.HasValue ||
                    !string.IsNullOrWhiteSpace(dto.RejectionReason))
                {
                    throw new InvalidOperationException(
                        "A requested refund cannot contain approval or processing details.");
                }

                if (await _unitOfWork.User.GetByIdAsync(dto.RequestedByUserId.Value) == null)
                    throw new InvalidOperationException("The requesting user does not exist.");

                var processedAmount =
                    await _unitOfWork.Refund.GetTotalProcessedAmountByOriginalTransactionIdAsync(
                        refund.OriginalTransactionId);

                ValidateRefundAmount(
                    dto.RefundType,
                    dto.Amount,
                    originalTransaction.Amount,
                    processedAmount);

                refund.RefundReference = dto.RefundReference.Trim();
                refund.RefundType = dto.RefundType;
                refund.Amount = dto.Amount;
                refund.Reason = dto.Reason.Trim();
                refund.DisputeId = dto.DisputeId;
            }
            else if (dto.Status is "Approved" or "Rejected")
            {
                EnsureRequestDetailsUnchanged(refund, dto);

                if (!dto.ApprovedByUserId.HasValue)
                    throw new InvalidOperationException("An approving or rejecting user is required.");

                if (dto.ApprovedByUserId.Value == refund.RequestedByUserId)
                    throw new InvalidOperationException("The requester cannot approve or reject their own refund.");

                if (await _unitOfWork.User.GetByIdAsync(dto.ApprovedByUserId.Value) == null)
                    throw new InvalidOperationException("The approving or rejecting user does not exist.");

                if (dto.RefundTransactionId.HasValue || dto.ProcessedAt.HasValue)
                    throw new InvalidOperationException("A refund cannot contain processing details before it is processed.");

                refund.ApprovedByUserId = dto.ApprovedByUserId.Value;
                refund.UpdatedBy = dto.ApprovedByUserId.Value;

                if (dto.Status == "Approved")
                {
                    if (!string.IsNullOrWhiteSpace(dto.RejectionReason))
                        throw new InvalidOperationException("An approved refund cannot contain a rejection reason.");

                    refund.Status = "Approved";
                    refund.RejectionReason = null;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                        throw new InvalidOperationException("A rejection reason is required.");

                    if (dto.RejectionReason.Trim().Length > 250)
                        throw new ArgumentException("The rejection reason cannot exceed 250 characters.");

                    refund.Status = "Rejected";
                    refund.RejectionReason = dto.RejectionReason.Trim();
                }
            }
            else
            {
                throw new InvalidOperationException("The requested refund status transition is not allowed.");
            }
        }
        else if (refund.Status == "Approved")
        {
            EnsureRequestDetailsUnchanged(refund, dto);

            if (dto.ApprovedByUserId != refund.ApprovedByUserId)
                throw new InvalidOperationException("The original approving user cannot be changed.");

            if (dto.Status == "Processed")
            {
                if (!dto.RefundTransactionId.HasValue)
                    throw new InvalidOperationException("A processed refund must be linked to its resulting transaction.");

                if (dto.ProcessedAt.HasValue)
                    throw new InvalidOperationException("ProcessedAt is generated by the service and cannot be supplied by the client.");

                var refundTransaction =
                    await _unitOfWork.Transaction.GetByIdAsync(dto.RefundTransactionId.Value);

                if (refundTransaction == null)
                    throw new InvalidOperationException("The resulting refund transaction does not exist.");

                if (refundTransaction.Status != "Posted")
                    throw new InvalidOperationException("The resulting refund transaction must be posted.");

                if (refundTransaction.TransactionId == refund.OriginalTransactionId)
                    throw new InvalidOperationException("The refund transaction must differ from the original transaction.");

                if (refundTransaction.Amount != refund.Amount)
                    throw new InvalidOperationException("The resulting transaction amount must match the refund amount.");

                var existingRefund =
                    await _unitOfWork.Refund.GetByRefundTransactionIdAsync(refundTransaction.TransactionId);

                if (existingRefund != null && existingRefund.RefundId != refund.RefundId)
                    throw new InvalidOperationException("The resulting transaction is already linked to another refund.");

                var alreadyProcessedAmount =
                    await _unitOfWork.Refund.GetTotalProcessedAmountByOriginalTransactionIdAsync(
                        refund.OriginalTransactionId);

                if (alreadyProcessedAmount + refund.Amount > originalTransaction.Amount)
                    throw new InvalidOperationException(
                        "Cumulative processed refunds cannot exceed the original transaction amount.");

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    refund.Status = "Processed";
                    refund.RefundTransactionId = refundTransaction.TransactionId;
                    refund.ProcessedAt = DateTime.UtcNow;
                    refund.UpdatedAt = DateTime.UtcNow;

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }

                return _mapper.Map<RefundDto>(refund);
            }
            else if (dto.Status == "Failed")
            {
                if (dto.RefundTransactionId.HasValue || dto.ProcessedAt.HasValue)
                    throw new InvalidOperationException("A failed refund cannot be linked as a successfully processed refund.");

                if (!string.IsNullOrWhiteSpace(dto.RejectionReason) &&
                    dto.RejectionReason.Trim().Length > 250)
                {
                    throw new ArgumentException("The failure reason cannot exceed 250 characters.");
                }

                refund.Status = "Failed";
                refund.RejectionReason = string.IsNullOrWhiteSpace(dto.RejectionReason)
                    ? null
                    : dto.RejectionReason.Trim();
                refund.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("The approved refund can only be processed or marked as failed.");
            }
        }
        else
        {
            throw new InvalidOperationException("The refund is not in a modifiable state.");
        }

        refund.UpdatedAt = DateTime.UtcNow;

        // The entity was loaded through the tracked repository query.
        // Do not replace its RowVersion or overwrite audit/navigation properties.
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RefundDto>(refund);
    }

    public async Task<bool> DeleteAsync(DeleteRefundDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.RefundId <= 0) return false;

        var refund = await _unitOfWork.Refund.GetByIdAsync(dto.RefundId);

        // The entity has no soft-delete field. Financial records must be retained.
        return refund == null ? false : false;
    }

    public Task<bool> ExistsByRefundReferenceAsync(string refundReference)
    {
        if (string.IsNullOrWhiteSpace(refundReference))
            return Task.FromResult(false);

        return _unitOfWork.Refund.ExistsByRefundReferenceAsync(refundReference.Trim());
    }

    // 2. ORIGINAL TRANSACTION AND DISPUTE HISTORY

    public async Task<IEnumerable<RefundDto>> GetByOriginalTransactionIdAsync(int originalTransactionId)
    {
        if (originalTransactionId <= 0) return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.GetByOriginalTransactionIdAsync(originalTransactionId);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<RefundDto?> GetByRefundTransactionIdAsync(int refundTransactionId)
    {
        if (refundTransactionId <= 0) return null;

        var refund = await _unitOfWork.Refund.GetByRefundTransactionIdAsync(refundTransactionId);
        return refund == null ? null : _mapper.Map<RefundDto>(refund);
    }

    public async Task<IEnumerable<RefundDto>> GetByDisputeIdAsync(int disputeId)
    {
        if (disputeId <= 0) return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.GetByDisputeIdAsync(disputeId);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetRefundsWithTransactionsAsync(int originalTransactionId)
    {
        if (originalTransactionId <= 0) return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.GetRefundsWithTransactionsAsync(originalTransactionId);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    // 3. REFUND STATUS AND TYPE

    public async Task<IEnumerable<RefundDto>> GetByStatusAsync(string status)
    {
        if (string.IsNullOrWhiteSpace(status)) return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.GetByStatusAsync(status.Trim());
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetRequestedRefundsAsync()
    {
        var refunds = await _unitOfWork.Refund.GetRequestedRefundsAsync();
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetApprovedRefundsAsync()
    {
        var refunds = await _unitOfWork.Refund.GetApprovedRefundsAsync();
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetProcessedRefundsAsync()
    {
        var refunds = await _unitOfWork.Refund.GetProcessedRefundsAsync();
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetRejectedRefundsAsync()
    {
        var refunds = await _unitOfWork.Refund.GetRejectedRefundsAsync();
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetFailedRefundsAsync()
    {
        var refunds = await _unitOfWork.Refund.GetFailedRefundsAsync();
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetByRefundTypeAsync(string refundType)
    {
        if (string.IsNullOrWhiteSpace(refundType)) return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.GetByRefundTypeAsync(refundType.Trim());
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetOutstandingRefundsAsync()
    {
        var refunds = await _unitOfWork.Refund.GetOutstandingRefundsAsync();
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    // 4. APPROVAL AND REQUEST TRACKING

    public async Task<IEnumerable<RefundDto>> GetByRequestedByUserIdAsync(int userId)
    {
        if (userId <= 0) return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.GetByRequestedByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetByApprovedByUserIdAsync(int userId)
    {
        if (userId <= 0) return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.GetByApprovedByUserIdAsync(userId);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetRequestedWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var refunds = await _unitOfWork.Refund.GetRequestedWithinRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    public async Task<IEnumerable<RefundDto>> GetProcessedWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var refunds = await _unitOfWork.Refund.GetProcessedWithinRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    // 5. FINANCIAL REPORTING

    public Task<decimal> GetTotalProcessedAmountByOriginalTransactionIdAsync(int originalTransactionId)
    {
        if (originalTransactionId <= 0) return Task.FromResult(0m);

        return _unitOfWork.Refund.GetTotalProcessedAmountByOriginalTransactionIdAsync(originalTransactionId);
    }

    public Task<decimal> GetTotalProcessedAmountByDisputeIdAsync(int disputeId)
    {
        if (disputeId <= 0) return Task.FromResult(0m);

        return _unitOfWork.Refund.GetTotalProcessedAmountByDisputeIdAsync(disputeId);
    }

    public Task<decimal> GetTotalProcessedAmountWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        return _unitOfWork.Refund.GetTotalProcessedAmountWithinRangeAsync(startDate, endDate);
    }

    public Task<int> CountByOriginalTransactionIdAsync(int originalTransactionId)
    {
        if (originalTransactionId <= 0) return Task.FromResult(0);

        return _unitOfWork.Refund.CountByOriginalTransactionIdAsync(originalTransactionId);
    }

    // 6. SEARCH

    public async Task<IEnumerable<RefundDto>> SearchRefundsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<RefundDto>();

        var refunds = await _unitOfWork.Refund.SearchRefundsAsync(searchTerm.Trim());
        return _mapper.Map<IEnumerable<RefundDto>>(refunds);
    }

    // PRIVATE VALIDATION HELPERS

    private static void ValidateRequestDetails(
        string refundReference,
        string refundType,
        decimal amount,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(refundReference))
            throw new ArgumentException("Refund reference is required.");

        if (refundReference.Trim().Length > 40)
            throw new ArgumentException("Refund reference cannot exceed 40 characters.");

        if (refundType is not ("Full" or "Partial"))
            throw new ArgumentException("Refund type must be Full or Partial.");

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Refund amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Refund reason is required.");

        if (reason.Trim().Length > 250)
            throw new ArgumentException("Refund reason cannot exceed 250 characters.");
    }

    private static void ValidateRefundAmount(
        string refundType,
        decimal amount,
        decimal originalAmount,
        decimal alreadyProcessedAmount)
    {
        var remainingAmount = originalAmount - alreadyProcessedAmount;

        if (remainingAmount <= 0)
            throw new InvalidOperationException("The original transaction has already been fully refunded.");

        if (amount > remainingAmount)
            throw new InvalidOperationException("The refund amount exceeds the remaining refundable amount.");

        if (refundType == "Full" && amount != remainingAmount)
            throw new InvalidOperationException("A Full refund must equal the remaining refundable amount.");

        if (refundType == "Partial" && amount >= remainingAmount)
            throw new InvalidOperationException("A Partial refund must be less than the remaining refundable amount.");
    }

    private static void EnsureRequestDetailsUnchanged(Refund refund, UpdateRefundDto dto)
    {
        if (refund.RefundReference != dto.RefundReference.Trim() ||
            refund.RefundType != dto.RefundType ||
            refund.Amount != dto.Amount ||
            refund.Reason != dto.Reason.Trim() ||
            refund.DisputeId != dto.DisputeId)
        {
            throw new InvalidOperationException(
                "Refund details cannot be changed while approving, rejecting or processing the refund.");
        }
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default || startDate > endDate)
            throw new ArgumentException("A valid date range is required.");
    }
}
