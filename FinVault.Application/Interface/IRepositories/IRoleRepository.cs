
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        // =========================================================
        // 1. ROLE IDENTIFICATION
        // =========================================================

        // Retrieves a role using its unique name.
        // Useful for resolving roles such as Teller, BranchManager and ComplianceOfficer.
        Task<Role?> GetByNameAsync(string name);

        // Retrieves complete role details with audit users, users and permissions.
        // Useful for reviewing the full access configuration of a banking role.
        Task<Role?> GetRoleDetailsAsync(int roleId);

        // Checks whether a role name already exists.
        // Useful for preventing duplicate role names that violate the unique database index.
        Task<bool> ExistsByNameAsync(string name);

        // =========================================================
        // 2. ROLE STATUS AND TYPE
        // =========================================================

        // Retrieves all active roles.
        // Useful for identifying roles available for user assignment.
        Task<IEnumerable<Role>> GetActiveRolesAsync();

        // Retrieves all inactive roles.
        // Useful for reviewing roles that are no longer available for assignment.
        Task<IEnumerable<Role>> GetInactiveRolesAsync();

        // Retrieves system-defined roles.
        // Useful for identifying protected roles used by the banking application.
        Task<IEnumerable<Role>> GetSystemRolesAsync();

        // Retrieves roles created by administrators or other authorized users.
        // Useful for reviewing application-defined roles separately from system roles.
        Task<IEnumerable<Role>> GetCustomRolesAsync();

        // =========================================================
        // 3. USER ASSIGNMENTS
        // =========================================================

        // Retrieves users assigned to a particular role.
        // Useful for reviewing which employees or users hold a specific banking role.
        Task<Role?> GetRoleWithUsersAsync(int roleId);

        // Retrieves roles that have at least one assigned user.
        // Useful for identifying roles currently in use.
        Task<IEnumerable<Role>> GetRolesWithUsersAsync();

        // Retrieves roles that have no assigned users.
        // Useful for identifying unused roles during access-control reviews.
        Task<IEnumerable<Role>> GetRolesWithoutUsersAsync();

        // Counts users assigned to a particular role.
        // Useful for assessing the impact of role deactivation or modification.
        Task<int> CountUsersByRoleAsync(int roleId);

        // Checks whether a user is assigned to a particular role.
        // Useful for validating role membership before access-control operations.
        Task<bool> IsUserAssignedToRoleAsync(int roleId, int userId);

        // =========================================================
        // 4. PERMISSION ASSIGNMENTS
        // =========================================================

        // Retrieves permissions assigned to a particular role.
        // Useful for reviewing the capabilities granted to that role.
        Task<Role?> GetRoleWithPermissionsAsync(int roleId);

        // Retrieves active permissions assigned to a particular role.
        // Useful for determining the currently enabled capabilities granted by a role.
        Task<IEnumerable<Permission>> GetActivePermissionsByRoleAsync(int roleId);

        // Retrieves roles that have at least one permission assigned.
        // Useful for identifying roles with configured access capabilities.
        Task<IEnumerable<Role>> GetRolesWithPermissionsAsync();

        // Retrieves roles without any permissions assigned.
        // Useful for identifying incomplete role configurations.
        Task<IEnumerable<Role>> GetRolesWithoutPermissionsAsync();

        // Counts permissions assigned to a particular role.
        // Useful for reviewing the size of a role's access-control scope.
        Task<int> CountPermissionsByRoleAsync(int roleId);

        // Checks whether a role contains a particular permission.
        // Useful for validating role-permission assignments.
        Task<bool> HasPermissionAsync(int roleId, int permissionId);

        // =========================================================
        // 5. AUDIT AND SEARCH
        // =========================================================

        // Retrieves roles created by a particular user.
        // Useful for auditing who introduced role definitions.
        Task<IEnumerable<Role>> GetByCreatedByAsync(int userId);

        // Retrieves roles last updated by a particular user.
        // Useful for tracking responsibility for role configuration changes.
        Task<IEnumerable<Role>> GetByUpdatedByAsync(int userId);

        // Searches roles by name or description.
        // Useful for locating roles in the access-control catalogue.
        Task<IEnumerable<Role>> SearchRolesAsync(string searchTerm);

        // =========================================================
        // 6. REPORTING
        // =========================================================

        // Retrieves role details with both users and permissions.
        // Useful for administrative access reviews and role audits.
        Task<IEnumerable<Role>> GetRolesWithUsersAndPermissionsAsync();

        // Counts active roles in the system.
        // Useful for monitoring the number of available role definitions.
        Task<int> CountActiveRolesAsync();
    }
}
