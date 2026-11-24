using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.AccountDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Account banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IAccountService
{
        Task<AccountDto?> GetByIdAsync(int accountId);
        Task<AccountDto?> GetByAccountNumberAsync(string accountNumber);
        Task<IReadOnlyList<AccountDto>> GetByCustomerIdAsync(int customerId);
        Task<AccountDto> OpenAccountAsync(CreateAccountDto dto);
        Task<AccountDto?> UpdateAccountAsync(int accountId, UpdateAccountDto dto);
        Task<bool> CloseAccountAsync(int accountId, string reason);
}
