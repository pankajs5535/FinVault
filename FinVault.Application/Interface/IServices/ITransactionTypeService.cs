using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.TransactionTypeDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the TransactionType banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface ITransactionTypeService
{
        Task<TransactionTypeDto?> GetByIdAsync(int transactionTypeId);
        Task<IReadOnlyList<TransactionTypeDto>> GetAllAsync();
        Task<TransactionTypeDto> CreateAsync(CreateTransactionTypeDto dto);
        Task<TransactionTypeDto?> UpdateAsync(int transactionTypeId, UpdateTransactionTypeDto dto);
        Task<bool> DeleteAsync(int transactionTypeId);
}
