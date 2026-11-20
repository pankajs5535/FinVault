using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RiskAssessmentDtos;

/// <summary>Returned to clients for Get All and Get By Id of RiskAssessment.</summary>
public class RiskAssessmentDto
{
    public int RiskAssessmentId { get; set; }

    public string AssessmentType { get; set; } = string.Empty;

    public decimal RiskScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string Decision { get; set; } = string.Empty;

    public string ModelVersion { get; set; } = string.Empty;

    public string? FactorsJson { get; set; }

    public DateTime AssessedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public int? CustomerId { get; set; }

    public int? AccountId { get; set; }

    public int? TransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
