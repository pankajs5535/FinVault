using FinVault.Application.DTOs.CustomerDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        // 1. CUSTOMER CRUD AND IDENTIFICATION
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int customerId);
        Task<CustomerDto?> GetByCustomerNumberAsync(string customerNumber);
        Task<CustomerDto?> GetByMobileNumberAsync(string mobileNumber);
        Task<CustomerDto?> GetByEmailAsync(string email);
        Task<CustomerDto?> GetCustomerDetailsAsync(int customerId);
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerDto?> UpdateAsync(UpdateCustomerDto dto);
        Task<bool> DeleteAsync(DeleteCustomerDto dto);

        // 2. CUSTOMER CLASSIFICATION
        Task<IEnumerable<CustomerDto>> GetCustomersByTypeAsync(string customerType);
        Task<IEnumerable<CustomerDto>> GetCustomersByGenderAsync(string gender);
        Task<IEnumerable<CustomerDto>> GetCustomersByRiskCategoryAsync(string riskCategory);
        Task<IEnumerable<CustomerDto>> GetCustomersByStatusAsync(string status);

        // 3. BRANCH AND RELATIONSHIP MANAGEMENT
        Task<IEnumerable<CustomerDto>> GetCustomersByHomeBranchIdAsync(int homeBranchId);
        Task<IEnumerable<CustomerDto>> GetCustomersByRelationshipManagerIdAsync(int employeeId);
        Task<IEnumerable<CustomerDto>> GetCustomersByNationalityCountryIdAsync(int countryId);
        Task<CustomerDto?> GetCustomerWithBranchAndManagerAsync(int customerId);

        // 4. ACCOUNT, KYC AND ADDRESS RELATIONSHIPS
        Task<CustomerDto?> GetCustomerWithAccountsAsync(int customerId);
        Task<CustomerDto?> GetCustomerWithKycHistoryAsync(int customerId);
        Task<CustomerDto?> GetCustomerWithAddressesAsync(int customerId);
        Task<IEnumerable<CustomerDto>> GetCustomersWithOnlineBankingAsync();

        // 5. ONBOARDING AND AUDIT HISTORY
        Task<IEnumerable<CustomerDto>> GetCustomersOnboardedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<CustomerDto>> GetRecentlyOnboardedCustomersAsync(int count);
        Task<IEnumerable<CustomerDto>> GetCustomersUpdatedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 6. SEARCH AND VALIDATION
        Task<IEnumerable<CustomerDto>> SearchAsync(
            string? searchTerm,
            string? status,
            string? riskCategory,
            int? homeBranchId,
            int pageNumber,
            int pageSize);

        Task<bool> ExistsByCustomerNumberAsync(string customerNumber);
        Task<bool> ExistsByMobileNumberAsync(string mobileNumber);
        Task<bool> ExistsByEmailAsync(string email);
        Task<int> CountCustomerAccountsAsync(int customerId);
    }
}