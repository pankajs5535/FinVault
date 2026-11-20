using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.InterestRateDtos;

/// <summary>Returned to clients for Get All and Get By Id of InterestRate.</summary>
public class InterestRateDto
{
    public int InterestRateId { get; set; }

    public string RateName { get; set; } = string.Empty;

    public string ProductCategory { get; set; } = string.Empty;

    public string? LoanType { get; set; }

    public decimal AnnualRate { get; set; }

    public decimal? SeniorCitizenAdditionalRate { get; set; }

    public string CompoundingFrequency { get; set; } = string.Empty;

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public int? MinTenureMonths { get; set; }

    public int? MaxTenureMonths { get; set; }

    public string? BenchmarkName { get; set; }

    public decimal? SpreadOverBenchmark { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public int? AccountTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
