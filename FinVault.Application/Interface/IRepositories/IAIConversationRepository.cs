
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IAIConversationRepository : IGenericRepository<AIConversation>
    {
        // =====================================================
        // 1. CONVERSATION IDENTIFICATION
        // =====================================================

        // Finds a conversation using its unique reference.
        // Useful for retrieving a conversation from an external chat or support reference.
        Task<AIConversation?> GetByConversationReferenceAsync(string conversationReference);

        // Retrieves a conversation with its related user, customer and escalation details.
        // Useful for viewing the complete ownership and escalation information.
        Task<AIConversation?> GetConversationDetailsAsync(int conversationId);


        // =====================================================
        // 2. USER AND CUSTOMER CONVERSATIONS
        // =====================================================

        // Retrieves all conversations belonging to a user.
        // Useful for displaying the user's AI assistant conversation history.
        Task<IEnumerable<AIConversation>> GetConversationsByUserIdAsync(int userId);

        // Retrieves all conversations associated with a customer.
        // Useful for reviewing AI assistant interactions related to a banking customer.
        Task<IEnumerable<AIConversation>> GetConversationsByCustomerIdAsync(int customerId);

        // Retrieves the latest conversations belonging to a user.
        // Useful for displaying recent conversations in the AI assistant interface.
        Task<IEnumerable<AIConversation>> GetRecentConversationsByUserIdAsync(int userId, int count);


        // =====================================================
        // 3. CONVERSATION STATUS AND CHANNEL
        // =====================================================

        // Retrieves conversations matching a particular lifecycle status.
        // Useful for monitoring active, closed, archived and escalated conversations.
        Task<IEnumerable<AIConversation>> GetConversationsByStatusAsync(string status);

        // Retrieves active conversations belonging to a user.
        // Useful for allowing a user to continue an ongoing conversation.
        Task<IEnumerable<AIConversation>> GetActiveConversationsByUserIdAsync(int userId);

        // Retrieves conversations created through a particular channel.
        // Useful for separating web, mobile and voice assistant activity.
        Task<IEnumerable<AIConversation>> GetConversationsByChannelAsync(string channel);

        // Retrieves conversations started within a specified date range.
        // Useful for operational reporting and conversation activity analysis.
        Task<IEnumerable<AIConversation>> GetConversationsByDateRangeAsync(DateTime startDate, DateTime endDate);


        // =====================================================
        // 4. ESCALATION MANAGEMENT
        // =====================================================

        // Retrieves conversations escalated to a particular employee.
        // Useful for tracking conversations assigned to human banking support staff.
        Task<IEnumerable<AIConversation>> GetEscalatedConversationsByEmployeeIdAsync(int employeeId);

        // Retrieves conversations that require human employee attention.
        // Useful for monitoring the human-support escalation queue.
        Task<IEnumerable<AIConversation>> GetEscalatedConversationsAsync();


        // =====================================================
        // 5. MESSAGE AND USAGE INFORMATION
        // =====================================================

        // Retrieves a conversation with its complete message history.
        // Useful for displaying the conversation in its original message sequence.
        Task<AIConversation?> GetConversationWithMessagesAsync(int conversationId);

        // Retrieves a conversation with its associated AI usage logs.
        // Useful for reviewing model usage and consumption for a conversation.
        Task<AIConversation?> GetConversationWithUsageLogsAsync(int conversationId);


        // =====================================================
        // 6. CONVERSATION SEARCH AND VALIDATION
        // =====================================================

        // Searches conversations by reference, title or summary.
        // Useful for locating conversations using known discussion details.
        Task<IEnumerable<AIConversation>> SearchConversationsAsync(string searchTerm, int pageNumber, int pageSize);

        // Checks whether a user has an active conversation.
        // Useful for preventing unnecessary duplicate active conversation sessions.
        Task<bool> HasActiveConversationAsync(int userId);
    }
}
