using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanRepaymentDtos;

/// <summary>Input for creating a LoanRepayment.</summary>
public class CreateLoanRepaymentDto
{
    [Required]
    [StringLength(40)]
    public string RepaymentReference { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Emi|PartPayment|ForeClosure|Penalty)$", ErrorMessage = "RepaymentType must be one of: Emi, PartPayment, ForeClosure, Penalty.")]
    public string RepaymentType { get; set; } = "Emi";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(AutoDebit|Nach|Online|Branch)$", ErrorMessage = "PaymentMode must be one of: AutoDebit, Nach, Online, Branch.")]
    public string PaymentMode { get; set; } = "AutoDebit";

    public DateTime PaidAt { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AmountPaid { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PrincipalComponent { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InterestComponent { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PenaltyComponent { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Pending|Successful|Failed|Reversed)$", ErrorMessage = "Status must be one of: Pending, Successful, Failed, Reversed.")]
    public string Status { get; set; } = "Pending";

    [StringLength(250)]
    public string? FailureReason { get; set; }

    [Range(1, int.MaxValue)]
    public int LoanId { get; set; }

    [Range(1, int.MaxValue)]
    public int? LoanScheduleId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }
}
