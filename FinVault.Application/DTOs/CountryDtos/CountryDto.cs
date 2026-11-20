using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CountryDtos;

/// <summary>Returned to clients for Get All and Get By Id of Country.</summary>
public class CountryDto
{
    public int CountryId { get; set; }

    public string IsoAlpha2Code { get; set; } = string.Empty;

    public string IsoAlpha3Code { get; set; } = string.Empty;

    public string NumericCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? PhoneCode { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
