
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class AIConversationRepository : GenericRepository<AIConversation>, IAIConversationRepository
    {
        public AIConversationRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. CONVERSATION IDENTIFICATION
        // =====================================================

        // Finds a conversation using its unique reference.
        // Useful for retrieving a conversation from an external chat or support reference.
        public async Task<AIConversation?> GetByConversationReferenceAsync(string conversationReference)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ConversationReference == conversationReference);
        }

        // Retrieves a conversation with its related user, customer and escalation details.
        // Useful for viewing the complete ownership and escalation information.
        public async Task<AIConversation?> GetConversationDetailsAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Customer)
                .Include(x => x.EscalatedToEmployee)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.AIConversationId == conversationId);
        }


        // =====================================================
        // 2. USER AND CUSTOMER CONVERSATIONS
        // =====================================================

        // Retrieves all conversations belonging to a user.
        // Useful for displaying the user's AI assistant conversation history.
        public async Task<IEnumerable<AIConversation>> GetConversationsByUserIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }

        // Retrieves all conversations associated with a customer.
        // Useful for reviewing AI assistant interactions related to a banking customer.
        public async Task<IEnumerable<AIConversation>> GetConversationsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }

        // Retrieves the latest conversations belonging to a user.
        // Useful for displaying recent conversations in the AI assistant interface.
        public async Task<IEnumerable<AIConversation>> GetRecentConversationsByUserIdAsync(int userId, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Conversation count must be greater than zero.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.StartedAt)
                .Take(count)
                .ToListAsync();
        }


        // =====================================================
        // 3. CONVERSATION STATUS AND CHANNEL
        // =====================================================

        // Retrieves conversations matching a particular lifecycle status.
        // Useful for monitoring active, closed, archived and escalated conversations.
        public async Task<IEnumerable<AIConversation>> GetConversationsByStatusAsync(string status)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }

        // Retrieves active conversations belonging to a user.
        // Useful for allowing a user to continue an ongoing conversation.
        public async Task<IEnumerable<AIConversation>> GetActiveConversationsByUserIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.UserId == userId && x.Status == "Active")
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }

        // Retrieves conversations created through a particular channel.
        // Useful for separating web, mobile and voice assistant activity.
        public async Task<IEnumerable<AIConversation>> GetConversationsByChannelAsync(string channel)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Channel == channel)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }

        // Retrieves conversations started within a specified date range.
        // Useful for operational reporting and conversation activity analysis.
        public async Task<IEnumerable<AIConversation>> GetConversationsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.StartedAt >= startDate && x.StartedAt <= endDate)
                .OrderByDescending(x => x.StartedAt)
                .ToListAsync();
        }


        // =====================================================
        // 4. ESCALATION MANAGEMENT
        // =====================================================

        // Retrieves conversations escalated to a particular employee.
        // Useful for tracking conversations assigned to human banking support staff.
        public async Task<IEnumerable<AIConversation>> GetEscalatedConversationsByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.EscalatedToEmployeeId == employeeId && x.Status == "Escalated")
                .OrderBy(x => x.StartedAt)
                .ToListAsync();
        }

        // Retrieves conversations that require human employee attention.
        // Useful for monitoring the human-support escalation queue.
        public async Task<IEnumerable<AIConversation>> GetEscalatedConversationsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Status == "Escalated")
                .Include(x => x.User)
                .Include(x => x.Customer)
                .Include(x => x.EscalatedToEmployee)
                .OrderBy(x => x.StartedAt)
                .ToListAsync();
        }


        // =====================================================
        // 5. MESSAGE AND USAGE INFORMATION
        // =====================================================

        // Retrieves a conversation with its complete message history.
        // Useful for displaying the conversation in its original message sequence.
        public async Task<AIConversation?> GetConversationWithMessagesAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.AIChatMessages.OrderBy(m => m.SequenceNumber))
                .FirstOrDefaultAsync(x => x.AIConversationId == conversationId);
        }

        // Retrieves a conversation with its associated AI usage logs.
        // Useful for reviewing model usage and consumption for a conversation.
        public async Task<AIConversation?> GetConversationWithUsageLogsAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.AIUsageLogs)
                .FirstOrDefaultAsync(x => x.AIConversationId == conversationId);
        }


        // =====================================================
        // 6. CONVERSATION SEARCH AND VALIDATION
        // =====================================================

        // Searches conversations by reference, title or summary.
        // Useful for locating conversations using known discussion details.
        public async Task<IEnumerable<AIConversation>> SearchConversationsAsync(string searchTerm, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty.", nameof(searchTerm));

            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            searchTerm = searchTerm.Trim();

            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.ConversationReference.Contains(searchTerm) ||
                    (x.Title != null && x.Title.Contains(searchTerm)) ||
                    (x.Summary != null && x.Summary.Contains(searchTerm)))
                .OrderByDescending(x => x.StartedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Checks whether a user has an active conversation.
        // Useful for preventing unnecessary duplicate active conversation sessions.
        public async Task<bool> HasActiveConversationAsync(int userId)
        {
            return await _dbSet.AnyAsync(x => x.UserId == userId && x.Status == "Active");
        }
    }
}
