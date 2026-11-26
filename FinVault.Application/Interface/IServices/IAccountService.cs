using FinVault.Application.DTOs.AccountDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IAccountService
    {
        // 1. ACCOUNT CRUD AND IDENTIFICATION
        Task<IEnumerable<AccountDto>> GetAllAsync();
        Task<AccountDto?> GetByIdAsync(int accountId);
        Task<AccountDto?> GetByAccountNumberAsync(string accountNumber);
        Task<AccountDto?> GetAccountDetailsAsync(int accountId);
        Task<AccountDto> CreateAsync(CreateAccountDto dto);
        Task<AccountDto?> UpdateAsync(UpdateAccountDto dto);
        Task<bool> DeleteAsync(DeleteAccountDto dto);

        // 2. ACCOUNT STATUS MANAGEMENT
        Task<IEnumerable<AccountDto>> GetAccountsByStatusAsync(string status);
        Task<IEnumerable<AccountDto>> GetActiveAccountsAsync();
        Task<bool> UpdateStatusAsync(int accountId, string status, string? statusReason);
        Task<bool> ActivateAsync(int accountId);
        Task<bool> FreezeAsync(int accountId, string reason);
        Task<bool> UnfreezeAsync(int accountId);
        Task<bool> MarkAsDormantAsync(int accountId);
        Task<bool> CloseAsync(int accountId, string reason);

        // 3. CUSTOMER AND ACCOUNT HOLDER RELATIONSHIPS
        Task<IEnumerable<AccountDto>> GetAccountsByCustomerIdAsync(int customerId);
        Task<IEnumerable<AccountDto>> GetJointAccountsAsync();

        // 4. BRANCH, ACCOUNT TYPE AND CURRENCY
        Task<IEnumerable<AccountDto>> GetAccountsByBranchIdAsync(int branchId);
        Task<IEnumerable<AccountDto>> GetAccountsByAccountTypeIdAsync(int accountTypeId);
        Task<IEnumerable<AccountDto>> GetAccountsByCurrencyIdAsync(int currencyId);

        // 5. BALANCE AND HOLD MONITORING
        Task<IEnumerable<AccountDto>> GetAccountsWithLowAvailableBalanceAsync(decimal threshold);
        Task<IEnumerable<AccountDto>> GetAccountsWithHoldsAsync();

        // 6. DEPOSIT MATURITY AND LINKED ACCOUNTS
        Task<IEnumerable<AccountDto>> GetLinkedDepositAccountsAsync(int payoutAccountId);
        Task<IEnumerable<AccountDto>> GetAccountsByMaturityDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AccountDto>> GetMaturedDepositAccountsAsync(DateTime asOfDate);

        // 7. ACCOUNT ACTIVITY
        Task<IEnumerable<AccountDto>> GetRecentlyActiveAccountsAsync(int count);

        // 8. SEARCH AND VALIDATION
        Task<IEnumerable<AccountDto>> SearchAsync(
            string? searchTerm,
            int? branchId,
            int? accountTypeId,
            int? currencyId,
            string? status,
            int pageNumber,
            int pageSize);

        Task<bool> IsAccountNumberExistsAsync(string accountNumber);
        Task<bool> IsAccountInUseAsync(int accountId);
        Task<bool> CanAccountBeDeletedAsync(int accountId);
    }
}


//Banking rules:
//-Never directly modify Balance, AvailableBalance, or HoldAmount through ordinary account CRUD; balance changes belong to the transaction/posting workflow.
//- Enforce valid status transitions and prevent closing accounts with outstanding balances, holds, or pending transactions.
//- Validate account number uniqueness, account type, branch, currency, and payout account.
//- Preserve RowVersion concurrency checks during updates.