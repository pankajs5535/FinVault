using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.UserDtos;

/// <summary>Input for updating a User.</summary>
public class UpdateUserDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

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

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
