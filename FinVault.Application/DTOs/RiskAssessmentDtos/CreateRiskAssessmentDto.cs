using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RiskAssessmentDtos;

/// <summary>Input for creating a RiskAssessment.</summary>
public class CreateRiskAssessmentDto
{
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Customer|Account|Transaction)$", ErrorMessage = "AssessmentType must be one of: Customer, Account, Transaction.")]
    public string AssessmentType { get; set; } = "Transaction";

    [Range(0, double.MaxValue)]
    public decimal RiskScore { get; set; }

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Low|Medium|High|Critical)$", ErrorMessage = "RiskLevel must be one of: Low, Medium, High, Critical.")]
    public string RiskLevel { get; set; } = "Low";

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Approve|Review|Challenge|Decline)$", ErrorMessage = "Decision must be one of: Approve, Review, Challenge, Decline.")]
    public string Decision { get; set; } = "Approve";

    [Required]
    [StringLength(30)]
    public string ModelVersion { get; set; } = string.Empty;

    public string? FactorsJson { get; set; }

    public DateTime AssessedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    [Range(1, int.MaxValue)]
    public int? CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }
}
