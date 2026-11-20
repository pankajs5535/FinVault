using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FeeDtos;

/// <summary>Returned to clients for Get All and Get By Id of Fee.</summary>
public class FeeDto
{
    public int FeeId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string CalculationMethod { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public decimal? FlatAmount { get; set; }

    public decimal? Percentage { get; set; }

    public decimal? MinimumAmount { get; set; }

    public decimal? MaximumAmount { get; set; }

    public bool IsGstApplicable { get; set; }

    public decimal GstRate { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public int? TransactionTypeId { get; set; }

    public int? PaymentMethodId { get; set; }

    public int? AccountTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
