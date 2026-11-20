using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ComplianceRecordDtos;

/// <summary>Returned to clients for Get All and Get By Id of ComplianceRecord.</summary>
public class ComplianceRecordDto
{
    public int ComplianceRecordId { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;

    public string RecordType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string RiskLevel { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string ReportingAuthority { get; set; } = string.Empty;

    public string? FilingReference { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? FiledAt { get; set; }

    public int? CustomerId { get; set; }

    public int? AccountId { get; set; }

    public int? TransactionId { get; set; }

    public int? FraudAlertId { get; set; }

    public int? FiledByEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
