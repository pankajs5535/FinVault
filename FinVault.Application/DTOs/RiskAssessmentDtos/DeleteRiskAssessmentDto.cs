using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RiskAssessmentDtos;

/// <summary>Input for deleting a RiskAssessment.</summary>
public class DeleteRiskAssessmentDto
{
    [Range(1, int.MaxValue)]
    public int RiskAssessmentId { get; set; }
}
