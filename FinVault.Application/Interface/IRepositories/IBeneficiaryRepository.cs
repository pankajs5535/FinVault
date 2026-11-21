
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IBeneficiaryRepository : IGenericRepository<Beneficiary>
    {
        // =====================================================
        // 1. BENEFICIARY IDENTIFICATION
        // =====================================================

        // Finds a beneficiary using its unique ID and customer ownership.
        // Useful for ensuring customers can access only their own saved beneficiaries.
        Task<Beneficiary?> GetByIdAndCustomerIdAsync(int beneficiaryId, int customerId);

        // Retrieves a beneficiary with its customer, internal account and payment details.
        // Useful for reviewing the complete beneficiary information before a transfer.
        Task<Beneficiary?> GetBeneficiaryDetailsAsync(int beneficiaryId);


        // =====================================================
        // 2. CUSTOMER BENEFICIARY MANAGEMENT
        // =====================================================

        // Retrieves all beneficiaries belonging to a customer.
        // Useful for displaying the customer's saved payee list.
        Task<IEnumerable<Beneficiary>> GetBeneficiariesByCustomerIdAsync(int customerId);

        // Retrieves active beneficiaries belonging to a customer.
        // Useful for displaying payees currently eligible for transfer processing.
        Task<IEnumerable<Beneficiary>> GetActiveBeneficiariesByCustomerIdAsync(int customerId);

        // Retrieves pending beneficiaries belonging to a customer.
        // Useful for monitoring newly added payees awaiting activation.
        Task<IEnumerable<Beneficiary>> GetPendingBeneficiariesByCustomerIdAsync(int customerId);

        // Retrieves blocked beneficiaries belonging to a customer.
        // Useful for reviewing payees that cannot currently receive transfers.
        Task<IEnumerable<Beneficiary>> GetBlockedBeneficiariesByCustomerIdAsync(int customerId);

        // Retrieves removed beneficiaries belonging to a customer.
        // Useful for reviewing historical payee records without treating them as active.
        Task<IEnumerable<Beneficiary>> GetRemovedBeneficiariesByCustomerIdAsync(int customerId);


        // =====================================================
        // 3. BENEFICIARY TYPE AND PAYMENT IDENTIFICATION
        // =====================================================

        // Retrieves beneficiaries belonging to a particular transfer type.
        // Useful for separating intra-bank, inter-bank and UPI payees.
        Task<IEnumerable<Beneficiary>> GetBeneficiariesByTypeAsync(int customerId, string beneficiaryType);

        // Finds a beneficiary using the customer's account number and IFSC code.
        // Useful for preventing duplicate bank-account beneficiaries.
        Task<Beneficiary?> GetByAccountAndIfscAsync(int customerId, string accountNumber, string ifscCode);

        // Finds a beneficiary using the customer's UPI virtual payment address.
        // Useful for identifying an existing UPI payee.
        Task<Beneficiary?> GetByUpiVpaAsync(int customerId, string upiVpa);


        // =====================================================
        // 4. FAVORITES AND COOLING PERIOD
        // =====================================================

        // Retrieves favorite beneficiaries belonging to a customer.
        // Useful for displaying frequently used payees.
        Task<IEnumerable<Beneficiary>> GetFavoriteBeneficiariesAsync(int customerId);

        // Retrieves pending beneficiaries whose cooling period has ended.
        // Useful for identifying payees eligible for activation.
        Task<IEnumerable<Beneficiary>> GetBeneficiariesReadyForActivationAsync(DateTime currentDateTime);

        // Retrieves beneficiaries whose cooling period is still in progress.
        // Useful for informing customers when a newly added payee becomes available.
        Task<IEnumerable<Beneficiary>> GetBeneficiariesInCoolingPeriodAsync(DateTime currentDateTime);


        // =====================================================
        // 5. INTERNAL ACCOUNT AND TRANSACTION LIMITS
        // =====================================================

        // Retrieves beneficiaries linked to an internal FinVault account.
        // Useful for reviewing saved payees associated with internal accounts.
        Task<IEnumerable<Beneficiary>> GetBeneficiariesByInternalAccountIdAsync(int internalAccountId);

        // Retrieves active beneficiaries whose per-transaction limit meets the specified amount.
        // Useful for identifying payees whose configured limit can accommodate a transfer.
        Task<IEnumerable<Beneficiary>> GetActiveBeneficiariesWithinTransactionLimitAsync(int customerId, decimal amount);


        // =====================================================
        // 6. BENEFICIARY SEARCH AND VALIDATION
        // =====================================================

        // Searches a customer's beneficiaries by name, nickname, account number, IFSC or UPI VPA.
        // Pagination prevents loading large beneficiary lists at once.
        Task<IEnumerable<Beneficiary>> SearchBeneficiariesAsync(int customerId, string searchTerm, int pageNumber, int pageSize);

        // Checks whether a beneficiary belongs to the specified customer.
        // Useful for validating beneficiary ownership before payment processing.
        Task<bool> ExistsForCustomerAsync(int beneficiaryId, int customerId);

        // Checks whether a customer has any active beneficiary matching a bank account and IFSC.
        // Useful for preventing duplicate bank-account payee registrations.
        Task<bool> HasActiveBankBeneficiaryAsync(int customerId, string accountNumber, string ifscCode);

        // Checks whether a customer has any active beneficiary matching a UPI VPA.
        // Useful for preventing duplicate UPI payee registrations.
        Task<bool> HasActiveUpiBeneficiaryAsync(int customerId, string upiVpa);
    }
}
