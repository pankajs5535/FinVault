using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ComplianceRecordDtos;

/// <summary>Input for updating a ComplianceRecord.</summary>
public class UpdateComplianceRecordDto
{
    [Range(1, int.MaxValue)]
    public int ComplianceRecordId { get; set; }

    [Required]
    [StringLength(40)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^(Str|Ctr|Ccr|Ntr|AmlScreening|PepCheck|SanctionsScreening|PeriodicKycReview|FatcaCrs)$", ErrorMessage = "RecordType must be one of: Str, Ctr, Ccr, Ntr, AmlScreening, PepCheck, SanctionsScreening, PeriodicKycReview, FatcaCrs.")]
    public string RecordType { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Low|Medium|High|Critical)$", ErrorMessage = "RiskLevel must be one of: Low, Medium, High, Critical.")]
    public string RiskLevel { get; set; } = "Low";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Open|UnderReview|Filed|Cleared|Closed)$", ErrorMessage = "Status must be one of: Open, UnderReview, Filed, Cleared, Closed.")]
    public string Status { get; set; } = "Open";

    [Required]
    [StringLength(50)]
    public string ReportingAuthority { get; set; } = "FIU-IND";

    [StringLength(50)]
    public string? FilingReference { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? FiledAt { get; set; }

    [Range(1, int.MaxValue)]
    public int? CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int? FraudAlertId { get; set; }

    [Range(1, int.MaxValue)]
    public int? FiledByEmployeeId { get; set; }
}
