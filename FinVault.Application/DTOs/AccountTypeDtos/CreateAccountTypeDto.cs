using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountTypeDtos;

/// <summary>Input for creating an AccountType.</summary>
public class CreateAccountTypeDto
{
    [Required]
    [StringLength(20)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Savings|Current|FixedDeposit|RecurringDeposit|Internal)$", ErrorMessage = "Category must be one of: Savings, Current, FixedDeposit, RecurringDeposit, Internal.")]
    public string Category { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MinimumBalance { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MinimumAverageBalance { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DailyTransactionLimit { get; set; }

    public bool AllowsOverdraft { get; set; }

    public bool IsInterestBearing { get; set; }

    [StringLength(20)]
    [RegularExpression(@"^(Monthly|Quarterly|HalfYearly|Yearly|AtMaturity)$", ErrorMessage = "InterestPayoutFrequency must be one of: Monthly, Quarterly, HalfYearly, Yearly, AtMaturity.")]
    public string? InterestPayoutFrequency { get; set; }

    [Range(0, int.MaxValue)]
    public int? MinTenureMonths { get; set; }

    [Range(0, int.MaxValue)]
    public int? MaxTenureMonths { get; set; }

    public bool IsActive { get; set; } = true;
}
