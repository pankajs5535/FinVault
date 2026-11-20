using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanScheduleDtos;

/// <summary>Input for updating a LoanSchedule.</summary>
public class UpdateLoanScheduleDto
{
    [Range(1, int.MaxValue)]
    public int LoanScheduleId { get; set; }

    [Range(0, int.MaxValue)]
    public int InstallmentNumber { get; set; }

    public DateTime DueDate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal OpeningBalance { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ClosingBalance { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PrincipalDue { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InterestDue { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PenaltyAccrued { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PrincipalPaid { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InterestPaid { get; set; }

    [Range(0, double.MaxValue)]
    public decimal PenaltyPaid { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Pending|PartiallyPaid|Paid|Overdue|Waived)$", ErrorMessage = "Status must be one of: Pending, PartiallyPaid, Paid, Overdue, Waived.")]
    public string Status { get; set; } = "Pending";

    public DateTime? PaidOn { get; set; }

    [Range(1, int.MaxValue)]
    public int LoanId { get; set; }

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
