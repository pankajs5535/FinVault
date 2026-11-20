using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ReportConfigurationDtos;

/// <summary>Input for deleting a ReportConfiguration.</summary>
public class DeleteReportConfigurationDto
{
    [Range(1, int.MaxValue)]
    public int ReportConfigurationId { get; set; }
}
