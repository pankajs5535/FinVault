using FinVault.Application.DTOs.PaymentMethodDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IPaymentMethodService
    {
        // 1 CRUD AND IDENTIFICATION
        Task<IEnumerable<PaymentMethodDto>> GetAllAsync();
        Task<PaymentMethodDto?> GetByIdAsync(int paymentMethodId);
        Task<PaymentMethodDto?> GetByCodeAsync(string code);
        Task<PaymentMethodDto?> GetByNameAsync(string name);
        Task<PaymentMethodDto?> GetPaymentMethodDetailsAsync(int paymentMethodId);
        Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto);
        Task<PaymentMethodDto?> UpdateAsync(UpdatePaymentMethodDto dto);
        Task<bool> DeleteAsync(int paymentMethodId);

        // 2 RAIL AND SETTLEMENT CLASSIFICATION
        Task<IEnumerable<PaymentMethodDto>> GetByRailAsync(string rail);
        Task<IEnumerable<PaymentMethodDto>> GetBySettlementTypeAsync(string settlementType);
        Task<IEnumerable<PaymentMethodDto>> Get24x7PaymentMethodsAsync();

        // 3 AVAILABILITY AND STATUS
        Task<IEnumerable<PaymentMethodDto>> GetActivePaymentMethodsAsync();
        Task<IEnumerable<PaymentMethodDto>> GetInactivePaymentMethodsAsync();
        Task<IEnumerable<PaymentMethodDto>> GetAvailableMethodsForAmountAsync(decimal amount);
        Task<bool> ActivateAsync(int paymentMethodId);
        Task<bool> DeactivateAsync(int paymentMethodId);

        // 4 LIMIT AND SETTLEMENT CONFIGURATION
        Task<IEnumerable<PaymentMethodDto>> GetByMinimumAmountRangeAsync(decimal minimumAmount, decimal maximumAmount);
        Task<IEnumerable<PaymentMethodDto>> GetWithDailyLimitAboveAsync(decimal minimumDailyLimit);
        Task<IEnumerable<PaymentMethodDto>> GetWithoutMaximumAmountAsync();
        Task<bool> UpdateAmountLimitsAsync(int paymentMethodId, decimal minAmount, decimal? maxAmount, decimal? dailyLimit);
        Task<bool> UpdateSettlementTypeAsync(int paymentMethodId, string settlementType);
        Task<bool> Update24x7AvailabilityAsync(int paymentMethodId, bool is24x7);

        // 5 RELATED PAYMENT OPERATIONS
        Task<PaymentMethodDto?> GetWithPaymentRequestsAsync(int paymentMethodId);
        Task<PaymentMethodDto?> GetWithFeesAsync(int paymentMethodId);
        Task<PaymentMethodDto?> GetWithReconciliationBatchesAsync(int paymentMethodId);
        Task<IEnumerable<PaymentMethodDto>> GetMethodsWithPaymentRequestsAsync();
        Task<IEnumerable<PaymentMethodDto>> GetMethodsWithFeesAsync();
        Task<IEnumerable<PaymentMethodDto>> GetMethodsWithReconciliationBatchesAsync();

        // 6 ADMINISTRATION AND SEARCH
        Task<IEnumerable<PaymentMethodDto>> GetCreatedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentMethodDto>> SearchAsync(string searchTerm);

        // 7 VALIDATION
        Task<bool> IsCodeExistsAsync(string code);
        Task<bool> IsPaymentMethodInUseAsync(int paymentMethodId);
        Task<bool> CanPaymentMethodBeDeletedAsync(int paymentMethodId);
    }
}