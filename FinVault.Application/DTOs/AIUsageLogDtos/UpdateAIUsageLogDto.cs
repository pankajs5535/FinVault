using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIUsageLogDtos;

/// <summary>Input for updating an AIUsageLog. AIUsageLog records are append-only, so only the key is accepted.</summary>
public class UpdateAIUsageLogDto
{
    [Range(1, int.MaxValue)]
    public int AIUsageLogId { get; set; }
}
