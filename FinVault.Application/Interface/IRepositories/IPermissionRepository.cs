
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        // =========================================================
        // 1. PERMISSION IDENTIFICATION
        // =========================================================

        // Retrieves a permission using its unique machine-readable code.
        // Useful for resolving capabilities such as Accounts.Read or Payments.Approve.
        Task<Permission?> GetByCodeAsync(string code);

        // Retrieves complete permission details with audit users and assigned roles.
        // Useful for reviewing permission ownership and role assignments.
        Task<Permission?> GetPermissionDetailsAsync(int permissionId);

        // Checks whether a permission code already exists.
        // Useful for preventing duplicate permission definitions.
        Task<bool> ExistsByCodeAsync(string code);

        // =========================================================
        // 2. MODULE AND STATUS
        // =========================================================

        // Retrieves permissions belonging to a particular module.
        // Useful for organizing access capabilities by banking functional area.
        Task<IEnumerable<Permission>> GetByModuleNameAsync(string moduleName);

        // Retrieves all active permissions.
        // Useful for identifying capabilities currently available for role assignment.
        Task<IEnumerable<Permission>> GetActivePermissionsAsync();

        // Retrieves all inactive permissions.
        // Useful for reviewing capabilities that have been disabled.
        Task<IEnumerable<Permission>> GetInactivePermissionsAsync();

        // Retrieves active permissions belonging to a particular module.
        // Useful for displaying only assignable capabilities within a module.
        Task<IEnumerable<Permission>> GetActivePermissionsByModuleAsync(string moduleName);

        // =========================================================
        // 3. ROLE ASSIGNMENTS
        // =========================================================

        // Retrieves permissions assigned to a particular role.
        // Useful for reviewing the capabilities granted to a banking role.
        Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId);

        // Retrieves permissions that are not assigned to any role.
        // Useful for identifying unused permission definitions during access reviews.
        Task<IEnumerable<Permission>> GetUnassignedPermissionsAsync();

        // Retrieves permissions with their assigned roles.
        // Useful for auditing which roles can exercise each capability.
        Task<IEnumerable<Permission>> GetPermissionsWithRolesAsync();

        // Checks whether a permission is assigned to a particular role.
        // Useful for validating role-permission assignments before changes.
        Task<bool> IsAssignedToRoleAsync(int permissionId, int roleId);

        // Counts roles assigned to a particular permission.
        // Useful for assessing the impact of disabling or changing a permission.
        Task<int> CountRolesByPermissionAsync(int permissionId);

        // =========================================================
        // 4. AUDIT AND SEARCH
        // =========================================================

        // Retrieves permissions created by a particular user.
        // Useful for auditing who introduced permission definitions.
        Task<IEnumerable<Permission>> GetByCreatedByAsync(int userId);

        // Retrieves permissions last updated by a particular user.
        // Useful for tracking responsibility for access-control changes.
        Task<IEnumerable<Permission>> GetByUpdatedByAsync(int userId);

        // Searches permissions by code, name, module or description.
        // Useful for locating capabilities in a large access-control catalogue.
        Task<IEnumerable<Permission>> SearchPermissionsAsync(string searchTerm);
    }
}
