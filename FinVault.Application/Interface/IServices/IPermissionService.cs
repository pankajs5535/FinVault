using FinVault.Application.DTOs.PermissionDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IPermissionService
    {
        // 1. CRUD AND IDENTIFICATION
        Task<IEnumerable<PermissionDto>> GetAllAsync();
        Task<PermissionDto?> GetByIdAsync(int permissionId);
        Task<PermissionDto?> GetByCodeAsync(string code);
        Task<PermissionDto?> GetPermissionDetailsAsync(int permissionId);
        Task<PermissionDto> CreateAsync(CreatePermissionDto dto);
        Task<PermissionDto?> UpdateAsync(UpdatePermissionDto dto);
        Task<bool> DeleteAsync(DeletePermissionDto dto);

        // 2. MODULE AND STATUS MANAGEMENT
        Task<IEnumerable<PermissionDto>> GetByModuleNameAsync(string moduleName);
        Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetInactivePermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetActivePermissionsByModuleAsync(string moduleName);
        Task<bool> ActivateAsync(int permissionId);
        Task<bool> DeactivateAsync(int permissionId);

        // 3. ROLE ASSIGNMENTS
        Task<IEnumerable<PermissionDto>> GetByRoleIdAsync(int roleId);
        Task<IEnumerable<PermissionDto>> GetUnassignedPermissionsAsync();
        Task<IEnumerable<PermissionDto>> GetPermissionsWithRolesAsync();
        Task<bool> IsAssignedToRoleAsync(int permissionId, int roleId);
        Task<int> CountRolesByPermissionAsync(int permissionId);

        // 4. AUDIT AND SEARCH
        Task<IEnumerable<PermissionDto>> GetByCreatedByAsync(int userId);
        Task<IEnumerable<PermissionDto>> GetByUpdatedByAsync(int userId);
        Task<IEnumerable<PermissionDto>> SearchPermissionsAsync(string searchTerm);

        // 5. VALIDATION
        Task<bool> IsCodeExistsAsync(string code);
        Task<bool> IsNameExistsAsync(string name);
        Task<bool> CanPermissionBeDeletedAsync(int permissionId);
    }
}


//Banking security rules:
//-Permission codes must be unique.
//- Inactive permissions must not grant access.
//- Changes to permissions must be audited.
//- Prevent deletion while permissions are assigned to roles.
//- Never allow a permission change to silently bypass authorization checks.