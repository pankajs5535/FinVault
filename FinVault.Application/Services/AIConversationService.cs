using AutoMapper;
using FinVault.Application.DTOs.AIConversationDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AIConversationService : IAIConversationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    private static readonly string[] ValidChannels = { "Web", "Mobile", "Voice" };
    private static readonly string[] ValidStatuses = { "Active", "Closed", "Archived", "Escalated" };

    public AIConversationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CONVERSATION CRUD

    public async Task<IEnumerable<AIConversationDto>> GetAllAsync()
    {
        var conversations = await _unitOfWork.AIConversation.GetAllAsync();
        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public async Task<AIConversationDto?> GetByIdAsync(int conversationId)
    {
        if (conversationId <= 0)
            return null;

        var conversation = await _unitOfWork.AIConversation.GetByIdAsync(conversationId);
        return conversation == null ? null : _mapper.Map<AIConversationDto>(conversation);
    }

    public async Task<AIConversationDto?> GetByConversationReferenceAsync(string conversationReference)
    {
        if (string.IsNullOrWhiteSpace(conversationReference))
            return null;

        var conversation = await _unitOfWork.AIConversation
            .GetByConversationReferenceAsync(conversationReference.Trim());

        return conversation == null ? null : _mapper.Map<AIConversationDto>(conversation);
    }

    public async Task<AIConversationDto?> GetConversationDetailsAsync(int conversationId)
    {
        if (conversationId <= 0)
            return null;

        var conversation = await _unitOfWork.AIConversation
            .GetConversationDetailsAsync(conversationId);

        return conversation == null ? null : _mapper.Map<AIConversationDto>(conversation);
    }

    public async Task<AIConversationDto> CreateAsync(CreateAIConversationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateConversationReference(dto.ConversationReference);
        ValidateChannel(dto.Channel);
        ValidateStatus(dto.Status);
        ValidateLanguage(dto.Language);
        ValidateLifecycle(dto.Status, dto.StartedAt, dto.EndedAt);
        ValidateEscalation(dto.Status, dto.EscalatedToEmployeeId);

        var reference = dto.ConversationReference.Trim();

        if (await _unitOfWork.AIConversation
            .GetByConversationReferenceAsync(reference) != null)
        {
            throw new InvalidOperationException(
                "A conversation with this reference already exists.");
        }

        if (await _unitOfWork.User.GetByIdAsync(dto.UserId) == null)
            throw new InvalidOperationException("The specified user does not exist.");

        if (dto.CustomerId.HasValue &&
            await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId.Value) == null)
        {
            throw new InvalidOperationException("The specified customer does not exist.");
        }

        var conversation = _mapper.Map<AIConversation>(dto);

        conversation.ConversationReference = reference;
        conversation.Channel = dto.Channel.Trim();
        conversation.Language = dto.Language.Trim();
        conversation.Status = dto.Status.Trim();
        conversation.Title = NormalizeOptionalText(dto.Title);
        conversation.Summary = NormalizeOptionalText(dto.Summary);
        conversation.MessageCount = 0;
        conversation.LastMessageAt = null;
        conversation.CreatedAt = DateTime.UtcNow;
        conversation.UpdatedAt = null;
        conversation.EndedAt = dto.EndedAt;

        await _unitOfWork.AIConversation.AddAsync(conversation);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AIConversationDto>(conversation);
    }

    public async Task<AIConversationDto?> UpdateAsync(UpdateAIConversationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AIConversationId <= 0)
            return null;

        ValidateConversationReference(dto.ConversationReference);
        ValidateChannel(dto.Channel);
        ValidateStatus(dto.Status);
        ValidateLanguage(dto.Language);
        ValidateLifecycle(dto.Status, dto.StartedAt, dto.EndedAt);
        ValidateEscalation(dto.Status, dto.EscalatedToEmployeeId);

        var conversation = await _unitOfWork.AIConversation
            .GetByIdAsync(dto.AIConversationId);

        if (conversation == null)
            return null;

        var reference = dto.ConversationReference.Trim();

        var existingReference = await _unitOfWork.AIConversation
            .GetByConversationReferenceAsync(reference);

        if (existingReference != null &&
            existingReference.AIConversationId != dto.AIConversationId)
        {
            throw new InvalidOperationException(
                "Another conversation already uses this reference.");
        }

        if (await _unitOfWork.User.GetByIdAsync(dto.UserId) == null)
            throw new InvalidOperationException("The specified user does not exist.");

        if (dto.CustomerId.HasValue &&
            await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId.Value) == null)
        {
            throw new InvalidOperationException("The specified customer does not exist.");
        }

        _mapper.Map(dto, conversation);

        conversation.ConversationReference = reference;
        conversation.Channel = dto.Channel.Trim();
        conversation.Language = dto.Language.Trim();
        conversation.Status = dto.Status.Trim();
        conversation.Title = NormalizeOptionalText(dto.Title);
        conversation.Summary = NormalizeOptionalText(dto.Summary);
        conversation.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.AIConversation.Update(conversation);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AIConversationDto>(conversation);
    }

    public async Task<bool> DeleteAsync(DeleteAIConversationDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AIConversationId <= 0)
            return false;

        var conversation = await _unitOfWork.AIConversation
            .GetByIdAsync(dto.AIConversationId);

        if (conversation == null)
            return false;

        var conversationWithMessages = await _unitOfWork.AIConversation
            .GetConversationWithMessagesAsync(dto.AIConversationId);

        var conversationWithUsageLogs = await _unitOfWork.AIConversation
            .GetConversationWithUsageLogsAsync(dto.AIConversationId);

        if (conversationWithMessages?.AIChatMessages?.Any() == true ||
            conversationWithUsageLogs?.AIUsageLogs?.Any() == true)
        {
            throw new InvalidOperationException(
                "A conversation containing messages or usage logs cannot be deleted. " +
                "Preserve its history for audit and usage tracking.");
        }

        _unitOfWork.AIConversation.Delete(conversation);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. USER AND CUSTOMER CONVERSATIONS

    public async Task<IEnumerable<AIConversationDto>> GetConversationsByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<AIConversationDto>();

        var conversations = await _unitOfWork.AIConversation
            .GetConversationsByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public async Task<IEnumerable<AIConversationDto>> GetConversationsByCustomerIdAsync(int customerId)
    {
        if (customerId <= 0)
            return Enumerable.Empty<AIConversationDto>();

        var conversations = await _unitOfWork.AIConversation
            .GetConversationsByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public async Task<IEnumerable<AIConversationDto>> GetRecentConversationsByUserIdAsync(
        int userId,
        int count)
    {
        if (userId <= 0 || count <= 0)
            return Enumerable.Empty<AIConversationDto>();

        var conversations = await _unitOfWork.AIConversation
            .GetRecentConversationsByUserIdAsync(userId, count);

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public async Task<IEnumerable<AIConversationDto>> GetActiveConversationsByUserIdAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<AIConversationDto>();

        var conversations = await _unitOfWork.AIConversation
            .GetActiveConversationsByUserIdAsync(userId);

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public Task<bool> HasActiveConversationAsync(int userId)
    {
        if (userId <= 0)
            return Task.FromResult(false);

        return _unitOfWork.AIConversation.HasActiveConversationAsync(userId);
    }

    // 3. CONVERSATION STATUS AND CHANNEL

    public async Task<IEnumerable<AIConversationDto>> GetConversationsByStatusAsync(string status)
    {
        ValidateStatus(status);

        var conversations = await _unitOfWork.AIConversation
            .GetConversationsByStatusAsync(status.Trim());

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public async Task<IEnumerable<AIConversationDto>> GetConversationsByChannelAsync(string channel)
    {
        ValidateChannel(channel);

        var conversations = await _unitOfWork.AIConversation
            .GetConversationsByChannelAsync(channel.Trim());

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public async Task<IEnumerable<AIConversationDto>> GetConversationsByDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var conversations = await _unitOfWork.AIConversation
            .GetConversationsByDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    // 4. HUMAN SUPPORT ESCALATION

    public async Task<IEnumerable<AIConversationDto>> GetEscalatedConversationsAsync()
    {
        var conversations = await _unitOfWork.AIConversation
            .GetEscalatedConversationsAsync();

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    public async Task<IEnumerable<AIConversationDto>> GetEscalatedConversationsByEmployeeIdAsync(
        int employeeId)
    {
        if (employeeId <= 0)
            return Enumerable.Empty<AIConversationDto>();

        var conversations = await _unitOfWork.AIConversation
            .GetEscalatedConversationsByEmployeeIdAsync(employeeId);

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    // 5. MESSAGE AND USAGE INFORMATION

    public async Task<AIConversationDto?> GetConversationWithMessagesAsync(int conversationId)
    {
        if (conversationId <= 0)
            return null;

        var conversation = await _unitOfWork.AIConversation
            .GetConversationWithMessagesAsync(conversationId);

        return conversation == null ? null : _mapper.Map<AIConversationDto>(conversation);
    }

    public async Task<AIConversationDto?> GetConversationWithUsageLogsAsync(int conversationId)
    {
        if (conversationId <= 0)
            return null;

        var conversation = await _unitOfWork.AIConversation
            .GetConversationWithUsageLogsAsync(conversationId);

        return conversation == null ? null : _mapper.Map<AIConversationDto>(conversation);
    }

    // 6. SEARCH

    public async Task<IEnumerable<AIConversationDto>> SearchConversationsAsync(
        string searchTerm,
        int pageNumber,
        int pageSize)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            throw new ArgumentException("Search term is required.", nameof(searchTerm));

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0 || pageSize > 200)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 200.");

        var conversations = await _unitOfWork.AIConversation
            .SearchConversationsAsync(searchTerm.Trim(), pageNumber, pageSize);

        return _mapper.Map<IEnumerable<AIConversationDto>>(conversations);
    }

    // VALIDATION

    private static void ValidateConversationReference(string? reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException("Conversation reference is required.");

        if (reference.Trim().Length > 40)
            throw new ArgumentException("Conversation reference cannot exceed 40 characters.");
    }

    private static void ValidateChannel(string? channel)
    {
        if (string.IsNullOrWhiteSpace(channel) ||
            !ValidChannels.Contains(channel.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Channel must be Web, Mobile, or Voice.");
        }
    }

    private static void ValidateStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status) ||
            !ValidStatuses.Contains(status.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Status must be Active, Closed, Archived, or Escalated.");
        }
    }

    private static void ValidateLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            throw new ArgumentException("Language is required.");

        if (language.Trim().Length > 10)
            throw new ArgumentException("Language cannot exceed 10 characters.");
    }

    private static void ValidateLifecycle(
        string status,
        DateTime startedAt,
        DateTime? endedAt)
    {
        if (startedAt == default)
            throw new ArgumentException("Conversation start date is required.");

        if (endedAt.HasValue && endedAt.Value < startedAt)
            throw new ArgumentException("End date cannot be earlier than start date.");

        var normalizedStatus = status.Trim();

        if ((normalizedStatus.Equals("Closed", StringComparison.OrdinalIgnoreCase) ||
             normalizedStatus.Equals("Archived", StringComparison.OrdinalIgnoreCase)) &&
            !endedAt.HasValue)
        {
            throw new ArgumentException(
                "An end date is required for Closed or Archived conversations.");
        }

        if (normalizedStatus.Equals("Active", StringComparison.OrdinalIgnoreCase) &&
            endedAt.HasValue)
        {
            throw new ArgumentException(
                "An Active conversation cannot have an end date.");
        }
    }

    private static void ValidateEscalation(string status, int? employeeId)
    {
        if (status.Trim().Equals("Escalated", StringComparison.OrdinalIgnoreCase) &&
            !employeeId.HasValue)
        {
            throw new ArgumentException(
                "An employee must be assigned to an escalated conversation.");
        }
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Both start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}