using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.ReconciliationBatchDtos;
using FinVault.Application.DTOs.ReconciliationRecordDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Reconciliation banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IReconciliationService
{
        Task<ReconciliationBatchDto?> GetBatchAsync(int batchId);
        Task<ReconciliationBatchDto> StartBatchAsync(CreateReconciliationBatchDto dto);
        Task<ReconciliationRecordDto> AddRecordAsync(CreateReconciliationRecordDto dto);
        Task<bool> CompleteBatchAsync(int batchId);
        Task<IReadOnlyList<ReconciliationRecordDto>> GetExceptionsAsync(int batchId);
}
