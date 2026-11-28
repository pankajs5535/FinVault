
using FinVault.Application.DTOs.BranchDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class BranchController : ControllerBase
{
    private readonly IBranchService _branchService;

    public BranchController(IBranchService branchService)
    {
        _branchService = branchService;
    }

    // ==================== CRUD AND IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _branchService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("{branchId:int}")]
    public async Task<IActionResult> GetById(int branchId)
    {
        var data = await _branchService.GetByIdAsync(branchId);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(data));
    }

    [HttpGet("code/{branchCode}")]
    public async Task<IActionResult> GetByBranchCode(string branchCode)
    {
        var data = await _branchService.GetByBranchCodeAsync(branchCode);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(data));
    }

    [HttpGet("ifsc/{ifscCode}")]
    public async Task<IActionResult> GetByIfscCode(string ifscCode)
    {
        var data = await _branchService.GetByIfscCodeAsync(ifscCode);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(data));
    }

    [HttpGet("micr/{micrCode}")]
    public async Task<IActionResult> GetByMicrCode(string micrCode)
    {
        var data = await _branchService.GetByMicrCodeAsync(micrCode);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(data));
    }

    [HttpGet("name/{branchName}")]
    public async Task<IActionResult> GetByBranchName(string branchName)
    {
        var data = await _branchService.GetByBranchNameAsync(branchName);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(data));
    }

    [HttpGet("{branchId:int}/details")]
    public async Task<IActionResult> GetBranchDetails(int branchId)
    {
        var data = await _branchService.GetBranchDetailsAsync(branchId);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchDto request)
    {
        var data = await _branchService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { branchId = data.BranchId },
            ApiResponse<BranchDto>.SuccessResponse(
                data, "Branch created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateBranchDto request)
    {
        var data = await _branchService.UpdateAsync(request);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse("Branch not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(
                data, "Branch updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteBranchDto request)
    {
        var result = await _branchService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Branch deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Branch does not exist or cannot be deleted because it is in use.", 409));
    }

    // ==================== STATUS AND LOCATION ====================

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveBranches()
    {
        var data = await _branchService.GetActiveBranchesAsync();
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("country/{countryId:int}")]
    public async Task<IActionResult> GetBranchesByCountryId(int countryId)
    {
        var data = await _branchService.GetBranchesByCountryIdAsync(countryId);
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("location")]
    public async Task<IActionResult> GetBranchesByLocation(
        [FromQuery] string state,
        [FromQuery] string city)
    {
        var data = await _branchService.GetBranchesByLocationAsync(state, city);
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("type/{branchType}")]
    public async Task<IActionResult> GetBranchesByType(string branchType)
    {
        var data = await _branchService.GetBranchesByTypeAsync(branchType);
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("recently-opened")]
    public async Task<IActionResult> GetRecentlyOpenedBranches(
        [FromQuery] int count = 10)
    {
        if (count < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be greater than zero.", 400));

        var data = await _branchService.GetRecentlyOpenedBranchesAsync(count);
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpPatch("{branchId:int}/activate")]
    public async Task<IActionResult> Activate(int branchId)
    {
        var result = await _branchService.ActivateAsync(branchId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Branch activated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Branch could not be activated. Verify its status and country.", 409));
    }

    [HttpPatch("{branchId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int branchId)
    {
        var result = await _branchService.DeactivateAsync(branchId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Branch deactivated successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Branch could not be deactivated.", 409));
    }

    [HttpPatch("{branchId:int}/close")]
    public async Task<IActionResult> CloseBranch(
        int branchId,
        [FromQuery] DateTime closedOn)
    {
        var result = await _branchService.CloseBranchAsync(branchId, closedOn);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Branch closed successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Branch could not be closed. It may already be closed or still be in use.", 409));
    }

    // ==================== BRANCH HIERARCHY AND MANAGEMENT ====================

    [HttpGet("{parentBranchId:int}/children")]
    public async Task<IActionResult> GetChildBranches(int parentBranchId)
    {
        var data = await _branchService.GetChildBranchesAsync(parentBranchId);
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("roots")]
    public async Task<IActionResult> GetRootBranches()
    {
        var data = await _branchService.GetRootBranchesAsync();
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("manager/{managerEmployeeId:int}")]
    public async Task<IActionResult> GetBranchesByManagerId(int managerEmployeeId)
    {
        var data = await _branchService.GetBranchesByManagerIdAsync(managerEmployeeId);
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("{branchId:int}/hierarchy")]
    public async Task<IActionResult> GetBranchHierarchy(int branchId)
    {
        var data = await _branchService.GetBranchHierarchyAsync(branchId);

        return data is null
            ? NotFound(ApiResponse<BranchDto>.FailureResponse(
                "Branch hierarchy not found.", 404))
            : Ok(ApiResponse<BranchDto>.SuccessResponse(data));
    }

    [HttpPut("{branchId:int}/manager/{managerEmployeeId:int}")]
    public async Task<IActionResult> AssignManager(
        int branchId,
        int managerEmployeeId)
    {
        var result = await _branchService.AssignManagerAsync(
            branchId, managerEmployeeId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Branch manager assigned successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Branch manager assignment failed.", 400));
    }

    [HttpDelete("{branchId:int}/manager")]
    public async Task<IActionResult> RemoveManager(int branchId)
    {
        var result = await _branchService.RemoveManagerAsync(branchId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Branch manager removed successfully."))
            : NotFound(ApiResponse<object>.FailureResponse("Branch not found.", 404));
    }

    [HttpPut("{branchId:int}/parent")]
    public async Task<IActionResult> ChangeParentBranch(
        int branchId,
        [FromQuery] int? parentBranchId)
    {
        var result = await _branchService.ChangeParentBranchAsync(
            branchId, parentBranchId);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Parent branch updated successfully."))
            : BadRequest(ApiResponse<object>.FailureResponse(
                "Parent branch assignment failed.", 400));
    }

    // ==================== BUSINESS RELATIONSHIPS ====================

    [HttpGet("with-employees")]
    public async Task<IActionResult> GetBranchesWithEmployees()
    {
        var data = await _branchService.GetBranchesWithEmployeesAsync();
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("with-accounts")]
    public async Task<IActionResult> GetBranchesWithAccounts()
    {
        var data = await _branchService.GetBranchesWithAccountsAsync();
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("with-loans")]
    public async Task<IActionResult> GetBranchesWithLoans()
    {
        var data = await _branchService.GetBranchesWithLoansAsync();
        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND VALIDATION ====================

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] int? countryId,
        [FromQuery] string? state,
        [FromQuery] string? city,
        [FromQuery] string? branchType,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number and page size must be greater than zero.", 400));

        var data = await _branchService.SearchAsync(
            searchTerm,
            countryId,
            state,
            city,
            branchType,
            isActive,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<BranchDto>>.SuccessResponse(data));
    }

    [HttpGet("exists/code/{branchCode}")]
    public async Task<IActionResult> IsBranchCodeExists(string branchCode)
    {
        var result = await _branchService.IsBranchCodeExistsAsync(branchCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/ifsc/{ifscCode}")]
    public async Task<IActionResult> IsIfscCodeExists(string ifscCode)
    {
        var result = await _branchService.IsIfscCodeExistsAsync(ifscCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/micr/{micrCode}")]
    public async Task<IActionResult> IsMicrCodeExists(string micrCode)
    {
        var result = await _branchService.IsMicrCodeExistsAsync(micrCode);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{branchId:int}/in-use")]
    public async Task<IActionResult> IsBranchInUse(int branchId)
    {
        var result = await _branchService.IsBranchInUseAsync(branchId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{branchId:int}/can-delete")]
    public async Task<IActionResult> CanBranchBeDeleted(int branchId)
    {
        var result = await _branchService.CanBranchBeDeletedAsync(branchId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
