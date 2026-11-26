using FinVault.Application.DTOs.PaymentMethodDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IPaymentMethodService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<PaymentMethodDto>> GetAllAsync();
        Task<PaymentMethodDto?> GetByIdAsync(int paymentMethodId);
        Task<PaymentMethodDto?> GetByCodeAsync(string code);
        Task<PaymentMethodDto?> GetByNameAsync(string name);
        Task<PaymentMethodDto?> GetPaymentMethodDetailsAsync(int paymentMethodId);
        Task<PaymentMethodDto> CreateAsync(CreatePaymentMethodDto dto);
        Task<PaymentMethodDto?> UpdateAsync(UpdatePaymentMethodDto dto);
        Task<bool> DeleteAsync(DeletePaymentMethodDto dto);
        Task<bool> ExistsByCodeAsync(string code);

        // 2. RAIL AND SETTLEMENT CLASSIFICATION
        Task<IEnumerable<PaymentMethodDto>> GetByRailAsync(string rail);
        Task<IEnumerable<PaymentMethodDto>> GetBySettlementTypeAsync(string settlementType);
        Task<IEnumerable<PaymentMethodDto>> Get24x7PaymentMethodsAsync();

        // 3. AVAILABILITY AND STATUS
        Task<IEnumerable<PaymentMethodDto>> GetActivePaymentMethodsAsync();
        Task<IEnumerable<PaymentMethodDto>> GetInactivePaymentMethodsAsync();
        Task<IEnumerable<PaymentMethodDto>> GetAvailableMethodsForAmountAsync(decimal amount);

        // 4. LIMIT AND SETTLEMENT CONFIGURATION
        Task<IEnumerable<PaymentMethodDto>> GetByMinimumAmountRangeAsync(
            decimal minimumAmount,
            decimal maximumAmount);
        Task<IEnumerable<PaymentMethodDto>> GetWithDailyLimitAboveAsync(decimal minimumDailyLimit);
        Task<IEnumerable<PaymentMethodDto>> GetWithoutMaximumAmountAsync();

        // 5. RELATED PAYMENT OPERATIONS
        Task<PaymentMethodDto?> GetWithPaymentRequestsAsync(int paymentMethodId);
        Task<PaymentMethodDto?> GetWithFeesAsync(int paymentMethodId);
        Task<PaymentMethodDto?> GetWithReconciliationBatchesAsync(int paymentMethodId);
        Task<IEnumerable<PaymentMethodDto>> GetMethodsWithPaymentRequestsAsync();
        Task<IEnumerable<PaymentMethodDto>> GetMethodsWithFeesAsync();
        Task<IEnumerable<PaymentMethodDto>> GetMethodsWithReconciliationBatchesAsync();

        // 6. ADMINISTRATION AND SEARCH
        Task<IEnumerable<PaymentMethodDto>> GetCreatedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<PaymentMethodDto>> SearchPaymentMethodsAsync(string searchTerm);
        Task<bool> IsPaymentMethodInUseAsync(int paymentMethodId);
    }
}


//Banking rules:
//-Payment method codes must be unique.
//- Enforce minimum amount, maximum amount, and daily limits before accepting payments.
//- Only active payment methods can be used for new payment requests.
//- Do not delete methods referenced by payment requests, fees, or reconciliation batches; deactivate them instead.
//- Validate rail and settlement type against the configured allowed values.