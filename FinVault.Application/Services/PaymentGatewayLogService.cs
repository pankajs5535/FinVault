
using AutoMapper;
using FinVault.Application.DTOs.PaymentGatewayLogDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;
using System.Text.RegularExpressions;

namespace FinVault.Application.Services;

public class PaymentGatewayLogService : IPaymentGatewayLogService
{
    private static readonly string[] ValidOperations =
    {
        "Initiate", "StatusCheck", "Callback", "Reversal", "Refund"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentGatewayLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. LOG IDENTIFICATION

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetAllAsync()
    {
        var logs = await _unitOfWork.PaymentGatewayLog.GetAllAsync();
        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<PaymentGatewayLogDto?> GetByIdAsync(int paymentGatewayLogId)
    {
        ValidateId(paymentGatewayLogId, nameof(paymentGatewayLogId));

        var log = await _unitOfWork.PaymentGatewayLog.GetByIdAsync(paymentGatewayLogId);
        return _mapper.Map<PaymentGatewayLogDto?>(log);
    }

    public async Task<PaymentGatewayLogDto?> GetLogDetailsAsync(int paymentGatewayLogId)
    {
        ValidateId(paymentGatewayLogId, nameof(paymentGatewayLogId));

        var log = await _unitOfWork.PaymentGatewayLog.GetLogDetailsAsync(paymentGatewayLogId);
        return _mapper.Map<PaymentGatewayLogDto?>(log);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByCorrelationIdAsync(string correlationId)
    {
        ValidateRequiredString(correlationId, nameof(correlationId), 64);

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByCorrelationIdAsync(correlationId.Trim());

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<PaymentGatewayLogDto?> GetLatestLogByPaymentRequestIdAsync(int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var log = await _unitOfWork.PaymentGatewayLog
            .GetLatestLogByPaymentRequestIdAsync(paymentRequestId);

        return _mapper.Map<PaymentGatewayLogDto?>(log);
    }

    public async Task<PaymentGatewayLogDto?> GetByPaymentRequestAndAttemptAsync(
        int paymentRequestId, int attemptNumber)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        if (attemptNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(attemptNumber),
                "Attempt number must be greater than zero.");

        var log = await _unitOfWork.PaymentGatewayLog
            .GetByPaymentRequestAndAttemptAsync(paymentRequestId, attemptNumber);

        return _mapper.Map<PaymentGatewayLogDto?>(log);
    }

    public async Task<PaymentGatewayLogDto> CreateAsync(CreatePaymentGatewayLogDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRequiredString(dto.GatewayName, nameof(dto.GatewayName), 50);
        ValidateRequiredString(dto.Operation, nameof(dto.Operation), 50);
        ValidateRequiredString(dto.CorrelationId, nameof(dto.CorrelationId), 64);
        ValidateId(dto.PaymentRequestId, nameof(dto.PaymentRequestId));

        if (!ValidOperations.Contains(dto.Operation.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid gateway operation.", nameof(dto.Operation));

        if (dto.AttemptNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(dto.AttemptNumber));

        if (dto.HttpStatusCode.HasValue &&
            (dto.HttpStatusCode.Value < 100 || dto.HttpStatusCode.Value > 599))
            throw new ArgumentOutOfRangeException(nameof(dto.HttpStatusCode));

        if (dto.DurationMs < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.DurationMs));

        ValidateOptionalString(dto.ResponseCode, 30, nameof(dto.ResponseCode));
        ValidateOptionalString(dto.ResponseMessage, 500, nameof(dto.ResponseMessage));
        ValidateOptionalString(dto.ErrorMessage, 1000, nameof(dto.ErrorMessage));

        var paymentRequest = await _unitOfWork.PaymentRequest
            .GetByIdAsync(dto.PaymentRequestId);

        if (paymentRequest == null)
            throw new InvalidOperationException("The specified payment request does not exist.");

        var existingAttempt = await _unitOfWork.PaymentGatewayLog
            .GetByPaymentRequestAndAttemptAsync(
                dto.PaymentRequestId, dto.AttemptNumber);

        if (existingAttempt != null)
            throw new InvalidOperationException(
                "A gateway log already exists for this payment request and attempt number.");

        var log = _mapper.Map<PaymentGatewayLog>(dto);

        log.GatewayName = dto.GatewayName.Trim();
        log.Operation = dto.Operation.Trim();
        log.CorrelationId = dto.CorrelationId.Trim();

        log.RequestPayload = MaskSensitivePayload(dto.RequestPayload);
        log.ResponsePayload = MaskSensitivePayload(dto.ResponsePayload);

        log.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.PaymentGatewayLog.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PaymentGatewayLogDto>(log);
    }

    // 2. PAYMENT REQUEST HISTORY

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByPaymentRequestIdAsync(int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByPaymentRequestIdAsync(paymentRequestId);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetLogsWithPaymentRequestAsync(int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetLogsWithPaymentRequestAsync(paymentRequestId);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByPaymentRequestIdsAsync(
        IEnumerable<int> paymentRequestIds)
    {
        ArgumentNullException.ThrowIfNull(paymentRequestIds);

        var ids = paymentRequestIds.Distinct().ToList();

        if (ids.Count == 0)
            return Enumerable.Empty<PaymentGatewayLogDto>();

        if (ids.Any(id => id <= 0))
            throw new ArgumentException("All payment request IDs must be positive.",
                nameof(paymentRequestIds));

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByPaymentRequestIdsAsync(ids);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    // 3. GATEWAY AND OPERATION TRACKING

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByGatewayNameAsync(string gatewayName)
    {
        ValidateRequiredString(gatewayName, nameof(gatewayName), 50);

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByGatewayNameAsync(gatewayName.Trim());

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByOperationAsync(string operation)
    {
        ValidateRequiredString(operation, nameof(operation), 50);
        ValidateOperation(operation);

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByOperationAsync(operation.Trim());

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByGatewayAndOperationAsync(
        string gatewayName, string operation)
    {
        ValidateRequiredString(gatewayName, nameof(gatewayName), 50);
        ValidateRequiredString(operation, nameof(operation), 50);
        ValidateOperation(operation);

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByGatewayAndOperationAsync(gatewayName.Trim(), operation.Trim());

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    // 4. SUCCESS AND FAILURE MONITORING

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetSuccessfulLogsAsync()
    {
        var logs = await _unitOfWork.PaymentGatewayLog.GetSuccessfulLogsAsync();
        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetFailedLogsAsync()
    {
        var logs = await _unitOfWork.PaymentGatewayLog.GetFailedLogsAsync();
        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetFailedLogsByPaymentRequestIdAsync(
        int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetFailedLogsByPaymentRequestIdAsync(paymentRequestId);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByResponseCodeAsync(string responseCode)
    {
        ValidateRequiredString(responseCode, nameof(responseCode), 30);

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByResponseCodeAsync(responseCode.Trim());

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetByHttpStatusCodeAsync(int httpStatusCode)
    {
        if (httpStatusCode < 100 || httpStatusCode > 599)
            throw new ArgumentOutOfRangeException(nameof(httpStatusCode));

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetByHttpStatusCodeAsync(httpStatusCode);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    // 5. DATE-BASED MONITORING

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetLogsWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetLogsWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetFailedLogsWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetFailedLogsWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    // 6. PERFORMANCE AND RETRY ANALYSIS

    public async Task<IEnumerable<PaymentGatewayLogDto>> GetSlowGatewayLogsAsync(
        int minimumDurationMs)
    {
        if (minimumDurationMs < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumDurationMs));

        var logs = await _unitOfWork.PaymentGatewayLog
            .GetSlowGatewayLogsAsync(minimumDurationMs);

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<double?> GetAverageDurationWithinRangeAsync(
        DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        return await _unitOfWork.PaymentGatewayLog
            .GetAverageDurationWithinRangeAsync(startDate, endDate);
    }

    public async Task<int> GetMaximumAttemptNumberAsync(int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        return await _unitOfWork.PaymentGatewayLog
            .GetMaximumAttemptNumberAsync(paymentRequestId);
    }

    // 7. SEARCH AND COUNTS

    public async Task<IEnumerable<PaymentGatewayLogDto>> SearchGatewayLogsAsync(string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm), 200);

        var logs = await _unitOfWork.PaymentGatewayLog
            .SearchGatewayLogsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<PaymentGatewayLogDto>>(logs);
    }

    public async Task<int> CountLogsByPaymentRequestIdAsync(int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        return await _unitOfWork.PaymentGatewayLog
            .CountLogsByPaymentRequestIdAsync(paymentRequestId);
    }

    public async Task<int> CountFailedLogsByGatewayAsync(string gatewayName)
    {
        ValidateRequiredString(gatewayName, nameof(gatewayName), 50);

        return await _unitOfWork.PaymentGatewayLog
            .CountFailedLogsByGatewayAsync(gatewayName.Trim());
    }

    // VALIDATION HELPERS

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
            throw new ArgumentException("Value cannot be empty.", parameterName);

        if (value.Trim().Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.", parameterName);
    }

    private static void ValidateOptionalString(
        string? value, int maximumLength, string parameterName)
    {
        if (value?.Length > maximumLength)
            throw new ArgumentException(
                $"Value cannot exceed {maximumLength} characters.", parameterName);
    }

    private static void ValidateOperation(string operation)
    {
        if (!ValidOperations.Contains(operation.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid gateway operation.", nameof(operation));
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default)
            throw new ArgumentException("Start date is required.", nameof(startDate));

        if (endDate == default)
            throw new ArgumentException("End date is required.", nameof(endDate));

        if (startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date.");
    }

    // Masks sensitive fields before gateway payloads are stored.
    private static string? MaskSensitivePayload(string? payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            return payload;

        // Mask values associated with sensitive JSON property names.
        payload = Regex.Replace(
            payload,
            "(\"(?:pan|cardNumber|card_number|cvv|cvc|pin|aadhaar|aadhar|aadhaarNumber|accountNumber|password)\"\\s*:\\s*\")(.*?)(\")",
            match =>
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                string suffix = match.Groups[3].Value;

                if (Regex.IsMatch(key,
                    "\"(?:cvv|cvc|pin|password)\"",
                    RegexOptions.IgnoreCase))
                    return key + "[REDACTED]" + suffix;

                return key + MaskValue(value) + suffix;
            },
            RegexOptions.IgnoreCase);

        // Mask standalone PAN-like values.
        payload = Regex.Replace(
            payload,
            @"\b[A-Z]{5}[0-9]{4}[A-Z]\b",
            "XXXXX0000X",
            RegexOptions.IgnoreCase);

        // Mask standalone 12-digit Aadhaar-like values.
        payload = Regex.Replace(
            payload,
            @"(?<!\d)\d{12}(?!\d)",
            "XXXXXXXX" + "0000");

        // Mask standalone card-number-like values (13–19 digits).
        payload = Regex.Replace(
            payload,
            @"(?<!\d)\d{13,19}(?!\d)",
            match => new string('X', match.Value.Length - 4) +
                     match.Value[^4..]);

        return payload;
    }

    private static string MaskValue(string value)
    {
        if (value.Length <= 4)
            return "[REDACTED]";

        return new string('X', value.Length - 4) + value[^4..];
    }
}
