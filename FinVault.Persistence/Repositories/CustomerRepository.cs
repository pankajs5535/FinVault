
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. CUSTOMER IDENTIFICATION
        // =====================================================

        // Finds a customer using the unique customer number.
        // Useful for identifying a customer during banking operations.
        public async Task<Customer?> GetByCustomerNumberAsync(string customerNumber)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerNumber == customerNumber);
        }

        // Finds a customer using the registered mobile number.
        // Useful for customer lookup and contact verification workflows.
        public async Task<Customer?> GetByMobileNumberAsync(string mobileNumber)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.MobileNumber == mobileNumber);
        }

        // Finds a customer using the registered email address.
        // Useful for customer lookup and communication workflows.
        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        // Retrieves a customer with identity, branch, account-holder and KYC details.
        // Useful for reviewing the customer's complete banking profile.
        public async Task<Customer?> GetCustomerDetailsAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.HomeBranch)
                .Include(x => x.RelationshipManager)
                .Include(x => x.NationalityCountry)
                .Include(x => x.Addresses)
                .Include(x => x.KYCDetailsHistory)
                .Include(x => x.AccountHolders)
                    .ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }


        // =====================================================
        // 2. CUSTOMER PROFILE AND CLASSIFICATION
        // =====================================================

        // Retrieves customers belonging to a particular customer type.
        // Useful for separating individual, minor and business customer portfolios.
        public async Task<IEnumerable<Customer>> GetCustomersByTypeAsync(string customerType)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerType == customerType)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves customers matching a particular gender.
        // Useful for demographic reporting where legally and operationally appropriate.
        public async Task<IEnumerable<Customer>> GetCustomersByGenderAsync(string gender)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Gender == gender)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves customers matching a particular risk category.
        // Useful for monitoring low, medium and high AML risk customer portfolios.
        public async Task<IEnumerable<Customer>> GetCustomersByRiskCategoryAsync(string riskCategory)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.RiskCategory == riskCategory)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves customers matching a particular status.
        // Useful for monitoring active, inactive, dormant, blocked and closed customers.
        public async Task<IEnumerable<Customer>> GetCustomersByStatusAsync(string status)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Status == status)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }


        // =====================================================
        // 3. BRANCH AND RELATIONSHIP MANAGEMENT
        // =====================================================

        // Retrieves customers assigned to a particular home branch.
        // Useful for branch-level customer portfolio management.
        public async Task<IEnumerable<Customer>> GetCustomersByHomeBranchIdAsync(int homeBranchId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.HomeBranchId == homeBranchId)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves customers assigned to a particular relationship manager.
        // Useful for reviewing an employee's managed customer portfolio.
        public async Task<IEnumerable<Customer>> GetCustomersByRelationshipManagerIdAsync(int employeeId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.RelationshipManagerId == employeeId)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves customers belonging to a particular nationality country.
        // Useful for nationality-based customer reporting.
        public async Task<IEnumerable<Customer>> GetCustomersByNationalityCountryIdAsync(int countryId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.NationalityCountryId == countryId)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves a customer with the home branch and relationship manager.
        // Useful for reviewing customer ownership and servicing responsibilities.
        public async Task<Customer?> GetCustomerWithBranchAndManagerAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.HomeBranch)
                .Include(x => x.RelationshipManager)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }


        // =====================================================
        // 4. CUSTOMER ACCOUNT AND KYC RELATIONSHIPS
        // =====================================================

        // Retrieves a customer with all associated account-holder records and accounts.
        // Useful for reviewing accounts in which the customer is a holder.
        public async Task<Customer?> GetCustomerWithAccountsAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.AccountHolders)
                    .ThenInclude(x => x.Account)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        // Retrieves a customer with the complete KYC history.
        // Useful for reviewing current and previous customer KYC records.
        public async Task<Customer?> GetCustomerWithKycHistoryAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.KYCDetailsHistory)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        // Retrieves a customer with all registered addresses.
        // Useful for reviewing residential, permanent and correspondence addresses.
        public async Task<Customer?> GetCustomerWithAddressesAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Addresses)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        // Retrieves customers linked to an online banking user account.
        // Useful for identifying customers with internet or mobile banking access.
        public async Task<IEnumerable<Customer>> GetCustomersWithOnlineBankingAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.UserId.HasValue)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }


        // =====================================================
        // 5. CUSTOMER ONBOARDING AND AUDIT
        // =====================================================

        // Retrieves customers onboarded within a specified date range.
        // Useful for monitoring customer acquisition and onboarding activity.
        public async Task<IEnumerable<Customer>> GetCustomersOnboardedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.OnboardedAt >= startDate && x.OnboardedAt <= endDate)
                .OrderByDescending(x => x.OnboardedAt)
                .ToListAsync();
        }

        // Retrieves recently onboarded customers.
        // Useful for monitoring the latest customer registrations.
        public async Task<IEnumerable<Customer>> GetRecentlyOnboardedCustomersAsync(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Customer count must be greater than zero.");

            return await _dbSet.AsNoTracking()
                .OrderByDescending(x => x.OnboardedAt)
                .Take(count)
                .ToListAsync();
        }

        // Retrieves customers whose records were updated within a specified date range.
        // Useful for reviewing customer profile maintenance activity.
        public async Task<IEnumerable<Customer>> GetCustomersUpdatedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.UpdatedAt.HasValue && x.UpdatedAt.Value >= startDate && x.UpdatedAt.Value <= endDate)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 6. CUSTOMER SEARCH AND VALIDATION
        // =====================================================

        // Searches customers by customer number, name, mobile number or email.
        // Optional filters and pagination support efficient customer lookup.
        public async Task<IEnumerable<Customer>> SearchCustomersAsync(string? searchTerm, string? status, string? riskCategory, int? homeBranchId, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            var query = _dbSet.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(x =>
                    x.CustomerNumber.Contains(searchTerm) ||
                    x.FirstName.Contains(searchTerm) ||
                    (x.MiddleName != null && x.MiddleName.Contains(searchTerm)) ||
                    x.LastName.Contains(searchTerm) ||
                    x.MobileNumber.Contains(searchTerm) ||
                    (x.Email != null && x.Email.Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status.Trim());

            if (!string.IsNullOrWhiteSpace(riskCategory))
                query = query.Where(x => x.RiskCategory == riskCategory.Trim());

            if (homeBranchId.HasValue)
                query = query.Where(x => x.HomeBranchId == homeBranchId.Value);

            return await query.OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Checks whether a customer number already exists.
        // Useful for preventing duplicate customer master records.
        public async Task<bool> ExistsByCustomerNumberAsync(string customerNumber)
        {
            return await _dbSet.AnyAsync(x => x.CustomerNumber == customerNumber);
        }

        // Checks whether a mobile number is already registered.
        // Useful for validating customer contact information during onboarding.
        public async Task<bool> ExistsByMobileNumberAsync(string mobileNumber)
        {
            return await _dbSet.AnyAsync(x => x.MobileNumber == mobileNumber);
        }

        // Checks whether an email address is already registered.
        // Useful for validating customer contact information during onboarding.
        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _dbSet.AnyAsync(x => x.Email == email);
        }

        // Counts account-holder records associated with a customer.
        // Useful for reviewing the customer's account relationships.
        public async Task<int> CountCustomerAccountsAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .SelectMany(x => x.AccountHolders)
                .CountAsync();
        }
    }
}
