
using AutoMapper;
using FinVault.Application.DTOs.PermissionDtos;
using FinVault.Application.DTOs.RoleDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var roles = await _unitOfWork.Role.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<RoleDto?> GetByIdAsync(int roleId)
    {
        var role = await _unitOfWork.Role.GetByIdAsync(roleId);
        return _mapper.Map<RoleDto?>(role);
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        var role = await _unitOfWork.Role.GetByNameAsync(name);
        return _mapper.Map<RoleDto?>(role);
    }

    public async Task<RoleDto?> GetRoleDetailsAsync(int roleId)
    {
        var role = await _unitOfWork.Role.GetRoleDetailsAsync(roleId);
        return _mapper.Map<RoleDto?>(role);
    }

    public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.IsSystemRole)
            throw new InvalidOperationException("System roles can only be created through authorized system initialization.");

        var name = dto.Name.Trim();

        if (await IsRoleNameExistsAsync(name))
            throw new InvalidOperationException("A role with the supplied name already exists.");

        var role = _mapper.Map<Role>(dto);
        role.Name = name;
        role.IsSystemRole = false;
        role.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Role.AddAsync(role);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto?> UpdateAsync(UpdateRoleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var role = await _unitOfWork.Role.GetByIdAsync(dto.RoleId);

        if (role == null)
            return null;

        if (role.IsSystemRole)
            throw new InvalidOperationException("System roles cannot be modified through normal administration.");

        if (dto.IsSystemRole)
            throw new InvalidOperationException("Custom roles cannot be converted into system roles.");

        var name = dto.Name.Trim();
        var existingRole = await _unitOfWork.Role.GetByNameAsync(name);

        if (existingRole != null && existingRole.RoleId != dto.RoleId)
            throw new InvalidOperationException("A role with the supplied name already exists.");

        if (!dto.IsActive && role.IsActive &&
            await _unitOfWork.Role.CountUsersByRoleAsync(dto.RoleId) > 0)
        {
            throw new InvalidOperationException("A role assigned to users cannot be deactivated.");
        }

        _mapper.Map(dto, role);
        role.Name = name;
        role.IsSystemRole = false;
        role.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Role.Update(role);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> DeleteAsync(DeleteRoleDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var role = await _unitOfWork.Role.GetByIdAsync(dto.RoleId);

        if (role == null || role.IsSystemRole ||
            !await CanRoleBeDeletedAsync(dto.RoleId))
            return false;

        _unitOfWork.Role.Delete(role);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. ROLE STATUS AND TYPE

    public async Task<IEnumerable<RoleDto>> GetActiveRolesAsync()
    {
        var roles = await _unitOfWork.Role.GetActiveRolesAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> GetInactiveRolesAsync()
    {
        var roles = await _unitOfWork.Role.GetInactiveRolesAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> GetSystemRolesAsync()
    {
        var roles = await _unitOfWork.Role.GetSystemRolesAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> GetCustomRolesAsync()
    {
        var roles = await _unitOfWork.Role.GetCustomRolesAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<bool> ActivateAsync(int roleId)
    {
        var role = await _unitOfWork.Role.GetByIdAsync(roleId);

        if (role == null || role.IsActive)
            return false;

        role.IsActive = true;
        role.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Role.Update(role);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int roleId)
    {
        var role = await _unitOfWork.Role.GetByIdAsync(roleId);

        if (role == null || !role.IsActive)
            return false;

        if (await _unitOfWork.Role.CountUsersByRoleAsync(roleId) > 0)
            throw new InvalidOperationException("A role assigned to users cannot be deactivated.");

        role.IsActive = false;
        role.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Role.Update(role);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. USER ASSIGNMENTS

    public async Task<RoleDto?> GetRoleWithUsersAsync(int roleId)
    {
        var role = await _unitOfWork.Role.GetRoleWithUsersAsync(roleId);
        return _mapper.Map<RoleDto?>(role);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesWithUsersAsync()
    {
        var roles = await _unitOfWork.Role.GetRolesWithUsersAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesWithoutUsersAsync()
    {
        var roles = await _unitOfWork.Role.GetRolesWithoutUsersAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<int> CountUsersByRoleAsync(int roleId)
    {
        return await _unitOfWork.Role.CountUsersByRoleAsync(roleId);
    }

    public async Task<bool> IsUserAssignedToRoleAsync(int roleId, int userId)
    {
        return await _unitOfWork.Role.IsUserAssignedToRoleAsync(roleId, userId);
    }

    public async Task<bool> AssignRoleToUserAsync(int roleId, int userId)
    {
        var role = await _unitOfWork.Role.GetByIdAsync(roleId);
        var user = await _unitOfWork.User.GetByIdAsync(userId);

        if (role == null || user == null || !role.IsActive || !user.IsActive)
            return false;

        if (user.RoleId == roleId)
            return false;

        user.RoleId = roleId;
        user.SecurityStamp = Guid.NewGuid().ToString("N");
        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.User.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveRoleFromUserAsync(int roleId, int userId)
    {
        // User.RoleId is required. A replacement role must be assigned instead.
        return false;
    }

    // 4. PERMISSION ASSIGNMENTS

    public async Task<RoleDto?> GetRoleWithPermissionsAsync(int roleId)
    {
        var role = await _unitOfWork.Role.GetRoleWithPermissionsAsync(roleId);
        return _mapper.Map<RoleDto?>(role);
    }

    public async Task<IEnumerable<PermissionDto>> GetActivePermissionsByRoleAsync(int roleId)
    {
        var permissions = await _unitOfWork.Role.GetActivePermissionsByRoleAsync(roleId);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesWithPermissionsAsync()
    {
        var roles = await _unitOfWork.Role.GetRolesWithPermissionsAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> GetRolesWithoutPermissionsAsync()
    {
        var roles = await _unitOfWork.Role.GetRolesWithoutPermissionsAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<int> CountPermissionsByRoleAsync(int roleId)
    {
        return await _unitOfWork.Role.CountPermissionsByRoleAsync(roleId);
    }

    public async Task<bool> HasPermissionAsync(int roleId, int permissionId)
    {
        return await _unitOfWork.Role.HasPermissionAsync(roleId, permissionId);
    }

    public async Task<bool> AssignPermissionAsync(int roleId, int permissionId)
    {
        var role = await _unitOfWork.Role.GetRoleWithPermissionsAsync(roleId);
        var permission = await _unitOfWork.Permission.GetByIdAsync(permissionId);

        if (role == null || permission == null || !role.IsActive || !permission.IsActive)
            return false;

        if (role.Permissions.Any(p => p.PermissionId == permissionId))
            return false;

        role.Permissions.Add(permission);
        role.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Role.Update(role);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemovePermissionAsync(int roleId, int permissionId)
    {
        var role = await _unitOfWork.Role.GetRoleWithPermissionsAsync(roleId);

        if (role == null)
            return false;

        var permission = role.Permissions.FirstOrDefault(p => p.PermissionId == permissionId);

        if (permission == null)
            return false;

        role.Permissions.Remove(permission);
        role.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Role.Update(role);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 5. AUDIT AND SEARCH

    public async Task<IEnumerable<RoleDto>> GetByCreatedByAsync(int userId)
    {
        var roles = await _unitOfWork.Role.GetByCreatedByAsync(userId);
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> GetByUpdatedByAsync(int userId)
    {
        var roles = await _unitOfWork.Role.GetByUpdatedByAsync(userId);
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<IEnumerable<RoleDto>> SearchRolesAsync(string searchTerm)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(searchTerm);

        var roles = await _unitOfWork.Role.SearchRolesAsync(searchTerm.Trim());
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    // 6. REPORTING AND VALIDATION

    public async Task<IEnumerable<RoleDto>> GetRolesWithUsersAndPermissionsAsync()
    {
        var roles = await _unitOfWork.Role.GetRolesWithUsersAndPermissionsAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<int> CountActiveRolesAsync()
    {
        return await _unitOfWork.Role.CountActiveRolesAsync();
    }

    public async Task<bool> IsRoleNameExistsAsync(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return await _unitOfWork.Role.ExistsByNameAsync(name.Trim());
    }

    public async Task<bool> CanRoleBeDeletedAsync(int roleId)
    {
        var role = await _unitOfWork.Role.GetByIdAsync(roleId);

        if (role == null || role.IsSystemRole)
            return false;

        return await _unitOfWork.Role.CountUsersByRoleAsync(roleId) == 0;
    }
}
