
using AutoMapper;
using FinVault.Application.DTOs.AIUsageLogDtos;
using FinVault.Application.Interface.IServices;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AIUsageLogService : IAIUsageLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AIUsageLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. USAGE LOG CRUD

    public async Task<IEnumerable<AIUsageLogDto>> GetAllAsync()
    {
        var logs = await _unitOfWork.AIUsageLog.GetAllAsync();
        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<AIUsageLogDto?> GetByIdAsync(int usageLogId)
    {
        if (usageLogId <= 0)
            return null;

        var log = await _unitOfWork.AIUsageLog.GetByIdAsync(usageLogId);

        return log == null ? null : _mapper.Map<AIUsageLogDto>(log);
    }

    public async Task<AIUsageLogDto?> GetByRequestIdAsync(string requestId)
    {
        if (string.IsNullOrWhiteSpace(requestId))
            return null;

        var log = await _unitOfWork.AIUsageLog
            .GetByRequestIdAsync(requestId.Trim());

        return log == null ? null : _mapper.Map<AIUsageLogDto>(log);
    }

    public async Task<AIUsageLogDto?> GetUsageLogDetailsAsync(int usageLogId)
    {
        if (usageLogId <= 0)
            return null;

        var log = await _unitOfWork.AIUsageLog
            .GetUsageLogDetailsAsync(usageLogId);

        return log == null ? null : _mapper.Map<AIUsageLogDto>(log);
    }

    public async Task<AIUsageLogDto> CreateAsync(CreateAIUsageLogDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateCreateDto(dto);

        if (dto.RequestId != null)
        {
            var existingLog = await _unitOfWork.AIUsageLog
                .GetByRequestIdAsync(dto.RequestId.Trim());

            if (existingLog != null)
            {
                throw new InvalidOperationException(
                    "A usage log with this request ID already exists.");
            }
        }

        if (dto.UserId.HasValue &&
            await _unitOfWork.User.GetByIdAsync(dto.UserId.Value) == null)
        {
            throw new InvalidOperationException("The specified user does not exist.");
        }

        AIConversation? conversation = null;

        if (dto.AIConversationId.HasValue)
        {
            conversation = await _unitOfWork.AIConversation
                .GetByIdAsync(dto.AIConversationId.Value);

            if (conversation == null)
                throw new InvalidOperationException(
                    "The specified AI conversation does not exist.");
        }

        if (dto.AIChatMessageId.HasValue)
        {
            var message = await _unitOfWork.AIChatMessage
                .GetByIdAsync(dto.AIChatMessageId.Value);

            if (message == null)
                throw new InvalidOperationException(
                    "The specified AI chat message does not exist.");

            if (dto.AIConversationId.HasValue &&
                message.AIConversationId != dto.AIConversationId.Value)
            {
                throw new InvalidOperationException(
                    "The specified chat message does not belong to the specified conversation.");
            }
        }

        var log = _mapper.Map<AIUsageLog>(dto);

        log.FeatureName = dto.FeatureName.Trim();
        log.Provider = dto.Provider.Trim();
        log.ModelName = dto.ModelName.Trim();
        log.RequestId = NormalizeOptionalText(dto.RequestId);
        log.ErrorCode = NormalizeOptionalText(dto.ErrorCode);
        log.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.AIUsageLog.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AIUsageLogDto>(log);
    }

    public async Task<bool> UpdateAsync(UpdateAIUsageLogDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AIUsageLogId <= 0)
            return false;

        var log = await _unitOfWork.AIUsageLog
            .GetByIdAsync(dto.AIUsageLogId);

        if (log == null)
            return false;

        throw new InvalidOperationException(
            "AI usage logs are append-only and cannot be modified.");
    }

    public async Task<bool> DeleteAsync(DeleteAIUsageLogDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AIUsageLogId <= 0)
            return false;

        var log = await _unitOfWork.AIUsageLog
            .GetByIdAsync(dto.AIUsageLogId);

        if (log == null)
            return false;

        throw new InvalidOperationException(
            "AI usage logs are append-only and cannot be deleted.");
    }

    // 2. USER AND CONVERSATION USAGE

    public async Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<AIUsageLogDto>();

        var logs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByConversationIdAsync(
        int conversationId)
    {
        if (conversationId <= 0)
            return Enumerable.Empty<AIUsageLogDto>();

        var logs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByConversationIdAsync(conversationId);

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByMessageIdAsync(int messageId)
    {
        if (messageId <= 0)
            return Enumerable.Empty<AIUsageLogDto>();

        var logs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByMessageIdAsync(messageId);

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    // 3. PROVIDER, MODEL AND FEATURE USAGE

    public async Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByProviderAsync(
        string provider)
    {
        ValidateRequiredText(provider, nameof(provider));

        var logs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByProviderAsync(provider.Trim());

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByModelAsync(
        string modelName)
    {
        ValidateRequiredText(modelName, nameof(modelName));

        var logs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByModelAsync(modelName.Trim());

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByFeatureAsync(
        string featureName)
    {
        ValidateRequiredText(featureName, nameof(featureName));

        var logs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByFeatureAsync(featureName.Trim());

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    // 4. SUCCESS AND FAILURE MONITORING

    public async Task<IEnumerable<AIUsageLogDto>> GetSuccessfulUsageLogsAsync()
    {
        var logs = await _unitOfWork.AIUsageLog
            .GetSuccessfulUsageLogsAsync();

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<IEnumerable<AIUsageLogDto>> GetFailedUsageLogsAsync()
    {
        var logs = await _unitOfWork.AIUsageLog
            .GetFailedUsageLogsAsync();

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<IEnumerable<AIUsageLogDto>> GetFailedUsageLogsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var logs = await _unitOfWork.AIUsageLog
            .GetFailedUsageLogsByDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    // 5. TOKEN, COST AND PERFORMANCE REPORTING

    public Task<int> GetTotalTokensByUserIdAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        return _unitOfWork.AIUsageLog.GetTotalTokensByUserIdAsync(userId);
    }

    public Task<decimal> GetEstimatedCostByUserIdAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        return _unitOfWork.AIUsageLog.GetEstimatedCostByUserIdAsync(userId);
    }

    public async Task<decimal> GetEstimatedCostByFeatureAsync(string featureName)
    {
        ValidateRequiredText(featureName, nameof(featureName));

        return await _unitOfWork.AIUsageLog
            .GetEstimatedCostByFeatureAsync(featureName.Trim());
    }

    public async Task<IEnumerable<AIUsageLogDto>> GetSlowUsageLogsAsync(
        int minimumLatencyMs)
    {
        if (minimumLatencyMs < 0)
            throw new ArgumentOutOfRangeException(nameof(minimumLatencyMs));

        var logs = await _unitOfWork.AIUsageLog
            .GetSlowUsageLogsAsync(minimumLatencyMs);

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    // 6. DATE RANGE AND PAGINATED SEARCH

    public async Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var logs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    public async Task<IEnumerable<AIUsageLogDto>> SearchUsageLogsAsync(
        string? provider,
        string? modelName,
        string? featureName,
        bool? isSuccess,
        DateTime? startDate,
        DateTime? endDate,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0 || pageSize > 200)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 200.");

        if (startDate.HasValue && endDate.HasValue &&
            endDate.Value < startDate.Value)
        {
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
        }

        var logs = await _unitOfWork.AIUsageLog.SearchUsageLogsAsync(
            NormalizeOptionalText(provider),
            NormalizeOptionalText(modelName),
            NormalizeOptionalText(featureName),
            isSuccess,
            startDate,
            endDate,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<AIUsageLogDto>>(logs);
    }

    // VALIDATION

    private static void ValidateCreateDto(CreateAIUsageLogDto dto)
    {
        ValidateRequiredText(dto.FeatureName, nameof(dto.FeatureName), 100);
        ValidateRequiredText(dto.Provider, nameof(dto.Provider), 50);
        ValidateRequiredText(dto.ModelName, nameof(dto.ModelName), 100);

        ValidateOptionalTextLength(dto.RequestId, nameof(dto.RequestId), 64);
        ValidateOptionalTextLength(dto.ErrorCode, nameof(dto.ErrorCode), 50);

        if (dto.LatencyMs < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.LatencyMs));

        if (dto.InputTokens < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.InputTokens));

        if (dto.OutputTokens < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.OutputTokens));

        if (dto.TotalTokens < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.TotalTokens));

        if (dto.EstimatedCostInr < 0)
            throw new ArgumentOutOfRangeException(nameof(dto.EstimatedCostInr));

        if ((long)dto.InputTokens + dto.OutputTokens != dto.TotalTokens)
        {
            throw new ArgumentException(
                "TotalTokens must equal InputTokens plus OutputTokens.");
        }

        if (dto.UserId.HasValue && dto.UserId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.UserId));

        if (dto.AIConversationId.HasValue && dto.AIConversationId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.AIConversationId));

        if (dto.AIChatMessageId.HasValue && dto.AIChatMessageId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(dto.AIChatMessageId));
    }

    private static void ValidateRequiredText(
        string? value,
        string parameterName,
        int? maximumLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);

        if (maximumLength.HasValue && value.Trim().Length > maximumLength.Value)
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

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
