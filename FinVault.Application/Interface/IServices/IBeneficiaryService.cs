using FinVault.Application.DTOs.BeneficiaryDtos;
using FinVault.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.Interfaces.Services
{
    public interface IBeneficiaryService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<BeneficiaryDto>> GetAllAsync();
        Task<BeneficiaryDto?> GetByIdAsync(int beneficiaryId);
        Task<BeneficiaryDto?> GetByIdAndCustomerIdAsync(int beneficiaryId, int customerId);
        Task<BeneficiaryDto?> GetBeneficiaryDetailsAsync(int beneficiaryId);
        Task<BeneficiaryDto> CreateAsync(CreateBeneficiaryDto dto);
        Task<BeneficiaryDto?> UpdateAsync(UpdateBeneficiaryDto dto);
        Task<bool> DeleteAsync(DeleteBeneficiaryDto dto);

        // 2. CUSTOMER BENEFICIARY MANAGEMENT
        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByCustomerIdAsync(int customerId);
        Task<IEnumerable<BeneficiaryDto>> GetActiveBeneficiariesByCustomerIdAsync(int customerId);
        Task<IEnumerable<BeneficiaryDto>> GetPendingBeneficiariesByCustomerIdAsync(int customerId);
        Task<IEnumerable<BeneficiaryDto>> GetBlockedBeneficiariesByCustomerIdAsync(int customerId);
        Task<IEnumerable<BeneficiaryDto>> GetRemovedBeneficiariesByCustomerIdAsync(int customerId);

        // 3. BENEFICIARY TYPE AND PAYMENT IDENTIFICATION
        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByTypeAsync(int customerId, string beneficiaryType);
        Task<BeneficiaryDto?> GetByAccountAndIfscAsync(int customerId, string accountNumber, string ifscCode);
        Task<BeneficiaryDto?> GetByUpiVpaAsync(int customerId, string upiVpa);

        // 4. FAVORITES AND COOLING PERIOD
        Task<IEnumerable<BeneficiaryDto>> GetFavoriteBeneficiariesAsync(int customerId);
        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesReadyForActivationAsync(DateTime currentDateTime);
        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesInCoolingPeriodAsync(DateTime currentDateTime);
        Task<bool> SetFavoriteAsync(int beneficiaryId, bool isFavorite);

        // 5. INTERNAL ACCOUNT AND TRANSACTION LIMITS
        Task<IEnumerable<BeneficiaryDto>> GetBeneficiariesByInternalAccountIdAsync(int internalAccountId);
        Task<IEnumerable<BeneficiaryDto>> GetActiveBeneficiariesWithinTransactionLimitAsync(int customerId, decimal amount);
        Task<bool> UpdatePerTransactionLimitAsync(int beneficiaryId, decimal? limit);

        // 6. STATUS AND ACTIVATION MANAGEMENT
        Task<bool> ActivateAsync(int beneficiaryId);
        Task<bool> BlockAsync(int beneficiaryId);
        Task<bool> RemoveAsync(int beneficiaryId);
        Task<bool> UpdateCoolingPeriodAsync(int beneficiaryId, DateTime coolingPeriodEndsAt);

        // 7. SEARCH AND VALIDATION
        Task<IEnumerable<BeneficiaryDto>> SearchAsync(int customerId, string searchTerm, int pageNumber, int pageSize);
        Task<bool> ExistsForCustomerAsync(int beneficiaryId, int customerId);
        Task<bool> HasActiveBankBeneficiaryAsync(int customerId, string accountNumber, string ifscCode);
        Task<bool> HasActiveUpiBeneficiaryAsync(int customerId, string upiVpa);
        Task<bool> CanBeneficiaryBeDeletedAsync(int beneficiaryId);
    }
}

//Banking rules:
//-Validate beneficiary ownership before every customer-specific operation.
//- Enforce the cooling period before activation.
//- Prevent duplicate active bank-account/IFSC and UPI beneficiaries for the same customer.
//- Validate beneficiary type: IntraBank, InterBank, or UPI.
//- Preserve RowVersion concurrency checks during updates.
//- Do not physically delete beneficiaries referenced by payment requests.