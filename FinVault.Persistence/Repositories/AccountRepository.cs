
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class AccountRepository : GenericRepository<Account>, IAccountRepository
{
    public AccountRepository(FinVaultDbContext context) : base(context)
    {
    }

    // =====================================================
    // 1. ACCOUNT IDENTIFICATION
    // =====================================================

    // Finds an account using its unique account number.
    // Useful for customer account identification and banking operations.
    public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
    }

    // Retrieves an account with its account type, branch and currency.
    // Useful for displaying complete account information.
    public async Task<Account?> GetAccountDetailsAsync(int accountId)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.AccountType)
            .Include(x => x.Branch)
            .Include(x => x.Currency)
            .Include(x => x.PayoutAccount)
            .Include(x => x.AccountHolders)
                .ThenInclude(x => x.Customer)
            .FirstOrDefaultAsync(x => x.AccountId == accountId);
    }


    // =====================================================
    // 2. ACCOUNT STATUS AND BALANCES
    // =====================================================

    // Retrieves accounts having a particular status.
    // Useful for account lifecycle management and operational monitoring.
    public async Task<IEnumerable<Account>> GetAccountsByStatusAsync(string status)
    {
        return await _dbSet.AsNoTracking().Where(x => x.Status == status).OrderBy(x => x.AccountNumber).ToListAsync();
    }

    // Retrieves all active accounts.
    // Useful for identifying accounts currently available for banking operations.
    public async Task<IEnumerable<Account>> GetActiveAccountsAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.Status == "Active").OrderBy(x => x.AccountNumber).ToListAsync();
    }

    // Retrieves accounts belonging to a particular branch.
    // Useful for branch-level account administration and reporting.
    public async Task<IEnumerable<Account>> GetAccountsByBranchIdAsync(int branchId)
    {
        return await _dbSet.AsNoTracking().Where(x => x.BranchId == branchId).OrderBy(x => x.AccountNumber).ToListAsync();
    }

    // Retrieves accounts belonging to a particular account type.
    // Useful for savings, current and deposit product management.
    public async Task<IEnumerable<Account>> GetAccountsByAccountTypeIdAsync(int accountTypeId)
    {
        return await _dbSet.AsNoTracking().Where(x => x.AccountTypeId == accountTypeId).OrderBy(x => x.AccountNumber).ToListAsync();
    }

    // Retrieves accounts maintained in a particular currency.
    // Useful for currency-based account reporting and administration.
    public async Task<IEnumerable<Account>> GetAccountsByCurrencyIdAsync(int currencyId)
    {
        return await _dbSet.AsNoTracking().Where(x => x.CurrencyId == currencyId).OrderBy(x => x.AccountNumber).ToListAsync();
    }

    // Retrieves accounts whose available balance is below the supplied amount.
    // Useful for low-balance monitoring and customer notification workflows.
    public async Task<IEnumerable<Account>> GetAccountsWithLowAvailableBalanceAsync(decimal threshold)
    {
        return await _dbSet.AsNoTracking().Where(x => x.AvailableBalance < threshold && x.Status == "Active").OrderBy(x => x.AvailableBalance).ToListAsync();
    }

    // Retrieves accounts with an outstanding hold amount.
    // Useful for monitoring funds reserved for pending transactions or other obligations.
    public async Task<IEnumerable<Account>> GetAccountsWithHoldsAsync()
    {
        return await _dbSet.AsNoTracking().Where(x => x.HoldAmount > 0).OrderBy(x => x.AccountNumber).ToListAsync();
    }


    // =====================================================
    // 3. ACCOUNT HOLDERS AND RELATIONSHIPS
    // =====================================================

    // Retrieves accounts belonging to a particular customer.
    // Useful for displaying all accounts associated with a customer.
    public async Task<IEnumerable<Account>> GetAccountsByCustomerIdAsync(int customerId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AccountHolders.Any(h => h.CustomerId == customerId && h.IsActive))
            .OrderBy(x => x.AccountNumber)
            .ToListAsync();
    }

    // Retrieves accounts that have more than one active account holder.
    // Useful for identifying joint accounts and their operating arrangements.
    public async Task<IEnumerable<Account>> GetJointAccountsAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.AccountHolders.Count(h => h.IsActive) > 1)
            .OrderBy(x => x.AccountNumber)
            .ToListAsync();
    }

    // Retrieves accounts linked to a particular payout account.
    // Useful for reviewing deposit accounts that transfer maturity or interest proceeds.
    public async Task<IEnumerable<Account>> GetLinkedDepositAccountsAsync(int payoutAccountId)
    {
        return await _dbSet.AsNoTracking().Where(x => x.PayoutAccountId == payoutAccountId).OrderBy(x => x.AccountNumber).ToListAsync();
    }


    // =====================================================
    // 4. DEPOSIT MATURITY AND ACCOUNT ACTIVITY
    // =====================================================

    // Retrieves deposit accounts maturing within the supplied date range.
    // Useful for maturity notifications and deposit renewal processing.
    public async Task<IEnumerable<Account>> GetAccountsByMaturityDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be earlier than start date.");

        return await _dbSet.AsNoTracking()
            .Where(x => x.MaturityDate >= startDate && x.MaturityDate <= endDate && x.Status != "Closed")
            .OrderBy(x => x.MaturityDate)
            .ToListAsync();
    }

    // Retrieves accounts that have reached maturity and are not closed.
    // Useful for identifying deposits requiring maturity settlement or renewal.
    public async Task<IEnumerable<Account>> GetMaturedDepositAccountsAsync(DateTime asOfDate)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.MaturityDate.HasValue && x.MaturityDate.Value <= asOfDate && x.Status != "Closed")
            .OrderBy(x => x.MaturityDate)
            .ToListAsync();
    }

    // Retrieves accounts ordered by their most recent transaction date.
    // Useful for monitoring account activity and identifying inactive accounts.
    public async Task<IEnumerable<Account>> GetRecentlyActiveAccountsAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        return await _dbSet.AsNoTracking()
            .Where(x => x.LastTransactionAt.HasValue)
            .OrderByDescending(x => x.LastTransactionAt)
            .Take(count)
            .ToListAsync();
    }


    // =====================================================
    // 5. ACCOUNT SEARCH AND VALIDATION
    // =====================================================

    // Searches accounts by account number, nominee name or status.
    // Supports optional branch, account type, currency and status filters.
    // Pagination prevents loading large account lists at once.
    public async Task<IEnumerable<Account>> SearchAccountsAsync(string? searchTerm, int? branchId, int? accountTypeId, int? currencyId, string? status, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            query = query.Where(x => x.AccountNumber.Contains(searchTerm) ||
                                     (x.NomineeName != null && x.NomineeName.Contains(searchTerm)));
        }

        if (branchId.HasValue)
            query = query.Where(x => x.BranchId == branchId.Value);

        if (accountTypeId.HasValue)
            query = query.Where(x => x.AccountTypeId == accountTypeId.Value);

        if (currencyId.HasValue)
            query = query.Where(x => x.CurrencyId == currencyId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status.Trim());

        return await query.OrderBy(x => x.AccountNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Checks whether an account is referenced by other banking records.
    // Useful before account deletion or closure.
    public async Task<bool> IsAccountInUseAsync(int accountId)
    {
        return await _dbSet.AnyAsync(x => x.AccountId == accountId &&
            (x.LinkedDepositAccounts.Any() ||
             x.AccountHolders.Any() ||
             x.BeneficiaryReferences.Any() ||
             x.OutgoingTransactions.Any() ||
             x.IncomingTransactions.Any() ||
             x.JournalEntryLines.Any() ||
             x.PaymentRequests.Any() ||
             x.Cards.Any() ||
             x.DisbursementLoans.Any() ||
             x.RepaymentLoans.Any() ||
             x.InterestCalculations.Any() ||
             x.RiskAssessments.Any() ||
             x.FraudAlerts.Any() ||
             x.ComplianceRecords.Any()));
    }
}
