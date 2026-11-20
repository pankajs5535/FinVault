using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.InterestCalculationDtos;

/// <summary>Input for deleting an InterestCalculation.</summary>
public class DeleteInterestCalculationDto
{
    [Range(1, int.MaxValue)]
    public int InterestCalculationId { get; set; }
}
