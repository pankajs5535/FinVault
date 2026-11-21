
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. EMPLOYEE IDENTIFICATION
        // =========================================================

        // Retrieves an employee using the unique employee code.
        // Useful for identifying bank staff during internal operations.
        public async Task<Employee?> GetByEmployeeCodeAsync(string employeeCode)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.EmployeeCode == employeeCode);
        }

        // Retrieves complete employee information with related records.
        // Useful for displaying an employee profile and organizational details.
        public async Task<Employee?> GetEmployeeDetailsAsync(int employeeId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Include(x => x.Branch)
                .Include(x => x.ReportsToEmployee)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.ManagedCustomers)
                .Include(x => x.VerifiedKYCRecords)
                .Include(x => x.VerifiedDocuments)
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
        }

        // Retrieves an employee using the associated user account.
        // Useful for connecting an employee record with their login identity.
        public async Task<Employee?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // =========================================================
        // 2. BRANCH AND DEPARTMENT MANAGEMENT
        // =========================================================

        // Retrieves employees assigned to a particular branch.
        // Useful for branch-level staff management and operational reporting.
        public async Task<IEnumerable<Employee>> GetByBranchIdAsync(int branchId)
        {
            return await _dbSet
                .Include(x => x.User)
                .Where(x => x.BranchId == branchId)
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        // Retrieves active employees assigned to a particular branch.
        // Useful for identifying staff currently available at a branch.
        public async Task<IEnumerable<Employee>> GetActiveEmployeesByBranchIdAsync(int branchId)
        {
            return await _dbSet
                .Where(x => x.BranchId == branchId && x.EmploymentStatus == "Active")
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves employees belonging to a particular department.
        // Useful for managing departments such as Operations, Loans and Compliance.
        public async Task<IEnumerable<Employee>> GetByDepartmentAsync(string department)
        {
            return await _dbSet
                .Where(x => x.Department == department)
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves employees holding a particular designation.
        // Useful for locating staff with specific responsibilities or authority.
        public async Task<IEnumerable<Employee>> GetByDesignationAsync(string designation)
        {
            return await _dbSet
                .Where(x => x.Designation == designation)
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // =========================================================
        // 3. EMPLOYMENT STATUS
        // =========================================================

        // Retrieves employees matching a particular employment status.
        // Useful for workforce monitoring and access-management workflows.
        public async Task<IEnumerable<Employee>> GetByEmploymentStatusAsync(string employmentStatus)
        {
            return await _dbSet
                .Where(x => x.EmploymentStatus == employmentStatus)
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves employees currently marked as active.
        // Useful for identifying staff eligible for operational assignments.
        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await _dbSet
                .Where(x => x.EmploymentStatus == "Active")
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves employees currently marked as on leave.
        // Useful for avoiding assignment of work to unavailable staff.
        public async Task<IEnumerable<Employee>> GetEmployeesOnLeaveAsync()
        {
            return await _dbSet
                .Where(x => x.EmploymentStatus == "OnLeave")
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves employees whose employment has ended.
        // Useful for historical reporting and employee lifecycle management.
        public async Task<IEnumerable<Employee>> GetFormerEmployeesAsync()
        {
            return await _dbSet
                .Where(x => x.EmploymentStatus == "Resigned" || x.EmploymentStatus == "Terminated")
                .OrderByDescending(x => x.DateOfLeaving)
                .ToListAsync();
        }

        // =========================================================
        // 4. REPORTING HIERARCHY
        // =========================================================

        // Retrieves employees reporting directly to a particular manager.
        // Useful for displaying the bank's employee reporting hierarchy.
        public async Task<IEnumerable<Employee>> GetDirectReportsAsync(int managerEmployeeId)
        {
            return await _dbSet
                .Where(x => x.ReportsToEmployeeId == managerEmployeeId)
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves an employee together with their reporting manager and direct reports.
        // Useful for reviewing an employee's position in the organizational structure.
        public async Task<Employee?> GetEmployeeHierarchyAsync(int employeeId)
        {
            return await _dbSet
                .Include(x => x.ReportsToEmployee)
                .Include(x => x.DirectReports)
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId);
        }

        // =========================================================
        // 5. EMPLOYMENT AND APPROVAL MANAGEMENT
        // =========================================================

        // Retrieves employees who joined within the supplied date range.
        // Useful for recruitment and workforce reporting.
        public async Task<IEnumerable<Employee>> GetEmployeesJoinedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.DateOfJoining >= startDate && x.DateOfJoining <= endDate)
                .OrderByDescending(x => x.DateOfJoining)
                .ToListAsync();
        }

        // Retrieves active employees whose approval limit meets the supplied amount.
        // Useful for identifying potential approvers for maker-checker workflows.
        public async Task<IEnumerable<Employee>> GetEmployeesWithApprovalLimitAsync(decimal amount)
        {
            return await _dbSet
                .Where(x => x.EmploymentStatus == "Active" &&
                            x.ApprovalLimit.HasValue &&
                            x.ApprovalLimit.Value >= amount)
                .OrderBy(x => x.ApprovalLimit)
                .ToListAsync();
        }

        // Retrieves employees whose approval limit is greater than or equal to the supplied amount.
        // Useful for finding staff authorized to handle transactions above a required threshold.
        public async Task<IEnumerable<Employee>> GetEligibleApproversAsync(decimal amount, int? branchId = null)
        {
            return await _dbSet
                .Where(x => x.EmploymentStatus == "Active" &&
                            x.ApprovalLimit.HasValue &&
                            x.ApprovalLimit.Value >= amount &&
                            (!branchId.HasValue || x.BranchId == branchId.Value))
                .OrderBy(x => x.ApprovalLimit)
                .ToListAsync();
        }

        // =========================================================
        // 6. BANKING WORKLOAD
        // =========================================================

        // Retrieves employees with assigned disputes.
        // Useful for monitoring complaint investigation responsibilities.
        public async Task<IEnumerable<Employee>> GetEmployeesWithAssignedDisputesAsync()
        {
            return await _dbSet
                .Include(x => x.AssignedDisputes)
                .Where(x => x.AssignedDisputes.Any())
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // Retrieves employees with fraud alerts assigned for review.
        // Useful for monitoring fraud investigation workloads.
        public async Task<IEnumerable<Employee>> GetEmployeesWithFraudReviewWorkloadAsync()
        {
            return await _dbSet
                .Include(x => x.ReviewedFraudAlerts)
                .Where(x => x.ReviewedFraudAlerts.Any())
                .OrderBy(x => x.FirstName)
                .ToListAsync();
        }

        // =========================================================
        // 7. SEARCH AND VALIDATION
        // =========================================================

        // Searches employees by employee code, name, department or designation.
        // Useful for internal staff directories and administrative operations.
        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.EmployeeCode.Contains(searchTerm) ||
                            x.FirstName.Contains(searchTerm) ||
                            x.LastName.Contains(searchTerm) ||
                            x.Department.Contains(searchTerm) ||
                            x.Designation.Contains(searchTerm))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        // Checks whether an employee code already exists.
        // Useful for preventing duplicate employee identifiers.
        public async Task<bool> ExistsByEmployeeCodeAsync(string employeeCode)
        {
            return await _dbSet.AnyAsync(x => x.EmployeeCode == employeeCode);
        }

        // Counts employees assigned to a particular branch.
        // Useful for branch staffing reports.
        public async Task<int> CountEmployeesByBranchIdAsync(int branchId)
        {
            return await _dbSet.CountAsync(x => x.BranchId == branchId);
        }
    }
}
