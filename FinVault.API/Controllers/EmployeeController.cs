
using FinVault.Application.DTOs.EmployeeDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _employeeService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("{employeeId:int}")]
    public async Task<IActionResult> GetById(int employeeId)
    {
        var data = await _employeeService.GetByIdAsync(employeeId);

        return data is null
            ? NotFound(ApiResponse<EmployeeDto>.FailureResponse("Employee not found.", 404))
            : Ok(ApiResponse<EmployeeDto>.SuccessResponse(data));
    }

    [HttpGet("code/{employeeCode}")]
    public async Task<IActionResult> GetByEmployeeCode(string employeeCode)
    {
        var data = await _employeeService.GetByEmployeeCodeAsync(employeeCode);

        return data is null
            ? NotFound(ApiResponse<EmployeeDto>.FailureResponse("Employee not found.", 404))
            : Ok(ApiResponse<EmployeeDto>.SuccessResponse(data));
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var data = await _employeeService.GetByUserIdAsync(userId);

        return data is null
            ? NotFound(ApiResponse<EmployeeDto>.FailureResponse("Employee not found.", 404))
            : Ok(ApiResponse<EmployeeDto>.SuccessResponse(data));
    }

    [HttpGet("{employeeId:int}/details")]
    public async Task<IActionResult> GetEmployeeDetails(int employeeId)
    {
        var data = await _employeeService.GetEmployeeDetailsAsync(employeeId);

        return data is null
            ? NotFound(ApiResponse<EmployeeDto>.FailureResponse("Employee details not found.", 404))
            : Ok(ApiResponse<EmployeeDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeDto request)
    {
        var data = await _employeeService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { employeeId = data.EmployeeId },
            ApiResponse<EmployeeDto>.SuccessResponse(
                data, "Employee created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateEmployeeDto request)
    {
        var data = await _employeeService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<EmployeeDto>.FailureResponse("Employee not found.", 404))
            : Ok(ApiResponse<EmployeeDto>.SuccessResponse(
                data, "Employee updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteEmployeeDto request)
    {
        var result = await _employeeService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Employee does not exist or cannot be deleted because it is referenced by other records.", 409));
    }

    // ==================== BRANCH AND DEPARTMENT MANAGEMENT ====================

    [HttpGet("branch/{branchId:int}")]
    public async Task<IActionResult> GetByBranchId(int branchId)
    {
        var data = await _employeeService.GetByBranchIdAsync(branchId);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("branch/{branchId:int}/active")]
    public async Task<IActionResult> GetActiveEmployeesByBranchId(int branchId)
    {
        var data = await _employeeService.GetActiveEmployeesByBranchIdAsync(branchId);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("department/{department}")]
    public async Task<IActionResult> GetByDepartment(string department)
    {
        var data = await _employeeService.GetByDepartmentAsync(department);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("designation/{designation}")]
    public async Task<IActionResult> GetByDesignation(string designation)
    {
        var data = await _employeeService.GetByDesignationAsync(designation);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpPatch("{employeeId:int}/transfer-branch")]
    public async Task<IActionResult> TransferBranch(
        int employeeId,
        [FromQuery] int newBranchId)
    {
        var result = await _employeeService.TransferBranchAsync(
            employeeId, newBranchId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee branch transferred successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Employee branch transfer failed.", 400));
    }

    [HttpPatch("{employeeId:int}/department")]
    public async Task<IActionResult> UpdateDepartment(
        int employeeId,
        [FromQuery] string department)
    {
        var result = await _employeeService.UpdateDepartmentAsync(
            employeeId, department);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee department updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Employee department update failed.", 400));
    }

    [HttpPatch("{employeeId:int}/designation")]
    public async Task<IActionResult> UpdateDesignation(
        int employeeId,
        [FromQuery] string designation)
    {
        var result = await _employeeService.UpdateDesignationAsync(
            employeeId, designation);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee designation updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Employee designation update failed.", 400));
    }

    // ==================== EMPLOYMENT STATUS ====================

    [HttpGet("employment-status/{employmentStatus}")]
    public async Task<IActionResult> GetByEmploymentStatus(string employmentStatus)
    {
        var data = await _employeeService.GetByEmploymentStatusAsync(employmentStatus);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveEmployees()
    {
        var data = await _employeeService.GetActiveEmployeesAsync();
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("on-leave")]
    public async Task<IActionResult> GetEmployeesOnLeave()
    {
        var data = await _employeeService.GetEmployeesOnLeaveAsync();
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("former")]
    public async Task<IActionResult> GetFormerEmployees()
    {
        var data = await _employeeService.GetFormerEmployeesAsync();
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpPatch("{employeeId:int}/employment-status")]
    public async Task<IActionResult> UpdateEmploymentStatus(
        int employeeId,
        [FromQuery] string employmentStatus)
    {
        var result = await _employeeService.UpdateEmploymentStatusAsync(
            employeeId, employmentStatus);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employment status updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Employment status update failed.", 400));
    }

    [HttpPatch("{employeeId:int}/mark-on-leave")]
    public async Task<IActionResult> MarkAsOnLeave(int employeeId)
    {
        var result = await _employeeService.MarkAsOnLeaveAsync(employeeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee marked as on leave."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Unable to mark employee as on leave.", 400));
    }

    [HttpPatch("{employeeId:int}/suspend")]
    public async Task<IActionResult> Suspend(int employeeId)
    {
        var result = await _employeeService.SuspendAsync(employeeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee suspended successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Employee suspension failed.", 400));
    }

    [HttpPatch("{employeeId:int}/reinstate")]
    public async Task<IActionResult> Reinstate(int employeeId)
    {
        var result = await _employeeService.ReinstateAsync(employeeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee reinstated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Employee reinstatement failed.", 400));
    }

    [HttpPatch("{employeeId:int}/resignation")]
    public async Task<IActionResult> RecordResignation(
        int employeeId,
        [FromQuery] DateTime dateOfLeaving)
    {
        var result = await _employeeService.RecordResignationAsync(
            employeeId, dateOfLeaving);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee resignation recorded successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Unable to record employee resignation.", 400));
    }

    [HttpPatch("{employeeId:int}/termination")]
    public async Task<IActionResult> RecordTermination(
        int employeeId,
        [FromQuery] DateTime dateOfLeaving)
    {
        var result = await _employeeService.RecordTerminationAsync(
            employeeId, dateOfLeaving);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Employee termination recorded successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Unable to record employee termination.", 400));
    }

    // ==================== REPORTING HIERARCHY ====================

    [HttpGet("{managerEmployeeId:int}/direct-reports")]
    public async Task<IActionResult> GetDirectReports(int managerEmployeeId)
    {
        var data = await _employeeService.GetDirectReportsAsync(managerEmployeeId);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("{employeeId:int}/hierarchy")]
    public async Task<IActionResult> GetEmployeeHierarchy(int employeeId)
    {
        var data = await _employeeService.GetEmployeeHierarchyAsync(employeeId);

        return data is null
            ? NotFound(ApiResponse<EmployeeDto>.FailureResponse(
                "Employee hierarchy not found.", 404))
            : Ok(ApiResponse<EmployeeDto>.SuccessResponse(data));
    }

    [HttpPut("{employeeId:int}/reporting-manager/{managerEmployeeId:int}")]
    public async Task<IActionResult> AssignReportingManager(
        int employeeId,
        int managerEmployeeId)
    {
        var result = await _employeeService.AssignReportingManagerAsync(
            employeeId, managerEmployeeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Reporting manager assigned successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Reporting manager assignment failed.", 400));
    }

    [HttpDelete("{employeeId:int}/reporting-manager")]
    public async Task<IActionResult> RemoveReportingManager(int employeeId)
    {
        var result = await _employeeService.RemoveReportingManagerAsync(employeeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Reporting manager removed successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Unable to remove reporting manager.", 400));
    }

    // ==================== APPROVAL AUTHORITY ====================

    [HttpGet("approval-limit")]
    public async Task<IActionResult> GetEmployeesWithApprovalLimit(
        [FromQuery] decimal amount)
    {
        var data = await _employeeService.GetEmployeesWithApprovalLimitAsync(amount);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("eligible-approvers")]
    public async Task<IActionResult> GetEligibleApprovers(
        [FromQuery] decimal amount,
        [FromQuery] int? branchId = null)
    {
        var data = await _employeeService.GetEligibleApproversAsync(amount, branchId);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpPatch("{employeeId:int}/approval-limit")]
    public async Task<IActionResult> UpdateApprovalLimit(
        int employeeId,
        [FromQuery] decimal? approvalLimit)
    {
        var result = await _employeeService.UpdateApprovalLimitAsync(
            employeeId, approvalLimit);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Approval limit updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Approval limit update failed.", 400));
    }

    [HttpGet("{employeeId:int}/can-approve")]
    public async Task<IActionResult> CanApproveAmount(
        int employeeId,
        [FromQuery] decimal amount)
    {
        var result = await _employeeService.CanApproveAmountAsync(
            employeeId, amount);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== BANKING WORKLOAD ====================

    [HttpGet("assigned-disputes")]
    public async Task<IActionResult> GetEmployeesWithAssignedDisputes()
    {
        var data = await _employeeService.GetEmployeesWithAssignedDisputesAsync();
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("fraud-review-workload")]
    public async Task<IActionResult> GetEmployeesWithFraudReviewWorkload()
    {
        var data = await _employeeService.GetEmployeesWithFraudReviewWorkloadAsync();
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    // ==================== REPORTING AND SEARCH ====================

    [HttpGet("joined-within-range")]
    public async Task<IActionResult> GetEmployeesJoinedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        var data = await _employeeService.GetEmployeesJoinedWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchEmployees(
        [FromQuery] string searchTerm)
    {
        var data = await _employeeService.SearchEmployeesAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(data));
    }

    [HttpGet("branch/{branchId:int}/count")]
    public async Task<IActionResult> CountEmployeesByBranchId(int branchId)
    {
        var count = await _employeeService.CountEmployeesByBranchIdAsync(branchId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== VALIDATION ====================

    [HttpGet("exists/code/{employeeCode}")]
    public async Task<IActionResult> ExistsByEmployeeCode(string employeeCode)
    {
        var result = await _employeeService.ExistsByEmployeeCodeAsync(employeeCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/user/{userId:int}")]
    public async Task<IActionResult> ExistsByUserId(int userId)
    {
        var result = await _employeeService.ExistsByUserIdAsync(userId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{employeeId:int}/can-delete")]
    public async Task<IActionResult> CanEmployeeBeDeleted(int employeeId)
    {
        var result = await _employeeService.CanEmployeeBeDeletedAsync(employeeId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
