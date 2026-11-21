
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class AIChatMessageRepository : GenericRepository<AIChatMessage>, IAIChatMessageRepository
    {
        public AIChatMessageRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. MESSAGE IDENTIFICATION
        // =====================================================

        // Finds a message using its conversation ID and sequence number.
        // Useful for retrieving a specific message within an AI conversation.
        public async Task<AIChatMessage?> GetByConversationAndSequenceAsync(int conversationId, int sequenceNumber)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.AIConversationId == conversationId && x.SequenceNumber == sequenceNumber);
        }

        // Retrieves a message with its conversation and usage-log details.
        // Useful for reviewing the context and usage associated with a message.
        public async Task<AIChatMessage?> GetMessageDetailsAsync(int messageId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.AIConversation)
                .Include(x => x.AIUsageLogs)
                .FirstOrDefaultAsync(x => x.AIChatMessageId == messageId);
        }


        // =====================================================
        // 2. CONVERSATION MESSAGE HISTORY
        // =====================================================

        // Retrieves all messages belonging to a conversation in sequence order.
        // Useful for displaying the complete AI conversation history.
        public async Task<IEnumerable<AIChatMessage>> GetMessagesByConversationIdAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId)
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync();
        }

        // Retrieves the most recent messages from a conversation.
        // Useful for supplying recent context to an AI model without loading the full history.
        public async Task<IEnumerable<AIChatMessage>> GetLatestMessagesAsync(int conversationId, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Message count must be greater than zero.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId)
                .OrderByDescending(x => x.SequenceNumber)
                .Take(count)
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync();
        }

        // Retrieves the latest assistant message in a conversation.
        // Useful for displaying the most recent AI response.
        public async Task<AIChatMessage?> GetLatestAssistantMessageAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId && x.SenderRole == "Assistant")
                .OrderByDescending(x => x.SequenceNumber)
                .FirstOrDefaultAsync();
        }

        // Retrieves messages created within a specified date range.
        // Useful for conversation history audits and usage investigations.
        public async Task<IEnumerable<AIChatMessage>> GetMessagesByDateRangeAsync(int conversationId, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync();
        }


        // =====================================================
        // 3. SENDER ROLE AND MODERATION
        // =====================================================

        // Retrieves messages sent by a particular role.
        // Useful for separating user, assistant, system and tool messages.
        public async Task<IEnumerable<AIChatMessage>> GetMessagesBySenderRoleAsync(int conversationId, string senderRole)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId && x.SenderRole == senderRole)
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync();
        }

        // Retrieves all messages flagged by moderation.
        // Useful for reviewing potentially unsafe or policy-violating AI content.
        public async Task<IEnumerable<AIChatMessage>> GetFlaggedMessagesAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.IsFlagged)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves flagged messages belonging to a conversation.
        // Useful for reviewing moderation issues within a specific conversation.
        public async Task<IEnumerable<AIChatMessage>> GetFlaggedMessagesByConversationIdAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId && x.IsFlagged)
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync();
        }


        // =====================================================
        // 4. MODEL AND USAGE INFORMATION
        // =====================================================

        // Retrieves messages generated using a particular AI model.
        // Useful for investigating model-specific responses and performance.
        public async Task<IEnumerable<AIChatMessage>> GetMessagesByModelNameAsync(string modelName)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.ModelName == modelName)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves messages that have associated usage-log records.
        // Useful for examining message-level AI usage tracking.
        public async Task<IEnumerable<AIChatMessage>> GetMessagesWithUsageLogsAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId && x.AIUsageLogs.Any())
                .Include(x => x.AIUsageLogs)
                .OrderBy(x => x.SequenceNumber)
                .ToListAsync();
        }


        // =====================================================
        // 5. MESSAGE SEARCH AND PAGINATION
        // =====================================================

        // Searches stored message content within a conversation.
        // Useful for locating previous discussion topics and responses.
        public async Task<IEnumerable<AIChatMessage>> SearchMessagesAsync(int conversationId, string searchTerm, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty.", nameof(searchTerm));

            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            searchTerm = searchTerm.Trim();

            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId && x.Content.Contains(searchTerm))
                .OrderBy(x => x.SequenceNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Counts messages belonging to a conversation.
        // Useful for conversation statistics and history management.
        public async Task<int> CountMessagesByConversationIdAsync(int conversationId)
        {
            return await _dbSet.CountAsync(x => x.AIConversationId == conversationId);
        }
    }
}
