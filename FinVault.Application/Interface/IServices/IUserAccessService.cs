using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.UserDtos;
using FinVault.Application.DTOs.RoleDtos;
using FinVault.Application.DTOs.PermissionDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the UserAccess banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IUserAccessService
{
        Task<UserDto?> GetUserAsync(int userId);
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto dto);
        Task<IReadOnlyList<RoleDto>> GetRolesAsync();
        Task<IReadOnlyList<PermissionDto>> GetPermissionsAsync();
        Task<bool> AssignRoleAsync(int userId, int roleId);
}
