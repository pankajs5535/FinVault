using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FraudAlertDtos;

/// <summary>Returned to clients for Get All and Get By Id of FraudAlert.</summary>
public class FraudAlertDto
{
    public int FraudAlertId { get; set; }

    public string AlertNumber { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public decimal? Score { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime RaisedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public string? ReviewNotes { get; set; }

    public string? ActionTaken { get; set; }

    public int FraudRuleId { get; set; }

    public int? TransactionId { get; set; }

    public int? CustomerId { get; set; }

    public int? AccountId { get; set; }

    public int? CardId { get; set; }

    public int? RiskAssessmentId { get; set; }

    public int? ReviewedByEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
