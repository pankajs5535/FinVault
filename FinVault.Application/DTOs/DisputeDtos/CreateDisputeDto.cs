using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.DisputeDtos;

/// <summary>Input for creating a Dispute.</summary>
public class CreateDisputeDto
{
    [Required]
    [StringLength(30)]
    public string DisputeNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    [RegularExpression(@"^(UnauthorizedTransaction|DuplicateCharge|WrongAmount|ServiceNotReceived|AtmCashNotDispensed|FailedTransactionDebited|Other)$", ErrorMessage = "DisputeType must be one of: UnauthorizedTransaction, DuplicateCharge, WrongAmount, ServiceNotReceived, AtmCashNotDispensed, FailedTransactionDebited, Other.")]
    public string DisputeType { get; set; } = "Other";

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal DisputedAmount { get; set; }

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^(Open|UnderInvestigation|ProvisionalCreditGiven|Resolved|Rejected|Escalated|Closed)$", ErrorMessage = "Status must be one of: Open, UnderInvestigation, ProvisionalCreditGiven, Resolved, Rejected, Escalated, Closed.")]
    public string Status { get; set; } = "Open";

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Low|Medium|High|Critical)$", ErrorMessage = "Priority must be one of: Low, Medium, High, Critical.")]
    public string Priority { get; set; } = "Medium";

    public DateTime RaisedAt { get; set; }

    public DateTime? DueBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    [StringLength(30)]
    [RegularExpression(@"^(InFavourOfCustomer|InFavourOfBank|Withdrawn)$", ErrorMessage = "Resolution must be one of: InFavourOfCustomer, InFavourOfBank, Withdrawn.")]
    public string? Resolution { get; set; }

    [StringLength(1000)]
    public string? ResolutionNotes { get; set; }

    [Range(1, int.MaxValue)]
    public int TransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int? CardTransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AssignedToEmployeeId { get; set; }
}
