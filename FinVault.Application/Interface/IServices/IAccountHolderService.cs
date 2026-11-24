using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.AccountHolderDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the AccountHolder banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IAccountHolderService
{
        Task<IReadOnlyList<AccountHolderDto>> GetByAccountIdAsync(int accountId);
        Task<AccountHolderDto> AddHolderAsync(CreateAccountHolderDto dto);
        Task<AccountHolderDto?> UpdateHolderAsync(int accountHolderId, UpdateAccountHolderDto dto);
        Task<bool> RemoveHolderAsync(int accountHolderId);
}
