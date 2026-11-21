
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories;

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    public BranchRepository(FinVaultDbContext context) : base(context)
    {
    }

    // =====================================================
    // 1. BRANCH IDENTIFICATION
    // =====================================================

    // Finds a branch using its unique branch code.
    // Useful for internal branch identification.
    public async Task<Branch?> GetByBranchCodeAsync(string branchCode)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.BranchCode == branchCode);
    }

    // Finds a branch using its unique 11-character IFSC code.
    // Useful for NEFT, RTGS and IMPS bank transfer identification.
    public async Task<Branch?> GetByIfscCodeAsync(string ifscCode)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.IfscCode == ifscCode);
    }

    // Finds a branch using its unique MICR code.
    // Useful for cheque processing and clearing operations.
    public async Task<Branch?> GetByMicrCodeAsync(string micrCode)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.MicrCode == micrCode);
    }

    // Finds branches using their name.
    // Branch names are not unique, so multiple results are possible.
    public async Task<IEnumerable<Branch>> GetBranchesByNameAsync(string branchName)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.BranchName == branchName)
            .OrderBy(x => x.BranchCode)
            .ToListAsync();
    }

    // =====================================================
    // 2. BRANCH STATUS AND LOCATION
    // =====================================================

    // Retrieves all active branches.
    // Useful when customers select an operational branch.
    public async Task<IEnumerable<Branch>> GetActiveBranchesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves branches belonging to a particular country.
    // Useful for country-based branch management.
    public async Task<IEnumerable<Branch>> GetBranchesByCountryIdAsync(int countryId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves branches in a particular state and city.
    // Useful for finding banking locations.
    public async Task<IEnumerable<Branch>> GetBranchesByLocationAsync(string state, string city)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.State == state && x.City == city)
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves branches based on their type.
    // Types: Regular, Main, Digital and Service.
    public async Task<IEnumerable<Branch>> GetBranchesByTypeAsync(string branchType)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.BranchType == branchType)
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves recently opened branches.
    // Useful for monitoring branch expansion.
    public async Task<IEnumerable<Branch>> GetRecentlyOpenedBranchesAsync(int count)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        return await _dbSet.AsNoTracking()
            .OrderByDescending(x => x.OpenedOn)
            .Take(count)
            .ToListAsync();
    }

    // =====================================================
    // 3. BRANCH HIERARCHY AND MANAGEMENT
    // =====================================================

    // Retrieves branches directly under a parent branch.
    // Useful for regional office and subordinate branch structures.
    public async Task<IEnumerable<Branch>> GetChildBranchesAsync(int parentBranchId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.ParentBranchId == parentBranchId)
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves branches without a parent branch.
    // Useful for identifying main or top-level branches.
    public async Task<IEnumerable<Branch>> GetRootBranchesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.ParentBranchId == null)
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves branches managed by a particular employee.
    // Useful for manager assignments and administration.
    public async Task<IEnumerable<Branch>> GetBranchesByManagerIdAsync(int managerEmployeeId)
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.ManagerEmployeeId == managerEmployeeId)
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves a branch with its country, parent branch and manager.
    // Useful for displaying complete branch information.
    public async Task<Branch?> GetBranchDetailsAsync(int branchId)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.Country)
            .Include(x => x.ParentBranch)
            .Include(x => x.ManagerEmployee)
            .FirstOrDefaultAsync(x => x.BranchId == branchId);
    }

    // Retrieves a branch with its parent and direct child branches.
    // Useful for viewing one level of the branch hierarchy.
    public async Task<Branch?> GetBranchHierarchyAsync(int branchId)
    {
        return await _dbSet.AsNoTracking()
            .Include(x => x.ParentBranch)
            .Include(x => x.ChildBranches)
            .FirstOrDefaultAsync(x => x.BranchId == branchId);
    }

    // =====================================================
    // 4. BRANCH BUSINESS RELATIONSHIPS
    // =====================================================

    // Retrieves branches that have associated employees.
    // Useful for workforce and branch staffing reports.
    public async Task<IEnumerable<Branch>> GetBranchesWithEmployeesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Employees.Any())
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves branches that have associated accounts.
    // Useful for identifying branches serving customers.
    public async Task<IEnumerable<Branch>> GetBranchesWithAccountsAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Accounts.Any())
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // Retrieves branches that have associated loans.
    // Useful for branch-level loan reporting.
    public async Task<IEnumerable<Branch>> GetBranchesWithLoansAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(x => x.Loans.Any())
            .OrderBy(x => x.BranchName)
            .ToListAsync();
    }

    // =====================================================
    // 5. BRANCH SEARCH AND VALIDATION
    // =====================================================

    // Searches branches by code, name, IFSC, MICR, state or city.
    // Supports country, branch type and active-status filters.
    // Pagination prevents loading large branch lists at once.
    public async Task<IEnumerable<Branch>> SearchBranchesAsync(string? searchTerm, int? countryId, string? state, string? city, string? branchType, bool? isActive, int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();

            query = query.Where(x =>
                x.BranchCode.Contains(searchTerm) ||
                x.BranchName.Contains(searchTerm) ||
                x.IfscCode.Contains(searchTerm) ||
                (x.MicrCode != null && x.MicrCode.Contains(searchTerm)) ||
                x.City.Contains(searchTerm) ||
                x.District.Contains(searchTerm) ||
                x.State.Contains(searchTerm) ||
                x.PinCode.Contains(searchTerm));
        }

        if (countryId.HasValue)
            query = query.Where(x => x.CountryId == countryId.Value);

        if (!string.IsNullOrWhiteSpace(state))
            query = query.Where(x => x.State == state.Trim());

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(x => x.City == city.Trim());

        if (!string.IsNullOrWhiteSpace(branchType))
            query = query.Where(x => x.BranchType == branchType.Trim());

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        return await query.OrderBy(x => x.BranchName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    // Checks whether a branch is referenced by customers, child branches,
    // employees, accounts or loans.
    // Useful before branch deletion or closure.
    public async Task<bool> IsBranchInUseAsync(int branchId)
    {
        return await _dbSet.AnyAsync(x =>
            x.BranchId == branchId &&
            (x.Customers.Any() ||
             x.ChildBranches.Any() ||
             x.Employees.Any() ||
             x.Accounts.Any() ||
             x.Loans.Any()));
    }

    public async Task<Branch?> GetByBranchNameAsync(string branchName)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.BranchName == branchName);
    }
}
