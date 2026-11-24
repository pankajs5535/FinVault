
using FinVault.Application.DTOs.BeneficiaryDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IBeneficiaryService
    {
       
        // 1. CRUD AND BENEFICIARY DETAILS
       

        Task<IEnumerable<BeneficiaryDto>> GetAllAsync();

        Task<BeneficiaryDto?> GetByIdAsync(int beneficiaryId);

        Task<BeneficiaryDto?> GetByIdAndCustomerIdAsync(int beneficiaryId, int customerId);

        Task<BeneficiaryDto?> GetBeneficiaryDetailsAsync(int beneficiaryId);

        Task<BeneficiaryDto> CreateAsync(CreateBeneficiaryDto dto);

        Task<BeneficiaryDto?> UpdateAsync(UpdateBeneficiaryDto dto);

        Task<bool> DeleteAsync(int beneficiaryId);

        Task<bool> RemoveBeneficiaryAsync(int beneficiaryId, int customerId);


       
        // 2. CUSTOMER BENEFICIARY MANAGEMENT
       

        Task<IEnumerable<BeneficiaryDto>> GetByCustomerIdAsync(int customerId);

        Task<IEnumerable<BeneficiaryDto>> GetActiveByCustomerIdAsync(int customerId);

        Task<IEnumerable<BeneficiaryDto>> GetPendingByCustomerIdAsync(int customerId);

        Task<IEnumerable<BeneficiaryDto>> GetBlockedByCustomerIdAsync(int customerId);

        Task<IEnumerable<BeneficiaryDto>> GetRemovedByCustomerIdAsync(int customerId);


       
        // 3. BENEFICIARY TYPE AND PAYMENT IDENTIFICATION
       

        Task<IEnumerable<BeneficiaryDto>> GetByTypeAsync(int customerId, string beneficiaryType);

        Task<BeneficiaryDto?> GetByAccountAndIfscAsync(int customerId, string accountNumber, string ifscCode);

        Task<BeneficiaryDto?> GetByUpiVpaAsync(int customerId, string upiVpa);

        Task<IEnumerable<BeneficiaryDto>> GetByInternalAccountIdAsync(int internalAccountId);


       
        // 4. FAVORITES AND COOLING PERIOD
       

        Task<IEnumerable<BeneficiaryDto>> GetFavoriteBeneficiariesAsync(int customerId);

        Task<bool> AddToFavoritesAsync(int beneficiaryId, int customerId);

        Task<bool> RemoveFromFavoritesAsync(int beneficiaryId, int customerId);

        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesReadyForActivationAsync(DateTime currentDateTime);

        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesInCoolingPeriodAsync(DateTime currentDateTime);

        Task<bool> ActivateBeneficiaryAsync(int beneficiaryId, int customerId);

        Task<bool> BlockBeneficiaryAsync(int beneficiaryId, int customerId);

        Task<bool> UnblockBeneficiaryAsync(int beneficiaryId, int customerId);


       
        // 5. TRANSFER LIMIT AND ELIGIBILITY
       

        Task<IEnumerable<BeneficiaryDto>> GetActiveBeneficiariesWithinTransactionLimitAsync(int customerId, decimal amount);

        Task<bool> IsEligibleForTransferAsync(int beneficiaryId, int customerId, decimal amount);

        Task<bool> IsWithinTransactionLimitAsync(int beneficiaryId, decimal amount);


       
        // 6. SEARCH AND FILTER
       

        Task<IEnumerable<BeneficiaryDto>> SearchAsync(int customerId, string searchTerm, int pageNumber, int pageSize);


       
        // 7. VALIDATION AND OWNERSHIP
       

        Task<bool> ExistsForCustomerAsync(int beneficiaryId, int customerId);

        Task<bool> HasActiveBankBeneficiaryAsync(int customerId, string accountNumber, string ifscCode);

        Task<bool> HasActiveUpiBeneficiaryAsync(int customerId, string upiVpa);
    }
}
