using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIUsageLogDtos;

/// <summary>Input for deleting an AIUsageLog.</summary>
public class DeleteAIUsageLogDto
{
    [Range(1, int.MaxValue)]
    public int AIUsageLogId { get; set; }
}
