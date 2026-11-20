using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CurrencyDtos;

/// <summary>Input for creating a Currency.</summary>
public class CreateCurrencyDto
{
    [Required]
    [StringLength(3)]
    public string CurrencyCode { get; set; } = string.Empty;

    [Required]
    [StringLength(3)]
    public string NumericCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(5)]
    public string Symbol { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int DecimalPlaces { get; set; } = 2;

    public bool IsBaseCurrency { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int? CountryId { get; set; }
}
