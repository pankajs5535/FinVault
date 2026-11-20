using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.InterestCalculationDtos;

/// <summary>Input for creating an InterestCalculation.</summary>
public class CreateInterestCalculationDto
{
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Accrual|Payout|Capitalisation)$", ErrorMessage = "CalculationType must be one of: Accrual, Payout, Capitalisation.")]
    public string CalculationType { get; set; } = "Accrual";

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    [Range(0, int.MaxValue)]
    public int DayCount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PrincipalBase { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AppliedRate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InterestAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TdsAmount { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Calculated|Posted|Reversed)$", ErrorMessage = "Status must be one of: Calculated, Posted, Reversed.")]
    public string Status { get; set; } = "Calculated";

    public DateTime CalculatedAt { get; set; }

    [Range(1, int.MaxValue)]
    public int? AccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? LoanId { get; set; }

    [Range(1, int.MaxValue)]
    public int InterestRateId { get; set; }

    [Range(1, int.MaxValue)]
    public int? JournalEntryId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }
}
