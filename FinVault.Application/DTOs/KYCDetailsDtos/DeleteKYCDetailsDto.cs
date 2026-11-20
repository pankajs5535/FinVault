using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.KYCDetailsDtos;

/// <summary>Input for deleting a KYCDetails.</summary>
public class DeleteKYCDetailsDto
{
    [Range(1, int.MaxValue)]
    public int KYCDetailsId { get; set; }
}
