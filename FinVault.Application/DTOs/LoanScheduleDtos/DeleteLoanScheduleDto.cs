using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanScheduleDtos;

/// <summary>Input for deleting a LoanSchedule.</summary>
public class DeleteLoanScheduleDto
{
    [Range(1, int.MaxValue)]
    public int LoanScheduleId { get; set; }
}
