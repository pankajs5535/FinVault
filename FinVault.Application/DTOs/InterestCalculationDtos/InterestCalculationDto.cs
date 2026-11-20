using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.InterestCalculationDtos;

/// <summary>Returned to clients for Get All and Get By Id of InterestCalculation.</summary>
public class InterestCalculationDto
{
    public int InterestCalculationId { get; set; }

    public string CalculationType { get; set; } = string.Empty;

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public int DayCount { get; set; }

    public decimal PrincipalBase { get; set; }

    public decimal AppliedRate { get; set; }

    public decimal InterestAmount { get; set; }

    public decimal TdsAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CalculatedAt { get; set; }

    public int? AccountId { get; set; }

    public int? LoanId { get; set; }

    public int InterestRateId { get; set; }

    public int? JournalEntryId { get; set; }

    public int? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
