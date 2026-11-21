
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class BeneficiaryRepository : GenericRepository<Beneficiary>, IBeneficiaryRepository
    {
        public BeneficiaryRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. BENEFICIARY IDENTIFICATION
        // =====================================================

        // Finds a beneficiary using its unique ID and customer ownership.
        // Useful for ensuring customers can access only their own saved beneficiaries.
        public async Task<Beneficiary?> GetByIdAndCustomerIdAsync(int beneficiaryId, int customerId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.BeneficiaryId == beneficiaryId && x.CustomerId == customerId);
        }

        // Retrieves a beneficiary with its customer, internal account and payment details.
        // Useful for reviewing the complete beneficiary information before a transfer.
        public async Task<Beneficiary?> GetBeneficiaryDetailsAsync(int beneficiaryId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.InternalAccount)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.BeneficiaryId == beneficiaryId);
        }


        // =====================================================
        // 2. CUSTOMER BENEFICIARY MANAGEMENT
        // =====================================================

        // Retrieves all beneficiaries belonging to a customer.
        // Useful for displaying the customer's saved payee list.
        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.BeneficiaryName)
                .ToListAsync();
        }

        // Retrieves active beneficiaries belonging to a customer.
        // Useful for displaying payees currently eligible for transfer processing.
        public async Task<IEnumerable<Beneficiary>> GetActiveBeneficiariesByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.Status == "Active")
                .OrderBy(x => x.BeneficiaryName)
                .ToListAsync();
        }

        // Retrieves pending beneficiaries belonging to a customer.
        // Useful for monitoring newly added payees awaiting activation.
        public async Task<IEnumerable<Beneficiary>> GetPendingBeneficiariesByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.Status == "Pending")
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves blocked beneficiaries belonging to a customer.
        // Useful for reviewing payees that cannot currently receive transfers.
        public async Task<IEnumerable<Beneficiary>> GetBlockedBeneficiariesByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.Status == "Blocked")
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // Retrieves removed beneficiaries belonging to a customer.
        // Useful for reviewing historical payee records without treating them as active.
        public async Task<IEnumerable<Beneficiary>> GetRemovedBeneficiariesByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.Status == "Removed")
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 3. BENEFICIARY TYPE AND PAYMENT IDENTIFICATION
        // =====================================================

        // Retrieves beneficiaries belonging to a particular transfer type.
        // Useful for separating intra-bank, inter-bank and UPI payees.
        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesByTypeAsync(int customerId, string beneficiaryType)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.BeneficiaryType == beneficiaryType)
                .OrderBy(x => x.BeneficiaryName)
                .ToListAsync();
        }

        // Finds a beneficiary using the customer's account number and IFSC code.
        // Useful for preventing duplicate bank-account beneficiaries.
        public async Task<Beneficiary?> GetByAccountAndIfscAsync(int customerId, string accountNumber, string ifscCode)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customerId &&
                    x.AccountNumber == accountNumber &&
                    x.IfscCode == ifscCode &&
                    x.Status != "Removed");
        }

        // Finds a beneficiary using the customer's UPI virtual payment address.
        // Useful for identifying an existing UPI payee.
        public async Task<Beneficiary?> GetByUpiVpaAsync(int customerId, string upiVpa)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customerId &&
                    x.UpiVpa == upiVpa &&
                    x.Status != "Removed");
        }


        // =====================================================
        // 4. FAVORITES AND COOLING PERIOD
        // =====================================================

        // Retrieves favorite beneficiaries belonging to a customer.
        // Useful for displaying frequently used payees.
        public async Task<IEnumerable<Beneficiary>> GetFavoriteBeneficiariesAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.IsFavorite && x.Status == "Active")
                .OrderBy(x => x.BeneficiaryName)
                .ToListAsync();
        }

        // Retrieves pending beneficiaries whose cooling period has ended.
        // Useful for identifying payees eligible for activation.
        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesReadyForActivationAsync(DateTime currentDateTime)
        {
            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.Status == "Pending" &&
                    x.CoolingPeriodEndsAt.HasValue &&
                    x.CoolingPeriodEndsAt.Value <= currentDateTime)
                .OrderBy(x => x.CoolingPeriodEndsAt)
                .ToListAsync();
        }

        // Retrieves beneficiaries whose cooling period is still in progress.
        // Useful for informing customers when a newly added payee becomes available.
        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesInCoolingPeriodAsync(DateTime currentDateTime)
        {
            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.Status == "Pending" &&
                    x.CoolingPeriodEndsAt.HasValue &&
                    x.CoolingPeriodEndsAt.Value > currentDateTime)
                .OrderBy(x => x.CoolingPeriodEndsAt)
                .ToListAsync();
        }


        // =====================================================
        // 5. INTERNAL ACCOUNT AND TRANSACTION LIMITS
        // =====================================================

        // Retrieves beneficiaries linked to an internal FinVault account.
        // Useful for reviewing saved payees associated with internal accounts.
        public async Task<IEnumerable<Beneficiary>> GetBeneficiariesByInternalAccountIdAsync(int internalAccountId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.InternalAccountId == internalAccountId)
                .OrderBy(x => x.BeneficiaryName)
                .ToListAsync();
        }

        // Retrieves active beneficiaries whose per-transaction limit meets the specified amount.
        // Useful for identifying payees whose configured limit can accommodate a transfer.
        public async Task<IEnumerable<Beneficiary>> GetActiveBeneficiariesWithinTransactionLimitAsync(int customerId, decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Transfer amount must be greater than zero.");

            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.CustomerId == customerId &&
                    x.Status == "Active" &&
                    (!x.PerTransactionLimit.HasValue || x.PerTransactionLimit.Value >= amount))
                .OrderBy(x => x.BeneficiaryName)
                .ToListAsync();
        }


        // =====================================================
        // 6. BENEFICIARY SEARCH AND VALIDATION
        // =====================================================

        // Searches a customer's beneficiaries by name, nickname, account number, IFSC or UPI VPA.
        // Pagination prevents loading large beneficiary lists at once.
        public async Task<IEnumerable<Beneficiary>> SearchBeneficiariesAsync(int customerId, string searchTerm, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty.", nameof(searchTerm));

            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            searchTerm = searchTerm.Trim();

            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.CustomerId == customerId &&
                    (x.BeneficiaryName.Contains(searchTerm) ||
                     (x.Nickname != null && x.Nickname.Contains(searchTerm)) ||
                     (x.AccountNumber != null && x.AccountNumber.Contains(searchTerm)) ||
                     (x.IfscCode != null && x.IfscCode.Contains(searchTerm)) ||
                     (x.UpiVpa != null && x.UpiVpa.Contains(searchTerm))))
                .OrderBy(x => x.BeneficiaryName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Checks whether a beneficiary belongs to the specified customer.
        // Useful for validating beneficiary ownership before payment processing.
        public async Task<bool> ExistsForCustomerAsync(int beneficiaryId, int customerId)
        {
            return await _dbSet.AnyAsync(x =>
                x.BeneficiaryId == beneficiaryId && x.CustomerId == customerId);
        }

        // Checks whether a customer has any active beneficiary matching a bank account and IFSC.
        // Useful for preventing duplicate bank-account payee registrations.
        public async Task<bool> HasActiveBankBeneficiaryAsync(int customerId, string accountNumber, string ifscCode)
        {
            return await _dbSet.AnyAsync(x =>
                x.CustomerId == customerId &&
                x.AccountNumber == accountNumber &&
                x.IfscCode == ifscCode &&
                x.Status == "Active");
        }

        // Checks whether a customer has any active beneficiary matching a UPI VPA.
        // Useful for preventing duplicate UPI payee registrations.
        public async Task<bool> HasActiveUpiBeneficiaryAsync(int customerId, string upiVpa)
        {
            return await _dbSet.AnyAsync(x =>
                x.CustomerId == customerId &&
                x.UpiVpa == upiVpa &&
                x.Status == "Active");
        }
    }
}
