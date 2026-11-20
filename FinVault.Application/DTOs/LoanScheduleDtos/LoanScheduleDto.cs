using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanScheduleDtos;

/// <summary>Returned to clients for Get All and Get By Id of LoanSchedule.</summary>
public class LoanScheduleDto
{
    public int LoanScheduleId { get; set; }

    public int InstallmentNumber { get; set; }

    public DateTime DueDate { get; set; }

    public decimal OpeningBalance { get; set; }

    public decimal ClosingBalance { get; set; }

    public decimal PrincipalDue { get; set; }

    public decimal InterestDue { get; set; }

    public decimal PenaltyAccrued { get; set; }

    public decimal PrincipalPaid { get; set; }

    public decimal InterestPaid { get; set; }

    public decimal PenaltyPaid { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? PaidOn { get; set; }

    public int LoanId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
