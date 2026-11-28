
using FinVault.Application.DTOs.RoleDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    // 1. CRUD AND IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetAllAsync()));

    [HttpGet("{roleId:int}")]
    public async Task<IActionResult> GetById(int roleId)
    {
        var result = await _roleService.GetByIdAsync(roleId);
        return result == null
            ? NotFound(ApiResponse<RoleDto>.FailureResponse("Role not found.", 404))
            : Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _roleService.GetByNameAsync(name);
        return result == null
            ? NotFound(ApiResponse<RoleDto>.FailureResponse("Role not found.", 404))
            : Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpGet("{roleId:int}/details")]
    public async Task<IActionResult> GetRoleDetails(int roleId)
    {
        var result = await _roleService.GetRoleDetailsAsync(roleId);
        return result == null
            ? NotFound(ApiResponse<RoleDto>.FailureResponse("Role not found.", 404))
            : Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto)
    {
        try
        {
            var result = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { roleId = result.RoleId },
                ApiResponse<RoleDto>.SuccessResponse(result, "Role created successfully.", 201));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<RoleDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRoleDto dto)
    {
        try
        {
            var result = await _roleService.UpdateAsync(dto);
            return result == null
                ? NotFound(ApiResponse<RoleDto>.FailureResponse("Role not found.", 404))
                : Ok(ApiResponse<RoleDto>.SuccessResponse(result, "Role updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<RoleDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteRoleDto dto)
    {
        var result = await _roleService.DeleteAsync(dto);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Role deleted successfully."))
            : Conflict(ApiResponse<bool>.FailureResponse(
                "Role does not exist, is a system role, or is assigned to users.", 409));
    }

    // 2. ROLE STATUS AND TYPE

    [HttpGet("active")]
    public async Task<IActionResult> GetActiveRoles()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetActiveRolesAsync()));

    [HttpGet("inactive")]
    public async Task<IActionResult> GetInactiveRoles()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetInactiveRolesAsync()));

    [HttpGet("system")]
    public async Task<IActionResult> GetSystemRoles()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetSystemRolesAsync()));

    [HttpGet("custom")]
    public async Task<IActionResult> GetCustomRoles()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetCustomRolesAsync()));

    [HttpPatch("{roleId:int}/activate")]
    public async Task<IActionResult> Activate(int roleId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _roleService.ActivateAsync(roleId)));

    [HttpPatch("{roleId:int}/deactivate")]
    public async Task<IActionResult> Deactivate(int roleId)
    {
        try
        {
            var result = await _roleService.DeactivateAsync(roleId);
            return result
                ? Ok(ApiResponse<bool>.SuccessResponse(true, "Role deactivated successfully."))
                : NotFound(ApiResponse<bool>.FailureResponse("Role not found or already inactive.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<bool>.FailureResponse(ex.Message, 409));
        }
    }

    // 3. USER ASSIGNMENTS

    [HttpGet("{roleId:int}/users")]
    public async Task<IActionResult> GetRoleWithUsers(int roleId)
    {
        var result = await _roleService.GetRoleWithUsersAsync(roleId);
        return result == null
            ? NotFound(ApiResponse<RoleDto>.FailureResponse("Role not found.", 404))
            : Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpGet("with-users")]
    public async Task<IActionResult> GetRolesWithUsers()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetRolesWithUsersAsync()));

    [HttpGet("without-users")]
    public async Task<IActionResult> GetRolesWithoutUsers()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetRolesWithoutUsersAsync()));

    [HttpGet("{roleId:int}/users/count")]
    public async Task<IActionResult> CountUsersByRole(int roleId)
        => Ok(ApiResponse<int>.SuccessResponse(await _roleService.CountUsersByRoleAsync(roleId)));

    [HttpGet("{roleId:int}/users/{userId:int}/assigned")]
    public async Task<IActionResult> IsUserAssignedToRole(int roleId, int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _roleService.IsUserAssignedToRoleAsync(roleId, userId)));

    [HttpPost("{roleId:int}/users/{userId:int}")]
    public async Task<IActionResult> AssignRoleToUser(int roleId, int userId)
    {
        var result = await _roleService.AssignRoleToUserAsync(roleId, userId);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Role assigned to user successfully."))
            : Conflict(ApiResponse<bool>.FailureResponse("Role or user is unavailable, inactive, or already assigned.", 409));
    }

    [HttpDelete("{roleId:int}/users/{userId:int}")]
    public async Task<IActionResult> RemoveRoleFromUser(int roleId, int userId)
    {
        var result = await _roleService.RemoveRoleFromUserAsync(roleId, userId);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Role removed from user successfully."))
            : Conflict(ApiResponse<bool>.FailureResponse(
                "Role removal is unavailable. The user requires a replacement role.", 409));
    }

    // 4. PERMISSION ASSIGNMENTS

    [HttpGet("{roleId:int}/permissions")]
    public async Task<IActionResult> GetRoleWithPermissions(int roleId)
    {
        var result = await _roleService.GetRoleWithPermissionsAsync(roleId);
        return result == null
            ? NotFound(ApiResponse<RoleDto>.FailureResponse("Role not found.", 404))
            : Ok(ApiResponse<RoleDto>.SuccessResponse(result));
    }

    [HttpGet("{roleId:int}/permissions/active")]
    public async Task<IActionResult> GetActivePermissionsByRole(int roleId)
        => Ok(ApiResponse<IEnumerable<FinVault.Application.DTOs.PermissionDtos.PermissionDto>>.SuccessResponse(
            await _roleService.GetActivePermissionsByRoleAsync(roleId)));

    [HttpGet("with-permissions")]
    public async Task<IActionResult> GetRolesWithPermissions()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetRolesWithPermissionsAsync()));

    [HttpGet("without-permissions")]
    public async Task<IActionResult> GetRolesWithoutPermissions()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetRolesWithoutPermissionsAsync()));

    [HttpGet("{roleId:int}/permissions/count")]
    public async Task<IActionResult> CountPermissionsByRole(int roleId)
        => Ok(ApiResponse<int>.SuccessResponse(await _roleService.CountPermissionsByRoleAsync(roleId)));

    [HttpGet("{roleId:int}/permissions/{permissionId:int}/exists")]
    public async Task<IActionResult> HasPermission(int roleId, int permissionId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _roleService.HasPermissionAsync(roleId, permissionId)));

    [HttpPost("{roleId:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> AssignPermission(int roleId, int permissionId)
    {
        var result = await _roleService.AssignPermissionAsync(roleId, permissionId);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Permission assigned successfully."))
            : Conflict(ApiResponse<bool>.FailureResponse(
                "Role or permission is unavailable, inactive, or already assigned.", 409));
    }

    [HttpDelete("{roleId:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> RemovePermission(int roleId, int permissionId)
    {
        var result = await _roleService.RemovePermissionAsync(roleId, permissionId);
        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Permission removed successfully."))
            : NotFound(ApiResponse<bool>.FailureResponse("Role or permission assignment not found.", 404));
    }

    // 5. AUDIT AND SEARCH

    [HttpGet("created-by/{userId:int}")]
    public async Task<IActionResult> GetByCreatedBy(int userId)
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetByCreatedByAsync(userId)));

    [HttpGet("updated-by/{userId:int}")]
    public async Task<IActionResult> GetByUpdatedBy(int userId)
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.GetByUpdatedByAsync(userId)));

    [HttpGet("search")]
    public async Task<IActionResult> SearchRoles([FromQuery] string searchTerm)
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(await _roleService.SearchRolesAsync(searchTerm)));

    // 6. REPORTING AND VALIDATION

    [HttpGet("with-users-and-permissions")]
    public async Task<IActionResult> GetRolesWithUsersAndPermissions()
        => Ok(ApiResponse<IEnumerable<RoleDto>>.SuccessResponse(
            await _roleService.GetRolesWithUsersAndPermissionsAsync()));

    [HttpGet("count/active")]
    public async Task<IActionResult> CountActiveRoles()
        => Ok(ApiResponse<int>.SuccessResponse(await _roleService.CountActiveRolesAsync()));

    [HttpGet("exists")]
    public async Task<IActionResult> IsRoleNameExists([FromQuery] string name)
        => Ok(ApiResponse<bool>.SuccessResponse(await _roleService.IsRoleNameExistsAsync(name)));

    [HttpGet("{roleId:int}/can-delete")]
    public async Task<IActionResult> CanRoleBeDeleted(int roleId)
        => Ok(ApiResponse<bool>.SuccessResponse(await _roleService.CanRoleBeDeletedAsync(roleId)));
}
