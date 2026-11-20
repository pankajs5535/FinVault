using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanDtos;

/// <summary>Returned to clients for Get All and Get By Id of Loan.</summary>
public class LoanDto
{
    public int LoanId { get; set; }

    public string LoanNumber { get; set; } = string.Empty;

    public string LoanType { get; set; } = string.Empty;

    public string? Purpose { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal RequestedAmount { get; set; }

    public decimal? SanctionedAmount { get; set; }

    public decimal DisbursedAmount { get; set; }

    public decimal OutstandingPrincipal { get; set; }

    public decimal OutstandingInterest { get; set; }

    public decimal? EmiAmount { get; set; }

    public decimal AnnualInterestRate { get; set; }

    public string InterestType { get; set; } = string.Empty;

    public int TenureMonths { get; set; }

    public DateTime ApplicationDate { get; set; }

    public DateTime? SanctionDate { get; set; }

    public DateTime? DisbursementDate { get; set; }

    public DateTime? FirstEmiDate { get; set; }

    public DateTime? MaturityDate { get; set; }

    public DateTime? ClosedOn { get; set; }

    public int DaysPastDue { get; set; }

    public string AssetClassification { get; set; } = string.Empty;

    public int CustomerId { get; set; }

    public int BranchId { get; set; }

    public int? DisbursementAccountId { get; set; }

    public int? RepaymentAccountId { get; set; }

    public int? InterestRateId { get; set; }

    public int? ApprovedByEmployeeId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
