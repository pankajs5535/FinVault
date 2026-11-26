using FinVault.Application.DTOs.AccountHolderDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAccountHolderService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<AccountHolderDto>> GetAllAsync();
        Task<AccountHolderDto?> GetByIdAsync(int accountHolderId);
        Task<AccountHolderDto?> GetByAccountAndCustomerAsync(int accountId, int customerId);
        Task<AccountHolderDto?> GetAccountHolderDetailsAsync(int accountHolderId);
        Task<AccountHolderDto> CreateAsync(CreateAccountHolderDto dto);
        Task<AccountHolderDto?> UpdateAsync(UpdateAccountHolderDto dto);
        Task<bool> DeleteAsync(DeleteAccountHolderDto dto);

        // 2. ACCOUNT AND CUSTOMER RELATIONSHIPS
        Task<IEnumerable<AccountHolderDto>> GetAccountHoldersByAccountIdAsync(int accountId);
        Task<IEnumerable<AccountHolderDto>> GetAccountHoldersByCustomerIdAsync(int customerId);

        // 3. STATUS AND HOLDER CLASSIFICATION
        Task<IEnumerable<AccountHolderDto>> GetActiveAccountHoldersAsync();
        Task<IEnumerable<AccountHolderDto>> GetInactiveAccountHoldersAsync();
        Task<IEnumerable<AccountHolderDto>> GetPrimaryAccountHoldersAsync();
        Task<IEnumerable<AccountHolderDto>> GetJointAccountHoldersAsync();
        Task<IEnumerable<AccountHolderDto>> GetGuardianAccountHoldersAsync();
        Task<IEnumerable<AccountHolderDto>> GetAuthorizedSignatoriesAsync();
        Task<IEnumerable<AccountHolderDto>> GetAccountOperatorsAsync(int accountId);

        // 4. EFFECTIVE PERIOD MANAGEMENT
        Task<IEnumerable<AccountHolderDto>> GetAccountHoldersEffectiveOnDateAsync(DateTime effectiveDate);
        Task<IEnumerable<AccountHolderDto>> GetAccountHoldersEndingWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 5. PERMISSIONS AND STATUS MANAGEMENT
        Task<bool> ActivateAsync(int accountHolderId);
        Task<bool> DeactivateAsync(int accountHolderId);
        Task<bool> UpdateOperatingPermissionAsync(int accountHolderId, bool canOperate);
        Task<bool> UpdateEffectivePeriodAsync(int accountHolderId, DateTime effectiveFrom, DateTime? effectiveTo);

        // 6. SEARCH AND VALIDATION
        Task<IEnumerable<AccountHolderDto>> SearchAsync(
            int? accountId,
            int? customerId,
            string? holderType,
            bool? isActive,
            bool? canOperate,
            int pageNumber,
            int pageSize);

        Task<bool> IsActiveAccountHolderAsync(int accountId, int customerId);
        Task<bool> CanCustomerOperateAccountAsync(int accountId, int customerId);
    }
}


//Banking rules:
//-Prevent duplicate active account - holder relationships for the same account and customer.
//- Enforce valid holder types: Primary, Joint, Guardian, AuthorizedSignatory.
//- Validate effective dates; EffectiveTo cannot precede EffectiveFrom.
//- Only active holders with CanOperate = true can operate accounts.
//- Enforce the account's primary-holder and guardian rules before changes.