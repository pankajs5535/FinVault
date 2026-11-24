using AutoMapper;
using FinVault.Application.DTOs.AccountDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Handles account opening, account profile updates and account closure.</summary>
public sealed class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves an account by its database ID, including related account details.
    public async Task<AccountDto?> GetByIdAsync(int accountId)
    {
        var account = await _unitOfWork.Account.GetAccountDetailsAsync(accountId);
        return account is null ? null : _mapper.Map<AccountDto>(account);
    }

    // 2. Retrieves an account using its unique account number.
    public async Task<AccountDto?> GetByAccountNumberAsync(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber)) throw new ArgumentException("Account number is required.", nameof(accountNumber));
        var account = await _unitOfWork.Account.GetByAccountNumberAsync(accountNumber.Trim());
        return account is null ? null : _mapper.Map<AccountDto>(account);
    }

    // 3. Returns every account associated with the supplied customer.
    public async Task<IReadOnlyList<AccountDto>> GetByCustomerIdAsync(int customerId)
    {
        var accounts = await _unitOfWork.Account.GetAccountsByCustomerIdAsync(customerId);
        return _mapper.Map<IReadOnlyList<AccountDto>>(accounts);
    }

    // 4. Opens a new account after validating uniqueness and required master records.
    public async Task<AccountDto> OpenAccountAsync(CreateAccountDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.Account.GetByAccountNumberAsync(dto.AccountNumber.Trim()) is not null)
            throw new InvalidOperationException("Account number is already in use.");
        if (await _unitOfWork.AccountType.GetByIdAsync(dto.AccountTypeId) is null ||
            await _unitOfWork.Branch.GetByIdAsync(dto.BranchId) is null ||
            await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId) is null)
            throw new InvalidOperationException("The selected account type, branch or currency does not exist.");
        if (dto.PayoutAccountId.HasValue && await _unitOfWork.Account.GetByIdAsync(dto.PayoutAccountId.Value) is null)
            throw new InvalidOperationException("The selected payout account does not exist.");
        if (dto.OverdraftLimit < 0) throw new ArgumentOutOfRangeException(nameof(dto.OverdraftLimit));

        var account = _mapper.Map<Account>(dto);
        account.AccountNumber = dto.AccountNumber.Trim();
        account.Status = "Pending";
        account.Balance = 0m;
        account.AvailableBalance = 0m;
        account.HoldAmount = 0m;
        account.OpenedOn = dto.OpenedOn == default ? DateTime.UtcNow : dto.OpenedOn;
        account.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Account.AddAsync(account);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AccountDto>(account);
    }

    // 5. Updates account configuration while preserving balances managed by transactions.
    public async Task<AccountDto?> UpdateAccountAsync(int accountId, UpdateAccountDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.AccountId != accountId)
            throw new ArgumentException("The route account ID and DTO account ID do not match.", nameof(dto));
        var account = await _unitOfWork.Account.GetByIdAsync(accountId);
        if (account is null) return null;
        var duplicate = await _unitOfWork.Account.GetByAccountNumberAsync(dto.AccountNumber.Trim());
        if (duplicate is not null && duplicate.AccountId != accountId)
            throw new InvalidOperationException("Another account already uses this account number.");
        if (await _unitOfWork.AccountType.GetByIdAsync(dto.AccountTypeId) is null ||
            await _unitOfWork.Branch.GetByIdAsync(dto.BranchId) is null ||
            await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId) is null)
            throw new InvalidOperationException("The selected account type, branch or currency does not exist.");

        _mapper.Map(dto, account);
        account.AccountNumber = dto.AccountNumber.Trim();
        account.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AccountDto>(account);
    }

    // 6. Closes an account only when no funds or held funds remain.
    public async Task<bool> CloseAccountAsync(int accountId, string reason)
    {
        var account = await _unitOfWork.Account.GetByIdAsync(accountId);
        if (account is null || account.Status == "Closed") return false;
        if (account.Balance != 0m || account.HoldAmount != 0m)
            throw new InvalidOperationException("An account must have zero balance and zero hold amount before closure.");
        account.Status = "Closed";
        account.StatusReason = string.IsNullOrWhiteSpace(reason) ? "Closed by request." : reason.Trim();
        account.ClosedOn = DateTime.UtcNow;
        account.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Account.Update(account);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
