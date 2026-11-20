using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CountryDtos;

/// <summary>Input for updating a Country.</summary>
public class UpdateCountryDto
{
    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }

    [Required]
    [StringLength(2)]
    public string IsoAlpha2Code { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string IsoAlpha3Code { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string NumericCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(6)]
    public string? PhoneCode { get; set; }

    public bool IsActive { get; set; } = true;
}
