
using FinVault.Application.DTOs.CustomerDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICustomerService
    {

        // 1. CRUD AND CUSTOMER IDENTIFICATION


        Task<IEnumerable<CustomerDto>> GetAllAsync();

        Task<CustomerDto?> GetByIdAsync(int customerId);

        Task<CustomerDto?> GetByCustomerNumberAsync(string customerNumber);

        Task<CustomerDto?> GetByMobileNumberAsync(string mobileNumber);

        Task<CustomerDto?> GetByEmailAsync(string email);

        Task<CustomerDto?> GetCustomerDetailsAsync(int customerId);

        Task<CustomerDto> CreateAsync(CreateCustomerDto dto);

        Task<CustomerDto?> UpdateAsync(UpdateCustomerDto dto);

        Task<bool> DeleteAsync(int customerId);



        // 2. CUSTOMER PROFILE AND CLASSIFICATION


        Task<IEnumerable<CustomerDto>> GetByTypeAsync(string customerType);

        Task<IEnumerable<CustomerDto>> GetByGenderAsync(string gender);

        Task<IEnumerable<CustomerDto>> GetByRiskCategoryAsync(string riskCategory);

        Task<IEnumerable<CustomerDto>> GetByStatusAsync(string status);

        Task<bool> UpdateRiskCategoryAsync(int customerId, string riskCategory);

        Task<bool> UpdateCustomerStatusAsync(int customerId, string status);



        // 3. BRANCH AND RELATIONSHIP MANAGEMENT


        Task<IEnumerable<CustomerDto>> GetByHomeBranchIdAsync(int homeBranchId);

        Task<IEnumerable<CustomerDto>> GetByRelationshipManagerIdAsync(int employeeId);

        Task<IEnumerable<CustomerDto>> GetByNationalityCountryIdAsync(int countryId);

        Task<CustomerDto?> GetCustomerWithBranchAndManagerAsync(int customerId);

        Task<bool> AssignHomeBranchAsync(int customerId, int branchId);

        Task<bool> AssignRelationshipManagerAsync(int customerId, int employeeId);



        // 4. ACCOUNT AND KYC RELATIONSHIPS


        Task<CustomerDto?> GetCustomerWithAccountsAsync(int customerId);

        Task<CustomerDto?> GetCustomerWithKycHistoryAsync(int customerId);

        Task<CustomerDto?> GetCustomerWithAddressesAsync(int customerId);

        Task<IEnumerable<CustomerDto>> GetCustomersWithOnlineBankingAsync();

        Task<int> CountCustomerAccountsAsync(int customerId);



        // 5. ONBOARDING AND AUDIT


        Task<IEnumerable<CustomerDto>> GetOnboardedWithinRangeAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<CustomerDto>> GetRecentlyOnboardedAsync(int count);

        Task<IEnumerable<CustomerDto>> GetUpdatedWithinRangeAsync(DateTime startDate, DateTime endDate);



        // 6. SEARCH AND FILTER


        Task<IEnumerable<CustomerDto>> SearchAsync(
            string? searchTerm,
            string? status,
            string? riskCategory,
            int? homeBranchId,
            int pageNumber,
            int pageSize);



        // 7. VALIDATION AND EXISTENCE


        Task<bool> IsCustomerNumberExistsAsync(string customerNumber);

        Task<bool> IsMobileNumberExistsAsync(string mobileNumber);

        Task<bool> IsEmailExistsAsync(string email);

        Task<bool> IsCustomerActiveAsync(int customerId);

        Task<bool> IsCustomerBlockedAsync(int customerId);

        Task<bool> CanCustomerOpenAccountAsync(int customerId);

        Task<bool> CanCustomerPerformTransactionsAsync(int customerId);
    }
}
