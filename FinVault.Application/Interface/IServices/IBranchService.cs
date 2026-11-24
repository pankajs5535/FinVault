using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.BranchDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Branch banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IBranchService
{
        Task<BranchDto?> GetByIdAsync(int branchId);
        Task<IReadOnlyList<BranchDto>> GetAllAsync();
        Task<BranchDto> CreateAsync(CreateBranchDto dto);
        Task<BranchDto?> UpdateAsync(int branchId, UpdateBranchDto dto);
        Task<bool> DeleteAsync(int branchId);
}
