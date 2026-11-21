
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories;

public interface IBranchRepository : IGenericRepository<Branch>
{
    // =====================================================
    // 1. BRANCH IDENTIFICATION
    // =====================================================

    // Finds a branch using its unique branch code.
    // Useful for internal branch identification.
    Task<Branch?> GetByBranchCodeAsync(string branchCode);

    // Finds a branch using its unique 11-character IFSC code.
    // Useful for NEFT, RTGS and IMPS bank transfer identification.
    Task<Branch?> GetByIfscCodeAsync(string ifscCode);

    // Finds a branch using its MICR code.
    // Useful for cheque processing and clearing operations.
    Task<Branch?> GetByMicrCodeAsync(string micrCode);

    // Finds a branch using its name.
    // Useful for branch administration and customer selection.
    Task<Branch?> GetByBranchNameAsync(string branchName);


    // =====================================================
    // 2. BRANCH STATUS AND LOCATION
    // =====================================================

    // Retrieves all active branches.
    // Useful when customers select an operational branch.
    Task<IEnumerable<Branch>> GetActiveBranchesAsync();

    // Retrieves branches belonging to a particular country.
    // Useful for country-based branch management.
    Task<IEnumerable<Branch>> GetBranchesByCountryIdAsync(int countryId);

    // Retrieves branches in a particular state and city.
    // Useful for finding nearby banking locations.
    Task<IEnumerable<Branch>> GetBranchesByLocationAsync(string state, string city);

    // Retrieves branches based on their type.
    // Types: Regular, Main, Digital and Service.
    // Useful for branch classification and administration.
    Task<IEnumerable<Branch>> GetBranchesByTypeAsync(string branchType);

    // Retrieves recently opened branches.
    // Useful for monitoring branch expansion.
    Task<IEnumerable<Branch>> GetRecentlyOpenedBranchesAsync(int count);


    // =====================================================
    // 3. BRANCH HIERARCHY AND MANAGEMENT
    // =====================================================

    // Retrieves branches directly under a parent branch.
    // Useful for regional office and subordinate branch structures.
    Task<IEnumerable<Branch>> GetChildBranchesAsync(int parentBranchId);

    // Retrieves branches without a parent branch.
    // Useful for identifying main or top-level branches.
    Task<IEnumerable<Branch>> GetRootBranchesAsync();

    // Retrieves branches managed by a particular employee.
    // Useful for manager assignments and administration.
    Task<IEnumerable<Branch>> GetBranchesByManagerIdAsync(int managerEmployeeId);

    // Retrieves a branch with its country, parent branch and manager.
    // Useful for displaying complete branch information.
    Task<Branch?> GetBranchDetailsAsync(int branchId);

    // Retrieves a branch with its parent and direct child branches.
    // Useful for viewing the branch hierarchy.
    Task<Branch?> GetBranchHierarchyAsync(int branchId);


    // =====================================================
    // 4. BRANCH BUSINESS RELATIONSHIPS
    // =====================================================

    // Retrieves branches that have associated employees.
    // Useful for workforce and branch staffing reports.
    Task<IEnumerable<Branch>> GetBranchesWithEmployeesAsync();

    // Retrieves branches that have associated accounts.
    // Useful for identifying branches actively serving customers.
    Task<IEnumerable<Branch>> GetBranchesWithAccountsAsync();

    // Retrieves branches that have associated loans.
    // Useful for branch-level loan reporting.
    Task<IEnumerable<Branch>> GetBranchesWithLoansAsync();


    // =====================================================
    // 5. BRANCH SEARCH AND VALIDATION
    // =====================================================

    // Searches branches by code, name, IFSC, MICR, state or city.
    // Supports optional country, branch type and active-status filters.
    // Pagination prevents loading large branch lists at once.
    Task<IEnumerable<Branch>> SearchBranchesAsync(string? searchTerm, int? countryId, string? state, string? city, string? branchType, bool? isActive, int pageNumber, int pageSize);

    // Checks whether a branch is referenced by customers, child branches,
    // employees, accounts or loans.
    // Useful before branch deletion or closure.
    Task<bool> IsBranchInUseAsync(int branchId);
}
