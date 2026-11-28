
using AutoMapper;
using FinVault.Application.DTOs.DisputeDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class DisputeService : IDisputeService
{
    private static readonly string[] ValidDisputeTypes =
    {
        "UnauthorizedTransaction",
        "DuplicateCharge",
        "WrongAmount",
        "ServiceNotReceived",
        "AtmCashNotDispensed",
        "FailedTransactionDebited",
        "Other"
    };

    private static readonly string[] ValidStatuses =
    {
        "Open",
        "UnderInvestigation",
        "ProvisionalCreditGiven",
        "Resolved",
        "Rejected",
        "Escalated",
        "Closed"
    };

    private static readonly string[] ValidPriorities =
    {
        "Low", "Medium", "High", "Critical"
    };

    private static readonly string[] ValidResolutions =
    {
        "InFavourOfCustomer",
        "InFavourOfBank",
        "Withdrawn"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public DisputeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. DISPUTE IDENTIFICATION

    public async Task<IEnumerable<DisputeDto>> GetAllAsync()
    {
        var disputes = await _unitOfWork.Dispute.GetAllAsync();
        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<DisputeDto?> GetByIdAsync(int disputeId)
    {
        ValidateId(disputeId, nameof(disputeId));

        var dispute = await _unitOfWork.Dispute.GetByIdAsync(disputeId);
        return _mapper.Map<DisputeDto?>(dispute);
    }

    public async Task<DisputeDto?> GetByDisputeNumberAsync(string disputeNumber)
    {
        ValidateRequiredString(disputeNumber, nameof(disputeNumber), 30);

        var dispute = await _unitOfWork.Dispute
            .GetByDisputeNumberAsync(disputeNumber.Trim());

        return _mapper.Map<DisputeDto?>(dispute);
    }

    public async Task<DisputeDto?> GetDisputeDetailsAsync(int disputeId)
    {
        ValidateId(disputeId, nameof(disputeId));

        var dispute = await _unitOfWork.Dispute
            .GetDisputeDetailsAsync(disputeId);

        return _mapper.Map<DisputeDto?>(dispute);
    }

    public async Task<bool> ExistsByDisputeNumberAsync(string disputeNumber)
    {
        ValidateRequiredString(disputeNumber, nameof(disputeNumber), 30);

        return await _unitOfWork.Dispute
            .ExistsByDisputeNumberAsync(disputeNumber.Trim());
    }

    // 2. DISPUTE CRUD

    public async Task<DisputeDto> CreateAsync(CreateDisputeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateDispute(
            dto.DisputeNumber,
            dto.DisputeType,
            dto.Description,
            dto.DisputedAmount,
            dto.Status,
            dto.Priority,
            dto.RaisedAt,
            dto.DueBy,
            dto.ResolvedAt,
            dto.Resolution,
            dto.ResolutionNotes);

        ValidateId(dto.TransactionId, nameof(dto.TransactionId));
        ValidateId(dto.CustomerId, nameof(dto.CustomerId));

        var disputeNumber = dto.DisputeNumber.Trim();

        if (await _unitOfWork.Dispute
            .ExistsByDisputeNumberAsync(disputeNumber))
        {
            throw new InvalidOperationException(
                "A dispute with this dispute number already exists.");
        }

        await ValidateReferencesAsync(
            dto.TransactionId,
            dto.CardTransactionId,
            dto.CustomerId);

        ValidateResolutionInformation(
            dto.Status,
            dto.Resolution,
            dto.ResolutionNotes,
            dto.ResolvedAt);

        var dispute = _mapper.Map<Dispute>(dto);

        dispute.DisputeNumber = disputeNumber;
        dispute.DisputeType = dto.DisputeType.Trim();
        dispute.Description = dto.Description.Trim();
        dispute.Status = dto.Status.Trim();
        dispute.Priority = dto.Priority.Trim();
        dispute.Resolution = dto.Resolution?.Trim();
        dispute.ResolutionNotes = dto.ResolutionNotes?.Trim();

        dispute.RaisedAt = dto.RaisedAt == default
            ? DateTime.UtcNow
            : dto.RaisedAt;

        dispute.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Dispute.AddAsync(dispute);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DisputeDto>(dispute);
    }

    public async Task<DisputeDto?> UpdateAsync(UpdateDisputeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.DisputeId, nameof(dto.DisputeId));

        ValidateDispute(
            dto.DisputeNumber,
            dto.DisputeType,
            dto.Description,
            dto.DisputedAmount,
            dto.Status,
            dto.Priority,
            dto.RaisedAt,
            dto.DueBy,
            dto.ResolvedAt,
            dto.Resolution,
            dto.ResolutionNotes);

        ValidateId(dto.TransactionId, nameof(dto.TransactionId));
        ValidateId(dto.CustomerId, nameof(dto.CustomerId));

        if (dto.RowVersion == null || dto.RowVersion.Length == 0)
            throw new ArgumentException("RowVersion is required.");

        var dispute = await _unitOfWork.Dispute
            .GetByIdAsync(dto.DisputeId);

        if (dispute == null)
            return null;

        // Check the concurrency token supplied by the client.
        if (!dispute.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "This dispute has been modified by another user. Reload the dispute before updating.");
        }

        if (dispute.Status is "Resolved" or "Rejected" or "Closed")
        {
            throw new InvalidOperationException(
                "Finalized disputes cannot be modified.");
        }

        var disputeNumber = dto.DisputeNumber.Trim();

        if (await _unitOfWork.Dispute
            .ExistsByDisputeNumberAsync(disputeNumber) &&
            !string.Equals(dispute.DisputeNumber, disputeNumber,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "A dispute with this dispute number already exists.");
        }

        await ValidateReferencesAsync(
            dto.TransactionId,
            dto.CardTransactionId,
            dto.CustomerId);

        ValidateResolutionInformation(
            dto.Status,
            dto.Resolution,
            dto.ResolutionNotes,
            dto.ResolvedAt);

        if (dto.RaisedAt == default)
            throw new ArgumentException("Raised date is required.");

        _mapper.Map(dto, dispute);

        dispute.DisputeNumber = disputeNumber;
        dispute.DisputeType = dto.DisputeType.Trim();
        dispute.Description = dto.Description.Trim();
        dispute.Status = dto.Status.Trim();
        dispute.Priority = dto.Priority.Trim();
        dispute.Resolution = dto.Resolution?.Trim();
        dispute.ResolutionNotes = dto.ResolutionNotes?.Trim();
        dispute.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Dispute.Update(dispute);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<DisputeDto>(dispute);
    }

    public async Task<bool> DeleteAsync(DeleteDisputeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.DisputeId, nameof(dto.DisputeId));

        var dispute = await _unitOfWork.Dispute
            .GetDisputeDetailsAsync(dto.DisputeId);

        if (dispute == null)
            return false;

        if (dispute.Status is "Resolved" or "Rejected" or "Closed")
        {
            throw new InvalidOperationException(
                "Finalized disputes cannot be deleted.");
        }

        if (dispute.Refunds.Any())
        {
            throw new InvalidOperationException(
                "Disputes associated with refunds cannot be deleted.");
        }

        _unitOfWork.Dispute.Delete(dispute);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. TRANSACTION-BASED DISPUTES

    public async Task<IEnumerable<DisputeDto>> GetByTransactionIdAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var disputes = await _unitOfWork.Dispute
            .GetByTransactionIdAsync(transactionId);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<IEnumerable<DisputeDto>> GetByCardTransactionIdAsync(int cardTransactionId)
    {
        ValidateId(cardTransactionId, nameof(cardTransactionId));

        var disputes = await _unitOfWork.Dispute
            .GetByCardTransactionIdAsync(cardTransactionId);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    // 4. CUSTOMER DISPUTES

    public async Task<IEnumerable<DisputeDto>> GetByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var disputes = await _unitOfWork.Dispute
            .GetByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<int> CountDisputesByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        return await _unitOfWork.Dispute
            .CountDisputesByCustomerIdAsync(customerId);
    }

    // 5. STATUS AND PRIORITY

    public async Task<IEnumerable<DisputeDto>> GetByStatusAsync(string status)
    {
        ValidateStatus(status);

        var disputes = await _unitOfWork.Dispute
            .GetByStatusAsync(status.Trim());

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<IEnumerable<DisputeDto>> GetByPriorityAsync(string priority)
    {
        ValidatePriority(priority);

        var disputes = await _unitOfWork.Dispute
            .GetByPriorityAsync(priority.Trim());

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    // 6. EMPLOYEE ASSIGNMENT

    public async Task<IEnumerable<DisputeDto>> GetByAssignedEmployeeIdAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));

        var disputes = await _unitOfWork.Dispute
            .GetByAssignedEmployeeIdAsync(employeeId);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<IEnumerable<DisputeDto>> GetUnassignedDisputesAsync()
    {
        var disputes = await _unitOfWork.Dispute.GetUnassignedDisputesAsync();

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    // 7. INVESTIGATION QUEUES

    public async Task<IEnumerable<DisputeDto>> GetOpenDisputesAsync()
    {
        var disputes = await _unitOfWork.Dispute.GetOpenDisputesAsync();

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<IEnumerable<DisputeDto>> GetOverdueDisputesAsync(DateTime currentDate)
    {
        ValidateDate(currentDate, nameof(currentDate));

        var disputes = await _unitOfWork.Dispute
            .GetOverdueDisputesAsync(currentDate);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<IEnumerable<DisputeDto>> GetDisputesDueWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var disputes = await _unitOfWork.Dispute
            .GetDisputesDueWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    // 8. DISPUTE TIMELINE

    public async Task<IEnumerable<DisputeDto>> GetDisputesRaisedWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var disputes = await _unitOfWork.Dispute
            .GetDisputesRaisedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    public async Task<IEnumerable<DisputeDto>> GetResolvedDisputesWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var disputes = await _unitOfWork.Dispute
            .GetResolvedDisputesWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    // 9. REFUND MANAGEMENT

    public async Task<IEnumerable<DisputeDto>> GetDisputesWithRefundsAsync()
    {
        var disputes = await _unitOfWork.Dispute.GetDisputesWithRefundsAsync();

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    // 10. SEARCH

    public async Task<IEnumerable<DisputeDto>> SearchDisputesAsync(string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm), 200);

        var disputes = await _unitOfWork.Dispute
            .SearchDisputesAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<DisputeDto>>(disputes);
    }

    // VALIDATION HELPERS

    private async Task ValidateReferencesAsync(
        int transactionId,
        int? cardTransactionId,
        int customerId)
    {
        if (await _unitOfWork.Transaction.GetByIdAsync(transactionId) == null)
            throw new InvalidOperationException("Transaction does not exist.");

        if (await _unitOfWork.Customer.GetByIdAsync(customerId) == null)
            throw new InvalidOperationException("Customer does not exist.");

        if (cardTransactionId.HasValue &&
            await _unitOfWork.CardTransaction
                .GetByIdAsync(cardTransactionId.Value) == null)
        {
            throw new InvalidOperationException(
                "Card transaction does not exist.");
        }
    }

    private static void ValidateDispute(
        string disputeNumber,
        string disputeType,
        string description,
        decimal disputedAmount,
        string status,
        string priority,
        DateTime raisedAt,
        DateTime? dueBy,
        DateTime? resolvedAt,
        string? resolution,
        string? resolutionNotes)
    {
        ValidateRequiredString(disputeNumber, nameof(disputeNumber), 30);
        ValidateRequiredString(description, nameof(description), 1000);
        ValidateDisputeType(disputeType);
        ValidateStatus(status);
        ValidatePriority(priority);

        if (disputedAmount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(disputedAmount),
                "Disputed amount must be greater than zero.");

        if (raisedAt != default && dueBy.HasValue &&
            dueBy.Value < raisedAt)
        {
            throw new ArgumentException(
                "Due date cannot be earlier than raised date.");
        }

        if (resolvedAt.HasValue && resolvedAt.Value == default)
            throw new ArgumentException("Invalid resolution date.");

        if (resolvedAt.HasValue && raisedAt != default &&
            resolvedAt.Value < raisedAt)
        {
            throw new ArgumentException(
                "Resolution date cannot be earlier than raised date.");
        }

        if (resolution != null)
            ValidateResolution(resolution);

        ValidateOptionalString(resolutionNotes, nameof(resolutionNotes), 1000);
    }

    private static void ValidateResolutionInformation(
        string status,
        string? resolution,
        string? resolutionNotes,
        DateTime? resolvedAt)
    {
        if (status == "Resolved")
        {
            if (string.IsNullOrWhiteSpace(resolution))
                throw new ArgumentException(
                    "Resolution is required when resolving a dispute.");

            if (!resolvedAt.HasValue)
                throw new ArgumentException(
                    "Resolved date is required.");

            if (string.IsNullOrWhiteSpace(resolutionNotes))
                throw new ArgumentException(
                    "Resolution notes are required.");
        }

        if (status == "Rejected" &&
            string.IsNullOrWhiteSpace(resolutionNotes))
        {
            throw new ArgumentException(
                "Resolution notes are required when rejecting a dispute.");
        }

        if (resolution != null)
            ValidateResolution(resolution);
    }

    private static void ValidateDisputeType(string value)
    {
        ValidateRequiredString(value, nameof(value), 40);

        if (!ValidDisputeTypes.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid dispute type.", nameof(value));
    }

    private static void ValidateStatus(string value)
    {
        ValidateRequiredString(value, nameof(value), 30);

        if (!ValidStatuses.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid dispute status.", nameof(value));
    }

    private static void ValidatePriority(string value)
    {
        ValidateRequiredString(value, nameof(value), 10);

        if (!ValidPriorities.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid dispute priority.", nameof(value));
    }

    private static void ValidateResolution(string value)
    {
        ValidateRequiredString(value, nameof(value), 30);

        if (!ValidResolutions.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid dispute resolution.", nameof(value));
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

    private static void ValidateOptionalString(
        string? value, string parameterName, int maximumLength)
    {
        if (value?.Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.",
                parameterName);
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException("A valid date is required.", parameterName);
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }
}
