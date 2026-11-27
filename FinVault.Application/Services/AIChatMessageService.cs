
using AutoMapper;
using FinVault.Application.DTOs.AIChatMessageDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AIChatMessageService : IAIChatMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly string[] ValidSenderRoles =
    {
        "System",
        "User",
        "Assistant",
        "Tool"
    };

    public AIChatMessageService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. MESSAGE CRUD AND IDENTIFICATION

    public async Task<IEnumerable<AIChatMessageDto>> GetAllAsync()
    {
        var messages = await _unitOfWork.AIChatMessage.GetAllAsync();
        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    public async Task<AIChatMessageDto?> GetByIdAsync(int messageId)
    {
        if (messageId <= 0)
            return null;

        var message = await _unitOfWork.AIChatMessage.GetByIdAsync(messageId);

        return message == null
            ? null
            : _mapper.Map<AIChatMessageDto>(message);
    }

    public async Task<AIChatMessageDto?> GetByConversationAndSequenceAsync(
        int conversationId,
        int sequenceNumber)
    {
        if (conversationId <= 0 || sequenceNumber < 0)
            return null;

        var message = await _unitOfWork.AIChatMessage
            .GetByConversationAndSequenceAsync(conversationId, sequenceNumber);

        return message == null
            ? null
            : _mapper.Map<AIChatMessageDto>(message);
    }

    public async Task<AIChatMessageDto?> GetMessageDetailsAsync(int messageId)
    {
        if (messageId <= 0)
            return null;

        var message = await _unitOfWork.AIChatMessage
            .GetMessageDetailsAsync(messageId);

        return message == null
            ? null
            : _mapper.Map<AIChatMessageDto>(message);
    }

    public async Task<AIChatMessageDto> CreateAsync(CreateAIChatMessageDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateMessage(dto.SequenceNumber, dto.SenderRole, dto.Content);
        ValidateOptionalFields(
            dto.ModelName,
            dto.FinishReason,
            dto.FlagReason,
            dto.InputTokens,
            dto.OutputTokens,
            dto.LatencyMs);

        var conversation = await _unitOfWork.AIConversation
            .GetByIdAsync(dto.AIConversationId);

        if (conversation == null)
            throw new InvalidOperationException("The specified AI conversation does not exist.");

        var existingMessage = await _unitOfWork.AIChatMessage
            .GetByConversationAndSequenceAsync(
                dto.AIConversationId,
                dto.SequenceNumber);

        if (existingMessage != null)
        {
            throw new InvalidOperationException(
                "A message with this sequence number already exists in the conversation.");
        }

        var message = _mapper.Map<AIChatMessage>(dto);

        message.SenderRole = dto.SenderRole;
        message.Content = dto.Content;
        message.ModelName = NormalizeOptionalText(dto.ModelName);
        message.FinishReason = NormalizeOptionalText(dto.FinishReason);
        message.FlagReason = NormalizeOptionalText(dto.FlagReason);
        message.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.AIChatMessage.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AIChatMessageDto>(message);
    }

    public async Task<AIChatMessageDto?> UpdateAsync(UpdateAIChatMessageDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AIChatMessageId <= 0)
            return null;

        ValidateMessage(dto.SequenceNumber, dto.SenderRole, dto.Content);
        ValidateOptionalFields(
            dto.ModelName,
            dto.FinishReason,
            dto.FlagReason,
            dto.InputTokens,
            dto.OutputTokens,
            dto.LatencyMs);

        var message = await _unitOfWork.AIChatMessage
            .GetByIdAsync(dto.AIChatMessageId);

        if (message == null)
            return null;

        var conversation = await _unitOfWork.AIConversation
            .GetByIdAsync(dto.AIConversationId);

        if (conversation == null)
            throw new InvalidOperationException("The specified AI conversation does not exist.");

        var existingMessage = await _unitOfWork.AIChatMessage
            .GetByConversationAndSequenceAsync(
                dto.AIConversationId,
                dto.SequenceNumber);

        if (existingMessage != null &&
            existingMessage.AIChatMessageId != dto.AIChatMessageId)
        {
            throw new InvalidOperationException(
                "Another message already uses this sequence number in the conversation.");
        }

        _mapper.Map(dto, message);

        message.SenderRole = dto.SenderRole;
        message.Content = dto.Content;
        message.ModelName = NormalizeOptionalText(dto.ModelName);
        message.FinishReason = NormalizeOptionalText(dto.FinishReason);
        message.FlagReason = NormalizeOptionalText(dto.FlagReason);

        _unitOfWork.AIChatMessage.Update(message);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AIChatMessageDto>(message);
    }

    public async Task<bool> DeleteAsync(DeleteAIChatMessageDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AIChatMessageId <= 0)
            return false;

        var message = await _unitOfWork.AIChatMessage
            .GetByIdAsync(dto.AIChatMessageId);

        if (message == null)
            return false;

        var usageLogs = await _unitOfWork.AIUsageLog
            .GetUsageLogsByMessageIdAsync(dto.AIChatMessageId);

        if (usageLogs.Any())
        {
            throw new InvalidOperationException(
                "This message cannot be deleted because AI usage logs are associated with it.");
        }

        _unitOfWork.AIChatMessage.Delete(message);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CONVERSATION MESSAGE HISTORY

    public async Task<IEnumerable<AIChatMessageDto>> GetMessagesByConversationIdAsync(
        int conversationId)
    {
        if (conversationId <= 0)
            return Enumerable.Empty<AIChatMessageDto>();

        var messages = await _unitOfWork.AIChatMessage
            .GetMessagesByConversationIdAsync(conversationId);

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    public async Task<IEnumerable<AIChatMessageDto>> GetLatestMessagesAsync(
        int conversationId,
        int count)
    {
        if (conversationId <= 0)
            return Enumerable.Empty<AIChatMessageDto>();

        if (count <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Message count must be greater than zero.");

        var messages = await _unitOfWork.AIChatMessage
            .GetLatestMessagesAsync(conversationId, count);

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    public async Task<AIChatMessageDto?> GetLatestAssistantMessageAsync(
        int conversationId)
    {
        if (conversationId <= 0)
            return null;

        var message = await _unitOfWork.AIChatMessage
            .GetLatestAssistantMessageAsync(conversationId);

        return message == null
            ? null
            : _mapper.Map<AIChatMessageDto>(message);
    }

    public async Task<IEnumerable<AIChatMessageDto>> GetMessagesByDateRangeAsync(
        int conversationId,
        DateTime startDate,
        DateTime endDate)
    {
        if (conversationId <= 0)
            return Enumerable.Empty<AIChatMessageDto>();

        ValidateDateRange(startDate, endDate);

        var messages = await _unitOfWork.AIChatMessage
            .GetMessagesByDateRangeAsync(conversationId, startDate, endDate);

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    // 3. SENDER ROLE AND MODERATION

    public async Task<IEnumerable<AIChatMessageDto>> GetMessagesBySenderRoleAsync(
        int conversationId,
        string senderRole)
    {
        if (conversationId <= 0)
            return Enumerable.Empty<AIChatMessageDto>();

        ValidateSenderRole(senderRole);

        var messages = await _unitOfWork.AIChatMessage
            .GetMessagesBySenderRoleAsync(conversationId, senderRole);

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    public async Task<IEnumerable<AIChatMessageDto>> GetFlaggedMessagesAsync()
    {
        var messages = await _unitOfWork.AIChatMessage
            .GetFlaggedMessagesAsync();

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    public async Task<IEnumerable<AIChatMessageDto>> GetFlaggedMessagesByConversationIdAsync(
        int conversationId)
    {
        if (conversationId <= 0)
            return Enumerable.Empty<AIChatMessageDto>();

        var messages = await _unitOfWork.AIChatMessage
            .GetFlaggedMessagesByConversationIdAsync(conversationId);

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    // 4. MODEL AND USAGE INFORMATION

    public async Task<IEnumerable<AIChatMessageDto>> GetMessagesByModelNameAsync(
        string modelName)
    {
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException("Model name is required.", nameof(modelName));

        var messages = await _unitOfWork.AIChatMessage
            .GetMessagesByModelNameAsync(modelName.Trim());

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    public async Task<IEnumerable<AIChatMessageDto>> GetMessagesWithUsageLogsAsync(
        int conversationId)
    {
        if (conversationId <= 0)
            return Enumerable.Empty<AIChatMessageDto>();

        var messages = await _unitOfWork.AIChatMessage
            .GetMessagesWithUsageLogsAsync(conversationId);

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    // 5. MESSAGE SEARCH AND PAGINATION

    public async Task<IEnumerable<AIChatMessageDto>> SearchMessagesAsync(
        int conversationId,
        string searchTerm,
        int pageNumber,
        int pageSize)
    {
        if (conversationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(conversationId));

        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term is required.", nameof(searchTerm));

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0 || pageSize > 200)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 200.");

        var messages = await _unitOfWork.AIChatMessage
            .SearchMessagesAsync(
                conversationId,
                searchTerm.Trim(),
                pageNumber,
                pageSize);

        return _mapper.Map<IEnumerable<AIChatMessageDto>>(messages);
    }

    public async Task<int> CountMessagesByConversationIdAsync(int conversationId)
    {
        if (conversationId <= 0)
            return 0;

        return await _unitOfWork.AIChatMessage
            .CountMessagesByConversationIdAsync(conversationId);
    }

    // VALIDATION

    private static void ValidateMessage(
        int sequenceNumber,
        string? senderRole,
        string? content)
    {
        if (sequenceNumber < 0)
            throw new ArgumentOutOfRangeException(nameof(sequenceNumber));

        ValidateSenderRole(senderRole);

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content is required.", nameof(content));
    }

    private static void ValidateSenderRole(string? senderRole)
    {
        if (string.IsNullOrWhiteSpace(senderRole) ||
            !ValidSenderRoles.Contains(senderRole, StringComparer.Ordinal))
        {
            throw new ArgumentException(
                "Sender role must be System, User, Assistant, or Tool.");
        }
    }

    private static void ValidateOptionalFields(
        string? modelName,
        string? finishReason,
        string? flagReason,
        int? inputTokens,
        int? outputTokens,
        int? latencyMs)
    {
        if (modelName?.Length > 100)
            throw new ArgumentException("Model name cannot exceed 100 characters.");

        if (finishReason?.Length > 30)
            throw new ArgumentException("Finish reason cannot exceed 30 characters.");

        if (flagReason?.Length > 250)
            throw new ArgumentException("Flag reason cannot exceed 250 characters.");

        if (inputTokens < 0)
            throw new ArgumentOutOfRangeException(nameof(inputTokens));

        if (outputTokens < 0)
            throw new ArgumentOutOfRangeException(nameof(outputTokens));

        if (latencyMs < 0)
            throw new ArgumentOutOfRangeException(nameof(latencyMs));
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
