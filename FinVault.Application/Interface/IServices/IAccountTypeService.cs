using FinVault.Application.DTOs.AccountTypeDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAccountTypeService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<AccountTypeDto>> GetAllAsync();
        Task<AccountTypeDto?> GetByIdAsync(int accountTypeId);
        Task<AccountTypeDto?> GetByCodeAsync(string code);
        Task<IEnumerable<AccountTypeDto>> GetByNameAsync(string name);
        Task<AccountTypeDto?> GetAccountTypeDetailsAsync(int accountTypeId);
        Task<AccountTypeDto> CreateAsync(CreateAccountTypeDto dto);
        Task<AccountTypeDto?> UpdateAsync(UpdateAccountTypeDto dto);
        Task<bool> DeleteAsync(DeleteAccountTypeDto dto);

        // 2. CATEGORY AND STATUS
        Task<IEnumerable<AccountTypeDto>> GetByCategoryAsync(string category);
        Task<IEnumerable<AccountTypeDto>> GetActiveAccountTypesAsync();
        Task<IEnumerable<AccountTypeDto>> GetInactiveAccountTypesAsync();
        Task<bool> ActivateAsync(int accountTypeId);
        Task<bool> DeactivateAsync(int accountTypeId);

        // 3. ACCOUNT PRODUCT FEATURES
        Task<IEnumerable<AccountTypeDto>> GetInterestBearingAccountTypesAsync();
        Task<IEnumerable<AccountTypeDto>> GetOverdraftAccountTypesAsync();
        Task<IEnumerable<AccountTypeDto>> GetAccountTypesByTenureRangeAsync(
            int minTenureMonths,
            int maxTenureMonths);

        // 4. BUSINESS RELATIONSHIPS
        Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithAccountsAsync();
        Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithInterestRatesAsync();
        Task<IEnumerable<AccountTypeDto>> GetAccountTypesWithFeesAsync();

        // 5. SEARCH AND ADMINISTRATION
        Task<IEnumerable<AccountTypeDto>> GetRecentlyCreatedAccountTypesAsync(int count);
        Task<IEnumerable<AccountTypeDto>> SearchAsync(
            string? searchTerm,
            string? category,
            bool? isActive,
            bool? isInterestBearing,
            int pageNumber,
            int pageSize);

        // 6. VALIDATION AND USAGE
        Task<bool> IsCodeExistsAsync(string code);
        Task<bool> IsNameExistsAsync(string name);
        Task<bool> IsAccountTypeInUseAsync(int accountTypeId);
        Task<bool> CanAccountTypeBeDeletedAsync(int accountTypeId);
    }
}


//Banking rules:
//-Account type codes and names must be unique.
//- Minimum balance and average balance cannot be negative.
//- Minimum tenure must not exceed maximum tenure.
//- Interest payout frequency must be valid for interest-bearing products.
//- Prevent deletion when accounts, interest rates, or fees reference the account type.
//- Do not deactivate an account type without considering existing accounts.