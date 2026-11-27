
using AutoMapper;
using FinVault.Application.DTOs.RiskAssessmentDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FinVault.Application.Services;

public class RiskAssessmentService : IRiskAssessmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly HashSet<string> AssessmentTypes =
        new(StringComparer.Ordinal) { "Customer", "Account", "Transaction" };

    private static readonly HashSet<string> RiskLevels =
        new(StringComparer.Ordinal) { "Low", "Medium", "High", "Critical" };

    private static readonly HashSet<string> Decisions =
        new(StringComparer.Ordinal) { "Approve", "Review", "Challenge", "Decline" };

    private static readonly HashSet<string> SensitivePropertyNames =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "pan", "pannumber", "aadhaar", "aadharnumber", "aadharnumber",
            "cardnumber", "primaryaccountnumber", "cvv", "cvc",
            "track1", "track2", "fullcardnumber"
        };

    public RiskAssessmentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. RISK ASSESSMENT IDENTIFICATION

    public async Task<IEnumerable<RiskAssessmentDto>> GetAllAsync()
    {
        var assessments = await _unitOfWork.RiskAssessment.GetAllAsync();
        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<RiskAssessmentDto?> GetByIdAsync(int riskAssessmentId)
    {
        if (riskAssessmentId <= 0) return null;

        var assessment = await _unitOfWork.RiskAssessment.GetByIdAsync(riskAssessmentId);

        return assessment == null
            ? null
            : _mapper.Map<RiskAssessmentDto>(assessment);
    }

    public async Task<RiskAssessmentDto?> GetRiskAssessmentDetailsAsync(int riskAssessmentId)
    {
        if (riskAssessmentId <= 0) return null;

        var assessment = await _unitOfWork.RiskAssessment
            .GetRiskAssessmentDetailsAsync(riskAssessmentId);

        return assessment == null
            ? null
            : _mapper.Map<RiskAssessmentDto>(assessment);
    }

    public async Task<RiskAssessmentDto?> GetByIdWithSubjectAsync(int riskAssessmentId)
    {
        if (riskAssessmentId <= 0) return null;

        var assessment = await _unitOfWork.RiskAssessment
            .GetByIdWithSubjectAsync(riskAssessmentId);

        return assessment == null
            ? null
            : _mapper.Map<RiskAssessmentDto>(assessment);
    }

    public async Task<RiskAssessmentDto> CreateAsync(CreateRiskAssessmentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateAssessment(
            dto.AssessmentType,
            dto.RiskScore,
            dto.RiskLevel,
            dto.Decision,
            dto.ModelVersion,
            dto.FactorsJson,
            dto.AssessedAt,
            dto.ExpiresAt,
            dto.CustomerId,
            dto.AccountId,
            dto.TransactionId);

        await ValidateSubjectAsync(
            dto.AssessmentType,
            dto.CustomerId,
            dto.AccountId,
            dto.TransactionId);

        var assessment = _mapper.Map<RiskAssessment>(dto);

        assessment.AssessmentType = dto.AssessmentType;
        assessment.ModelVersion = dto.ModelVersion.Trim();
        assessment.CreatedAt = DateTime.UtcNow;
        assessment.UpdatedAt = null;
        assessment.CreatedBy = null;
        assessment.UpdatedBy = null;

        await _unitOfWork.RiskAssessment.AddAsync(assessment);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RiskAssessmentDto>(assessment);
    }

    public async Task<RiskAssessmentDto?> UpdateAsync(UpdateRiskAssessmentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.RiskAssessmentId <= 0) return null;

        var existingAssessment = await _unitOfWork.RiskAssessment
            .GetByIdAsync(dto.RiskAssessmentId);

        if (existingAssessment == null) return null;

        // Risk assessments are historical evaluation records.
        // Create a new assessment instead of overwriting an existing evaluation.
        throw new InvalidOperationException(
            "Risk assessments are immutable historical records. Create a new assessment instead of updating an existing assessment.");
    }

    public async Task<bool> DeleteAsync(DeleteRiskAssessmentDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.RiskAssessmentId <= 0) return false;

        var assessment = await _unitOfWork.RiskAssessment
            .GetByIdAsync(dto.RiskAssessmentId);

        if (assessment == null) return false;

        // Preserve risk assessment history for audit and investigation.
        throw new InvalidOperationException(
            "Risk assessments cannot be deleted because they are required for historical audit and investigation.");
    }

    // 2. CUSTOMER ASSESSMENTS

    public async Task<IEnumerable<RiskAssessmentDto>> GetByCustomerIdAsync(int customerId)
    {
        if (customerId <= 0) return Enumerable.Empty<RiskAssessmentDto>();

        var assessments = await _unitOfWork.RiskAssessment
            .GetByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<RiskAssessmentDto?> GetLatestByCustomerIdAsync(int customerId)
    {
        if (customerId <= 0) return null;

        var assessment = await _unitOfWork.RiskAssessment
            .GetLatestByCustomerIdAsync(customerId);

        return assessment == null
            ? null
            : _mapper.Map<RiskAssessmentDto>(assessment);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetCustomerAssessmentsWithinRangeAsync(
        int customerId,
        DateTime startDate,
        DateTime endDate)
    {
        if (customerId <= 0) return Enumerable.Empty<RiskAssessmentDto>();

        ValidateDateRange(startDate, endDate);

        var assessments = await _unitOfWork.RiskAssessment
            .GetCustomerAssessmentsWithinRangeAsync(customerId, startDate, endDate);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    // 3. ACCOUNT ASSESSMENTS

    public async Task<IEnumerable<RiskAssessmentDto>> GetByAccountIdAsync(int accountId)
    {
        if (accountId <= 0) return Enumerable.Empty<RiskAssessmentDto>();

        var assessments = await _unitOfWork.RiskAssessment
            .GetByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<RiskAssessmentDto?> GetLatestByAccountIdAsync(int accountId)
    {
        if (accountId <= 0) return null;

        var assessment = await _unitOfWork.RiskAssessment
            .GetLatestByAccountIdAsync(accountId);

        return assessment == null
            ? null
            : _mapper.Map<RiskAssessmentDto>(assessment);
    }

    // 4. TRANSACTION ASSESSMENTS

    public async Task<IEnumerable<RiskAssessmentDto>> GetByTransactionIdAsync(int transactionId)
    {
        if (transactionId <= 0) return Enumerable.Empty<RiskAssessmentDto>();

        var assessments = await _unitOfWork.RiskAssessment
            .GetByTransactionIdAsync(transactionId);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<RiskAssessmentDto?> GetLatestByTransactionIdAsync(int transactionId)
    {
        if (transactionId <= 0) return null;

        var assessment = await _unitOfWork.RiskAssessment
            .GetLatestByTransactionIdAsync(transactionId);

        return assessment == null
            ? null
            : _mapper.Map<RiskAssessmentDto>(assessment);
    }

    // 5. ASSESSMENT TYPE AND RISK LEVEL

    public async Task<IEnumerable<RiskAssessmentDto>> GetByAssessmentTypeAsync(string assessmentType)
    {
        if (!AssessmentTypes.Contains(assessmentType ?? string.Empty))
            throw new ArgumentException("Assessment type must be Customer, Account or Transaction.");

        var assessments = await _unitOfWork.RiskAssessment
            .GetByAssessmentTypeAsync(assessmentType);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetByRiskLevelAsync(string riskLevel)
    {
        if (!RiskLevels.Contains(riskLevel ?? string.Empty))
            throw new ArgumentException("Risk level must be Low, Medium, High or Critical.");

        var assessments = await _unitOfWork.RiskAssessment
            .GetByRiskLevelAsync(riskLevel);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetByDecisionAsync(string decision)
    {
        if (!Decisions.Contains(decision ?? string.Empty))
            throw new ArgumentException("Decision must be Approve, Review, Challenge or Decline.");

        var assessments = await _unitOfWork.RiskAssessment
            .GetByDecisionAsync(decision);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetHighAndCriticalRiskAssessmentsAsync()
    {
        var assessments = await _unitOfWork.RiskAssessment
            .GetHighAndCriticalRiskAssessmentsAsync();

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetByMinimumRiskScoreAsync(decimal minimumRiskScore)
    {
        if (minimumRiskScore < 0 || minimumRiskScore > 100)
            throw new ArgumentOutOfRangeException(
                nameof(minimumRiskScore),
                "Minimum risk score must be between 0 and 100.");

        var assessments = await _unitOfWork.RiskAssessment
            .GetByMinimumRiskScoreAsync(minimumRiskScore);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    // 6. MODEL AND VALIDITY

    public async Task<IEnumerable<RiskAssessmentDto>> GetByModelVersionAsync(string modelVersion)
    {
        if (string.IsNullOrWhiteSpace(modelVersion))
            throw new ArgumentException("Model version is required.");

        var assessments = await _unitOfWork.RiskAssessment
            .GetByModelVersionAsync(modelVersion.Trim());

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetNonExpiredAssessmentsAsync(
        DateTime currentDateTime)
    {
        if (currentDateTime == default)
            throw new ArgumentException("A valid current date and time is required.");

        var assessments = await _unitOfWork.RiskAssessment
            .GetNonExpiredAssessmentsAsync(currentDateTime);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetExpiredAssessmentsAsync(
        DateTime currentDateTime)
    {
        if (currentDateTime == default)
            throw new ArgumentException("A valid current date and time is required.");

        var assessments = await _unitOfWork.RiskAssessment
            .GetExpiredAssessmentsAsync(currentDateTime);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    // 7. DATE AND AUDIT

    public async Task<IEnumerable<RiskAssessmentDto>> GetAssessedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var assessments = await _unitOfWork.RiskAssessment
            .GetAssessedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetByCreatedByAsync(int userId)
    {
        if (userId <= 0) return Enumerable.Empty<RiskAssessmentDto>();

        var assessments = await _unitOfWork.RiskAssessment
            .GetByCreatedByAsync(userId);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> GetByUpdatedByAsync(int userId)
    {
        if (userId <= 0) return Enumerable.Empty<RiskAssessmentDto>();

        var assessments = await _unitOfWork.RiskAssessment
            .GetByUpdatedByAsync(userId);

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    // 8. FRAUD ALERTS AND SEARCH

    public async Task<IEnumerable<RiskAssessmentDto>> GetAssessmentsWithFraudAlertsAsync()
    {
        var assessments = await _unitOfWork.RiskAssessment
            .GetAssessmentsWithFraudAlertsAsync();

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    public async Task<RiskAssessmentDto?> GetAssessmentWithFraudAlertsAsync(int riskAssessmentId)
    {
        if (riskAssessmentId <= 0) return null;

        var assessment = await _unitOfWork.RiskAssessment
            .GetAssessmentWithFraudAlertsAsync(riskAssessmentId);

        return assessment == null
            ? null
            : _mapper.Map<RiskAssessmentDto>(assessment);
    }

    public async Task<IEnumerable<RiskAssessmentDto>> SearchRiskAssessmentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<RiskAssessmentDto>();

        var assessments = await _unitOfWork.RiskAssessment
            .SearchRiskAssessmentsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<RiskAssessmentDto>>(assessments);
    }

    // 9. REPORTING

    public Task<int> CountByRiskLevelWithinRangeAsync(
        string riskLevel,
        DateTime startDate,
        DateTime endDate)
    {
        if (!RiskLevels.Contains(riskLevel ?? string.Empty))
            throw new ArgumentException("Risk level must be Low, Medium, High or Critical.");

        ValidateDateRange(startDate, endDate);

        return _unitOfWork.RiskAssessment
            .CountByRiskLevelWithinRangeAsync(riskLevel, startDate, endDate);
    }

    public Task<decimal> GetAverageRiskScoreByAssessmentTypeAsync(string assessmentType)
    {
        if (!AssessmentTypes.Contains(assessmentType ?? string.Empty))
            throw new ArgumentException("Assessment type must be Customer, Account or Transaction.");

        return _unitOfWork.RiskAssessment
            .GetAverageRiskScoreByAssessmentTypeAsync(assessmentType);
    }

    // PRIVATE VALIDATION METHODS

    private async Task ValidateSubjectAsync(
        string assessmentType,
        int? customerId,
        int? accountId,
        int? transactionId)
    {
        switch (assessmentType)
        {
            case "Customer":
                if (!customerId.HasValue || accountId.HasValue || transactionId.HasValue)
                    throw new ArgumentException(
                        "A Customer assessment must contain only CustomerId.");

                if (await _unitOfWork.Customer.GetByIdAsync(customerId.Value) == null)
                    throw new InvalidOperationException("The specified customer does not exist.");

                break;

            case "Account":
                if (!accountId.HasValue || customerId.HasValue || transactionId.HasValue)
                    throw new ArgumentException(
                        "An Account assessment must contain only AccountId.");

                if (await _unitOfWork.Account.GetByIdAsync(accountId.Value) == null)
                    throw new InvalidOperationException("The specified account does not exist.");

                break;

            case "Transaction":
                if (!transactionId.HasValue || customerId.HasValue || accountId.HasValue)
                    throw new ArgumentException(
                        "A Transaction assessment must contain only TransactionId.");

                if (await _unitOfWork.Transaction.GetByIdAsync(transactionId.Value) == null)
                    throw new InvalidOperationException("The specified transaction does not exist.");

                break;

            default:
                throw new ArgumentException("Unsupported assessment type.");
        }
    }

    private static void ValidateAssessment(
        string assessmentType,
        decimal riskScore,
        string riskLevel,
        string decision,
        string modelVersion,
        string? factorsJson,
        DateTime assessedAt,
        DateTime? expiresAt,
        int? customerId,
        int? accountId,
        int? transactionId)
    {
        if (!AssessmentTypes.Contains(assessmentType ?? string.Empty))
            throw new ArgumentException("Assessment type must be Customer, Account or Transaction.");

        if (riskScore < 0 || riskScore > 100)
            throw new ArgumentOutOfRangeException(
                nameof(riskScore),
                "Risk score must be between 0 and 100.");

        if (!RiskLevels.Contains(riskLevel ?? string.Empty))
            throw new ArgumentException("Risk level must be Low, Medium, High or Critical.");

        if (!Decisions.Contains(decision ?? string.Empty))
            throw new ArgumentException("Decision must be Approve, Review, Challenge or Decline.");

        if (string.IsNullOrWhiteSpace(modelVersion))
            throw new ArgumentException("Model version is required.");

        if (modelVersion.Trim().Length > 30)
            throw new ArgumentException("Model version cannot exceed 30 characters.");

        if (assessedAt == default)
            throw new ArgumentException("Assessment date and time are required.");

        if (expiresAt.HasValue && expiresAt.Value <= assessedAt)
            throw new ArgumentException("Expiry date must be later than assessment date.");

        if (customerId.HasValue && customerId.Value <= 0)
            throw new ArgumentException("CustomerId must be greater than zero.");

        if (accountId.HasValue && accountId.Value <= 0)
            throw new ArgumentException("AccountId must be greater than zero.");

        if (transactionId.HasValue && transactionId.Value <= 0)
            throw new ArgumentException("TransactionId must be greater than zero.");

        ValidateFactorsJson(factorsJson);
    }

    private static void ValidateFactorsJson(string? factorsJson)
    {
        if (string.IsNullOrWhiteSpace(factorsJson))
            return;

        try
        {
            using var document = JsonDocument.Parse(factorsJson);
            ValidateJsonElement(document.RootElement);
        }
        catch (JsonException)
        {
            throw new ArgumentException("FactorsJson must contain valid JSON.");
        }
    }

    private static void ValidateJsonElement(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var normalizedName = Regex.Replace(
                        property.Name,
                        "[^a-zA-Z0-9]",
                        string.Empty).ToLowerInvariant();

                    if (SensitivePropertyNames.Contains(normalizedName))
                        throw new ArgumentException(
                            "FactorsJson must not contain raw PAN, Aadhaar or card information.");

                    ValidateJsonElement(property.Value);
                }

                break;

            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                    ValidateJsonElement(item);

                break;

            case JsonValueKind.String:
                ValidateSensitiveValue(element.GetString());
                break;

            case JsonValueKind.Number:
                ValidateSensitiveValue(element.GetRawText());
                break;
        }
    }

    private static void ValidateSensitiveValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        var normalizedValue = value.Trim();

        var containsPan = Regex.IsMatch(
            normalizedValue,
            @"\b[A-Z]{5}[0-9]{4}[A-Z]\b",
            RegexOptions.IgnoreCase);

        var containsAadhaar = Regex.IsMatch(
            normalizedValue,
            @"(?<!\d)(?:\d[\s-]?){11}\d(?!\d)");

        var digitsOnly = Regex.Replace(normalizedValue, @"\D", string.Empty);

        var containsCardNumber =
            digitsOnly.Length >= 13 &&
            digitsOnly.Length <= 19 &&
            normalizedValue.Length <= 25;

        if (containsPan || containsAadhaar || containsCardNumber)
            throw new ArgumentException(
                "FactorsJson must not contain raw PAN, Aadhaar or card information.");
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default || startDate > endDate)
            throw new ArgumentException("A valid date range is required.");
    }
}
