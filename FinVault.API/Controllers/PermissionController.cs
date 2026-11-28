
using FinVault.Application.DTOs.PermissionDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionController : ControllerBase
{
    private readonly IPermissionService _permissionService;

    public PermissionController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    // 1. CRUD AND IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetAllAsync()));

    [HttpGet("{permissionId:int}")]
    public async Task<IActionResult> GetById(int permissionId)
    {
        var result = await _permissionService.GetByIdAsync(permissionId);
        return result == null
            ? NotFound(ApiResponse<PermissionDto>.FailureResponse("Permission not found.", 404))
            : Ok(ApiResponse<PermissionDto>.SuccessResponse(result));
    }

    [HttpGet("code/{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var result = await _permissionService.GetByCodeAsync(code);
        return result == null
            ? NotFound(ApiResponse<PermissionDto>.FailureResponse("Permission not found.", 404))
            : Ok(ApiResponse<PermissionDto>.SuccessResponse(result));
    }

    [HttpGet("{permissionId:int}/details")]
    public async Task<IActionResult> GetPermissionDetails(int permissionId)
    {
        var result = await _permissionService.GetPermissionDetailsAsync(permissionId);
        return result == null
            ? NotFound(ApiResponse<PermissionDto>.FailureResponse("Permission not found.", 404))
            : Ok(ApiResponse<PermissionDto>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto)
    {
        try
        {
            var result = await _permissionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { permissionId = result.PermissionId },
                ApiResponse<PermissionDto>.SuccessResponse(result, "Permission created successfully.", 201));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<PermissionDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdatePermissionDto dto)
    {
        try
        {
            var result = await _permissionService.UpdateAsync(dto);
            return result == null
                ? NotFound(ApiResponse<PermissionDto>.FailureResponse("Permission not found.", 404))
                : Ok(ApiResponse<PermissionDto>.SuccessResponse(result, "Permission updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<PermissionDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeletePermissionDto dto)
    {
        var result = await _permissionService.DeleteAsync(dto);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Permission deleted successfully."))
            : Conflict(ApiResponse<bool>.FailureResponse(
                "Permission not found or is assigned to one or more roles.", 409));
    }

    // 2. MODULE AND STATUS MANAGEMENT

    [HttpGet("module/{moduleName}")]
    public async Task<IActionResult> GetByModuleName(string moduleName)
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetByModuleNameAsync(moduleName)));

    [HttpGet("active")]
    public async Task<IActionResult> GetActivePermissions()
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetActivePermissionsAsync()));

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactivePermissions()
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetInactivePermissionsAsync()));

    [HttpGet("module/{moduleName}/active")]
    public async Task<IActionResult> GetActivePermissionsByModule(string moduleName)
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetActivePermissionsByModuleAsync(moduleName)));

    [HttpPatch("{permissionId:int}/activate")]
    public async Task<IActionResult> Activate(int permissionId)
    {
        var result = await _permissionService.ActivateAsync(permissionId);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Permission activated successfully."))
            : NotFound(ApiResponse<bool>.FailureResponse("Permission not found or already active.", 404));
    }

    [HttpPatch("{permissionId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int permissionId)
    {
        var result = await _permissionService.DeactivateAsync(permissionId);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Permission deactivated successfully."))
            : NotFound(ApiResponse<bool>.FailureResponse("Permission not found or already inactive.", 404));
    }

    // 3. ROLE ASSIGNMENTS

    [HttpGet("role/{roleId:int}")]
    public async Task<IActionResult> GetByRoleId(int roleId)
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetByRoleIdAsync(roleId)));

    [HttpGet("unassigned")]
    public async Task<IActionResult> GetUnassignedPermissions()
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetUnassignedPermissionsAsync()));

    [HttpGet("with-roles")]
    public async Task<IActionResult> GetPermissionsWithRoles()
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetPermissionsWithRolesAsync()));

    [HttpGet("{permissionId:int}/role/{roleId:int}/assigned")]
    public async Task<IActionResult> IsAssignedToRole(int permissionId, int roleId)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _permissionService.IsAssignedToRoleAsync(permissionId, roleId)));

    [HttpGet("{permissionId:int}/roles/count")]
    public async Task<IActionResult> CountRolesByPermission(int permissionId)
        => Ok(ApiResponse<int>.SuccessResponse(
            await _permissionService.CountRolesByPermissionAsync(permissionId)));

    // 4. AUDIT AND SEARCH

    [HttpGet("created-by/{userId:int}")]
    public async Task<IActionResult> GetByCreatedBy(int userId)
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetByCreatedByAsync(userId)));

    [HttpGet("updated-by/{userId:int}")]
    public async Task<IActionResult> GetByUpdatedBy(int userId)
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.GetByUpdatedByAsync(userId)));

    [HttpGet("search")]
    public async Task<IActionResult> SearchPermissions([FromQuery] string searchTerm)
        => Ok(ApiResponse<IEnumerable<PermissionDto>>.SuccessResponse(
            await _permissionService.SearchPermissionsAsync(searchTerm)));

    // 5. VALIDATION

    [HttpGet("exists/code")]
    public async Task<IActionResult> IsCodeExists([FromQuery] string code)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _permissionService.IsCodeExistsAsync(code)));

    [HttpGet("exists/name")]
    public async Task<IActionResult> IsNameExists([FromQuery] string name)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _permissionService.IsNameExistsAsync(name)));

    [HttpGet("{permissionId:int}/can-delete")]
    public async Task<IActionResult> CanPermissionBeDeleted(int permissionId)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _permissionService.CanPermissionBeDeletedAsync(permissionId)));
}
