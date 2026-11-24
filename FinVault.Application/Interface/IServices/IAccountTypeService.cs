using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.AccountTypeDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the AccountType banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IAccountTypeService
{
        Task<AccountTypeDto?> GetByIdAsync(int accountTypeId);
        Task<IReadOnlyList<AccountTypeDto>> GetAllAsync();
        Task<AccountTypeDto> CreateAsync(CreateAccountTypeDto dto);
        Task<AccountTypeDto?> UpdateAsync(int accountTypeId, UpdateAccountTypeDto dto);
        Task<bool> DeleteAsync(int accountTypeId);
}
