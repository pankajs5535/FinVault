
using FinVault.Application.DTOs.AccountDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAccountService
    {

        // 1. CRUD AND ACCOUNT IDENTIFICATION


        Task<IEnumerable<AccountDto>> GetAllAsync();

        Task<AccountDto?> GetByIdAsync(int accountId);

        Task<AccountDto?> GetByAccountNumberAsync(string accountNumber);

        Task<AccountDto?> GetAccountDetailsAsync(int accountId);

        Task<AccountDto> CreateAsync(CreateAccountDto dto);

        Task<AccountDto?> UpdateAsync(UpdateAccountDto dto);

        Task<bool> DeleteAsync(int accountId);



        // 2. ACCOUNT STATUS MANAGEMENT


        Task<IEnumerable<AccountDto>> GetByStatusAsync(string status);

        Task<IEnumerable<AccountDto>> GetActiveAccountsAsync();

        Task<IEnumerable<AccountDto>> GetPendingAccountsAsync();

        Task<IEnumerable<AccountDto>> GetDormantAccountsAsync();

        Task<IEnumerable<AccountDto>> GetInactiveAccountsAsync();

        Task<IEnumerable<AccountDto>> GetFrozenAccountsAsync();

        Task<IEnumerable<AccountDto>> GetClosedAccountsAsync();

        Task<bool> ActivateAccountAsync(int accountId);

        Task<bool> FreezeAccountAsync(int accountId, string reason);

        Task<bool> UnfreezeAccountAsync(int accountId);

        Task<bool> MarkAsDormantAsync(int accountId);

        Task<bool> CloseAccountAsync(int accountId, string reason);



        // 3. BALANCE AND HOLD INFORMATION


        Task<IEnumerable<AccountDto>> GetAccountsWithLowAvailableBalanceAsync(decimal threshold);

        Task<IEnumerable<AccountDto>> GetAccountsWithHoldsAsync();

        Task<decimal> GetAccountBalanceAsync(int accountId);

        Task<decimal> GetAvailableBalanceAsync(int accountId);

        Task<decimal> GetHoldAmountAsync(int accountId);

        Task<bool> HasSufficientAvailableBalanceAsync(int accountId, decimal amount);



        // 4. CUSTOMER AND ACCOUNT HOLDER RELATIONSHIPS


        Task<IEnumerable<AccountDto>> GetByCustomerIdAsync(int customerId);

        Task<IEnumerable<AccountDto>> GetByBranchIdAsync(int branchId);

        Task<IEnumerable<AccountDto>> GetByAccountTypeIdAsync(int accountTypeId);

        Task<IEnumerable<AccountDto>> GetByCurrencyIdAsync(int currencyId);

        Task<IEnumerable<AccountDto>> GetJointAccountsAsync();

        Task<IEnumerable<AccountDto>> GetLinkedDepositAccountsAsync(int payoutAccountId);



        // 5. DEPOSIT AND MATURITY MANAGEMENT


        Task<IEnumerable<AccountDto>> GetAccountsByMaturityDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<AccountDto>> GetMaturedDepositAccountsAsync(DateTime asOfDate);

        Task<IEnumerable<AccountDto>> GetAutoRenewalAccountsAsync();

        Task<bool> UpdateAutoRenewalAsync(int accountId, bool autoRenew);

        Task<bool> UpdatePayoutAccountAsync(int accountId, int? payoutAccountId);



        // 6. ACCOUNT ACTIVITY


        Task<IEnumerable<AccountDto>> GetRecentlyActiveAccountsAsync(int count);

        Task<IEnumerable<AccountDto>> GetInactiveSinceAsync(DateTime lastTransactionBefore);

        Task<IEnumerable<AccountDto>> GetAccountsOpenedWithinRangeAsync(DateTime startDate, DateTime endDate);



        // 7. SEARCH AND FILTER
        

        Task<IEnumerable<AccountDto>> SearchAsync(
            string? searchTerm,
            int? branchId,
            int? accountTypeId,
            int? currencyId,
            string? status,
            int pageNumber,
            int pageSize);



        // 8. VALIDATION AND EXISTENCE


        Task<bool> IsAccountNumberExistsAsync(string accountNumber);

        Task<bool> IsAccountInUseAsync(int accountId);

        Task<bool> CanAccountBeClosedAsync(int accountId);

        Task<bool> CanAccountBeDeletedAsync(int accountId);

        Task<bool> IsAccountActiveAsync(int accountId);

        Task<bool> IsAccountFrozenAsync(int accountId);

        Task<bool> IsAccountEligibleForTransactionsAsync(int accountId);
    }
}
