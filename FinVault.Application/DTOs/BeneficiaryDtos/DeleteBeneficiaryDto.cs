using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.BeneficiaryDtos;

/// <summary>Input for deleting a Beneficiary.</summary>
public class DeleteBeneficiaryDto
{
    [Range(1, int.MaxValue)]
    public int BeneficiaryId { get; set; }
}
