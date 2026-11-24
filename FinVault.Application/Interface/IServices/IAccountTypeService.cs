
using FinVault.Application.DTOs.AccountTypeDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAccountTypeService
    {

        // 1. CRUD OPERATIONS


        Task<IEnumerable<AccountTypeDto>> GetAllAsync();

        Task<AccountTypeDto?> GetByIdAsync(int accountTypeId);

        Task<AccountTypeDto?> GetByCodeAsync(string code);

        Task<IEnumerable<AccountTypeDto>> GetByNameAsync(string name);

        Task<AccountTypeDto> CreateAsync(CreateAccountTypeDto dto);

        Task<AccountTypeDto?> UpdateAsync(UpdateAccountTypeDto dto);

        Task<bool> DeleteAsync(int accountTypeId);



        // 2. CATEGORY AND STATUS OPERATIONS


        Task<IEnumerable<AccountTypeDto>> GetByCategoryAsync(string category);

        Task<IEnumerable<AccountTypeDto>> GetActiveAccountTypesAsync();

        Task<IEnumerable<AccountTypeDto>> GetInactiveAccountTypesAsync();

        Task<bool> ActivateAsync(int accountTypeId);

        Task<bool> DeactivateAsync(int accountTypeId);



        // 3. INTEREST AND OVERDRAFT OPERATIONS


        Task<IEnumerable<AccountTypeDto>> GetInterestBearingAccountTypesAsync();

        Task<IEnumerable<AccountTypeDto>> GetOverdraftAccountTypesAsync();

        Task<IEnumerable<AccountTypeDto>> GetAccountTypesByTenureRangeAsync(
            int minTenureMonths,
            int maxTenureMonths);



        // 4. ACCOUNT TYPE RELATIONSHIPS


        Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithAccountsAsync();

        Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithInterestRatesAsync();

        Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithFeesAsync();

        Task<AccountTypeDto?> GetAccountTypeDetailsAsync(int accountTypeId);



        // 5. SEARCH AND FILTER OPERATIONS


        Task<IEnumerable<AccountTypeDto>> SearchAsync(
            string? searchTerm,
            string? category,
            bool? isActive,
            bool? isInterestBearing,
            int pageNumber,
            int pageSize);

        Task<IEnumerable<AccountTypeDto>> GetRecentlyCreatedAccountTypesAsync(int count);



        // 6. VALIDATION AND EXISTENCE OPERATIONS


        Task<bool> IsCodeExistsAsync(string code);

        Task<bool> IsNameExistsAsync(string name);

        Task<bool> IsAccountTypeInUseAsync(int accountTypeId);
    }
}
