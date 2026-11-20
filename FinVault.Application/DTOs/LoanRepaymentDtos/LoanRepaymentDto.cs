using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanRepaymentDtos;

/// <summary>Returned to clients for Get All and Get By Id of LoanRepayment.</summary>
public class LoanRepaymentDto
{
    public int LoanRepaymentId { get; set; }

    public string RepaymentReference { get; set; } = string.Empty;

    public string RepaymentType { get; set; } = string.Empty;

    public string PaymentMode { get; set; } = string.Empty;

    public DateTime PaidAt { get; set; }

    public decimal AmountPaid { get; set; }

    public decimal PrincipalComponent { get; set; }

    public decimal InterestComponent { get; set; }

    public decimal PenaltyComponent { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? FailureReason { get; set; }

    public int LoanId { get; set; }

    public int? LoanScheduleId { get; set; }

    public int? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
