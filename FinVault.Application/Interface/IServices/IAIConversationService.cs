using FinVault.Application.DTOs.AIConversationDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAIConversationService
    {
        // 1. CONVERSATION CRUD
        Task<IEnumerable<AIConversationDto>> GetAllAsync();
        Task<AIConversationDto?> GetByIdAsync(int conversationId);
        Task<AIConversationDto?> GetByConversationReferenceAsync(string conversationReference);
        Task<AIConversationDto?> GetConversationDetailsAsync(int conversationId);
        Task<AIConversationDto> CreateAsync(CreateAIConversationDto dto);
        Task<AIConversationDto?> UpdateAsync(UpdateAIConversationDto dto);
        Task<bool> DeleteAsync(DeleteAIConversationDto dto);

        // 2. USER AND CUSTOMER CONVERSATIONS
        Task<IEnumerable<AIConversationDto>> GetConversationsByUserIdAsync(int userId);
        Task<IEnumerable<AIConversationDto>> GetConversationsByCustomerIdAsync(int customerId);
        Task<IEnumerable<AIConversationDto>> GetRecentConversationsByUserIdAsync(
            int userId,
            int count);
        Task<IEnumerable<AIConversationDto>> GetActiveConversationsByUserIdAsync(int userId);
        Task<bool> HasActiveConversationAsync(int userId);

        // 3. CONVERSATION STATUS AND CHANNEL
        Task<IEnumerable<AIConversationDto>> GetConversationsByStatusAsync(string status);
        Task<IEnumerable<AIConversationDto>> GetConversationsByChannelAsync(string channel);
        Task<IEnumerable<AIConversationDto>> GetConversationsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 4. HUMAN SUPPORT ESCALATION
        Task<IEnumerable<AIConversationDto>> GetEscalatedConversationsAsync();
        Task<IEnumerable<AIConversationDto>> GetEscalatedConversationsByEmployeeIdAsync(
            int employeeId);

        // 5. MESSAGE AND USAGE INFORMATION
        Task<AIConversationDto?> GetConversationWithMessagesAsync(int conversationId);
        Task<AIConversationDto?> GetConversationWithUsageLogsAsync(int conversationId);

        // 6. SEARCH
        Task<IEnumerable<AIConversationDto>> SearchConversationsAsync(
            string searchTerm,
            int pageNumber,
            int pageSize);
    }
}


//Banking and AI rules:
//-Conversation references must be unique.
//- Only the owning user or an authorized employee can access conversation history.
//- Validate channel values: Web, Mobile, Voice.
//- Validate status values: Active, Closed, Archived, Escalated.
//- Escalated conversations should identify the assigned employee.
//- Preserve conversation messages and AI usage logs for audit and usage tracking.
//- Do not expose sensitive banking information in AI conversation summaries.