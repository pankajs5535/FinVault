using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIConversationDtos;

/// <summary>Returned to clients for Get All and Get By Id of AIConversation.</summary>
public class AIConversationDto
{
    public int AIConversationId { get; set; }

    public string ConversationReference { get; set; } = string.Empty;

    public string? Title { get; set; }

    public string Channel { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; }

    public DateTime? LastMessageAt { get; set; }

    public DateTime? EndedAt { get; set; }

    public int MessageCount { get; set; }

    public int UserId { get; set; }

    public int? CustomerId { get; set; }

    public int? EscalatedToEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
