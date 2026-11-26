using FinVault.Application.DTOs.FeeDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IFeeService
    {
        // 1. FEE IDENTIFICATION
        Task<IEnumerable<FeeDto>> GetAllAsync();
        Task<FeeDto?> GetByIdAsync(int feeId);
        Task<FeeDto?> GetByCodeAsync(string code);
        Task<FeeDto?> GetFeeDetailsAsync(int feeId);
        Task<IEnumerable<FeeDto>> GetByNameAsync(string name);
        Task<bool> ExistsByCodeAsync(string code);

        // 2. FEE CRUD
        Task<FeeDto> CreateAsync(CreateFeeDto dto);
        Task<FeeDto?> UpdateAsync(UpdateFeeDto dto);
        Task<bool> DeleteAsync(DeleteFeeDto dto);

        // 3. FEE CLASSIFICATION
        Task<IEnumerable<FeeDto>> GetByCalculationMethodAsync(string calculationMethod);
        Task<IEnumerable<FeeDto>> GetByFrequencyAsync(string frequency);

        // 4. BANKING ENTITY ASSOCIATIONS
        Task<IEnumerable<FeeDto>> GetByTransactionTypeIdAsync(int transactionTypeId);
        Task<IEnumerable<FeeDto>> GetByPaymentMethodIdAsync(int paymentMethodId);
        Task<IEnumerable<FeeDto>> GetByAccountTypeIdAsync(int accountTypeId);

        // 5. EFFECTIVE DATE MANAGEMENT
        Task<IEnumerable<FeeDto>> GetEffectiveFeesAsync(DateTime effectiveDate);
        Task<IEnumerable<FeeDto>> GetUpcomingFeesAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<FeeDto>> GetActiveFeesAsync();
        Task<IEnumerable<FeeDto>> GetInactiveFeesAsync();

        // 6. TAX AND AMOUNT MANAGEMENT
        Task<IEnumerable<FeeDto>> GetGstApplicableFeesAsync();
        Task<IEnumerable<FeeDto>> GetFeesWithinAmountRangeAsync(
            decimal minimumAmount,
            decimal maximumAmount);

        // 7. TRANSACTION AND USAGE TRACKING
        Task<IEnumerable<FeeDto>> GetFeesWithChargedTransactionsAsync();

        // 8. AUDIT AND SEARCH
        Task<IEnumerable<FeeDto>> GetFeesCreatedByUserAsync(int userId);
        Task<IEnumerable<FeeDto>> SearchFeesAsync(string searchTerm);
    }
}


//Banking rules:
//-Fee codes must be unique.
//- Calculation methods: Flat, Percentage, Slab.
//- Frequencies: PerTransaction, Monthly, Quarterly, Yearly, OneTime.
//- Validate minimum and maximum amounts.
//- Apply GST only when IsGstApplicable is true.
//- Apply fees according to their effective dates and active status.
//- Preserve fee configurations already referenced by transactions.