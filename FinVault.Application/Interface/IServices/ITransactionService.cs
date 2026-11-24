using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.TransactionDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Transaction banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface ITransactionService
{
        Task<TransactionDto?> GetByIdAsync(int transactionId);
        Task<IReadOnlyList<TransactionDto>> GetByAccountIdAsync(int accountId);
        Task<TransactionDto> TransferAsync(CreateTransactionDto dto);
        Task<TransactionDto?> ReverseAsync(int transactionId, string reason);
}
