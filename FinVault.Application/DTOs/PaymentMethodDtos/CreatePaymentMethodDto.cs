using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentMethodDtos;

/// <summary>Input for creating a PaymentMethod.</summary>
public class CreatePaymentMethodDto
{
    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(UPI|NEFT|RTGS|IMPS|NACH|Cheque|Card|Internal)$", ErrorMessage = "Rail must be one of: UPI, NEFT, RTGS, IMPS, NACH, Cheque, Card, Internal.")]
    public string Rail { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MinAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DailyLimit { get; set; }

    public bool Is24x7 { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(RealTime|DeferredNet|Batch)$", ErrorMessage = "SettlementType must be one of: RealTime, DeferredNet, Batch.")]
    public string SettlementType { get; set; } = "RealTime";

    public bool IsActive { get; set; } = true;
}
