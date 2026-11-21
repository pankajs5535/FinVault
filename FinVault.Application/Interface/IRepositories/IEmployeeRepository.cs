
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        // =========================================================
        // 1. EMPLOYEE IDENTIFICATION
        // =========================================================

        // Retrieves an employee using the unique employee code.
        // Useful for identifying bank staff during internal operations.
        Task<Employee?> GetByEmployeeCodeAsync(string employeeCode);

        // Retrieves complete employee information with related records.
        // Useful for displaying an employee profile and organizational details.
        Task<Employee?> GetEmployeeDetailsAsync(int employeeId);

        // Retrieves an employee using the associated user account.
        // Useful for connecting an employee record with their login identity.
        Task<Employee?> GetByUserIdAsync(int userId);

        // =========================================================
        // 2. BRANCH AND DEPARTMENT MANAGEMENT
        // =========================================================

        // Retrieves employees assigned to a particular branch.
        // Useful for branch-level staff management and operational reporting.
        Task<IEnumerable<Employee>> GetByBranchIdAsync(int branchId);

        // Retrieves active employees assigned to a particular branch.
        // Useful for identifying staff currently available at a branch.
        Task<IEnumerable<Employee>> GetActiveEmployeesByBranchIdAsync(int branchId);

        // Retrieves employees belonging to a particular department.
        // Useful for managing departments such as Operations, Loans and Compliance.
        Task<IEnumerable<Employee>> GetByDepartmentAsync(string department);

        // Retrieves employees holding a particular designation.
        // Useful for locating staff with specific responsibilities or authority.
        Task<IEnumerable<Employee>> GetByDesignationAsync(string designation);

        // =========================================================
        // 3. EMPLOYMENT STATUS
        // =========================================================

        // Retrieves employees matching a particular employment status.
        // Useful for workforce monitoring and access-management workflows.
        Task<IEnumerable<Employee>> GetByEmploymentStatusAsync(string employmentStatus);

        // Retrieves employees currently marked as active.
        // Useful for identifying staff eligible for operational assignments.
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();

        // Retrieves employees currently marked as on leave.
        // Useful for avoiding assignment of work to unavailable staff.
        Task<IEnumerable<Employee>> GetEmployeesOnLeaveAsync();

        // Retrieves employees whose employment has ended.
        // Useful for historical reporting and employee lifecycle management.
        Task<IEnumerable<Employee>> GetFormerEmployeesAsync();

        // =========================================================
        // 4. REPORTING HIERARCHY
        // =========================================================

        // Retrieves employees reporting directly to a particular manager.
        // Useful for displaying the bank's employee reporting hierarchy.
        Task<IEnumerable<Employee>> GetDirectReportsAsync(int managerEmployeeId);

        // Retrieves an employee together with their reporting manager and direct reports.
        // Useful for reviewing an employee's position in the organizational structure.
        Task<Employee?> GetEmployeeHierarchyAsync(int employeeId);

        // =========================================================
        // 5. EMPLOYMENT AND APPROVAL MANAGEMENT
        // =========================================================

        // Retrieves employees who joined within the supplied date range.
        // Useful for recruitment and workforce reporting.
        Task<IEnumerable<Employee>> GetEmployeesJoinedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves active employees whose approval limit meets the supplied amount.
        // Useful for identifying potential approvers for maker-checker workflows.
        Task<IEnumerable<Employee>> GetEmployeesWithApprovalLimitAsync(decimal amount);

        // Retrieves employees whose approval limit is greater than or equal to the supplied amount.
        // Useful for finding staff authorized to handle transactions above a required threshold.
        Task<IEnumerable<Employee>> GetEligibleApproversAsync(decimal amount, int? branchId = null);

        // =========================================================
        // 6. BANKING WORKLOAD
        // =========================================================

        // Retrieves employees with assigned disputes.
        // Useful for monitoring complaint investigation responsibilities.
        Task<IEnumerable<Employee>> GetEmployeesWithAssignedDisputesAsync();

        // Retrieves employees with fraud alerts assigned for review.
        // Useful for monitoring fraud investigation workloads.
        Task<IEnumerable<Employee>> GetEmployeesWithFraudReviewWorkloadAsync();

        // =========================================================
        // 7. SEARCH AND VALIDATION
        // =========================================================

        // Searches employees by employee code, name, department or designation.
        // Useful for internal staff directories and administrative operations.
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);

        // Checks whether an employee code already exists.
        // Useful for preventing duplicate employee identifiers.
        Task<bool> ExistsByEmployeeCodeAsync(string employeeCode);

        // Counts employees assigned to a particular branch.
        // Useful for branch staffing reports.
        Task<int> CountEmployeesByBranchIdAsync(int branchId);
    }
}
