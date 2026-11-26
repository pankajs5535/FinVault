using FinVault.Application.DTOs.BranchDtos;
using FinVault.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.Interfaces.Services
{
    public interface IBranchService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<BranchDto>> GetAllAsync();
        Task<BranchDto?> GetByIdAsync(int branchId);
        Task<BranchDto?> GetByBranchCodeAsync(string branchCode);
        Task<BranchDto?> GetByIfscCodeAsync(string ifscCode);
        Task<BranchDto?> GetByMicrCodeAsync(string micrCode);
        Task<BranchDto?> GetByBranchNameAsync(string branchName);
        Task<BranchDto?> GetBranchDetailsAsync(int branchId);
        Task<BranchDto> CreateAsync(CreateBranchDto dto);
        Task<BranchDto?> UpdateAsync(UpdateBranchDto dto);
        Task<bool> DeleteAsync(DeleteBranchDto dto);

        // 2. STATUS AND LOCATION
        Task<IEnumerable<BranchDto>> GetActiveBranchesAsync();
        Task<IEnumerable<BranchDto>> GetBranchesByCountryIdAsync(int countryId);
        Task<IEnumerable<BranchDto>> GetBranchesByLocationAsync(string state, string city);
        Task<IEnumerable<BranchDto>> GetBranchesByTypeAsync(string branchType);
        Task<IEnumerable<BranchDto>> GetRecentlyOpenedBranchesAsync(int count);
        Task<bool> ActivateAsync(int branchId);
        Task<bool> DeactivateAsync(int branchId);
        Task<bool> CloseBranchAsync(int branchId, DateTime closedOn);

        // 3. BRANCH HIERARCHY AND MANAGEMENT
        Task<IEnumerable<BranchDto>> GetChildBranchesAsync(int parentBranchId);
        Task<IEnumerable<BranchDto>> GetRootBranchesAsync();
        Task<IEnumerable<BranchDto>> GetBranchesByManagerIdAsync(int managerEmployeeId);
        Task<BranchDto?> GetBranchHierarchyAsync(int branchId);
        Task<bool> AssignManagerAsync(int branchId, int managerEmployeeId);
        Task<bool> RemoveManagerAsync(int branchId);
        Task<bool> ChangeParentBranchAsync(int branchId, int? parentBranchId);

        // 4. BUSINESS RELATIONSHIPS
        Task<IEnumerable<BranchDto>> GetBranchesWithEmployeesAsync();
        Task<IEnumerable<BranchDto>> GetBranchesWithAccountsAsync();
        Task<IEnumerable<BranchDto>> GetBranchesWithLoansAsync();

        // 5. SEARCH AND VALIDATION
        Task<IEnumerable<BranchDto>> SearchAsync(
            string? searchTerm,
            int? countryId,
            string? state,
            string? city,
            string? branchType,
            bool? isActive,
            int pageNumber,
            int pageSize);

        Task<bool> IsBranchCodeExistsAsync(string branchCode);
        Task<bool> IsIfscCodeExistsAsync(string ifscCode);
        Task<bool> IsMicrCodeExistsAsync(string micrCode);
        Task<bool> IsBranchInUseAsync(int branchId);
        Task<bool> CanBranchBeDeletedAsync(int branchId);
    }
}


//Banking rules: IFSC must follow the Indian 11-character format; prevent circular branch hierarchies;
//validate manager and parent branch assignments; do not delete branches referenced by customers, employees, accounts, or loans.