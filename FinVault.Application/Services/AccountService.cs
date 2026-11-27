using AutoMapper;
using FinVault.Application.DTOs.AccountDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AccountService : IAccountService
{
    private static readonly string[] ValidStatuses =
    {
        "Pending", "Active", "Dormant", "Inactive", "Frozen", "Closed"
    };

    private static readonly string[] ValidModes =
    {
        "Single", "Joint", "EitherOrSurvivor", "FormerOrSurvivor"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. ACCOUNT CRUD AND IDENTIFICATION

    public async Task<IEnumerable<AccountDto>> GetAllAsync()
    {
        var accounts = await _unitOfWork.Account.GetAllAsync();
        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<AccountDto?> GetByIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);
        return _mapper.Map<AccountDto?>(account);
    }

    public async Task<AccountDto?> GetByAccountNumberAsync(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return null;

        var account = await _unitOfWork.Account
            .GetByAccountNumberAsync(NormalizeAccountNumber(accountNumber));

        return _mapper.Map<AccountDto?>(account);
    }

    public async Task<AccountDto?> GetAccountDetailsAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var account = await _unitOfWork.Account
            .GetAccountDetailsAsync(accountId);

        return _mapper.Map<AccountDto?>(account);
    }

    public async Task<AccountDto> CreateAsync(CreateAccountDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var accountNumber = NormalizeAccountNumber(dto.AccountNumber);
        var status = NormalizeStatus(dto.Status);
        var mode = NormalizeMode(dto.ModeOfOperation);

        ValidateAccountDates(dto.OpenedOn, dto.ClosedOn, status);
        ValidateAccountTerms(
            dto.OverdraftLimit,
            dto.PrincipalAmount,
            dto.MaturityAmount,
            dto.ContractedInterestRate,
            dto.TenureMonths,
            dto.MaturityDate);

        if (await IsAccountNumberExistsAsync(accountNumber))
            throw new InvalidOperationException(
                $"Account number '{accountNumber}' already exists.");

        var accountType = await _unitOfWork.AccountType
            .GetByIdAsync(dto.AccountTypeId);

        if (accountType == null || !accountType.IsActive)
            throw new InvalidOperationException(
                "The selected account type does not exist or is inactive.");

        var branch = await _unitOfWork.Branch.GetByIdAsync(dto.BranchId);

        if (branch == null || !branch.IsActive || branch.ClosedOn.HasValue)
            throw new InvalidOperationException(
                "The selected branch does not exist or is inactive.");

        var currency = await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId);

        if (currency == null || !currency.IsActive)
            throw new InvalidOperationException(
                "The selected currency does not exist or is inactive.");

        await ValidateDepositTermsAsync(
            accountType,
            dto.PrincipalAmount,
            dto.MaturityAmount,
            dto.ContractedInterestRate,
            dto.TenureMonths,
            dto.MaturityDate);

        await ValidatePayoutAccountAsync(
            dto.PayoutAccountId,
            dto.CurrencyId,
            null);

        if (status == "Closed")
            throw new InvalidOperationException(
                "An account cannot be created directly in Closed status.");

        var account = _mapper.Map<Account>(dto);

        account.AccountNumber = accountNumber;
        account.Status = status;
        account.ModeOfOperation = mode;
        account.StatusReason = NormalizeOptionalText(dto.StatusReason);
        account.NomineeName = NormalizeOptionalText(dto.NomineeName);
        account.NomineeRelationship =
            NormalizeOptionalText(dto.NomineeRelationship);

        account.OpenedOn = dto.OpenedOn == default
            ? DateTime.UtcNow
            : dto.OpenedOn;

        account.ClosedOn = null;

        // Balances must only be changed through the transaction/posting workflow.
        account.Balance = 0m;
        account.AvailableBalance = 0m;
        account.HoldAmount = 0m;

        account.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Account.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountDto>(account);
    }

    public async Task<AccountDto?> UpdateAsync(UpdateAccountDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AccountId, nameof(dto.AccountId));

        var account = await _unitOfWork.Account.GetByIdAsync(dto.AccountId);

        if (account == null)
            return null;

        if (dto.RowVersion == null ||
            dto.RowVersion.Length == 0 ||
            account.RowVersion == null ||
            !account.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "The account was modified by another operation. Reload the account and try again.");
        }

        var accountNumber = NormalizeAccountNumber(dto.AccountNumber);
        var status = NormalizeStatus(dto.Status);
        var mode = NormalizeMode(dto.ModeOfOperation);

        ValidateAccountDates(dto.OpenedOn, dto.ClosedOn, status);
        ValidateAccountTerms(
            dto.OverdraftLimit,
            dto.PrincipalAmount,
            dto.MaturityAmount,
            dto.ContractedInterestRate,
            dto.TenureMonths,
            dto.MaturityDate);

        var existingAccount = await _unitOfWork.Account
            .GetByAccountNumberAsync(accountNumber);

        if (existingAccount != null &&
            existingAccount.AccountId != dto.AccountId)
        {
            throw new InvalidOperationException(
                $"Account number '{accountNumber}' already exists.");
        }

        var accountType = await _unitOfWork.AccountType
            .GetByIdAsync(dto.AccountTypeId);

        if (accountType == null || !accountType.IsActive)
            throw new InvalidOperationException(
                "The selected account type does not exist or is inactive.");

        var branch = await _unitOfWork.Branch.GetByIdAsync(dto.BranchId);

        if (branch == null || !branch.IsActive || branch.ClosedOn.HasValue)
            throw new InvalidOperationException(
                "The selected branch does not exist or is inactive.");

        var currency = await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId);

        if (currency == null || !currency.IsActive)
            throw new InvalidOperationException(
                "The selected currency does not exist or is inactive.");

        await ValidateDepositTermsAsync(
            accountType,
            dto.PrincipalAmount,
            dto.MaturityAmount,
            dto.ContractedInterestRate,
            dto.TenureMonths,
            dto.MaturityDate);

        await ValidatePayoutAccountAsync(
            dto.PayoutAccountId,
            dto.CurrencyId,
            dto.AccountId);

        if (account.Status == "Closed" && status != "Closed")
            throw new InvalidOperationException(
                "A closed account cannot be reopened through ordinary account update.");

        if (status != account.Status &&
            !IsValidStatusTransition(account.Status, status))
        {
            throw new InvalidOperationException(
                $"The account cannot transition from '{account.Status}' to '{status}'.");
        }

        if (status == "Closed")
            ValidateCanClose(account);

        var existingBalance = account.Balance;
        var existingAvailableBalance = account.AvailableBalance;
        var existingHoldAmount = account.HoldAmount;
        var existingRowVersion = account.RowVersion;
        var existingCreatedAt = account.CreatedAt;
        var existingCreatedBy = account.CreatedBy;
        var existingClosedOn = account.ClosedOn;

        _mapper.Map(dto, account);

        account.AccountNumber = accountNumber;
        account.Status = status;
        account.ModeOfOperation = mode;
        account.StatusReason = NormalizeOptionalText(dto.StatusReason);
        account.NomineeName = NormalizeOptionalText(dto.NomineeName);
        account.NomineeRelationship =
            NormalizeOptionalText(dto.NomineeRelationship);

        // Never allow ordinary CRUD to modify financial balances.
        account.Balance = existingBalance;
        account.AvailableBalance = existingAvailableBalance;
        account.HoldAmount = existingHoldAmount;

        // Preserve database-generated and audit fields.
        account.RowVersion = existingRowVersion;
        account.CreatedAt = existingCreatedAt;
        account.CreatedBy = existingCreatedBy;

        account.ClosedOn = status == "Closed"
            ? existingClosedOn ?? dto.ClosedOn ?? DateTime.UtcNow
            : null;

        account.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AccountDto>(account);
    }

    public async Task<bool> DeleteAsync(DeleteAccountDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AccountId, nameof(dto.AccountId));

        var account = await _unitOfWork.Account.GetByIdAsync(dto.AccountId);

        if (account == null ||
            !await CanAccountBeDeletedAsync(dto.AccountId))
            return false;

        _unitOfWork.Account.Delete(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. ACCOUNT STATUS MANAGEMENT

    public async Task<IEnumerable<AccountDto>> GetAccountsByStatusAsync(string status)
    {
        status = NormalizeStatus(status);

        var accounts = await _unitOfWork.Account
            .GetAccountsByStatusAsync(status);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<IEnumerable<AccountDto>> GetActiveAccountsAsync()
    {
        var accounts = await _unitOfWork.Account.GetActiveAccountsAsync();
        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<bool> UpdateStatusAsync(
        int accountId,
        string status,
        string? statusReason)
    {
        ValidateId(accountId, nameof(accountId));

        status = NormalizeStatus(status);
        statusReason = NormalizeOptionalText(statusReason);

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null)
            return false;

        if (account.Status == status)
            return true;

        if (!IsValidStatusTransition(account.Status, status))
            throw new InvalidOperationException(
                $"The account cannot transition from '{account.Status}' to '{status}'.");

        if (status == "Frozen" && string.IsNullOrWhiteSpace(statusReason))
            throw new ArgumentException(
                "A reason is required when freezing an account.",
                nameof(statusReason));

        if (status == "Closed")
            ValidateCanClose(account);

        account.Status = status;
        account.StatusReason = statusReason;
        account.ClosedOn = status == "Closed"
            ? DateTime.UtcNow
            : null;
        account.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ActivateAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null)
            return false;

        if (account.Status == "Active")
            return true;

        if (account.Status == "Frozen")
            throw new InvalidOperationException(
                "Use UnfreezeAsync to reactivate a frozen account.");

        if (!IsValidStatusTransition(account.Status, "Active"))
            throw new InvalidOperationException(
                $"An account in '{account.Status}' status cannot be activated.");

        account.Status = "Active";
        account.StatusReason = null;
        account.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> FreezeAsync(int accountId, string reason)
    {
        ValidateId(accountId, nameof(accountId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "A reason is required when freezing an account.",
                nameof(reason));

        return await UpdateStatusAsync(accountId, "Frozen", reason.Trim());
    }

    public async Task<bool> UnfreezeAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null)
            return false;

        if (account.Status != "Frozen")
            return false;

        account.Status = "Active";
        account.StatusReason = null;
        account.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkAsDormantAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null)
            return false;

        if (account.Status == "Dormant")
            return true;

        if (account.Status != "Active")
            throw new InvalidOperationException(
                "Only an active account can be marked as dormant.");

        account.Status = "Dormant";
        account.StatusReason = "Marked dormant.";
        account.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CloseAsync(int accountId, string reason)
    {
        ValidateId(accountId, nameof(accountId));

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException(
                "A closing reason is required.",
                nameof(reason));

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null)
            return false;

        if (account.Status == "Closed")
            return true;

        if (!IsValidStatusTransition(account.Status, "Closed"))
            throw new InvalidOperationException(
                $"An account in '{account.Status}' status cannot be closed.");

        ValidateCanClose(account);

        account.Status = "Closed";
        account.StatusReason = reason.Trim();
        account.ClosedOn = DateTime.UtcNow;
        account.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. CUSTOMER AND ACCOUNT HOLDER RELATIONSHIPS

    public async Task<IEnumerable<AccountDto>> GetAccountsByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var customer = await _unitOfWork.Customer.GetByIdAsync(customerId);

        if (customer == null)
            return Enumerable.Empty<AccountDto>();

        var accounts = await _unitOfWork.Account
            .GetAccountsByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<IEnumerable<AccountDto>> GetJointAccountsAsync()
    {
        var accounts = await _unitOfWork.Account.GetJointAccountsAsync();
        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    // 4. BRANCH, ACCOUNT TYPE AND CURRENCY

    public async Task<IEnumerable<AccountDto>> GetAccountsByBranchIdAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        var accounts = await _unitOfWork.Account
            .GetAccountsByBranchIdAsync(branchId);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsByAccountTypeIdAsync(
        int accountTypeId)
    {
        ValidateId(accountTypeId, nameof(accountTypeId));

        var accounts = await _unitOfWork.Account
            .GetAccountsByAccountTypeIdAsync(accountTypeId);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsByCurrencyIdAsync(int currencyId)
    {
        ValidateId(currencyId, nameof(currencyId));

        var accounts = await _unitOfWork.Account
            .GetAccountsByCurrencyIdAsync(currencyId);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    // 5. BALANCE AND HOLD MONITORING

    public async Task<IEnumerable<AccountDto>> GetAccountsWithLowAvailableBalanceAsync(
        decimal threshold)
    {
        if (threshold < 0)
            throw new ArgumentOutOfRangeException(nameof(threshold));

        var accounts = await _unitOfWork.Account
            .GetAccountsWithLowAvailableBalanceAsync(threshold);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsWithHoldsAsync()
    {
        var accounts = await _unitOfWork.Account.GetAccountsWithHoldsAsync();
        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    // 6. DEPOSIT MATURITY AND LINKED ACCOUNTS

    public async Task<IEnumerable<AccountDto>> GetLinkedDepositAccountsAsync(
        int payoutAccountId)
    {
        ValidateId(payoutAccountId, nameof(payoutAccountId));

        var payoutAccount = await _unitOfWork.Account
            .GetByIdAsync(payoutAccountId);

        if (payoutAccount == null)
            return Enumerable.Empty<AccountDto>();

        var accounts = await _unitOfWork.Account
            .GetLinkedDepositAccountsAsync(payoutAccountId);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<IEnumerable<AccountDto>> GetAccountsByMaturityDateRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var accounts = await _unitOfWork.Account
            .GetAccountsByMaturityDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<IEnumerable<AccountDto>> GetMaturedDepositAccountsAsync(
        DateTime asOfDate)
    {
        if (asOfDate == default)
            throw new ArgumentException(
                "A valid maturity reference date is required.",
                nameof(asOfDate));

        var accounts = await _unitOfWork.Account
            .GetMaturedDepositAccountsAsync(asOfDate);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    // 7. ACCOUNT ACTIVITY

    public async Task<IEnumerable<AccountDto>> GetRecentlyActiveAccountsAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Count must be greater than zero.");

        var accounts = await _unitOfWork.Account
            .GetRecentlyActiveAccountsAsync(count);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    // 8. SEARCH AND VALIDATION

    public async Task<IEnumerable<AccountDto>> SearchAsync(
        string? searchTerm,
        int? branchId,
        int? accountTypeId,
        int? currencyId,
        string? status,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (branchId.HasValue)
            ValidateId(branchId.Value, nameof(branchId));

        if (accountTypeId.HasValue)
            ValidateId(accountTypeId.Value, nameof(accountTypeId));

        if (currencyId.HasValue)
            ValidateId(currencyId.Value, nameof(currencyId));

        var normalizedStatus = string.IsNullOrWhiteSpace(status)
            ? null
            : NormalizeStatus(status);

        var accounts = await _unitOfWork.Account.SearchAccountsAsync(
            searchTerm?.Trim(),
            branchId,
            accountTypeId,
            currencyId,
            normalizedStatus,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<bool> IsAccountNumberExistsAsync(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            return false;

        var account = await _unitOfWork.Account
            .GetByAccountNumberAsync(NormalizeAccountNumber(accountNumber));

        return account != null;
    }

    public async Task<bool> IsAccountInUseAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        return await _unitOfWork.Account.IsAccountInUseAsync(accountId);
    }

    public async Task<bool> CanAccountBeDeletedAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var account = await _unitOfWork.Account.GetByIdAsync(accountId);

        if (account == null)
            return false;

        if (account.Status != "Closed")
            return false;

        if (account.Balance != 0 ||
            account.AvailableBalance != 0 ||
            account.HoldAmount != 0)
            return false;

        return !await IsAccountInUseAsync(accountId);
    }

    // PRIVATE VALIDATION AND BUSINESS-RULE HELPERS

    private async Task ValidateDepositTermsAsync(
        AccountType accountType,
        decimal? principalAmount,
        decimal? maturityAmount,
        decimal? contractedInterestRate,
        int? tenureMonths,
        DateTime? maturityDate)
    {
        var isDeposit = accountType.Category is "FixedDeposit" or "RecurringDeposit";

        if (isDeposit)
        {
            if (!principalAmount.HasValue || principalAmount.Value <= 0)
                throw new InvalidOperationException(
                    "A positive principal amount is required for deposit accounts.");

            if (!tenureMonths.HasValue || tenureMonths.Value <= 0)
                throw new InvalidOperationException(
                    "A positive tenure is required for deposit accounts.");

            if (!maturityDate.HasValue)
                throw new InvalidOperationException(
                    "A maturity date is required for deposit accounts.");

            if (maturityDate.Value.Date <= DateTime.UtcNow.Date)
                throw new InvalidOperationException(
                    "The deposit maturity date must be in the future.");

            if (accountType.MinTenureMonths.HasValue &&
                tenureMonths.Value < accountType.MinTenureMonths.Value)
            {
                throw new InvalidOperationException(
                    "The deposit tenure is below the account type minimum.");
            }

            if (accountType.MaxTenureMonths.HasValue &&
                tenureMonths.Value > accountType.MaxTenureMonths.Value)
            {
                throw new InvalidOperationException(
                    "The deposit tenure exceeds the account type maximum.");
            }
        }
        else if (principalAmount.HasValue ||
                 maturityAmount.HasValue ||
                 contractedInterestRate.HasValue ||
                 tenureMonths.HasValue ||
                 maturityDate.HasValue)
        {
            throw new InvalidOperationException(
                "Deposit terms can only be provided for FixedDeposit or RecurringDeposit account types.");
        }

        await Task.CompletedTask;
    }

    private async Task ValidatePayoutAccountAsync(
        int? payoutAccountId,
        int currencyId,
        int? currentAccountId)
    {
        if (!payoutAccountId.HasValue)
            return;

        ValidateId(payoutAccountId.Value, nameof(payoutAccountId));

        if (currentAccountId.HasValue &&
            payoutAccountId.Value == currentAccountId.Value)
        {
            throw new InvalidOperationException(
                "An account cannot be its own payout account.");
        }

        var payoutAccount = await _unitOfWork.Account
            .GetByIdAsync(payoutAccountId.Value);

        if (payoutAccount == null)
            throw new InvalidOperationException(
                "The selected payout account does not exist.");

        if (payoutAccount.Status != "Active")
            throw new InvalidOperationException(
                "The payout account must be active.");

        if (payoutAccount.CurrencyId != currencyId)
            throw new InvalidOperationException(
                "The payout account and deposit account must use the same currency.");
    }

    private static void ValidateCanClose(Account account)
    {
        if (account.Balance != 0)
            throw new InvalidOperationException(
                "An account with an outstanding balance cannot be closed.");

        if (account.AvailableBalance != 0)
            throw new InvalidOperationException(
                "An account with an outstanding available balance cannot be closed.");

        if (account.HoldAmount != 0)
            throw new InvalidOperationException(
                "An account with outstanding holds cannot be closed.");
    }

    private static bool IsValidStatusTransition(string current, string next)
    {
        return current switch
        {
            "Pending" => next is "Active" or "Inactive" or "Closed",
            "Active" => next is "Dormant" or "Inactive" or "Frozen" or "Closed",
            "Dormant" => next is "Active" or "Inactive" or "Frozen" or "Closed",
            "Inactive" => next is "Active" or "Frozen" or "Closed",
            "Frozen" => next is "Active" or "Closed",
            "Closed" => false,
            _ => false
        };
    }

    private static void ValidateAccountDates(
        DateTime openedOn,
        DateTime? closedOn,
        string status)
    {
        if (openedOn != default && openedOn.Date > DateTime.UtcNow.Date)
            throw new ArgumentException(
                "Account opening date cannot be in the future.",
                nameof(openedOn));

        if (closedOn.HasValue &&
            (openedOn == default || closedOn.Value.Date < openedOn.Date))
        {
            throw new ArgumentException(
                "Account closing date cannot be earlier than opening date.",
                nameof(closedOn));
        }

        if (status == "Closed" && !closedOn.HasValue)
            throw new InvalidOperationException(
                "A closing date is required for a closed account.");
    }

    private static void ValidateAccountTerms(
        decimal overdraftLimit,
        decimal? principalAmount,
        decimal? maturityAmount,
        decimal? contractedInterestRate,
        int? tenureMonths,
        DateTime? maturityDate)
    {
        if (overdraftLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(overdraftLimit));

        if (principalAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(principalAmount));

        if (maturityAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(maturityAmount));

        if (contractedInterestRate < 0)
            throw new ArgumentOutOfRangeException(nameof(contractedInterestRate));

        if (tenureMonths <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(tenureMonths),
                "Tenure must be greater than zero.");

        if (maturityDate.HasValue && maturityDate.Value == default)
            throw new ArgumentException(
                "Maturity date is invalid.",
                nameof(maturityDate));
    }

    private static string NormalizeAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException(
                "Account number is required.",
                nameof(accountNumber));

        var normalized = accountNumber.Trim();

        if (normalized.Length is < 9 or > 18 ||
            !normalized.All(char.IsDigit))
        {
            throw new ArgumentException(
                "Account number must contain 9 to 18 digits.",
                nameof(accountNumber));
        }

        return normalized;
    }

    private static string NormalizeStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException(
                "Account status is required.",
                nameof(status));

        var normalized = ValidStatuses.FirstOrDefault(x =>
            string.Equals(x, status.Trim(), StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid account status. Allowed values: {string.Join(", ", ValidStatuses)}.",
            nameof(status));
    }

    private static string NormalizeMode(string mode)
    {
        if (string.IsNullOrWhiteSpace(mode))
            throw new ArgumentException(
                "Mode of operation is required.",
                nameof(mode));

        var normalized = ValidModes.FirstOrDefault(x =>
            string.Equals(x, mode.Trim(), StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid mode of operation. Allowed values: {string.Join(", ", ValidModes)}.",
            nameof(mode));
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException(
                "Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }
}