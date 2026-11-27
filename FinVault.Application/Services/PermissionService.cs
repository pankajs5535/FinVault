
using AutoMapper;
using FinVault.Application.DTOs.PermissionDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PermissionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<PermissionDto>> GetAllAsync()
    {
        var permissions = await _unitOfWork.Permission.GetAllAsync();
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<PermissionDto?> GetByIdAsync(int permissionId)
    {
        if (permissionId <= 0)
            return null;

        var permission = await _unitOfWork.Permission.GetByIdAsync(permissionId);
        return permission == null ? null : _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto?> GetByCodeAsync(string code)
    {
        ValidateRequiredText(code, nameof(code), 100);

        var permission = await _unitOfWork.Permission.GetByCodeAsync(code.Trim());
        return permission == null ? null : _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto?> GetPermissionDetailsAsync(int permissionId)
    {
        if (permissionId <= 0)
            return null;

        var permission = await _unitOfWork.Permission.GetPermissionDetailsAsync(permissionId);
        return permission == null ? null : _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto> CreateAsync(CreatePermissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRequiredText(dto.Code, nameof(dto.Code), 100);
        ValidateRequiredText(dto.Name, nameof(dto.Name), 100);
        ValidateRequiredText(dto.ModuleName, nameof(dto.ModuleName), 50);

        var code = dto.Code.Trim();

        if (await _unitOfWork.Permission.ExistsByCodeAsync(code))
            throw new InvalidOperationException("A permission with this code already exists.");

        var name = dto.Name.Trim();

        if (await IsNameExistsAsync(name))
            throw new InvalidOperationException("A permission with this name already exists.");

        var permission = _mapper.Map<Permission>(dto);

        permission.Code = code;
        permission.Name = name;
        permission.ModuleName = dto.ModuleName.Trim();
        permission.Description = dto.Description?.Trim();
        permission.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Permission.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PermissionDto>(permission);
    }

    public async Task<PermissionDto?> UpdateAsync(UpdatePermissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.PermissionId <= 0)
            return null;

        ValidateRequiredText(dto.Code, nameof(dto.Code), 100);
        ValidateRequiredText(dto.Name, nameof(dto.Name), 100);
        ValidateRequiredText(dto.ModuleName, nameof(dto.ModuleName), 50);

        var permission = await _unitOfWork.Permission.GetByIdAsync(dto.PermissionId);

        if (permission == null)
            return null;

        var code = dto.Code.Trim();

        var existingCode = await _unitOfWork.Permission.GetByCodeAsync(code);

        if (existingCode != null && existingCode.PermissionId != dto.PermissionId)
            throw new InvalidOperationException("A permission with this code already exists.");

        var name = dto.Name.Trim();

        if (await IsNameExistsAsync(name))
        {
            var existingName = await _unitOfWork.Permission.FindAsync(
                p => p.Name == name && p.PermissionId != dto.PermissionId);

            if (existingName.Any())
                throw new InvalidOperationException("A permission with this name already exists.");
        }

        permission.Code = code;
        permission.Name = name;
        permission.ModuleName = dto.ModuleName.Trim();
        permission.Description = dto.Description?.Trim();
        permission.IsActive = dto.IsActive;
        permission.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Permission.Update(permission);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PermissionDto>(permission);
    }

    public async Task<bool> DeleteAsync(DeletePermissionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.PermissionId <= 0)
            return false;

        var permission = await _unitOfWork.Permission.GetByIdAsync(dto.PermissionId);

        if (permission == null || !await CanPermissionBeDeletedAsync(dto.PermissionId))
            return false;

        _unitOfWork.Permission.Delete(permission);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. MODULE AND STATUS MANAGEMENT

    public async Task<IEnumerable<PermissionDto>> GetByModuleNameAsync(string moduleName)
    {
        ValidateRequiredText(moduleName, nameof(moduleName), 50);

        var permissions = await _unitOfWork.Permission.GetByModuleNameAsync(moduleName.Trim());
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> GetActivePermissionsAsync()
    {
        var permissions = await _unitOfWork.Permission.GetActivePermissionsAsync();
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> GetInactivePermissionsAsync()
    {
        var permissions = await _unitOfWork.Permission.GetInactivePermissionsAsync();
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> GetActivePermissionsByModuleAsync(string moduleName)
    {
        ValidateRequiredText(moduleName, nameof(moduleName), 50);

        var permissions = await _unitOfWork.Permission.GetActivePermissionsByModuleAsync(moduleName.Trim());
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<bool> ActivateAsync(int permissionId)
    {
        var permission = await _unitOfWork.Permission.GetByIdAsync(permissionId);

        if (permission == null || permission.IsActive)
            return false;

        permission.IsActive = true;
        permission.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Permission.Update(permission);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int permissionId)
    {
        var permission = await _unitOfWork.Permission.GetByIdAsync(permissionId);

        if (permission == null || !permission.IsActive)
            return false;

        permission.IsActive = false;
        permission.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Permission.Update(permission);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. ROLE ASSIGNMENTS

    public async Task<IEnumerable<PermissionDto>> GetByRoleIdAsync(int roleId)
    {
        if (roleId <= 0)
            return Enumerable.Empty<PermissionDto>();

        var permissions = await _unitOfWork.Permission.GetByRoleIdAsync(roleId);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> GetUnassignedPermissionsAsync()
    {
        var permissions = await _unitOfWork.Permission.GetUnassignedPermissionsAsync();
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> GetPermissionsWithRolesAsync()
    {
        var permissions = await _unitOfWork.Permission.GetPermissionsWithRolesAsync();
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<bool> IsAssignedToRoleAsync(int permissionId, int roleId)
    {
        if (permissionId <= 0 || roleId <= 0)
            return false;

        return await _unitOfWork.Permission.IsAssignedToRoleAsync(permissionId, roleId);
    }

    public async Task<int> CountRolesByPermissionAsync(int permissionId)
    {
        if (permissionId <= 0)
            return 0;

        return await _unitOfWork.Permission.CountRolesByPermissionAsync(permissionId);
    }

    // 4. AUDIT AND SEARCH

    public async Task<IEnumerable<PermissionDto>> GetByCreatedByAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<PermissionDto>();

        var permissions = await _unitOfWork.Permission.GetByCreatedByAsync(userId);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> GetByUpdatedByAsync(int userId)
    {
        if (userId <= 0)
            return Enumerable.Empty<PermissionDto>();

        var permissions = await _unitOfWork.Permission.GetByUpdatedByAsync(userId);
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    public async Task<IEnumerable<PermissionDto>> SearchPermissionsAsync(string searchTerm)
    {
        ValidateRequiredText(searchTerm, nameof(searchTerm), 250);

        var permissions = await _unitOfWork.Permission.SearchPermissionsAsync(searchTerm.Trim());
        return _mapper.Map<IEnumerable<PermissionDto>>(permissions);
    }

    // 5. VALIDATION

    public Task<bool> IsCodeExistsAsync(string code)
    {
        ValidateRequiredText(code, nameof(code), 100);
        return _unitOfWork.Permission.ExistsByCodeAsync(code.Trim());
    }

    public async Task<bool> IsNameExistsAsync(string name)
    {
        ValidateRequiredText(name, nameof(name), 100);

        return await _unitOfWork.Permission.ExistsAsync(
            p => p.Name == name.Trim());
    }

    public async Task<bool> CanPermissionBeDeletedAsync(int permissionId)
    {
        if (permissionId <= 0)
            return false;

        var permission = await _unitOfWork.Permission.GetByIdAsync(permissionId);

        if (permission == null)
            return false;

        return await _unitOfWork.Permission.CountRolesByPermissionAsync(permissionId) == 0;
    }

    // PRIVATE VALIDATION

    private static void ValidateRequiredText(string value, string parameterName, int maxLength)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);

        if (value.Trim().Length > maxLength)
            throw new ArgumentException(
                $"{parameterName} cannot exceed {maxLength} characters.",
                parameterName);
    }
}
