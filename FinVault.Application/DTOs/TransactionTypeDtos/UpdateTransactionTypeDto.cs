using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.TransactionTypeDtos;

/// <summary>Input for updating a TransactionType.</summary>
public class UpdateTransactionTypeDto
{
    [Range(1, int.MaxValue)]
    public int TransactionTypeId { get; set; }

    [Required]
    [StringLength(30)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^(Deposit|Withdrawal|Transfer|Payment|Fee|Interest|Refund|Reversal|Loan|Card)$", ErrorMessage = "Category must be one of: Deposit, Withdrawal, Transfer, Payment, Fee, Interest, Refund, Reversal, Loan, Card.")]
    public string Category { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Description { get; set; }

    public bool RequiresApproval { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? ApprovalThreshold { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? DailyLimit { get; set; }

    public bool IsReversible { get; set; } = true;

    public bool IsActive { get; set; } = true;
}
