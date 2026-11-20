using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CurrencyDtos;

/// <summary>Returned to clients for Get All and Get By Id of Currency.</summary>
public class CurrencyDto
{
    public int CurrencyId { get; set; }

    public string CurrencyCode { get; set; } = string.Empty;

    public string NumericCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Symbol { get; set; } = string.Empty;

    public int DecimalPlaces { get; set; }

    public bool IsBaseCurrency { get; set; }

    public bool IsActive { get; set; }

    public int? CountryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
