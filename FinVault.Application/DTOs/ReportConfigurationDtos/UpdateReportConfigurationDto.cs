using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReportConfigurationDtos;

/// <summary>Input for updating a ReportConfiguration.</summary>
public class UpdateReportConfigurationDto
{
    [Range(1, int.MaxValue)]
    public int ReportConfigurationId { get; set; }

    [Required]
    [StringLength(50)]
    public string ReportCode { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Statement|Regulatory|Operational|Financial|Analytics)$", ErrorMessage = "ReportType must be one of: Statement, Regulatory, Operational, Financial, Analytics.")]
    public string ReportType { get; set; } = "Operational";

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Pdf|Excel|Csv|Json)$", ErrorMessage = "OutputFormat must be one of: Pdf, Excel, Csv, Json.")]
    public string OutputFormat { get; set; } = "Pdf";

    public bool IsRegulatory { get; set; }

    public string? QueryDefinition { get; set; }

    public string? ParametersJson { get; set; }

    public bool IsScheduled { get; set; }

    [StringLength(50)]
    public string? ScheduleCron { get; set; }

    public DateTime? LastRunAt { get; set; }

    public DateTime? NextRunAt { get; set; }

    [StringLength(1000)]
    public string? Recipients { get; set; }

    [Range(0, int.MaxValue)]
    public int RetentionDays { get; set; } = 365;

    public bool IsActive { get; set; } = true;

    [Range(1, int.MaxValue)]
    public int? OwnerUserId { get; set; }
}
