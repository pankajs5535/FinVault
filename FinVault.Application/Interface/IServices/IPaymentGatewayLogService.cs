using FinVault.Application.DTOs.PaymentGatewayLogDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IPaymentGatewayLogService
    {
        // 1. LOG IDENTIFICATION
        Task<IEnumerable<PaymentGatewayLogDto>> GetAllAsync();
        Task<PaymentGatewayLogDto?> GetByIdAsync(int paymentGatewayLogId);
        Task<PaymentGatewayLogDto?> GetLogDetailsAsync(int paymentGatewayLogId);
        Task<IEnumerable<PaymentGatewayLogDto>> GetByCorrelationIdAsync(string correlationId);
        Task<PaymentGatewayLogDto?> GetLatestLogByPaymentRequestIdAsync(int paymentRequestId);
        Task<PaymentGatewayLogDto?> GetByPaymentRequestAndAttemptAsync(int paymentRequestId, int attemptNumber);
        Task<PaymentGatewayLogDto> CreateAsync(CreatePaymentGatewayLogDto dto);

        // 2. PAYMENT REQUEST HISTORY
        Task<IEnumerable<PaymentGatewayLogDto>> GetByPaymentRequestIdAsync(int paymentRequestId);
        Task<IEnumerable<PaymentGatewayLogDto>> GetLogsWithPaymentRequestAsync(int paymentRequestId);
        Task<IEnumerable<PaymentGatewayLogDto>> GetByPaymentRequestIdsAsync(IEnumerable<int> paymentRequestIds);

        // 3. GATEWAY AND OPERATION TRACKING
        Task<IEnumerable<PaymentGatewayLogDto>> GetByGatewayNameAsync(string gatewayName);
        Task<IEnumerable<PaymentGatewayLogDto>> GetByOperationAsync(string operation);
        Task<IEnumerable<PaymentGatewayLogDto>> GetByGatewayAndOperationAsync(string gatewayName, string operation);

        // 4. SUCCESS AND FAILURE MONITORING
        Task<IEnumerable<PaymentGatewayLogDto>> GetSuccessfulLogsAsync();
        Task<IEnumerable<PaymentGatewayLogDto>> GetFailedLogsAsync();
        Task<IEnumerable<PaymentGatewayLogDto>> GetFailedLogsByPaymentRequestIdAsync(int paymentRequestId);
        Task<IEnumerable<PaymentGatewayLogDto>> GetByResponseCodeAsync(string responseCode);
        Task<IEnumerable<PaymentGatewayLogDto>> GetByHttpStatusCodeAsync(int httpStatusCode);

        // 5. DATE-BASED MONITORING
        Task<IEnumerable<PaymentGatewayLogDto>> GetLogsWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentGatewayLogDto>> GetFailedLogsWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 6. PERFORMANCE AND RETRY ANALYSIS
        Task<IEnumerable<PaymentGatewayLogDto>> GetSlowGatewayLogsAsync(int minimumDurationMs);
        Task<double?> GetAverageDurationWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetMaximumAttemptNumberAsync(int paymentRequestId);

        // 7. SEARCH AND COUNTS
        Task<IEnumerable<PaymentGatewayLogDto>> SearchGatewayLogsAsync(string searchTerm);
        Task<int> CountLogsByPaymentRequestIdAsync(int paymentRequestId);
        Task<int> CountFailedLogsByGatewayAsync(string gatewayName);
    }
}



//Banking rules:
//-Gateway logs are append - only; do not expose update or delete operations.
//- Mask sensitive data before storing request and response payloads.
//- Never log PAN, CVV, PIN, or Aadhaar values.
//- Preserve correlation IDs and attempt numbers for investigation and retry tracking.