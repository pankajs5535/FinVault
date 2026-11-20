using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AddressDtos;

/// <summary>Returned to clients for Get All and Get By Id of Address.</summary>
public class AddressDto
{
    public int AddressId { get; set; }

    public string AddressType { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string? Landmark { get; set; }

    public string City { get; set; } = string.Empty;

    public string? District { get; set; }

    public string State { get; set; } = string.Empty;

    public string PinCode { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public int CustomerId { get; set; }

    public int CountryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
