
using AutoMapper;
using FinVault.Application.DTOs.FraudAlertDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class FraudAlertService : IFraudAlertService
{
    private static readonly string[] ValidSeverities =
        { "Low", "Medium", "High", "Critical" };

    private static readonly string[] ValidStatuses =
        { "Open", "UnderReview", "ConfirmedFraud", "FalsePositive", "Closed" };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FraudAlertService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. ALERT IDENTIFICATION

    public async Task<IEnumerable<FraudAlertDto>> GetAllAsync()
    {
        var alerts = await _unitOfWork.FraudAlert.GetAllAsync();
        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<FraudAlertDto?> GetByIdAsync(int fraudAlertId)
    {
        ValidateId(fraudAlertId, nameof(fraudAlertId));

        var alert = await _unitOfWork.FraudAlert.GetByIdAsync(fraudAlertId);
        return _mapper.Map<FraudAlertDto?>(alert);
    }

    public async Task<FraudAlertDto?> GetByAlertNumberAsync(string alertNumber)
    {
        ValidateRequiredString(alertNumber, nameof(alertNumber), 30);

        var alert = await _unitOfWork.FraudAlert
            .GetByAlertNumberAsync(alertNumber.Trim());

        return _mapper.Map<FraudAlertDto?>(alert);
    }

    public async Task<FraudAlertDto?> GetFraudAlertDetailsAsync(int fraudAlertId)
    {
        ValidateId(fraudAlertId, nameof(fraudAlertId));

        var alert = await _unitOfWork.FraudAlert
            .GetFraudAlertDetailsAsync(fraudAlertId);

        return _mapper.Map<FraudAlertDto?>(alert);
    }

    public async Task<bool> ExistsByAlertNumberAsync(string alertNumber)
    {
        ValidateRequiredString(alertNumber, nameof(alertNumber), 30);

        return await _unitOfWork.FraudAlert
            .ExistsByAlertNumberAsync(alertNumber.Trim());
    }

    public async Task<FraudAlertDto> CreateAsync(CreateFraudAlertDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateAlert(
            dto.AlertNumber,
            dto.Severity,
            dto.Description,
            dto.Status,
            dto.Score,
            dto.RaisedAt,
            dto.ReviewedAt,
            dto.ReviewNotes,
            dto.ActionTaken);

        ValidateId(dto.FraudRuleId, nameof(dto.FraudRuleId));

        var alertNumber = dto.AlertNumber.Trim();

        if (await _unitOfWork.FraudAlert
            .ExistsByAlertNumberAsync(alertNumber))
        {
            throw new InvalidOperationException(
                "A fraud alert with this alert number already exists.");
        }

        await ValidateReferencesAsync(
            dto.FraudRuleId,
            dto.TransactionId,
            dto.CustomerId,
            dto.AccountId,
            dto.CardId,
            dto.RiskAssessmentId);

        ValidateReviewInformation(
            dto.Status,
            dto.ReviewedAt,
            dto.ReviewNotes,
            dto.ActionTaken,
            dto.ReviewedByEmployeeId);

        var alert = _mapper.Map<FraudAlert>(dto);

        alert.AlertNumber = alertNumber;
        alert.Severity = dto.Severity.Trim();
        alert.Status = dto.Status.Trim();
        alert.Description = dto.Description.Trim();
        alert.ReviewNotes = dto.ReviewNotes?.Trim();
        alert.ActionTaken = dto.ActionTaken?.Trim();

        alert.RaisedAt = dto.RaisedAt == default
            ? DateTime.UtcNow
            : dto.RaisedAt;

        alert.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.FraudAlert.AddAsync(alert);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FraudAlertDto>(alert);
    }

    public async Task<FraudAlertDto?> UpdateAsync(UpdateFraudAlertDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.FraudAlertId, nameof(dto.FraudAlertId));

        ValidateAlert(
            dto.AlertNumber,
            dto.Severity,
            dto.Description,
            dto.Status,
            dto.Score,
            dto.RaisedAt,
            dto.ReviewedAt,
            dto.ReviewNotes,
            dto.ActionTaken);

        ValidateId(dto.FraudRuleId, nameof(dto.FraudRuleId));

        var alert = await _unitOfWork.FraudAlert
            .GetByIdAsync(dto.FraudAlertId);

        if (alert == null)
            return null;

        if (alert.Status is "ConfirmedFraud" or "FalsePositive" or "Closed")
        {
            throw new InvalidOperationException(
                "Finalized fraud alerts cannot be modified.");
        }

        var alertNumber = dto.AlertNumber.Trim();

        if (await _unitOfWork.FraudAlert
            .ExistsByAlertNumberAsync(alertNumber) &&
            !string.Equals(alert.AlertNumber, alertNumber,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "A fraud alert with this alert number already exists.");
        }

        await ValidateReferencesAsync(
            dto.FraudRuleId,
            dto.TransactionId,
            dto.CustomerId,
            dto.AccountId,
            dto.CardId,
            dto.RiskAssessmentId);

        ValidateReviewInformation(
            dto.Status,
            dto.ReviewedAt,
            dto.ReviewNotes,
            dto.ActionTaken,
            dto.ReviewedByEmployeeId);

        if (dto.RaisedAt == default)
            throw new ArgumentException("Raised date is required.");

        _mapper.Map(dto, alert);

        alert.AlertNumber = alertNumber;
        alert.Severity = dto.Severity.Trim();
        alert.Status = dto.Status.Trim();
        alert.Description = dto.Description.Trim();
        alert.ReviewNotes = dto.ReviewNotes?.Trim();
        alert.ActionTaken = dto.ActionTaken?.Trim();
        alert.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.FraudAlert.Update(alert);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<FraudAlertDto>(alert);
    }

    public async Task<bool> DeleteAsync(DeleteFraudAlertDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.FraudAlertId, nameof(dto.FraudAlertId));

        var alert = await _unitOfWork.FraudAlert
            .GetFraudAlertDetailsAsync(dto.FraudAlertId);

        if (alert == null)
            return false;

        if (alert.Status is "ConfirmedFraud" or "FalsePositive")
        {
            throw new InvalidOperationException(
                "Confirmed fraud and false-positive alerts must be preserved for audit purposes.");
        }

        if (alert.ComplianceRecords.Any())
        {
            throw new InvalidOperationException(
                "Fraud alerts associated with compliance records cannot be deleted.");
        }

        _unitOfWork.FraudAlert.Delete(alert);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. ALERT STATUS AND SEVERITY

    public async Task<IEnumerable<FraudAlertDto>> GetByStatusAsync(string status)
    {
        ValidateStatus(status);

        var alerts = await _unitOfWork.FraudAlert
            .GetByStatusAsync(status.Trim());

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetBySeverityAsync(string severity)
    {
        ValidateSeverity(severity);

        var alerts = await _unitOfWork.FraudAlert
            .GetBySeverityAsync(severity.Trim());

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetAlertsAboveScoreAsync(decimal minimumScore)
    {
        if (minimumScore < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumScore));

        var alerts = await _unitOfWork.FraudAlert
            .GetAlertsAboveScoreAsync(minimumScore);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetActiveFraudAlertsAsync()
    {
        var alerts = await _unitOfWork.FraudAlert
            .GetActiveFraudAlertsAsync();

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    // 3. BANKING ENTITY ASSOCIATIONS

    public async Task<IEnumerable<FraudAlertDto>> GetByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var alerts = await _unitOfWork.FraudAlert
            .GetByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var alerts = await _unitOfWork.FraudAlert
            .GetByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetByTransactionIdAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var alerts = await _unitOfWork.FraudAlert
            .GetByTransactionIdAsync(transactionId);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetByCardIdAsync(int cardId)
    {
        ValidateId(cardId, nameof(cardId));

        var alerts = await _unitOfWork.FraudAlert
            .GetByCardIdAsync(cardId);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetByFraudRuleIdAsync(int fraudRuleId)
    {
        ValidateId(fraudRuleId, nameof(fraudRuleId));

        var alerts = await _unitOfWork.FraudAlert
            .GetByFraudRuleIdAsync(fraudRuleId);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetByRiskAssessmentIdAsync(int riskAssessmentId)
    {
        ValidateId(riskAssessmentId, nameof(riskAssessmentId));

        var alerts = await _unitOfWork.FraudAlert
            .GetByRiskAssessmentIdAsync(riskAssessmentId);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    // 4. INVESTIGATION ASSIGNMENT

    public async Task<IEnumerable<FraudAlertDto>> GetByReviewedByEmployeeIdAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));

        var alerts = await _unitOfWork.FraudAlert
            .GetByReviewedByEmployeeIdAsync(employeeId);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetUnassignedFraudAlertsAsync()
    {
        var alerts = await _unitOfWork.FraudAlert
            .GetUnassignedFraudAlertsAsync();

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    // 5. INVESTIGATION OUTCOMES

    public async Task<IEnumerable<FraudAlertDto>> GetConfirmedFraudAlertsAsync()
    {
        var alerts = await _unitOfWork.FraudAlert
            .GetConfirmedFraudAlertsAsync();

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetFalsePositiveAlertsAsync()
    {
        var alerts = await _unitOfWork.FraudAlert
            .GetFalsePositiveAlertsAsync();

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetAlertsWithComplianceRecordsAsync()
    {
        var alerts = await _unitOfWork.FraudAlert
            .GetAlertsWithComplianceRecordsAsync();

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    // 6. ALERT TIMELINE AND SEARCH

    public async Task<IEnumerable<FraudAlertDto>> GetAlertsRaisedWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var alerts = await _unitOfWork.FraudAlert
            .GetAlertsRaisedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> GetAlertsReviewedWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var alerts = await _unitOfWork.FraudAlert
            .GetAlertsReviewedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    public async Task<IEnumerable<FraudAlertDto>> SearchFraudAlertsAsync(string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm), 200);

        var alerts = await _unitOfWork.FraudAlert
            .SearchFraudAlertsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<FraudAlertDto>>(alerts);
    }

    // 7. REPORTING

    public async Task<int> CountAlertsByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        return await _unitOfWork.FraudAlert
            .CountAlertsByCustomerIdAsync(customerId);
    }

    // VALIDATION HELPERS

    private async Task ValidateReferencesAsync(
        int fraudRuleId,
        int? transactionId,
        int? customerId,
        int? accountId,
        int? cardId,
        int? riskAssessmentId)
    {
        if (await _unitOfWork.FraudRule.GetByIdAsync(fraudRuleId) == null)
            throw new InvalidOperationException("Fraud rule does not exist.");

        if (transactionId.HasValue &&
            await _unitOfWork.Transaction.GetByIdAsync(transactionId.Value) == null)
            throw new InvalidOperationException("Transaction does not exist.");

        if (customerId.HasValue &&
            await _unitOfWork.Customer.GetByIdAsync(customerId.Value) == null)
            throw new InvalidOperationException("Customer does not exist.");

        if (accountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(accountId.Value) == null)
            throw new InvalidOperationException("Account does not exist.");

        if (cardId.HasValue &&
            await _unitOfWork.Card.GetByIdAsync(cardId.Value) == null)
            throw new InvalidOperationException("Card does not exist.");

        if (riskAssessmentId.HasValue &&
            await _unitOfWork.RiskAssessment.GetByIdAsync(riskAssessmentId.Value) == null)
            throw new InvalidOperationException("Risk assessment does not exist.");
    }

    private static void ValidateAlert(
        string alertNumber,
        string severity,
        string description,
        string status,
        decimal? score,
        DateTime raisedAt,
        DateTime? reviewedAt,
        string? reviewNotes,
        string? actionTaken)
    {
        ValidateRequiredString(alertNumber, nameof(alertNumber), 30);
        ValidateSeverity(severity);
        ValidateStatus(status);
        ValidateRequiredString(description, nameof(description), 500);
        ValidateOptionalString(reviewNotes, nameof(reviewNotes), 1000);
        ValidateOptionalString(actionTaken, nameof(actionTaken), 100);

        if (score < 0)
            throw new ArgumentOutOfRangeException(nameof(score));

        if (reviewedAt.HasValue && reviewedAt.Value == default)
            throw new ArgumentException("Invalid review date.");

        if (raisedAt != default && reviewedAt.HasValue &&
            reviewedAt.Value < raisedAt)
        {
            throw new ArgumentException(
                "Review date cannot be earlier than the alert raised date.");
        }
    }

    private static void ValidateReviewInformation(
        string status,
        DateTime? reviewedAt,
        string? reviewNotes,
        string? actionTaken,
        int? reviewedByEmployeeId)
    {
        if (reviewedByEmployeeId.HasValue)
            ValidateId(reviewedByEmployeeId.Value, nameof(reviewedByEmployeeId));

        if (status == "Open" &&
            (reviewedAt.HasValue || reviewedByEmployeeId.HasValue))
        {
            throw new ArgumentException(
                "An open alert cannot contain review assignment details.");
        }

        if (status is "UnderReview" or "ConfirmedFraud" or "FalsePositive" or "Closed")
        {
            if (!reviewedAt.HasValue)
                throw new ArgumentException(
                    "Review date is required for this alert status.");

            if (string.IsNullOrWhiteSpace(reviewNotes))
                throw new ArgumentException(
                    "Review notes are required for reviewed alerts.");

            if (string.IsNullOrWhiteSpace(actionTaken))
                throw new ArgumentException(
                    "Action taken is required for reviewed alerts.");
        }
    }

    private static void ValidateSeverity(string severity)
    {
        ValidateRequiredString(severity, nameof(severity), 10);

        if (!ValidSeverities.Contains(severity.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid fraud alert severity.");
    }

    private static void ValidateStatus(string status)
    {
        ValidateRequiredString(status, nameof(status), 20);

        if (!ValidStatuses.Contains(status.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid fraud alert status.");
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

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Valid start and end dates are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }
}
