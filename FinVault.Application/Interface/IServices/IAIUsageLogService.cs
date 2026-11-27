
using FinVault.Application.DTOs.AIUsageLogDtos;

namespace FinVault.Application.Interface.IServices
{
    public interface IAIUsageLogService
    {
        // 1. USAGE LOG CRUD
        Task<IEnumerable<AIUsageLogDto>> GetAllAsync();
        Task<AIUsageLogDto?> GetByIdAsync(int usageLogId);
        Task<AIUsageLogDto?> GetByRequestIdAsync(string requestId);
        Task<AIUsageLogDto?> GetUsageLogDetailsAsync(int usageLogId);
        Task<AIUsageLogDto> CreateAsync(CreateAIUsageLogDto dto);
        Task<bool> UpdateAsync(UpdateAIUsageLogDto dto);
        Task<bool> DeleteAsync(DeleteAIUsageLogDto dto);

        // 2. USER AND CONVERSATION USAGE
        Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByUserIdAsync(int userId);
        Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByConversationIdAsync(int conversationId);
        Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByMessageIdAsync(int messageId);

        // 3. PROVIDER, MODEL AND FEATURE USAGE
        Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByProviderAsync(string provider);
        Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByModelAsync(string modelName);
        Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByFeatureAsync(string featureName);

        // 4. SUCCESS AND FAILURE MONITORING
        Task<IEnumerable<AIUsageLogDto>> GetSuccessfulUsageLogsAsync();
        Task<IEnumerable<AIUsageLogDto>> GetFailedUsageLogsAsync();
        Task<IEnumerable<AIUsageLogDto>> GetFailedUsageLogsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 5. TOKEN, COST AND PERFORMANCE REPORTING
        Task<int> GetTotalTokensByUserIdAsync(int userId);
        Task<decimal> GetEstimatedCostByUserIdAsync(int userId);
        Task<decimal> GetEstimatedCostByFeatureAsync(string featureName);
        Task<IEnumerable<AIUsageLogDto>> GetSlowUsageLogsAsync(int minimumLatencyMs);

        // 6. DATE RANGE AND PAGINATED SEARCH
        Task<IEnumerable<AIUsageLogDto>> GetUsageLogsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate);

        Task<IEnumerable<AIUsageLogDto>> SearchUsageLogsAsync(
            string? provider,
            string? modelName,
            string? featureName,
            bool? isSuccess,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);
    }
}
