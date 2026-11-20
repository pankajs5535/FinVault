using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AddressDtos;

/// <summary>Input for updating an Address.</summary>
public class UpdateAddressDto
{
    [Range(1, int.MaxValue)]
    public int AddressId { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Residential|Permanent|Correspondence|Office|Registered)$", ErrorMessage = "AddressType must be one of: Residential, Permanent, Correspondence, Office, Registered.")]
    public string AddressType { get; set; } = "Residential";

    [Required]
    [StringLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(200)]
    public string? AddressLine2 { get; set; }

    [StringLength(100)]
    public string? Landmark { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [StringLength(100)]
    public string? District { get; set; }

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(6)]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "PIN code must be 6 digits.")]
    public string PinCode { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedAt { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }
}
