using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.UserDtos;

/// <summary>Input for creating a User.</summary>
public class CreateUserDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(256)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [StringLength(15)]
    [Phone]
    public string? PhoneNumber { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Customer|Employee|Admin|System)$", ErrorMessage = "UserType must be one of: Customer, Employee, Admin, System.")]
    public string UserType { get; set; } = "Customer";

    public bool IsActive { get; set; } = true;

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public bool IsTwoFactorEnabled { get; set; }

    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }

    /// <summary>Plain-text password supplied once; the service layer hashes it into PasswordHash. Never returned.</summary>
    [Required]
    [StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
