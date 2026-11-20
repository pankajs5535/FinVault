using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.ComplianceRecordDtos;

/// <summary>Input for deleting a ComplianceRecord.</summary>
public class DeleteComplianceRecordDto
{
    [Range(1, int.MaxValue)]
    public int ComplianceRecordId { get; set; }
}
