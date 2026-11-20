using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReportConfigurationDtos;

/// <summary>Returned to clients for Get All and Get By Id of ReportConfiguration.</summary>
public class ReportConfigurationDto
{
    public int ReportConfigurationId { get; set; }

    public string ReportCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ReportType { get; set; } = string.Empty;

    public string OutputFormat { get; set; } = string.Empty;

    public bool IsRegulatory { get; set; }

    public string? QueryDefinition { get; set; }

    public string? ParametersJson { get; set; }

    public bool IsScheduled { get; set; }

    public string? ScheduleCron { get; set; }

    public DateTime? LastRunAt { get; set; }

    public DateTime? NextRunAt { get; set; }

    public string? Recipients { get; set; }

    public int RetentionDays { get; set; }

    public bool IsActive { get; set; }

    public int? OwnerUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
