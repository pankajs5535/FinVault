
using AutoMapper;
using FinVault.Application.DTOs.EmployeeDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public sealed class EmployeeService : IEmployeeService
{
    private static readonly string[] ValidStatuses =
        { "Active", "OnLeave", "Suspended", "Resigned", "Terminated" };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUser;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        => _mapper.Map<IEnumerable<EmployeeDto>>(await _unitOfWork.Employee.GetAllAsync());

    public async Task<EmployeeDto?> GetByIdAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return _mapper.Map<EmployeeDto?>(await _unitOfWork.Employee.GetByIdAsync(employeeId));
    }

    public async Task<EmployeeDto?> GetByEmployeeCodeAsync(string employeeCode)
    {
        RequireText(employeeCode, nameof(employeeCode), 20);
        return _mapper.Map<EmployeeDto?>(
            await _unitOfWork.Employee.GetByEmployeeCodeAsync(employeeCode.Trim()));
    }

    public async Task<EmployeeDto?> GetByUserIdAsync(int userId)
    {
        ValidateId(userId, nameof(userId));
        return _mapper.Map<EmployeeDto?>(await _unitOfWork.Employee.GetByUserIdAsync(userId));
    }

    public async Task<EmployeeDto?> GetEmployeeDetailsAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return _mapper.Map<EmployeeDto?>(
            await _unitOfWork.Employee.GetEmployeeDetailsAsync(employeeId));
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateEmployeeFields(dto.EmployeeCode, dto.FirstName, dto.LastName,
            dto.Designation, dto.Department, dto.DateOfJoining, dto.DateOfLeaving,
            dto.EmploymentStatus, dto.ApprovalLimit);

        var code = dto.EmployeeCode.Trim();
        if (await _unitOfWork.Employee.ExistsByEmployeeCodeAsync(code))
            throw new InvalidOperationException("Employee code already exists.");

        if (await _unitOfWork.Employee.GetByUserIdAsync(dto.UserId) is not null)
            throw new InvalidOperationException("This user is already linked to an employee.");

        if (await _unitOfWork.User.GetByIdAsync(dto.UserId) is null)
            throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

        if (await _unitOfWork.Branch.GetByIdAsync(dto.BranchId) is null)
            throw new InvalidOperationException($"Branch with ID {dto.BranchId} does not exist.");

        await ValidateManagerAsync(null, dto.ReportsToEmployeeId);

        var employee = _mapper.Map<Employee>(dto);
        employee.EmployeeCode = code;
        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Designation = dto.Designation.Trim();
        employee.Department = dto.Department.Trim();
        employee.EmploymentStatus = dto.EmploymentStatus.Trim();
        employee.CreatedAt = DateTime.UtcNow;
        employee.CreatedBy = _currentUser.UserId;

        await _unitOfWork.Employee.AddAsync(employee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<EmployeeDto?> UpdateAsync(UpdateEmployeeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.EmployeeId, nameof(dto.EmployeeId));
        ValidateEmployeeFields(dto.EmployeeCode, dto.FirstName, dto.LastName,
            dto.Designation, dto.Department, dto.DateOfJoining, dto.DateOfLeaving,
            dto.EmploymentStatus, dto.ApprovalLimit);

        var employee = await _unitOfWork.Employee.GetByIdAsync(dto.EmployeeId);
        if (employee is null)
            return null;

        var code = dto.EmployeeCode.Trim();
        var existingCode = await _unitOfWork.Employee.GetByEmployeeCodeAsync(code);
        if (existingCode is not null && existingCode.EmployeeId != dto.EmployeeId)
            throw new InvalidOperationException("Employee code already exists.");

        var existingUser = await _unitOfWork.Employee.GetByUserIdAsync(dto.UserId);
        if (existingUser is not null && existingUser.EmployeeId != dto.EmployeeId)
            throw new InvalidOperationException("This user is already linked to another employee.");

        if (await _unitOfWork.User.GetByIdAsync(dto.UserId) is null)
            throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

        if (await _unitOfWork.Branch.GetByIdAsync(dto.BranchId) is null)
            throw new InvalidOperationException($"Branch with ID {dto.BranchId} does not exist.");

        await ValidateManagerAsync(dto.EmployeeId, dto.ReportsToEmployeeId);

        _mapper.Map(dto, employee);
        employee.EmployeeCode = code;
        employee.FirstName = dto.FirstName.Trim();
        employee.LastName = dto.LastName.Trim();
        employee.Designation = dto.Designation.Trim();
        employee.Department = dto.Department.Trim();
        employee.EmploymentStatus = dto.EmploymentStatus.Trim();
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = _currentUser.UserId;

        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<EmployeeDto>(employee);
    }

    public async Task<bool> DeleteAsync(DeleteEmployeeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.EmployeeId, nameof(dto.EmployeeId));

        if (!await CanEmployeeBeDeletedAsync(dto.EmployeeId))
            throw new InvalidOperationException(
                "Employee cannot be deleted because banking, compliance, reporting or operational records reference this employee.");

        var employee = await _unitOfWork.Employee.GetByIdAsync(dto.EmployeeId);
        if (employee is null)
            return false;

        _unitOfWork.Employee.Delete(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 2. BRANCH AND DEPARTMENT MANAGEMENT

    public async Task<IEnumerable<EmployeeDto>> GetByBranchIdAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));
        return MapMany(await _unitOfWork.Employee.GetByBranchIdAsync(branchId));
    }

    public async Task<IEnumerable<EmployeeDto>> GetActiveEmployeesByBranchIdAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));
        return MapMany(await _unitOfWork.Employee.GetActiveEmployeesByBranchIdAsync(branchId));
    }

    public async Task<IEnumerable<EmployeeDto>> GetByDepartmentAsync(string department)
    {
        RequireText(department, nameof(department), 100);
        return MapMany(await _unitOfWork.Employee.GetByDepartmentAsync(department.Trim()));
    }

    public async Task<IEnumerable<EmployeeDto>> GetByDesignationAsync(string designation)
    {
        RequireText(designation, nameof(designation), 100);
        return MapMany(await _unitOfWork.Employee.GetByDesignationAsync(designation.Trim()));
    }

    public async Task<bool> TransferBranchAsync(int employeeId, int newBranchId)
    {
        ValidateId(employeeId, nameof(employeeId));
        ValidateId(newBranchId, nameof(newBranchId));

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        if (await _unitOfWork.Branch.GetByIdAsync(newBranchId) is null)
            throw new InvalidOperationException($"Branch with ID {newBranchId} does not exist.");

        employee.BranchId = newBranchId;
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public Task<bool> UpdateDepartmentAsync(int employeeId, string department)
        => UpdateTextFieldAsync(employeeId, department, nameof(department), 100,
            (employee, value) => employee.Department = value);

    public Task<bool> UpdateDesignationAsync(int employeeId, string designation)
        => UpdateTextFieldAsync(employeeId, designation, nameof(designation), 100,
            (employee, value) => employee.Designation = value);

    // 3. EMPLOYMENT STATUS

    public async Task<IEnumerable<EmployeeDto>> GetByEmploymentStatusAsync(string employmentStatus)
    {
        ValidateStatus(employmentStatus);
        return MapMany(await _unitOfWork.Employee.GetByEmploymentStatusAsync(employmentStatus.Trim()));
    }

    public async Task<IEnumerable<EmployeeDto>> GetActiveEmployeesAsync()
        => MapMany(await _unitOfWork.Employee.GetActiveEmployeesAsync());

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesOnLeaveAsync()
        => MapMany(await _unitOfWork.Employee.GetEmployeesOnLeaveAsync());

    public async Task<IEnumerable<EmployeeDto>> GetFormerEmployeesAsync()
        => MapMany(await _unitOfWork.Employee.GetFormerEmployeesAsync());

    public async Task<bool> UpdateEmploymentStatusAsync(int employeeId, string employmentStatus)
    {
        ValidateId(employeeId, nameof(employeeId));
        ValidateStatus(employmentStatus);

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        var status = employmentStatus.Trim();
        if (status is "Resigned" or "Terminated" && !employee.DateOfLeaving.HasValue)
            throw new InvalidOperationException("Set the employee leaving date using the resignation or termination operation.");

        employee.EmploymentStatus = status;
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public Task<bool> MarkAsOnLeaveAsync(int employeeId)
        => UpdateEmploymentStatusAsync(employeeId, "OnLeave");

    public Task<bool> SuspendAsync(int employeeId)
        => UpdateEmploymentStatusAsync(employeeId, "Suspended");

    public async Task<bool> ReinstateAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        if (employee.EmploymentStatus != "Suspended" && employee.EmploymentStatus != "OnLeave")
            throw new InvalidOperationException("Only suspended or on-leave employees can be reinstated.");

        employee.EmploymentStatus = "Active";
        employee.DateOfLeaving = null;
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public Task<bool> RecordResignationAsync(int employeeId, DateTime dateOfLeaving)
        => RecordLeavingAsync(employeeId, dateOfLeaving, "Resigned");

    public Task<bool> RecordTerminationAsync(int employeeId, DateTime dateOfLeaving)
        => RecordLeavingAsync(employeeId, dateOfLeaving, "Terminated");

    // 4. REPORTING HIERARCHY

    public async Task<IEnumerable<EmployeeDto>> GetDirectReportsAsync(int managerEmployeeId)
    {
        ValidateId(managerEmployeeId, nameof(managerEmployeeId));
        return MapMany(await _unitOfWork.Employee.GetDirectReportsAsync(managerEmployeeId));
    }

    public async Task<EmployeeDto?> GetEmployeeHierarchyAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return _mapper.Map<EmployeeDto?>(
            await _unitOfWork.Employee.GetEmployeeHierarchyAsync(employeeId));
    }

    public async Task<bool> AssignReportingManagerAsync(int employeeId, int managerEmployeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        ValidateId(managerEmployeeId, nameof(managerEmployeeId));

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        await ValidateManagerAsync(employeeId, managerEmployeeId);
        employee.ReportsToEmployeeId = managerEmployeeId;
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveReportingManagerAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        employee.ReportsToEmployeeId = null;
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 5. APPROVAL AUTHORITY

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesWithApprovalLimitAsync(decimal amount)
    {
        ValidateAmount(amount);
        return MapMany(await _unitOfWork.Employee.GetEmployeesWithApprovalLimitAsync(amount));
    }

    public async Task<IEnumerable<EmployeeDto>> GetEligibleApproversAsync(decimal amount, int? branchId = null)
    {
        ValidateAmount(amount);
        if (branchId.HasValue)
            ValidateId(branchId.Value, nameof(branchId));

        return MapMany(await _unitOfWork.Employee.GetEligibleApproversAsync(amount, branchId));
    }

    public async Task<bool> UpdateApprovalLimitAsync(int employeeId, decimal? approvalLimit)
    {
        ValidateId(employeeId, nameof(employeeId));
        if (approvalLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(approvalLimit));

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        employee.ApprovalLimit = approvalLimit;
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CanApproveAmountAsync(int employeeId, decimal amount)
    {
        ValidateId(employeeId, nameof(employeeId));
        ValidateAmount(amount);

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        return employee is not null &&
               employee.EmploymentStatus == "Active" &&
               employee.ApprovalLimit.HasValue &&
               employee.ApprovalLimit.Value >= amount;
    }

    // 6. BANKING WORKLOAD

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesWithAssignedDisputesAsync()
        => MapMany(await _unitOfWork.Employee.GetEmployeesWithAssignedDisputesAsync());

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesWithFraudReviewWorkloadAsync()
        => MapMany(await _unitOfWork.Employee.GetEmployeesWithFraudReviewWorkloadAsync());

    // 7. REPORTING AND SEARCH

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesJoinedWithinRangeAsync(DateTime startDate, DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);
        return MapMany(await _unitOfWork.Employee.GetEmployeesJoinedWithinRangeAsync(startDate, endDate));
    }

    public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm)
    {
        RequireText(searchTerm, nameof(searchTerm), 200);
        return MapMany(await _unitOfWork.Employee.SearchEmployeesAsync(searchTerm.Trim()));
    }

    public async Task<int> CountEmployeesByBranchIdAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));
        return await _unitOfWork.Employee.CountEmployeesByBranchIdAsync(branchId);
    }

    // 8. VALIDATION

    public async Task<bool> ExistsByEmployeeCodeAsync(string employeeCode)
    {
        RequireText(employeeCode, nameof(employeeCode), 20);
        return await _unitOfWork.Employee.ExistsByEmployeeCodeAsync(employeeCode.Trim());
    }

    public async Task<bool> ExistsByUserIdAsync(int userId)
    {
        ValidateId(userId, nameof(userId));
        return await _unitOfWork.Employee.GetByUserIdAsync(userId) is not null;
    }

    public async Task<bool> CanEmployeeBeDeletedAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        return
            !await _unitOfWork.Customer.ExistsAsync(x => x.RelationshipManagerId == employeeId) &&
            !await _unitOfWork.KYCDetails.ExistsAsync(x => x.VerifiedByEmployeeId == employeeId) &&
            !await _unitOfWork.Document.ExistsAsync(x => x.VerifiedByEmployeeId == employeeId) &&
            !await _unitOfWork.Branch.ExistsAsync(x => x.ManagerEmployeeId == employeeId) &&
            !await _unitOfWork.Employee.ExistsAsync(x => x.ReportsToEmployeeId == employeeId) &&
            !await _unitOfWork.Loan.ExistsAsync(x => x.ApprovedByEmployeeId == employeeId) &&
            !await _unitOfWork.Dispute.ExistsAsync(x => x.AssignedToEmployeeId == employeeId) &&
            !await _unitOfWork.FraudAlert.ExistsAsync(x => x.ReviewedByEmployeeId == employeeId) &&
            !await _unitOfWork.ComplianceRecord.ExistsAsync(x => x.FiledByEmployeeId == employeeId) &&
            !await _unitOfWork.AIConversation.ExistsAsync(x => x.EscalatedToEmployeeId == employeeId);
    }

    // PRIVATE HELPERS

    private async Task<bool> UpdateTextFieldAsync(
        int employeeId,
        string value,
        string parameterName,
        int maxLength,
        Action<Employee, string> update)
    {
        ValidateId(employeeId, nameof(employeeId));
        RequireText(value, parameterName, maxLength);

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        update(employee, value.Trim());
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task<bool> RecordLeavingAsync(int employeeId, DateTime dateOfLeaving, string status)
    {
        ValidateId(employeeId, nameof(employeeId));
        if (dateOfLeaving.Date > DateTime.UtcNow.Date)
            throw new ArgumentOutOfRangeException(nameof(dateOfLeaving), "Leaving date cannot be in the future.");

        var employee = await _unitOfWork.Employee.GetByIdAsync(employeeId);
        if (employee is null)
            return false;

        if (dateOfLeaving.Date < employee.DateOfJoining.Date)
            throw new ArgumentException("Leaving date cannot be earlier than the joining date.");

        employee.DateOfLeaving = dateOfLeaving;
        employee.EmploymentStatus = status;
        employee.ApprovalLimit = null;
        Touch(employee);
        _unitOfWork.Employee.Update(employee);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private async Task ValidateManagerAsync(int? employeeId, int? managerEmployeeId)
    {
        if (!managerEmployeeId.HasValue)
            return;

        if (managerEmployeeId.Value <= 0)
            throw new ArgumentOutOfRangeException(nameof(managerEmployeeId));

        if (employeeId.HasValue && employeeId.Value == managerEmployeeId.Value)
            throw new InvalidOperationException("An employee cannot report to themselves.");

        var manager = await _unitOfWork.Employee.GetByIdAsync(managerEmployeeId.Value);
        if (manager is null)
            throw new InvalidOperationException("Reporting manager does not exist.");

        var visited = new HashSet<int>();
        var currentManagerId = managerEmployeeId;

        while (currentManagerId.HasValue)
        {
            if (employeeId.HasValue && currentManagerId.Value == employeeId.Value)
                throw new InvalidOperationException("Reporting manager assignment would create a circular hierarchy.");

            if (!visited.Add(currentManagerId.Value))
                throw new InvalidOperationException("An existing circular reporting hierarchy was detected.");

            var currentManager = await _unitOfWork.Employee.GetByIdAsync(currentManagerId.Value);
            currentManagerId = currentManager?.ReportsToEmployeeId;
        }
    }

    private void Touch(Employee employee)
    {
        employee.UpdatedAt = DateTime.UtcNow;
        employee.UpdatedBy = _currentUser.UserId;
    }

    private IEnumerable<EmployeeDto> MapMany(IEnumerable<Employee> employees)
        => _mapper.Map<IEnumerable<EmployeeDto>>(employees);

    private static void ValidateEmployeeFields(
        string employeeCode, string firstName, string lastName,
        string designation, string department, DateTime dateOfJoining,
        DateTime? dateOfLeaving, string employmentStatus, decimal? approvalLimit)
    {
        RequireText(employeeCode, nameof(employeeCode), 20);
        RequireText(firstName, nameof(firstName), 100);
        RequireText(lastName, nameof(lastName), 100);
        RequireText(designation, nameof(designation), 100);
        RequireText(department, nameof(department), 100);
        ValidateStatus(employmentStatus);

        if (dateOfJoining == default || dateOfJoining.Date > DateTime.UtcNow.Date)
            throw new ArgumentException("Date of joining must be a valid date and cannot be in the future.");

        if (dateOfLeaving.HasValue && dateOfLeaving.Value.Date < dateOfJoining.Date)
            throw new ArgumentException("Date of leaving cannot be earlier than date of joining.");

        if (approvalLimit < 0)
            throw new ArgumentOutOfRangeException(nameof(approvalLimit));

        if (employmentStatus is "Resigned" or "Terminated" && !dateOfLeaving.HasValue)
            throw new ArgumentException("Date of leaving is required for resigned or terminated employees.");
    }

    private static void ValidateStatus(string status)
    {
        RequireText(status, nameof(status), 20);
        if (!ValidStatuses.Contains(status.Trim(), StringComparer.Ordinal))
            throw new ArgumentException("Invalid employment status.", nameof(status));
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(parameterName);
    }

    private static void RequireText(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);

        if (value.Trim().Length > maxLength)
            throw new ArgumentException($"Value cannot exceed {maxLength} characters.", parameterName);
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be earlier than or equal to end date.");
    }
}
