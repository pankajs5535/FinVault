
using FinVault.Application.DTOs.AccountHolderDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAccountHolderService
    {
      
        // 1. CRUD OPERATIONS
      

        Task<IEnumerable<AccountHolderDto>> GetAllAsync();

        Task<AccountHolderDto?> GetByIdAsync(int accountHolderId);

        Task<AccountHolderDto?> GetByAccountAndCustomerAsync(int accountId, int customerId);

        Task<AccountHolderDto?> GetAccountHolderDetailsAsync(int accountHolderId);

        Task<AccountHolderDto> CreateAsync(CreateAccountHolderDto dto);

        Task<AccountHolderDto?> UpdateAsync(UpdateAccountHolderDto dto);

        Task<bool> DeleteAsync(int accountHolderId);


      
        // 2. ACCOUNT AND CUSTOMER RELATIONSHIPS
      

        Task<IEnumerable<AccountHolderDto>> GetByAccountIdAsync(int accountId);

        Task<IEnumerable<AccountHolderDto>> GetByCustomerIdAsync(int customerId);

        Task<IEnumerable<AccountHolderDto>> GetActiveAccountHoldersAsync();

        Task<IEnumerable<AccountHolderDto>> GetInactiveAccountHoldersAsync();


      
        // 3. HOLDER TYPES AND OPERATING PERMISSIONS
      

        Task<IEnumerable<AccountHolderDto>> GetPrimaryAccountHoldersAsync();

        Task<IEnumerable<AccountHolderDto>> GetJointAccountHoldersAsync();

        Task<IEnumerable<AccountHolderDto>> GetGuardianAccountHoldersAsync();

        Task<IEnumerable<AccountHolderDto>> GetAuthorizedSignatoriesAsync();

        Task<IEnumerable<AccountHolderDto>> GetAccountOperatorsAsync(int accountId);

        Task<bool> UpdateOperatingPermissionAsync(int accountHolderId, bool canOperate);


      
        // 4. EFFECTIVE PERIOD MANAGEMENT
      

        Task<IEnumerable<AccountHolderDto>> GetEffectiveOnDateAsync(DateTime effectiveDate);

        Task<IEnumerable<AccountHolderDto>> GetEndingWithinRangeAsync(DateTime startDate, DateTime endDate);

        Task<bool> EndAccountHolderRelationshipAsync(int accountHolderId, DateTime effectiveTo);

        Task<bool> ReactivateAccountHolderAsync(int accountHolderId);


      
        // 5. SEARCH AND FILTER
      

        Task<IEnumerable<AccountHolderDto>> SearchAsync(
            int? accountId,
            int? customerId,
            string? holderType,
            bool? isActive,
            bool? canOperate,
            int pageNumber,
            int pageSize);


      
        // 6. VALIDATION AND AUTHORIZATION
      

        Task<bool> IsActiveAccountHolderAsync(int accountId, int customerId);

        Task<bool> CanCustomerOperateAccountAsync(int accountId, int customerId);

        Task<bool> IsPrimaryHolderAsync(int accountId, int customerId);

        Task<bool> CanAddAccountHolderAsync(int accountId, int customerId);

        Task<bool> HasActiveAccountHolderAsync(int accountId, int customerId);
    }
}
