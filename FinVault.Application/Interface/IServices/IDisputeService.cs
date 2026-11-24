using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.DisputeDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Dispute banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IDisputeService
{
        Task<DisputeDto?> GetByIdAsync(int disputeId);
        Task<IReadOnlyList<DisputeDto>> GetByCustomerIdAsync(int customerId);
        Task<DisputeDto> RaiseAsync(CreateDisputeDto dto);
        Task<DisputeDto?> UpdateAsync(int disputeId, UpdateDisputeDto dto);
        Task<bool> ResolveAsync(int disputeId, string resolution);
}
