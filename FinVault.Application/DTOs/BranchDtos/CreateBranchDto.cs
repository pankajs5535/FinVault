using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.BranchDtos;

/// <summary>Input for creating a Branch.</summary>
public class CreateBranchDto
{
    [Required]
    [StringLength(10)]
    public string BranchCode { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string BranchName { get; set; } = string.Empty;

    [Required]
    [StringLength(11)]
    [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code.")]
    public string IfscCode { get; set; } = string.Empty;

    [StringLength(9)]
    public string? MicrCode { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Regular|Main|Digital|Service)$", ErrorMessage = "BranchType must be one of: Regular, Main, Digital, Service.")]
    public string BranchType { get; set; } = "Regular";

    [Required]
    [StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string District { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(6)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN code must be 6 digits.")]
    public string PinCode { get; set; } = string.Empty;

    [StringLength(15)]
    [Phone]
    public string? PhoneNumber { get; set; }

    [StringLength(256)]
    [EmailAddress]
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime OpenedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ParentBranchId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ManagerEmployeeId { get; set; }
}
