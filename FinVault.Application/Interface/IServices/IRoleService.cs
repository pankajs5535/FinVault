using FinVault.Application.DTOs.PermissionDtos;
using FinVault.Application.DTOs.RoleDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IRoleService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto?> GetByIdAsync(int roleId);
        Task<RoleDto?> GetByNameAsync(string name);
        Task<RoleDto?> GetRoleDetailsAsync(int roleId);
        Task<RoleDto> CreateAsync(CreateRoleDto dto);
        Task<RoleDto?> UpdateAsync(UpdateRoleDto dto);
        Task<bool> DeleteAsync(DeleteRoleDto dto);

        // 2. ROLE STATUS AND TYPE
        Task<IEnumerable<RoleDto>> GetActiveRolesAsync();
        Task<IEnumerable<RoleDto>> GetInactiveRolesAsync();
        Task<IEnumerable<RoleDto>> GetSystemRolesAsync();
        Task<IEnumerable<RoleDto>> GetCustomRolesAsync();
        Task<bool> ActivateAsync(int roleId);
        Task<bool> DeactivateAsync(int roleId);

        // 3. USER ASSIGNMENTS
        Task<RoleDto?> GetRoleWithUsersAsync(int roleId);
        Task<IEnumerable<RoleDto>> GetRolesWithUsersAsync();
        Task<IEnumerable<RoleDto>> GetRolesWithoutUsersAsync();
        Task<int> CountUsersByRoleAsync(int roleId);
        Task<bool> IsUserAssignedToRoleAsync(int roleId, int userId);
        Task<bool> AssignRoleToUserAsync(int roleId, int userId);
        Task<bool> RemoveRoleFromUserAsync(int roleId, int userId);

        // 4. PERMISSION ASSIGNMENTS
        Task<RoleDto?> GetRoleWithPermissionsAsync(int roleId);
        Task<IEnumerable<PermissionDto>> GetActivePermissionsByRoleAsync(int roleId);
        Task<IEnumerable<RoleDto>> GetRolesWithPermissionsAsync();
        Task<IEnumerable<RoleDto>> GetRolesWithoutPermissionsAsync();
        Task<int> CountPermissionsByRoleAsync(int roleId);
        Task<bool> HasPermissionAsync(int roleId, int permissionId);
        Task<bool> AssignPermissionAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionAsync(int roleId, int permissionId);

        // 5. AUDIT AND SEARCH
        Task<IEnumerable<RoleDto>> GetByCreatedByAsync(int userId);
        Task<IEnumerable<RoleDto>> GetByUpdatedByAsync(int userId);
        Task<IEnumerable<RoleDto>> SearchRolesAsync(string searchTerm);

        // 6. REPORTING AND VALIDATION
        Task<IEnumerable<RoleDto>> GetRolesWithUsersAndPermissionsAsync();
        Task<int> CountActiveRolesAsync();
        Task<bool> IsRoleNameExistsAsync(string name);
        Task<bool> CanRoleBeDeletedAsync(int roleId);
    }
}

//Banking security rules:
//-System roles cannot be edited or deleted through normal administration.
//- Prevent deletion of roles assigned to users.
//- Validate permission existence and prevent duplicate role-permission assignments.
//- Changes to roles and permissions must be audited.
//- Deactivating a role must account for users currently assigned to it.