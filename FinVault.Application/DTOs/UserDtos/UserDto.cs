using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.UserDtos;

/// <summary>Returned to clients for Get All and Get By Id of User.</summary>
public class UserDto
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime? PasswordChangedAt { get; set; }

    public string UserType { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public int FailedLoginAttempts { get; set; }

    public DateTime? LockoutEndAt { get; set; }

    public bool IsTwoFactorEnabled { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public string? LastLoginIp { get; set; }

    public int RoleId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
