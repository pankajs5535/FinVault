using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIChatMessageDtos;

/// <summary>Returned to clients for Get All and Get By Id of AIChatMessage.</summary>
public class AIChatMessageDto
{
    public int AIChatMessageId { get; set; }

    public int SequenceNumber { get; set; }

    public string SenderRole { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string? ModelName { get; set; }

    public int? InputTokens { get; set; }

    public int? OutputTokens { get; set; }

    public int? LatencyMs { get; set; }

    public string? FinishReason { get; set; }

    public bool IsFlagged { get; set; }

    public string? FlagReason { get; set; }

    public int AIConversationId { get; set; }

    public DateTime CreatedAt { get; set; }
}
