
using FinVault.Application.DTOs.AIChatMessageDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAIChatMessageService
    {
        // 1. MESSAGE CRUD AND IDENTIFICATION
        Task<IEnumerable<AIChatMessageDto>> GetAllAsync();
        Task<AIChatMessageDto?> GetByIdAsync(int messageId);
        Task<AIChatMessageDto?> GetByConversationAndSequenceAsync(
            int conversationId,
            int sequenceNumber);
        Task<AIChatMessageDto?> GetMessageDetailsAsync(int messageId);
        Task<AIChatMessageDto> CreateAsync(CreateAIChatMessageDto dto);
        Task<AIChatMessageDto?> UpdateAsync(UpdateAIChatMessageDto dto);
        Task<bool> DeleteAsync(DeleteAIChatMessageDto dto);

        // 2. CONVERSATION MESSAGE HISTORY
        Task<IEnumerable<AIChatMessageDto>> GetMessagesByConversationIdAsync(
            int conversationId);
        Task<IEnumerable<AIChatMessageDto>> GetLatestMessagesAsync(
            int conversationId,
            int count);
        Task<AIChatMessageDto?> GetLatestAssistantMessageAsync(
            int conversationId);
        Task<IEnumerable<AIChatMessageDto>> GetMessagesByDateRangeAsync(
            int conversationId,
            DateTime startDate,
            DateTime endDate);

        // 3. SENDER ROLE AND MODERATION
        Task<IEnumerable<AIChatMessageDto>> GetMessagesBySenderRoleAsync(
            int conversationId,
            string senderRole);
        Task<IEnumerable<AIChatMessageDto>> GetFlaggedMessagesAsync();
        Task<IEnumerable<AIChatMessageDto>> GetFlaggedMessagesByConversationIdAsync(
            int conversationId);

        // 4. MODEL AND USAGE INFORMATION
        Task<IEnumerable<AIChatMessageDto>> GetMessagesByModelNameAsync(
            string modelName);
        Task<IEnumerable<AIChatMessageDto>> GetMessagesWithUsageLogsAsync(
            int conversationId);

        // 5. MESSAGE SEARCH AND PAGINATION
        Task<IEnumerable<AIChatMessageDto>> SearchMessagesAsync(
            int conversationId,
            string searchTerm,
            int pageNumber,
            int pageSize);
        Task<int> CountMessagesByConversationIdAsync(int conversationId);
    }
}
