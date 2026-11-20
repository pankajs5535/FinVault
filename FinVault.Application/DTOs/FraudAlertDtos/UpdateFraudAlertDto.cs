using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FraudAlertDtos;

/// <summary>Input for updating a FraudAlert.</summary>
public class UpdateFraudAlertDto
{
    [Range(1, int.MaxValue)]
    public int FraudAlertId { get; set; }

    [Required]
    [StringLength(30)]
    public string AlertNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Low|Medium|High|Critical)$", ErrorMessage = "Severity must be one of: Low, Medium, High, Critical.")]
    public string Severity { get; set; } = "Medium";

    [Range(0, double.MaxValue)]
    public decimal? Score { get; set; }

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Open|UnderReview|ConfirmedFraud|FalsePositive|Closed)$", ErrorMessage = "Status must be one of: Open, UnderReview, ConfirmedFraud, FalsePositive, Closed.")]
    public string Status { get; set; } = "Open";

    public DateTime RaisedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    [StringLength(1000)]
    public string? ReviewNotes { get; set; }

    [StringLength(100)]
    public string? ActionTaken { get; set; }

    [Range(1, int.MaxValue)]
    public int FraudRuleId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int? CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? CardId { get; set; }

    [Range(1, int.MaxValue)]
    public int? RiskAssessmentId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ReviewedByEmployeeId { get; set; }
}
