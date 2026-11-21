
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. ROLE IDENTIFICATION
        // =========================================================

        // Retrieves a role using its unique name.
        // Useful for resolving roles such as Teller, BranchManager and ComplianceOfficer.
        public async Task<Role?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Name == name);
        }

        // Retrieves complete role details with audit users, users and permissions.
        // Useful for reviewing the full access configuration of a banking role.
        public async Task<Role?> GetRoleDetailsAsync(int roleId)
        {
            return await _dbSet
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.Users)
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x => x.RoleId == roleId);
        }

        // Checks whether a role name already exists.
        // Useful for preventing duplicate role names that violate the unique database index.
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(x => x.Name == name);
        }

        // =========================================================
        // 2. ROLE STATUS AND TYPE
        // =========================================================

        // Retrieves all active roles.
        // Useful for identifying roles available for user assignment.
        public async Task<IEnumerable<Role>> GetActiveRolesAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves all inactive roles.
        // Useful for reviewing roles that are no longer available for assignment.
        public async Task<IEnumerable<Role>> GetInactiveRolesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves system-defined roles.
        // Useful for identifying protected roles used by the banking application.
        public async Task<IEnumerable<Role>> GetSystemRolesAsync()
        {
            return await _dbSet
                .Where(x => x.IsSystemRole)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves roles created by administrators or other authorized users.
        // Useful for reviewing application-defined roles separately from system roles.
        public async Task<IEnumerable<Role>> GetCustomRolesAsync()
        {
            return await _dbSet
                .Where(x => !x.IsSystemRole)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 3. USER ASSIGNMENTS
        // =========================================================

        // Retrieves users assigned to a particular role.
        // Useful for reviewing which employees or users hold a specific banking role.
        public async Task<Role?> GetRoleWithUsersAsync(int roleId)
        {
            return await _dbSet
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.RoleId == roleId);
        }

        // Retrieves roles that have at least one assigned user.
        // Useful for identifying roles currently in use.
        public async Task<IEnumerable<Role>> GetRolesWithUsersAsync()
        {
            return await _dbSet
                .Where(x => x.Users.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves roles that have no assigned users.
        // Useful for identifying unused roles during access-control reviews.
        public async Task<IEnumerable<Role>> GetRolesWithoutUsersAsync()
        {
            return await _dbSet
                .Where(x => !x.Users.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Counts users assigned to a particular role.
        // Useful for assessing the impact of role deactivation or modification.
        public async Task<int> CountUsersByRoleAsync(int roleId)
        {
            return await _dbSet
                .Where(x => x.RoleId == roleId)
                .SelectMany(x => x.Users)
                .CountAsync();
        }

        // Checks whether a user is assigned to a particular role.
        // Useful for validating role membership before access-control operations.
        public async Task<bool> IsUserAssignedToRoleAsync(int roleId, int userId)
        {
            return await _dbSet.AnyAsync(x =>
                x.RoleId == roleId &&
                x.Users.Any(u => u.UserId == userId));
        }

        // =========================================================
        // 4. PERMISSION ASSIGNMENTS
        // =========================================================

        // Retrieves permissions assigned to a particular role.
        // Useful for reviewing the capabilities granted to that role.
        public async Task<Role?> GetRoleWithPermissionsAsync(int roleId)
        {
            return await _dbSet
                .Include(x => x.Permissions)
                .FirstOrDefaultAsync(x => x.RoleId == roleId);
        }

        // Retrieves active permissions assigned to a particular role.
        // Useful for determining the currently enabled capabilities granted by a role.
        public async Task<IEnumerable<Permission>> GetActivePermissionsByRoleAsync(int roleId)
        {
            return await _dbSet
                .Where(x => x.RoleId == roleId)
                .SelectMany(x => x.Permissions)
                .Where(x => x.IsActive)
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves roles that have at least one permission assigned.
        // Useful for identifying roles with configured access capabilities.
        public async Task<IEnumerable<Role>> GetRolesWithPermissionsAsync()
        {
            return await _dbSet
                .Where(x => x.Permissions.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves roles without any permissions assigned.
        // Useful for identifying incomplete role configurations.
        public async Task<IEnumerable<Role>> GetRolesWithoutPermissionsAsync()
        {
            return await _dbSet
                .Where(x => !x.Permissions.Any())
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Counts permissions assigned to a particular role.
        // Useful for reviewing the size of a role's access-control scope.
        public async Task<int> CountPermissionsByRoleAsync(int roleId)
        {
            return await _dbSet
                .Where(x => x.RoleId == roleId)
                .SelectMany(x => x.Permissions)
                .CountAsync();
        }

        // Checks whether a role contains a particular permission.
        // Useful for validating role-permission assignments.
        public async Task<bool> HasPermissionAsync(int roleId, int permissionId)
        {
            return await _dbSet.AnyAsync(x =>
                x.RoleId == roleId &&
                x.Permissions.Any(p => p.PermissionId == permissionId));
        }

        // =========================================================
        // 5. AUDIT AND SEARCH
        // =========================================================

        // Retrieves roles created by a particular user.
        // Useful for auditing who introduced role definitions.
        public async Task<IEnumerable<Role>> GetByCreatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves roles last updated by a particular user.
        // Useful for tracking responsibility for role configuration changes.
        public async Task<IEnumerable<Role>> GetByUpdatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UpdatedBy == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // Searches roles by name or description.
        // Useful for locating roles in the access-control catalogue.
        public async Task<IEnumerable<Role>> SearchRolesAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.Name.Contains(searchTerm) ||
                            (x.Description != null && x.Description.Contains(searchTerm)))
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 6. REPORTING
        // =========================================================

        // Retrieves role details with both users and permissions.
        // Useful for administrative access reviews and role audits.
        public async Task<IEnumerable<Role>> GetRolesWithUsersAndPermissionsAsync()
        {
            return await _dbSet
                .Include(x => x.Users)
                .Include(x => x.Permissions)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Counts active roles in the system.
        // Useful for monitoring the number of available role definitions.
        public async Task<int> CountActiveRolesAsync()
        {
            return await _dbSet.CountAsync(x => x.IsActive);
        }
    }
}
