
using FinVault.Application.DTOs.BranchDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IBranchService
    {
       
        // 1. CRUD OPERATIONS
       

        Task<IReadOnlyList<BranchDto>> GetAllAsync();

        Task<BranchDto?> GetByIdAsync(int branchId);

        Task<BranchDto?> GetByBranchCodeAsync(string branchCode);

        Task<BranchDto?> GetByIfscCodeAsync(string ifscCode);

        Task<BranchDto?> GetByMicrCodeAsync(string micrCode);

        Task<BranchDto?> GetByBranchNameAsync(string branchName);

        Task<BranchDto> CreateAsync(CreateBranchDto dto);

        Task<BranchDto?> UpdateAsync(int branchId, UpdateBranchDto dto);

        Task<bool> DeleteAsync(int branchId);

        Task DeleteMultipleAsync(IEnumerable<int> branchIds);


       
        // 2. STATUS OPERATIONS
       

        Task<IReadOnlyList<BranchDto>> GetActiveBranchesAsync();

        Task<IReadOnlyList<BranchDto>> GetInactiveBranchesAsync();

        Task<bool> ActivateAsync(int branchId);

        Task<bool> DeactivateAsync(int branchId);

        Task<IReadOnlyList<BranchDto>> GetClosedBranchesAsync();


       
        // 3. LOCATION AND CLASSIFICATION OPERATIONS
       

        Task<IReadOnlyList<BranchDto>> GetByCountryIdAsync(int countryId);

        Task<IReadOnlyList<BranchDto>> GetByLocationAsync(string state, string city);

        Task<IReadOnlyList<BranchDto>> GetByBranchTypeAsync(string branchType);

        Task<IReadOnlyList<BranchDto>> GetRecentlyOpenedBranchesAsync(int count);


       
        // 4. BRANCH HIERARCHY AND MANAGEMENT
       

        Task<IReadOnlyList<BranchDto>> GetChildBranchesAsync(int parentBranchId);

        Task<IReadOnlyList<BranchDto>> GetRootBranchesAsync();

        Task<IReadOnlyList<BranchDto>> GetByManagerIdAsync(int managerEmployeeId);

        Task<BranchDto?> GetBranchDetailsAsync(int branchId);


       
        // 5. BUSINESS RELATIONSHIP OPERATIONS
       

        Task<IReadOnlyList<BranchDto>> GetBranchesWithEmployeesAsync();

        Task<IReadOnlyList<BranchDto>> GetBranchesWithAccountsAsync();

        Task<IReadOnlyList<BranchDto>> GetBranchesWithLoansAsync();


       
        // 6. SEARCH AND FILTER OPERATIONS
       

        Task<IReadOnlyList<BranchDto>> SearchAsync(
            string? searchTerm,
            int? countryId,
            string? state,
            string? city,
            string? branchType,
            bool? isActive,
            int pageNumber,
            int pageSize);


       
        // 7. VALIDATION AND EXISTENCE OPERATIONS
       

        Task<bool> IsBranchCodeExistsAsync(string branchCode);

        Task<bool> IsIfscCodeExistsAsync(string ifscCode);

        Task<bool> IsMicrCodeExistsAsync(string micrCode);

        Task<bool> IsBranchNameExistsAsync(string branchName);

        Task<bool> IsBranchInUseAsync(int branchId);
    }
}
