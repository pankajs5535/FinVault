
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class AIUsageLogRepository : GenericRepository<AIUsageLog>, IAIUsageLogRepository
    {
        public AIUsageLogRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. USAGE LOG IDENTIFICATION
        // =====================================================

        // Finds a usage log using its external request ID.
        // Useful for tracing a specific AI provider request.
        public async Task<AIUsageLog?> GetByRequestIdAsync(string requestId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.RequestId == requestId);
        }

        // Retrieves a usage log with its related user, conversation and message.
        // Useful for investigating the complete context of an AI request.
        public async Task<AIUsageLog?> GetUsageLogDetailsAsync(int usageLogId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.AIConversation)
                .Include(x => x.AIChatMessage)
                .FirstOrDefaultAsync(x => x.AIUsageLogId == usageLogId);
        }


        // =====================================================
        // 2. USER AND CONVERSATION USAGE
        // =====================================================

        // Retrieves usage logs associated with a particular user.
        // Useful for monitoring individual user AI consumption.
        public async Task<IEnumerable<AIUsageLog>> GetUsageLogsByUserIdAsync(int userId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves usage logs associated with a particular conversation.
        // Useful for reviewing AI consumption during a conversation.
        public async Task<IEnumerable<AIUsageLog>> GetUsageLogsByConversationIdAsync(int conversationId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AIConversationId == conversationId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves usage logs associated with a particular chat message.
        // Useful for tracing the AI request that generated or processed a message.
        public async Task<IEnumerable<AIUsageLog>> GetUsageLogsByMessageIdAsync(int messageId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AIChatMessageId == messageId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 3. PROVIDER, MODEL AND FEATURE USAGE
        // =====================================================

        // Retrieves usage logs for a particular AI provider.
        // Useful for comparing usage across AI service providers.
        public async Task<IEnumerable<AIUsageLog>> GetUsageLogsByProviderAsync(string provider)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Provider == provider)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves usage logs for a particular AI model.
        // Useful for monitoring model-specific usage and cost.
        public async Task<IEnumerable<AIUsageLog>> GetUsageLogsByModelAsync(string modelName)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.ModelName == modelName)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves usage logs for a particular application feature.
        // Useful for identifying which banking application features consume AI resources.
        public async Task<IEnumerable<AIUsageLog>> GetUsageLogsByFeatureAsync(string featureName)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.FeatureName == featureName)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 4. SUCCESS AND FAILURE MONITORING
        // =====================================================

        // Retrieves successful AI requests.
        // Useful for monitoring completed AI operations.
        public async Task<IEnumerable<AIUsageLog>> GetSuccessfulUsageLogsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.IsSuccess)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves failed AI requests.
        // Useful for investigating provider errors and unsuccessful AI operations.
        public async Task<IEnumerable<AIUsageLog>> GetFailedUsageLogsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => !x.IsSuccess)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves failed AI requests within a specified date range.
        // Useful for monitoring operational failures and troubleshooting incidents.
        public async Task<IEnumerable<AIUsageLog>> GetFailedUsageLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => !x.IsSuccess && x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 5. TOKEN, COST AND PERFORMANCE REPORTING
        // =====================================================

        // Calculates total token consumption for a user.
        // Useful for monitoring user-level AI consumption and quota usage.
        public async Task<int> GetTotalTokensByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId && x.IsSuccess)
                .SumAsync(x => (int?)x.TotalTokens) ?? 0;
        }

        // Calculates estimated AI cost for a user.
        // Useful for monitoring and allocating AI usage costs.
        public async Task<decimal> GetEstimatedCostByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId && x.IsSuccess)
                .SumAsync(x => (decimal?)x.EstimatedCostInr) ?? 0m;
        }

        // Calculates estimated AI cost for a feature.
        // Useful for identifying the cost of individual application features.
        public async Task<decimal> GetEstimatedCostByFeatureAsync(string featureName)
        {
            return await _dbSet
                .Where(x => x.FeatureName == featureName && x.IsSuccess)
                .SumAsync(x => (decimal?)x.EstimatedCostInr) ?? 0m;
        }

        // Retrieves usage logs with latency greater than the specified threshold.
        // Useful for identifying slow AI provider responses.
        public async Task<IEnumerable<AIUsageLog>> GetSlowUsageLogsAsync(int minimumLatencyMs)
        {
            if (minimumLatencyMs < 0)
                throw new ArgumentOutOfRangeException(nameof(minimumLatencyMs), "Latency threshold cannot be negative.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.LatencyMs >= minimumLatencyMs)
                .OrderByDescending(x => x.LatencyMs)
                .ToListAsync();
        }


        // =====================================================
        // 6. DATE RANGE AND PAGINATED SEARCH
        // =====================================================

        // Retrieves usage logs created within a specified date range.
        // Useful for daily, weekly and monthly AI usage reporting.
        public async Task<IEnumerable<AIUsageLog>> GetUsageLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Searches usage logs using provider, model, feature and success status filters.
        // Pagination prevents loading large usage histories at once.
        public async Task<IEnumerable<AIUsageLog>> SearchUsageLogsAsync(string? provider, string? modelName, string? featureName, bool? isSuccess, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
                throw new ArgumentException("End date cannot be earlier than start date.");

            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(provider))
                query = query.Where(x => x.Provider == provider.Trim());

            if (!string.IsNullOrWhiteSpace(modelName))
                query = query.Where(x => x.ModelName == modelName.Trim());

            if (!string.IsNullOrWhiteSpace(featureName))
                query = query.Where(x => x.FeatureName == featureName.Trim());

            if (isSuccess.HasValue)
                query = query.Where(x => x.IsSuccess == isSuccess.Value);

            if (startDate.HasValue)
                query = query.Where(x => x.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.CreatedAt <= endDate.Value);

            return await query.OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
