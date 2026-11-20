using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.InterestRateDtos;

/// <summary>Input for updating an InterestRate.</summary>
public class UpdateInterestRateDto
{
    [Range(1, int.MaxValue)]
    public int InterestRateId { get; set; }

    [Required]
    [StringLength(100)]
    public string RateName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Savings|Current|FixedDeposit|RecurringDeposit|Loan)$", ErrorMessage = "ProductCategory must be one of: Savings, Current, FixedDeposit, RecurringDeposit, Loan.")]
    public string ProductCategory { get; set; } = string.Empty;

    [StringLength(20)]
    public string? LoanType { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AnnualRate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SeniorCitizenAdditionalRate { get; set; }

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Simple|Daily|Monthly|Quarterly|HalfYearly|Yearly)$", ErrorMessage = "CompoundingFrequency must be one of: Simple, Daily, Monthly, Quarterly, HalfYearly, Yearly.")]
    public string CompoundingFrequency { get; set; } = "Quarterly";

    [Range(0, double.MaxValue)]
    public decimal? MinAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxAmount { get; set; }

    [Range(0, int.MaxValue)]
    public int? MinTenureMonths { get; set; }

    [Range(0, int.MaxValue)]
    public int? MaxTenureMonths { get; set; }

    [StringLength(50)]
    public string? BenchmarkName { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SpreadOverBenchmark { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int? AccountTypeId { get; set; }
}
