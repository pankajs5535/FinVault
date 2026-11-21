
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IAIChatMessageRepository : IGenericRepository<AIChatMessage>
    {
        // =====================================================
        // 1. MESSAGE IDENTIFICATION
        // =====================================================

        // Finds a message using its conversation ID and sequence number.
        // Useful for retrieving a specific message within an AI conversation.
        Task<AIChatMessage?> GetByConversationAndSequenceAsync(int conversationId, int sequenceNumber);

        // Retrieves a message with its conversation and usage-log details.
        // Useful for reviewing the context and usage associated with a message.
        Task<AIChatMessage?> GetMessageDetailsAsync(int messageId);


        // =====================================================
        // 2. CONVERSATION MESSAGE HISTORY
        // =====================================================

        // Retrieves all messages belonging to a conversation in sequence order.
        // Useful for displaying the complete AI conversation history.
        Task<IEnumerable<AIChatMessage>> GetMessagesByConversationIdAsync(int conversationId);

        // Retrieves the most recent messages from a conversation.
        // Useful for supplying recent context to an AI model without loading the full history.
        Task<IEnumerable<AIChatMessage>> GetLatestMessagesAsync(int conversationId, int count);

        // Retrieves the latest assistant message in a conversation.
        // Useful for displaying the most recent AI response.
        Task<AIChatMessage?> GetLatestAssistantMessageAsync(int conversationId);

        // Retrieves messages created within a specified date range.
        // Useful for conversation history audits and usage investigations.
        Task<IEnumerable<AIChatMessage>> GetMessagesByDateRangeAsync(int conversationId, DateTime startDate, DateTime endDate);


        // =====================================================
        // 3. SENDER ROLE AND MODERATION
        // =====================================================

        // Retrieves messages sent by a particular role.
        // Useful for separating user, assistant, system and tool messages.
        Task<IEnumerable<AIChatMessage>> GetMessagesBySenderRoleAsync(int conversationId, string senderRole);

        // Retrieves all messages flagged by moderation.
        // Useful for reviewing potentially unsafe or policy-violating AI content.
        Task<IEnumerable<AIChatMessage>> GetFlaggedMessagesAsync();

        // Retrieves flagged messages belonging to a conversation.
        // Useful for reviewing moderation issues within a specific conversation.
        Task<IEnumerable<AIChatMessage>> GetFlaggedMessagesByConversationIdAsync(int conversationId);


        // =====================================================
        // 4. MODEL AND USAGE INFORMATION
        // =====================================================

        // Retrieves messages generated using a particular AI model.
        // Useful for investigating model-specific responses and performance.
        Task<IEnumerable<AIChatMessage>> GetMessagesByModelNameAsync(string modelName);

        // Retrieves messages that have associated usage-log records.
        // Useful for examining message-level AI usage tracking.
        Task<IEnumerable<AIChatMessage>> GetMessagesWithUsageLogsAsync(int conversationId);


        // =====================================================
        // 5. MESSAGE SEARCH AND PAGINATION
        // =====================================================

        // Searches stored message content within a conversation.
        // Useful for locating previous discussion topics and responses.
        Task<IEnumerable<AIChatMessage>> SearchMessagesAsync(int conversationId, string searchTerm, int pageNumber, int pageSize);

        // Counts messages belonging to a conversation.
        // Useful for conversation statistics and history management.
        Task<int> CountMessagesByConversationIdAsync(int conversationId);
    }
}
