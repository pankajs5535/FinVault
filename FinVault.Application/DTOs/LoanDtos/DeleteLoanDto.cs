using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanDtos;

/// <summary>Input for deleting a Loan.</summary>
public class DeleteLoanDto
{
    [Range(1, int.MaxValue)]
    public int LoanId { get; set; }
}
