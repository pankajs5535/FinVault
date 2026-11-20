using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FeeDtos;

/// <summary>Input for creating a Fee.</summary>
public class CreateFeeDto
{
    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Flat|Percentage|Slab)$", ErrorMessage = "CalculationMethod must be one of: Flat, Percentage, Slab.")]
    public string CalculationMethod { get; set; } = "Flat";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(PerTransaction|Monthly|Quarterly|Yearly|OneTime)$", ErrorMessage = "Frequency must be one of: PerTransaction, Monthly, Quarterly, Yearly, OneTime.")]
    public string Frequency { get; set; } = "PerTransaction";

    [Range(0, double.MaxValue)]
    public decimal? FlatAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Percentage { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinimumAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaximumAmount { get; set; }

    public bool IsGstApplicable { get; set; } = true;

    [Range(0, double.MaxValue)]
    public decimal GstRate { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int? TransactionTypeId { get; set; }

    [Range(1, int.MaxValue)]
    public int? PaymentMethodId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AccountTypeId { get; set; }
}
