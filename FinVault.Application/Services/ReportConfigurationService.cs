
using System.Text.Json;
using AutoMapper;
using FinVault.Application.DTOs.ReportConfigurationDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class ReportConfigurationService : IReportConfigurationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly string[] ValidReportTypes =
    {
        "Statement", "Regulatory", "Operational", "Financial", "Analytics"
    };

    private static readonly string[] ValidOutputFormats =
    {
        "Pdf", "Excel", "Csv", "Json"
    };

    public ReportConfigurationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. REPORT IDENTIFICATION

    public async Task<IEnumerable<ReportConfigurationDto>> GetAllAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration.GetAllAsync();
        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<ReportConfigurationDto?> GetByIdAsync(int reportConfigurationId)
    {
        if (reportConfigurationId <= 0)
            return null;

        var report = await _unitOfWork.ReportConfiguration
            .GetByIdAsync(reportConfigurationId);

        return report == null ? null : _mapper.Map<ReportConfigurationDto>(report);
    }

    public async Task<ReportConfigurationDto?> GetByReportCodeAsync(string reportCode)
    {
        ValidateRequiredText(reportCode, nameof(reportCode), 50);

        var report = await _unitOfWork.ReportConfiguration
            .GetByReportCodeAsync(reportCode.Trim());

        return report == null ? null : _mapper.Map<ReportConfigurationDto>(report);
    }

    public Task<bool> ExistsByReportCodeAsync(string reportCode)
    {
        ValidateRequiredText(reportCode, nameof(reportCode), 50);

        return _unitOfWork.ReportConfiguration
            .ExistsByReportCodeAsync(reportCode.Trim());
    }

    // 2. REPORT CRUD AND DETAILS

    public async Task<ReportConfigurationDto?> GetReportConfigurationDetailsAsync(
        int reportConfigurationId)
    {
        if (reportConfigurationId <= 0)
            return null;

        var report = await _unitOfWork.ReportConfiguration
            .GetReportConfigurationDetailsAsync(reportConfigurationId);

        return report == null ? null : _mapper.Map<ReportConfigurationDto>(report);
    }

    public async Task<IEnumerable<ReportConfigurationDto>> GetAllWithOwnersAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetAllWithOwnersAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<ReportConfigurationDto> CreateAsync(
        CreateReportConfigurationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateReport(dto);

        var reportCode = dto.ReportCode.Trim();

        if (await _unitOfWork.ReportConfiguration
            .ExistsByReportCodeAsync(reportCode))
        {
            throw new InvalidOperationException(
                "A report configuration with this report code already exists.");
        }

        await ValidateOwnerAsync(dto.OwnerUserId);

        var report = _mapper.Map<ReportConfiguration>(dto);

        report.ReportCode = reportCode;
        report.Name = dto.Name.Trim();
        report.Description = NormalizeOptionalText(dto.Description);
        report.ReportType = dto.ReportType.Trim();
        report.OutputFormat = dto.OutputFormat.Trim();
        report.ScheduleCron = NormalizeOptionalText(dto.ScheduleCron);
        report.Recipients = NormalizeOptionalText(dto.Recipients);
        report.CreatedAt = DateTime.UtcNow;
        report.UpdatedAt = null;
        report.CreatedBy = null;
        report.UpdatedBy = null;

        await _unitOfWork.ReportConfiguration.AddAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReportConfigurationDto>(report);
    }

    public async Task<ReportConfigurationDto?> UpdateAsync(
        UpdateReportConfigurationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.ReportConfigurationId <= 0)
            return null;

        ValidateReport(dto);

        var report = await _unitOfWork.ReportConfiguration
            .GetByIdAsync(dto.ReportConfigurationId);

        if (report == null)
            return null;

        var reportCode = dto.ReportCode.Trim();

        var existingReport = await _unitOfWork.ReportConfiguration
            .GetByReportCodeAsync(reportCode);

        if (existingReport != null &&
            existingReport.ReportConfigurationId != dto.ReportConfigurationId)
        {
            throw new InvalidOperationException(
                "Another report configuration already uses this report code.");
        }

        await ValidateOwnerAsync(dto.OwnerUserId);

        _mapper.Map(dto, report);

        report.ReportCode = reportCode;
        report.Name = dto.Name.Trim();
        report.Description = NormalizeOptionalText(dto.Description);
        report.ReportType = dto.ReportType.Trim();
        report.OutputFormat = dto.OutputFormat.Trim();
        report.ScheduleCron = NormalizeOptionalText(dto.ScheduleCron);
        report.Recipients = NormalizeOptionalText(dto.Recipients);
        report.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ReportConfiguration.Update(report);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ReportConfigurationDto>(report);
    }

    public async Task<bool> DeleteAsync(DeleteReportConfigurationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.ReportConfigurationId <= 0)
            return false;

        var report = await _unitOfWork.ReportConfiguration
            .GetByIdAsync(dto.ReportConfigurationId);

        if (report == null)
            return false;

        _unitOfWork.ReportConfiguration.Delete(report);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. REPORT TYPE AND FORMAT

    public async Task<IEnumerable<ReportConfigurationDto>> GetByReportTypeAsync(
        string reportType)
    {
        ValidateAllowedValue(
            reportType,
            ValidReportTypes,
            nameof(reportType));

        var reports = await _unitOfWork.ReportConfiguration
            .GetByReportTypeAsync(reportType.Trim());

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>> GetByOutputFormatAsync(
        string outputFormat)
    {
        ValidateAllowedValue(
            outputFormat,
            ValidOutputFormats,
            nameof(outputFormat));

        var reports = await _unitOfWork.ReportConfiguration
            .GetByOutputFormatAsync(outputFormat.Trim());

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>> GetRegulatoryReportsAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetRegulatoryReportsAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    // 4. REPORT STATUS

    public async Task<IEnumerable<ReportConfigurationDto>> GetActiveReportsAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetActiveReportsAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>> GetInactiveReportsAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetInactiveReportsAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    // 5. SCHEDULED REPORTS

    public async Task<IEnumerable<ReportConfigurationDto>> GetScheduledReportsAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetScheduledReportsAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>> GetReportsDueForExecutionAsync(
        DateTime currentDateTime)
    {
        if (currentDateTime == default)
            throw new ArgumentException("Current date and time are required.");

        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsDueForExecutionAsync(currentDateTime);

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetScheduledReportsWithinRangeAsync(
            DateTime startDateTime,
            DateTime endDateTime)
    {
        ValidateDateRange(startDateTime, endDateTime);

        var reports = await _unitOfWork.ReportConfiguration
            .GetScheduledReportsWithinRangeAsync(startDateTime, endDateTime);

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetReportsPreviouslyExecutedAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsPreviouslyExecutedAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetReportsNeverExecutedAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsNeverExecutedAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    // 6. USER OWNERSHIP

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetReportsByOwnerUserIdAsync(int ownerUserId)
    {
        if (ownerUserId <= 0)
            return Enumerable.Empty<ReportConfigurationDto>();

        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsByOwnerUserIdAsync(ownerUserId);

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetReportsWithoutOwnerAsync()
    {
        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsWithoutOwnerAsync();

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    // 7. REPORT EXECUTION HISTORY

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetReportsLastRunWithinRangeAsync(
            DateTime startDateTime,
            DateTime endDateTime)
    {
        ValidateDateRange(startDateTime, endDateTime);

        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsLastRunWithinRangeAsync(startDateTime, endDateTime);

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetReportsWithNextRunBeforeAsync(DateTime dateTime)
    {
        if (dateTime == default)
            throw new ArgumentException("Date and time are required.");

        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsWithNextRunBeforeAsync(dateTime);

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    // 8. RETENTION AND SEARCH

    public async Task<IEnumerable<ReportConfigurationDto>>
        GetReportsByMinimumRetentionDaysAsync(int minimumRetentionDays)
    {
        if (minimumRetentionDays < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumRetentionDays));

        var reports = await _unitOfWork.ReportConfiguration
            .GetReportsByMinimumRetentionDaysAsync(minimumRetentionDays);

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    public async Task<IEnumerable<ReportConfigurationDto>>
        SearchReportConfigurationsAsync(string searchTerm)
    {
        ValidateRequiredText(searchTerm, nameof(searchTerm));

        var reports = await _unitOfWork.ReportConfiguration
            .SearchReportConfigurationsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<ReportConfigurationDto>>(reports);
    }

    // 9. REPORTING

    public Task<int> CountByReportTypeAsync(string reportType)
    {
        ValidateAllowedValue(
            reportType,
            ValidReportTypes,
            nameof(reportType));

        return _unitOfWork.ReportConfiguration
            .CountByReportTypeAsync(reportType.Trim());
    }

    // VALIDATION

    private async Task ValidateOwnerAsync(int? ownerUserId)
    {
        if (!ownerUserId.HasValue)
            return;

        if (ownerUserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(ownerUserId));

        if (await _unitOfWork.User.GetByIdAsync(ownerUserId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified report owner does not exist.");
        }
    }

    private static void ValidateReport(CreateReportConfigurationDto dto)
    {
        ValidateRequiredText(dto.ReportCode, nameof(dto.ReportCode), 50);
        ValidateRequiredText(dto.Name, nameof(dto.Name), 150);

        ValidateAllowedValue(
            dto.ReportType,
            ValidReportTypes,
            nameof(dto.ReportType));

        ValidateAllowedValue(
            dto.OutputFormat,
            ValidOutputFormats,
            nameof(dto.OutputFormat));

        ValidateOptionalText(dto.Description, nameof(dto.Description), 500);
        ValidateOptionalText(dto.ScheduleCron, nameof(dto.ScheduleCron), 50);
        ValidateOptionalText(dto.Recipients, nameof(dto.Recipients), 1000);

        if (dto.RetentionDays < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.RetentionDays));

        if (dto.OwnerUserId.HasValue && dto.OwnerUserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.OwnerUserId));

        if (dto.LastRunAt.HasValue && dto.LastRunAt.Value == default)
            throw new ArgumentException("LastRunAt is invalid.");

        if (dto.NextRunAt.HasValue && dto.NextRunAt.Value == default)
            throw new ArgumentException("NextRunAt is invalid.");

        if (dto.IsScheduled && string.IsNullOrWhiteSpace(dto.ScheduleCron))
        {
            throw new ArgumentException(
                "ScheduleCron is required for scheduled reports.");
        }

        ValidateJson(dto.ParametersJson, nameof(dto.ParametersJson));
    }

    private static void ValidateReport(UpdateReportConfigurationDto dto)
    {
        ValidateRequiredText(dto.ReportCode, nameof(dto.ReportCode), 50);
        ValidateRequiredText(dto.Name, nameof(dto.Name), 150);

        ValidateAllowedValue(
            dto.ReportType,
            ValidReportTypes,
            nameof(dto.ReportType));

        ValidateAllowedValue(
            dto.OutputFormat,
            ValidOutputFormats,
            nameof(dto.OutputFormat));

        ValidateOptionalText(dto.Description, nameof(dto.Description), 500);
        ValidateOptionalText(dto.ScheduleCron, nameof(dto.ScheduleCron), 50);
        ValidateOptionalText(dto.Recipients, nameof(dto.Recipients), 1000);

        if (dto.RetentionDays < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.RetentionDays));

        if (dto.OwnerUserId.HasValue && dto.OwnerUserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.OwnerUserId));

        if (dto.LastRunAt.HasValue && dto.LastRunAt.Value == default)
            throw new ArgumentException("LastRunAt is invalid.");

        if (dto.NextRunAt.HasValue && dto.NextRunAt.Value == default)
            throw new ArgumentException("NextRunAt is invalid.");

        if (dto.IsScheduled && string.IsNullOrWhiteSpace(dto.ScheduleCron))
        {
            throw new ArgumentException(
                "ScheduleCron is required for scheduled reports.");
        }

        ValidateJson(dto.ParametersJson, nameof(dto.ParametersJson));
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

    private static void ValidateOptionalText(
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

    private static void ValidateJson(string? json, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(json))
            return;

        try
        {
            using var document = JsonDocument.Parse(json);
        }
        catch (JsonException exception)
        {
            throw new ArgumentException(
                "The supplied value must contain valid JSON.",
                parameterName,
                exception);
        }
    }

    private static void ValidateDateRange(DateTime startDateTime, DateTime endDateTime)
    {
        if (startDateTime == default || endDateTime == default)
            throw new ArgumentException("Start and end date/time are required.");

        if (endDateTime < startDateTime)
            throw new ArgumentException(
                "End date/time cannot be earlier than start date/time.");
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
