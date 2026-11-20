using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FraudAlertDtos;

/// <summary>Input for deleting a FraudAlert.</summary>
public class DeleteFraudAlertDto
{
    [Range(1, int.MaxValue)]
    public int FraudAlertId { get; set; }
}
