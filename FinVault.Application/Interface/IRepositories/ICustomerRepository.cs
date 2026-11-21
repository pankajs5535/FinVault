
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        // =====================================================
        // 1. CUSTOMER IDENTIFICATION
        // =====================================================

        // Finds a customer using the unique customer number.
        // Useful for identifying a customer during banking operations.
        Task<Customer?> GetByCustomerNumberAsync(string customerNumber);

        // Finds a customer using the registered mobile number.
        // Useful for customer lookup and contact verification workflows.
        Task<Customer?> GetByMobileNumberAsync(string mobileNumber);

        // Finds a customer using the registered email address.
        // Useful for customer lookup and communication workflows.
        Task<Customer?> GetByEmailAsync(string email);

        // Retrieves a customer with identity, branch, account-holder and KYC details.
        // Useful for reviewing the customer's complete banking profile.
        Task<Customer?> GetCustomerDetailsAsync(int customerId);


        // =====================================================
        // 2. CUSTOMER PROFILE AND CLASSIFICATION
        // =====================================================

        // Retrieves customers belonging to a particular customer type.
        // Useful for separating individual, minor and business customer portfolios.
        Task<IEnumerable<Customer>> GetCustomersByTypeAsync(string customerType);

        // Retrieves customers matching a particular gender.
        // Useful for demographic reporting where legally and operationally appropriate.
        Task<IEnumerable<Customer>> GetCustomersByGenderAsync(string gender);

        // Retrieves customers matching a particular risk category.
        // Useful for monitoring low, medium and high AML risk customer portfolios.
        Task<IEnumerable<Customer>> GetCustomersByRiskCategoryAsync(string riskCategory);

        // Retrieves customers matching a particular status.
        // Useful for monitoring active, inactive, dormant, blocked and closed customers.
        Task<IEnumerable<Customer>> GetCustomersByStatusAsync(string status);


        // =====================================================
        // 3. BRANCH AND RELATIONSHIP MANAGEMENT
        // =====================================================

        // Retrieves customers assigned to a particular home branch.
        // Useful for branch-level customer portfolio management.
        Task<IEnumerable<Customer>> GetCustomersByHomeBranchIdAsync(int homeBranchId);

        // Retrieves customers assigned to a particular relationship manager.
        // Useful for reviewing an employee's managed customer portfolio.
        Task<IEnumerable<Customer>> GetCustomersByRelationshipManagerIdAsync(int employeeId);

        // Retrieves customers belonging to a particular nationality country.
        // Useful for nationality-based customer reporting.
        Task<IEnumerable<Customer>> GetCustomersByNationalityCountryIdAsync(int countryId);

        // Retrieves a customer with the home branch and relationship manager.
        // Useful for reviewing customer ownership and servicing responsibilities.
        Task<Customer?> GetCustomerWithBranchAndManagerAsync(int customerId);


        // =====================================================
        // 4. CUSTOMER ACCOUNT AND KYC RELATIONSHIPS
        // =====================================================

        // Retrieves a customer with all associated account-holder records and accounts.
        // Useful for reviewing accounts in which the customer is a holder.
        Task<Customer?> GetCustomerWithAccountsAsync(int customerId);

        // Retrieves a customer with the complete KYC history.
        // Useful for reviewing current and previous customer KYC records.
        Task<Customer?> GetCustomerWithKycHistoryAsync(int customerId);

        // Retrieves a customer with all registered addresses.
        // Useful for reviewing residential, permanent and correspondence addresses.
        Task<Customer?> GetCustomerWithAddressesAsync(int customerId);

        // Retrieves customers linked to an online banking user account.
        // Useful for identifying customers with internet or mobile banking access.
        Task<IEnumerable<Customer>> GetCustomersWithOnlineBankingAsync();


        // =====================================================
        // 5. CUSTOMER ONBOARDING AND AUDIT
        // =====================================================

        // Retrieves customers onboarded within a specified date range.
        // Useful for monitoring customer acquisition and onboarding activity.
        Task<IEnumerable<Customer>> GetCustomersOnboardedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves recently onboarded customers.
        // Useful for monitoring the latest customer registrations.
        Task<IEnumerable<Customer>> GetRecentlyOnboardedCustomersAsync(int count);

        // Retrieves customers whose records were updated within a specified date range.
        // Useful for reviewing customer profile maintenance activity.
        Task<IEnumerable<Customer>> GetCustomersUpdatedWithinRangeAsync(DateTime startDate, DateTime endDate);


        // =====================================================
        // 6. CUSTOMER SEARCH AND VALIDATION
        // =====================================================

        // Searches customers by customer number, name, mobile number or email.
        // Optional filters and pagination support efficient customer lookup.
        Task<IEnumerable<Customer>> SearchCustomersAsync(string? searchTerm, string? status, string? riskCategory, int? homeBranchId, int pageNumber, int pageSize);

        // Checks whether a customer number already exists.
        // Useful for preventing duplicate customer master records.
        Task<bool> ExistsByCustomerNumberAsync(string customerNumber);

        // Checks whether a mobile number is already registered.
        // Useful for validating customer contact information during onboarding.
        Task<bool> ExistsByMobileNumberAsync(string mobileNumber);

        // Checks whether an email address is already registered.
        // Useful for validating customer contact information during onboarding.
        Task<bool> ExistsByEmailAsync(string email);

        // Counts account-holder records associated with a customer.
        // Useful for reviewing the customer's account relationships.
        Task<int> CountCustomerAccountsAsync(int customerId);
    }
}
