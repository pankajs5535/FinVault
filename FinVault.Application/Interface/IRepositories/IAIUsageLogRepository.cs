
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IAIUsageLogRepository : IGenericRepository<AIUsageLog>
    {
        // =====================================================
        // 1. USAGE LOG IDENTIFICATION
        // =====================================================

        // Finds a usage log using its external request ID.
        // Useful for tracing a specific AI provider request.
        Task<AIUsageLog?> GetByRequestIdAsync(string requestId);

        // Retrieves a usage log with its related user, conversation and message.
        // Useful for investigating the complete context of an AI request.
        Task<AIUsageLog?> GetUsageLogDetailsAsync(int usageLogId);


        // =====================================================
        // 2. USER AND CONVERSATION USAGE
        // =====================================================

        // Retrieves usage logs associated with a particular user.
        // Useful for monitoring individual user AI consumption.
        Task<IEnumerable<AIUsageLog>> GetUsageLogsByUserIdAsync(int userId);

        // Retrieves usage logs associated with a particular conversation.
        // Useful for reviewing AI consumption during a conversation.
        Task<IEnumerable<AIUsageLog>> GetUsageLogsByConversationIdAsync(int conversationId);

        // Retrieves usage logs associated with a particular chat message.
        // Useful for tracing the AI request that generated or processed a message.
        Task<IEnumerable<AIUsageLog>> GetUsageLogsByMessageIdAsync(int messageId);


        // =====================================================
        // 3. PROVIDER, MODEL AND FEATURE USAGE
        // =====================================================

        // Retrieves usage logs for a particular AI provider.
        // Useful for comparing usage across AI service providers.
        Task<IEnumerable<AIUsageLog>> GetUsageLogsByProviderAsync(string provider);

        // Retrieves usage logs for a particular AI model.
        // Useful for monitoring model-specific usage and cost.
        Task<IEnumerable<AIUsageLog>> GetUsageLogsByModelAsync(string modelName);

        // Retrieves usage logs for a particular application feature.
        // Useful for identifying which banking application features consume AI resources.
        Task<IEnumerable<AIUsageLog>> GetUsageLogsByFeatureAsync(string featureName);


        // =====================================================
        // 4. SUCCESS AND FAILURE MONITORING
        // =====================================================

        // Retrieves successful AI requests.
        // Useful for monitoring completed AI operations.
        Task<IEnumerable<AIUsageLog>> GetSuccessfulUsageLogsAsync();

        // Retrieves failed AI requests.
        // Useful for investigating provider errors and unsuccessful AI operations.
        Task<IEnumerable<AIUsageLog>> GetFailedUsageLogsAsync();

        // Retrieves failed AI requests within a specified date range.
        // Useful for monitoring operational failures and troubleshooting incidents.
        Task<IEnumerable<AIUsageLog>> GetFailedUsageLogsByDateRangeAsync(DateTime startDate, DateTime endDate);


        // =====================================================
        // 5. TOKEN, COST AND PERFORMANCE REPORTING
        // =====================================================

        // Calculates total token consumption for a user.
        // Useful for monitoring user-level AI consumption and quota usage.
        Task<int> GetTotalTokensByUserIdAsync(int userId);

        // Calculates estimated AI cost for a user.
        // Useful for monitoring and allocating AI usage costs.
        Task<decimal> GetEstimatedCostByUserIdAsync(int userId);

        // Calculates estimated AI cost for a feature.
        // Useful for identifying the cost of individual application features.
        Task<decimal> GetEstimatedCostByFeatureAsync(string featureName);

        // Retrieves usage logs with latency greater than the specified threshold.
        // Useful for identifying slow AI provider responses.
        Task<IEnumerable<AIUsageLog>> GetSlowUsageLogsAsync(int minimumLatencyMs);


        // =====================================================
        // 6. DATE RANGE AND PAGINATED SEARCH
        // =====================================================

        // Retrieves usage logs created within a specified date range.
        // Useful for daily, weekly and monthly AI usage reporting.
        Task<IEnumerable<AIUsageLog>> GetUsageLogsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // Searches usage logs using provider, model, feature and success status filters.
        // Pagination prevents loading large usage histories at once.
        Task<IEnumerable<AIUsageLog>> SearchUsageLogsAsync(string? provider, string? modelName, string? featureName, bool? isSuccess, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    }
}
