using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanRepaymentDtos;

/// <summary>Input for deleting a LoanRepayment.</summary>
public class DeleteLoanRepaymentDto
{
    [Range(1, int.MaxValue)]
    public int LoanRepaymentId { get; set; }
}
