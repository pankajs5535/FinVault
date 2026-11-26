using FinVault.Application.DTOs.EmployeeDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(int employeeId);
        Task<EmployeeDto?> GetByEmployeeCodeAsync(string employeeCode);
        Task<EmployeeDto?> GetByUserIdAsync(int userId);
        Task<EmployeeDto?> GetEmployeeDetailsAsync(int employeeId);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
        Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto);
        Task<bool> DeleteAsync(DeleteEmployeeDto dto);

        // 2. BRANCH AND DEPARTMENT MANAGEMENT
        Task<IEnumerable<EmployeeDto>> GetByBranchIdAsync(int branchId);
        Task<IEnumerable<EmployeeDto>> GetActiveEmployeesByBranchIdAsync(int branchId);
        Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(string department);
        Task<IEnumerable<EmployeeDto>> GetByDesignationAsync(string designation);
        Task<bool> TransferBranchAsync(int employeeId, int newBranchId);
        Task<bool> UpdateDepartmentAsync(int employeeId, string department);
        Task<bool> UpdateDesignationAsync(int employeeId, string designation);

        // 3. EMPLOYMENT STATUS
        Task<IEnumerable<EmployeeDto>> GetByEmploymentStatusAsync(string employmentStatus);
        Task<IEnumerable<EmployeeDto>> GetActiveEmployeesAsync();
        Task<IEnumerable<EmployeeDto>> GetEmployeesOnLeaveAsync();
        Task<IEnumerable<EmployeeDto>> GetFormerEmployeesAsync();
        Task<bool> UpdateEmploymentStatusAsync(int employeeId, string employmentStatus);
        Task<bool> MarkAsOnLeaveAsync(int employeeId);
        Task<bool> SuspendAsync(int employeeId);
        Task<bool> ReinstateAsync(int employeeId);
        Task<bool> RecordResignationAsync(int employeeId, DateTime dateOfLeaving);
        Task<bool> RecordTerminationAsync(int employeeId, DateTime dateOfLeaving);

        // 4. REPORTING HIERARCHY
        Task<IEnumerable<EmployeeDto>> GetDirectReportsAsync(int managerEmployeeId);
        Task<EmployeeDto?> GetEmployeeHierarchyAsync(int employeeId);
        Task<bool> AssignReportingManagerAsync(int employeeId, int managerEmployeeId);
        Task<bool> RemoveReportingManagerAsync(int employeeId);

        // 5. APPROVAL AUTHORITY
        Task<IEnumerable<EmployeeDto>> GetEmployeesWithApprovalLimitAsync(decimal amount);
        Task<IEnumerable<EmployeeDto>> GetEligibleApproversAsync(decimal amount, int? branchId = null);
        Task<bool> UpdateApprovalLimitAsync(int employeeId, decimal? approvalLimit);
        Task<bool> CanApproveAmountAsync(int employeeId, decimal amount);

        // 6. BANKING WORKLOAD
        Task<IEnumerable<EmployeeDto>> GetEmployeesWithAssignedDisputesAsync();
        Task<IEnumerable<EmployeeDto>> GetEmployeesWithFraudReviewWorkloadAsync();

        // 7. REPORTING AND SEARCH
        Task<IEnumerable<EmployeeDto>> GetEmployeesJoinedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm);
        Task<int> CountEmployeesByBranchIdAsync(int branchId);

        // 8. VALIDATION
        Task<bool> ExistsByEmployeeCodeAsync(string employeeCode);
        Task<bool> ExistsByUserIdAsync(int userId);
        Task<bool> CanEmployeeBeDeletedAsync(int employeeId);
    }
}

//Banking rules:
//-Each employee must be linked to one valid User and one Branch.
//- Prevent duplicate employee codes and duplicate User assignments.
//- Prevent circular reporting hierarchies and self-reporting.
//- Only active employees with sufficient ApprovalLimit can approve transactions.
//- Former or suspended employees must not retain operational approval authority.
//- Prevent deletion when the employee is referenced by banking, KYC, loan, dispute, fraud, or compliance records.