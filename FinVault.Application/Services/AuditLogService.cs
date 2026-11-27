
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using AutoMapper;
using FinVault.Application.DTOs.AuditLogDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly string[] ValidOutcomes = { "Success", "Failure" };

    public AuditLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. AUDIT LOG IDENTIFICATION

    public async Task<IEnumerable<AuditLogDto>> GetAllAsync()
    {
        var logs = await _unitOfWork.AuditLog.GetAllAsync();
        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<AuditLogDto?> GetByIdAsync(int auditLogId)
    {
        if (auditLogId <= 0)
            return null;

        var log = await _unitOfWork.AuditLog.GetByIdAsync(auditLogId);

        return log == null ? null : _mapper.Map<AuditLogDto>(log);
    }

    public async Task<AuditLogDto?> GetAuditLogDetailsAsync(int auditLogId)
    {
        if (auditLogId <= 0)
            return null;

        var log = await _unitOfWork.AuditLog
            .GetAuditLogDetailsAsync(auditLogId);

        return log == null ? null : _mapper.Map<AuditLogDto>(log);
    }

    public async Task<AuditLogDto?> GetByEntryHashAsync(string entryHash)
    {
        if (string.IsNullOrWhiteSpace(entryHash))
            return null;

        var log = await _unitOfWork.AuditLog
            .GetByEntryHashAsync(entryHash.Trim());

        return log == null ? null : _mapper.Map<AuditLogDto>(log);
    }

    public async Task<AuditLogDto> CreateAsync(CreateAuditLogDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateCreateDto(dto);

        if (dto.UserId.HasValue &&
            await _unitOfWork.User.GetByIdAsync(dto.UserId.Value) == null)
        {
            throw new InvalidOperationException(
                "The specified user does not exist.");
        }

        var latestLog = await _unitOfWork.AuditLog.GetLatestAuditLogAsync();

        var log = _mapper.Map<AuditLog>(dto);

        log.Action = dto.Action.Trim();
        log.EntityName = dto.EntityName.Trim();
        log.Outcome = dto.Outcome.Trim();
        log.OldValuesMasked = MaskSensitiveData(dto.OldValuesMasked);
        log.NewValuesMasked = MaskSensitiveData(dto.NewValuesMasked);
        log.IpAddress = NormalizeOptionalText(dto.IpAddress);
        log.UserAgent = NormalizeOptionalText(dto.UserAgent);
        log.CorrelationId = NormalizeOptionalText(dto.CorrelationId);

        log.PreviousHash = latestLog?.EntryHash;
        log.CreatedAt = DateTime.UtcNow;
        log.EntryHash = GenerateEntryHash(log);

        await _unitOfWork.AuditLog.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AuditLogDto>(log);
    }

    // 2. ENTITY CHANGE HISTORY

    public async Task<IEnumerable<AuditLogDto>> GetEntityHistoryAsync(
        string entityName,
        int entityId)
    {
        ValidateRequiredText(entityName, nameof(entityName));

        if (entityId <= 0)
            throw new ArgumentOutOfRangeException(nameof(entityId));

        var logs = await _unitOfWork.AuditLog
            .GetEntityHistoryAsync(entityName.Trim(), entityId);

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsByEntityNameAsync(
        string entityName)
    {
        ValidateRequiredText(entityName, nameof(entityName));

        var logs = await _unitOfWork.AuditLog
            .GetLogsByEntityNameAsync(entityName.Trim());

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsByEntityAndActionAsync(
        string entityName,
        string action)
    {
        ValidateRequiredText(entityName, nameof(entityName));
        ValidateRequiredText(action, nameof(action));

        var logs = await _unitOfWork.AuditLog
            .GetLogsByEntityAndActionAsync(
                entityName.Trim(),
                action.Trim());

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public Task<int> CountEntityHistoryAsync(string entityName, int entityId)
    {
        ValidateRequiredText(entityName, nameof(entityName));

        if (entityId <= 0)
            throw new ArgumentOutOfRangeException(nameof(entityId));

        return _unitOfWork.AuditLog
            .CountEntityHistoryAsync(entityName.Trim(), entityId);
    }

    // 3. USER ACTIVITY HISTORY

    public async Task<IEnumerable<AuditLogDto>> GetLogsByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<AuditLogDto>();

        var logs = await _unitOfWork.AuditLog
            .GetLogsByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsByCorrelationIdAsync(
        string correlationId)
    {
        ValidateRequiredText(correlationId, nameof(correlationId));

        var logs = await _unitOfWork.AuditLog
            .GetLogsByCorrelationIdAsync(correlationId.Trim());

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    // 4. OUTCOME AND SECURITY MONITORING

    public async Task<IEnumerable<AuditLogDto>> GetLogsByOutcomeAsync(
        string outcome)
    {
        ValidateOutcome(outcome);

        var logs = await _unitOfWork.AuditLog
            .GetLogsByOutcomeAsync(outcome.Trim());

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<IEnumerable<AuditLogDto>> GetSecuritySensitiveLogsAsync()
    {
        var logs = await _unitOfWork.AuditLog
            .GetSecuritySensitiveLogsAsync();

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<IEnumerable<AuditLogDto>>
        GetFailedSecuritySensitiveLogsAsync()
    {
        var logs = await _unitOfWork.AuditLog
            .GetFailedSecuritySensitiveLogsAsync();

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    // 5. DATE RANGE AND INTEGRITY INVESTIGATION

    public async Task<IEnumerable<AuditLogDto>> GetLogsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var logs = await _unitOfWork.AuditLog
            .GetLogsByDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsWithHashChainAsync(
        int pageNumber,
        int pageSize)
    {
        ValidatePagination(pageNumber, pageSize);

        var logs = await _unitOfWork.AuditLog
            .GetLogsWithHashChainAsync(pageNumber, pageSize);

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    public async Task<AuditLogDto?> GetLatestAuditLogAsync()
    {
        var log = await _unitOfWork.AuditLog.GetLatestAuditLogAsync();

        return log == null ? null : _mapper.Map<AuditLogDto>(log);
    }

    // 6. PAGINATED AUDIT SEARCH

    public async Task<IEnumerable<AuditLogDto>> SearchAuditLogsAsync(
        string? entityName,
        string? action,
        string? outcome,
        int? userId,
        bool? isSecuritySensitive,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize)
    {
        ValidatePagination(pageNumber, pageSize);

        if (!string.IsNullOrWhiteSpace(outcome))
            ValidateOutcome(outcome);

        if (userId.HasValue && userId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        if (startDate.HasValue && endDate.HasValue &&
            endDate.Value < startDate.Value)
        {
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
        }

        var logs = await _unitOfWork.AuditLog.SearchAuditLogsAsync(
            NormalizeOptionalText(entityName),
            NormalizeOptionalText(action),
            NormalizeOptionalText(outcome),
            userId,
            isSecuritySensitive,
            startDate,
            endDate,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<AuditLogDto>>(logs);
    }

    // VALIDATION

    private static void ValidateCreateDto(CreateAuditLogDto dto)
    {
        ValidateRequiredText(dto.Action, nameof(dto.Action), 50);
        ValidateRequiredText(dto.EntityName, nameof(dto.EntityName), 100);
        ValidateOutcome(dto.Outcome);

        if (dto.EntityId < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.EntityId));

        if (dto.UserId <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.UserId));

        if (dto.IpAddress?.Length > 45)
            throw new ArgumentException("IP address cannot exceed 45 characters.");

        if (dto.UserAgent?.Length > 500)
            throw new ArgumentException("User agent cannot exceed 500 characters.");

        if (dto.CorrelationId?.Length > 64)
            throw new ArgumentException("Correlation ID cannot exceed 64 characters.");
    }

    private static void ValidateOutcome(string? outcome)
    {
        if (string.IsNullOrWhiteSpace(outcome) ||
            !ValidOutcomes.Contains(outcome.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Outcome must be Success or Failure.");
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

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0 || pageSize > 200)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 200.");
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }

    // MASKING

    private static string? MaskSensitiveData(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        var result = value;

        // Mask Aadhaar numbers.
        result = Regex.Replace(
            result,
            @"(?<!\d)\d{12}(?!\d)",
            "XXXX-XXXX-XXXX");

        // Mask payment card numbers.
        result = Regex.Replace(
            result,
            @"(?<!\d)(?:\d[ -]?){13,19}(?!\d)",
            "XXXX-XXXX-XXXX-XXXX");

        // Mask PAN numbers.
        result = Regex.Replace(
            result,
            @"\b[A-Z]{5}\d{4}[A-Z]\b",
            "XXXXX0000X",
            RegexOptions.IgnoreCase);

        // Mask password, token and secret values in key-value text.
        result = Regex.Replace(
            result,
            @"(?i)(password|passwd|token|secret|api[_-]?key)\s*[:=]\s*([^,\s;]+)",
            "$1=********");

        return result;
    }

    // HASH CHAIN

    private static string GenerateEntryHash(AuditLog log)
    {
        var payload = JsonSerializer.Serialize(new
        {
            log.PreviousHash,
            log.Action,
            log.EntityName,
            log.EntityId,
            log.Outcome,
            log.IsSecuritySensitive,
            log.OldValuesMasked,
            log.NewValuesMasked,
            log.IpAddress,
            log.UserAgent,
            log.CorrelationId,
            log.UserId,
            CreatedAt = log.CreatedAt.ToUniversalTime().ToString("O")
        });

        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));

        return Convert.ToHexString(hashBytes);
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
