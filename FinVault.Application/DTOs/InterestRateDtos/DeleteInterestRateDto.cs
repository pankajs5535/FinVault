using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.InterestRateDtos;

/// <summary>Input for deleting an InterestRate.</summary>
public class DeleteInterestRateDto
{
    [Range(1, int.MaxValue)]
    public int InterestRateId { get; set; }
}
