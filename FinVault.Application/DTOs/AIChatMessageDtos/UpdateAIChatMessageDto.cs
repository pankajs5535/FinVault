using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIChatMessageDtos;

/// <summary>Input for updating an AIChatMessage.</summary>
public class UpdateAIChatMessageDto
{
    [Range(1, int.MaxValue)]
    public int AIChatMessageId { get; set; }

    [Range(0, int.MaxValue)]
    public int SequenceNumber { get; set; }

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(System|User|Assistant|Tool)$", ErrorMessage = "SenderRole must be one of: System, User, Assistant, Tool.")]
    public string SenderRole { get; set; } = "User";

    [Required]
    public string Content { get; set; } = string.Empty;

    [StringLength(100)]
    public string? ModelName { get; set; }

    [Range(0, int.MaxValue)]
    public int? InputTokens { get; set; }

    [Range(0, int.MaxValue)]
    public int? OutputTokens { get; set; }

    [Range(0, int.MaxValue)]
    public int? LatencyMs { get; set; }

    [StringLength(30)]
    public string? FinishReason { get; set; }

    public bool IsFlagged { get; set; }

    [StringLength(250)]
    public string? FlagReason { get; set; }

    [Range(1, int.MaxValue)]
    public int AIConversationId { get; set; }
}
