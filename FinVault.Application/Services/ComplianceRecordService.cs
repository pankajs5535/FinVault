
using AutoMapper;
using FinVault.Application.DTOs.ComplianceRecordDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class ComplianceRecordService : IComplianceRecordService
{
    private static readonly string[] ValidRecordTypes =
    {
        "Str", "Ctr", "Ccr", "Ntr", "AmlScreening",
        "PepCheck", "SanctionsScreening", "PeriodicKycReview", "FatcaCrs"
    };

    private static readonly string[] ValidRiskLevels =
    {
        "Low", "Medium", "High", "Critical"
    };

    private static readonly string[] ValidStatuses =
    {
        "Open", "UnderReview", "Filed", "Cleared", "Closed"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ComplianceRecordService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<ComplianceRecordDto>> GetAllAsync()
    {
        var records = await _unitOfWork.ComplianceRecord.GetAllAsync();
        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<ComplianceRecordDto?> GetByIdAsync(int complianceRecordId)
    {
        ValidateId(complianceRecordId, nameof(complianceRecordId));

        var record = await _unitOfWork.ComplianceRecord
            .GetByIdAsync(complianceRecordId);

        return _mapper.Map<ComplianceRecordDto?>(record);
    }

    public async Task<ComplianceRecordDto?> GetByReferenceNumberAsync(string referenceNumber)
    {
        ValidateRequiredString(referenceNumber, nameof(referenceNumber), 40);

        var record = await _unitOfWork.ComplianceRecord
            .GetByReferenceNumberAsync(referenceNumber.Trim());

        return _mapper.Map<ComplianceRecordDto?>(record);
    }

    public async Task<ComplianceRecordDto?> GetComplianceRecordDetailsAsync(int complianceRecordId)
    {
        ValidateId(complianceRecordId, nameof(complianceRecordId));

        var record = await _unitOfWork.ComplianceRecord
            .GetComplianceRecordDetailsAsync(complianceRecordId);

        return _mapper.Map<ComplianceRecordDto?>(record);
    }

    public async Task<ComplianceRecordDto> CreateAsync(CreateComplianceRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRecord(dto.RecordType, dto.RiskLevel, dto.Status);
        ValidateRecordDates(dto.DueDate, dto.FiledAt);
        ValidateRequiredString(dto.ReferenceNumber, nameof(dto.ReferenceNumber), 40);
        ValidateRequiredString(dto.Description, nameof(dto.Description), 1000);
        ValidateRequiredString(dto.ReportingAuthority, nameof(dto.ReportingAuthority), 50);
        ValidateOptionalString(dto.FilingReference, nameof(dto.FilingReference), 50);

        var referenceNumber = dto.ReferenceNumber.Trim();

        if (await _unitOfWork.ComplianceRecord
            .ExistsByReferenceNumberAsync(referenceNumber))
        {
            throw new InvalidOperationException(
                "A compliance record with this reference number already exists.");
        }

        await ValidateForeignKeysAsync(
            dto.CustomerId,
            dto.AccountId,
            dto.TransactionId,
            dto.FraudAlertId,
            dto.FiledByEmployeeId);

        if (dto.Status == "Filed" && !dto.FiledAt.HasValue)
            throw new ArgumentException("Filed date is required for filed records.");

        var record = _mapper.Map<ComplianceRecord>(dto);

        record.ReferenceNumber = referenceNumber;
        record.RecordType = dto.RecordType.Trim();
        record.RiskLevel = dto.RiskLevel.Trim();
        record.Status = dto.Status.Trim();
        record.Description = dto.Description.Trim();
        record.ReportingAuthority = dto.ReportingAuthority.Trim();
        record.FilingReference = dto.FilingReference?.Trim();
        record.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.ComplianceRecord.AddAsync(record);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ComplianceRecordDto>(record);
    }

    public async Task<ComplianceRecordDto?> UpdateAsync(UpdateComplianceRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.ComplianceRecordId, nameof(dto.ComplianceRecordId));
        ValidateRecord(dto.RecordType, dto.RiskLevel, dto.Status);
        ValidateRecordDates(dto.DueDate, dto.FiledAt);
        ValidateRequiredString(dto.ReferenceNumber, nameof(dto.ReferenceNumber), 40);
        ValidateRequiredString(dto.Description, nameof(dto.Description), 1000);
        ValidateRequiredString(dto.ReportingAuthority, nameof(dto.ReportingAuthority), 50);
        ValidateOptionalString(dto.FilingReference, nameof(dto.FilingReference), 50);

        var record = await _unitOfWork.ComplianceRecord
            .GetByIdAsync(dto.ComplianceRecordId);

        if (record == null)
            return null;

        // Filed regulatory records cannot be directly modified.
        if (record.Status == "Filed")
            throw new InvalidOperationException(
                "Filed compliance records cannot be directly modified. Use a controlled correction procedure.");

        if (await _unitOfWork.ComplianceRecord.ExistsByReferenceNumberAsync(
            dto.ReferenceNumber.Trim()) &&
            !string.Equals(record.ReferenceNumber, dto.ReferenceNumber.Trim(),
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "A compliance record with this reference number already exists.");
        }

        await ValidateForeignKeysAsync(
            dto.CustomerId,
            dto.AccountId,
            dto.TransactionId,
            dto.FraudAlertId,
            dto.FiledByEmployeeId);

        if (dto.Status == "Filed" && !dto.FiledAt.HasValue)
            throw new ArgumentException("Filed date is required for filed records.");

        _mapper.Map(dto, record);

        record.ReferenceNumber = dto.ReferenceNumber.Trim();
        record.RecordType = dto.RecordType.Trim();
        record.RiskLevel = dto.RiskLevel.Trim();
        record.Status = dto.Status.Trim();
        record.Description = dto.Description.Trim();
        record.ReportingAuthority = dto.ReportingAuthority.Trim();
        record.FilingReference = dto.FilingReference?.Trim();
        record.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ComplianceRecord.Update(record);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ComplianceRecordDto>(record);
    }

    public async Task<bool> DeleteAsync(DeleteComplianceRecordDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.ComplianceRecordId, nameof(dto.ComplianceRecordId));

        var record = await _unitOfWork.ComplianceRecord
            .GetByIdAsync(dto.ComplianceRecordId);

        if (record == null)
            return false;

        if (record.Status == "Filed")
            throw new InvalidOperationException(
                "Filed regulatory records cannot be deleted.");

        _unitOfWork.ComplianceRecord.Delete(record);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ExistsByReferenceNumberAsync(string referenceNumber)
    {
        ValidateRequiredString(referenceNumber, nameof(referenceNumber), 40);

        return await _unitOfWork.ComplianceRecord
            .ExistsByReferenceNumberAsync(referenceNumber.Trim());
    }

    // 2. CUSTOMER AND BANKING RECORD RELATIONSHIPS

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByTransactionIdAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByTransactionIdAsync(transactionId);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByFraudAlertIdAsync(int fraudAlertId)
    {
        ValidateId(fraudAlertId, nameof(fraudAlertId));

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByFraudAlertIdAsync(fraudAlertId);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    // 3. RECORD TYPE AND RISK CLASSIFICATION

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByTypeAsync(string recordType)
    {
        ValidateRecordType(recordType);

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByTypeAsync(recordType.Trim());

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByRiskLevelAsync(string riskLevel)
    {
        ValidateRiskLevel(riskLevel);

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByRiskLevelAsync(riskLevel.Trim());

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByStatusAsync(string status)
    {
        ValidateStatus(status);

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByStatusAsync(status.Trim());

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    // 4. REVIEW AND FILING MANAGEMENT

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsByFiledByEmployeeIdAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsByFiledByEmployeeIdAsync(employeeId);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetOverdueRecordsAsync(DateTime currentDate)
    {
        if (currentDate == default)
            throw new ArgumentException("Current date is required.", nameof(currentDate));

        var records = await _unitOfWork.ComplianceRecord
            .GetOverdueRecordsAsync(currentDate);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetRecordsDueWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var records = await _unitOfWork.ComplianceRecord
            .GetRecordsDueWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<IEnumerable<ComplianceRecordDto>> GetFiledRecordsWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var records = await _unitOfWork.ComplianceRecord
            .GetFiledRecordsWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    // 5. COMPLIANCE SEARCH AND REPORTING

    public async Task<IEnumerable<ComplianceRecordDto>> SearchComplianceRecordsAsync(
        string? searchTerm,
        string? recordType,
        string? status,
        string? riskLevel,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0 || pageSize > 500)
            throw new ArgumentOutOfRangeException(nameof(pageSize),
                "Page size must be between 1 and 500.");

        if (startDate.HasValue && endDate.HasValue &&
            endDate.Value < startDate.Value)
        {
            throw new ArgumentException("End date cannot be earlier than start date.");
        }

        if (!string.IsNullOrWhiteSpace(recordType))
            ValidateRecordType(recordType);

        if (!string.IsNullOrWhiteSpace(status))
            ValidateStatus(status);

        if (!string.IsNullOrWhiteSpace(riskLevel))
            ValidateRiskLevel(riskLevel);

        var records = await _unitOfWork.ComplianceRecord
            .SearchComplianceRecordsAsync(
                searchTerm?.Trim(),
                recordType?.Trim(),
                status?.Trim(),
                riskLevel?.Trim(),
                startDate,
                endDate,
                pageNumber,
                pageSize);

        return _mapper.Map<IEnumerable<ComplianceRecordDto>>(records);
    }

    public async Task<int> CountRecordsByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        return await _unitOfWork.ComplianceRecord
            .CountRecordsByCustomerIdAsync(customerId);
    }

    // VALIDATION HELPERS

    private async Task ValidateForeignKeysAsync(
        int? customerId,
        int? accountId,
        int? transactionId,
        int? fraudAlertId,
        int? employeeId)
    {
        if (customerId.HasValue &&
            await _unitOfWork.Customer.GetByIdAsync(customerId.Value) == null)
            throw new InvalidOperationException("Customer does not exist.");

        if (accountId.HasValue &&
            await _unitOfWork.Account.GetByIdAsync(accountId.Value) == null)
            throw new InvalidOperationException("Account does not exist.");

        if (transactionId.HasValue &&
            await _unitOfWork.Transaction.GetByIdAsync(transactionId.Value) == null)
            throw new InvalidOperationException("Transaction does not exist.");

        if (fraudAlertId.HasValue &&
            await _unitOfWork.FraudAlert.GetByIdAsync(fraudAlertId.Value) == null)
            throw new InvalidOperationException("Fraud alert does not exist.");

    }

    private static void ValidateRecord(
        string recordType, string riskLevel, string status)
    {
        ValidateRecordType(recordType);
        ValidateRiskLevel(riskLevel);
        ValidateStatus(status);
    }

    private static void ValidateRecordType(string value)
    {
        ValidateRequiredString(value, nameof(value), 30);

        if (!ValidRecordTypes.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid compliance record type.", nameof(value));
    }

    private static void ValidateRiskLevel(string value)
    {
        ValidateRequiredString(value, nameof(value), 10);

        if (!ValidRiskLevels.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid compliance risk level.", nameof(value));
    }

    private static void ValidateStatus(string value)
    {
        ValidateRequiredString(value, nameof(value), 20);

        if (!ValidStatuses.Contains(value.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid compliance status.", nameof(value));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(parameterName,
                "ID must be greater than zero.");
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

    private static void ValidateRecordDates(DateTime? dueDate, DateTime? filedAt)
    {
        if (dueDate.HasValue && dueDate.Value == default)
            throw new ArgumentException("Invalid due date.");

        if (filedAt.HasValue && filedAt.Value == default)
            throw new ArgumentException("Invalid filing date.");

        if (dueDate.HasValue && filedAt.HasValue &&
            filedAt.Value < dueDate.Value)
        {
            // Late filing is allowed; no restriction is applied.
        }
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Valid start and end dates are required.");

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");
    }
}
