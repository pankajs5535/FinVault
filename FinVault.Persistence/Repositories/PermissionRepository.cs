
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class PermissionRepository : GenericRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. PERMISSION IDENTIFICATION
        // =========================================================

        // Retrieves a permission using its unique machine-readable code.
        // Useful for resolving capabilities such as Accounts.Read or Payments.Approve.
        public async Task<Permission?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Code == code);
        }

        // Retrieves complete permission details with audit users and assigned roles.
        // Useful for reviewing permission ownership and role assignments.
        public async Task<Permission?> GetPermissionDetailsAsync(int permissionId)
        {
            return await _dbSet
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.PermissionId == permissionId);
        }

        // Checks whether a permission code already exists.
        // Useful for preventing duplicate permission definitions.
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _dbSet.AnyAsync(x => x.Code == code);
        }

        // =========================================================
        // 2. MODULE AND STATUS
        // =========================================================

        // Retrieves permissions belonging to a particular module.
        // Useful for organizing access capabilities by banking functional area.
        public async Task<IEnumerable<Permission>> GetByModuleNameAsync(string moduleName)
        {
            return await _dbSet
                .Where(x => x.ModuleName == moduleName)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves all active permissions.
        // Useful for identifying capabilities currently available for role assignment.
        public async Task<IEnumerable<Permission>> GetActivePermissionsAsync()
        {
            return await _dbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves all inactive permissions.
        // Useful for reviewing capabilities that have been disabled.
        public async Task<IEnumerable<Permission>> GetInactivePermissionsAsync()
        {
            return await _dbSet
                .Where(x => !x.IsActive)
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves active permissions belonging to a particular module.
        // Useful for displaying only assignable capabilities within a module.
        public async Task<IEnumerable<Permission>> GetActivePermissionsByModuleAsync(string moduleName)
        {
            return await _dbSet
                .Where(x => x.IsActive && x.ModuleName == moduleName)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =========================================================
        // 3. ROLE ASSIGNMENTS
        // =========================================================

        // Retrieves permissions assigned to a particular role.
        // Useful for reviewing the capabilities granted to a banking role.
        public async Task<IEnumerable<Permission>> GetByRoleIdAsync(int roleId)
        {
            return await _dbSet
                .Where(x => x.Roles.Any(r => r.RoleId == roleId))
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves permissions that are not assigned to any role.
        // Useful for identifying unused permission definitions during access reviews.
        public async Task<IEnumerable<Permission>> GetUnassignedPermissionsAsync()
        {
            return await _dbSet
                .Where(x => !x.Roles.Any())
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        // Retrieves permissions with their assigned roles.
        // Useful for auditing which roles can exercise each capability.
        public async Task<IEnumerable<Permission>> GetPermissionsWithRolesAsync()
        {
            return await _dbSet
                .Include(x => x.Roles)
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }

        // Checks whether a permission is assigned to a particular role.
        // Useful for validating role-permission assignments before changes.
        public async Task<bool> IsAssignedToRoleAsync(int permissionId, int roleId)
        {
            return await _dbSet.AnyAsync(x =>
                x.PermissionId == permissionId &&
                x.Roles.Any(r => r.RoleId == roleId));
        }

        // Counts roles assigned to a particular permission.
        // Useful for assessing the impact of disabling or changing a permission.
        public async Task<int> CountRolesByPermissionAsync(int permissionId)
        {
            return await _dbSet
                .Where(x => x.PermissionId == permissionId)
                .SelectMany(x => x.Roles)
                .CountAsync();
        }

        // =========================================================
        // 4. AUDIT AND SEARCH
        // =========================================================

        // Retrieves permissions created by a particular user.
        // Useful for auditing who introduced permission definitions.
        public async Task<IEnumerable<Permission>> GetByCreatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.CreatedBy == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves permissions last updated by a particular user.
        // Useful for tracking responsibility for access-control changes.
        public async Task<IEnumerable<Permission>> GetByUpdatedByAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UpdatedBy == userId)
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // Searches permissions by code, name, module or description.
        // Useful for locating capabilities in a large access-control catalogue.
        public async Task<IEnumerable<Permission>> SearchPermissionsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(x => x.Code.Contains(searchTerm) ||
                            x.Name.Contains(searchTerm) ||
                            x.ModuleName.Contains(searchTerm) ||
                            (x.Description != null && x.Description.Contains(searchTerm)))
                .OrderBy(x => x.ModuleName)
                .ThenBy(x => x.Name)
                .ToListAsync();
        }
    }
}
