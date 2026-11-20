using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIConversationDtos;

/// <summary>Input for updating an AIConversation.</summary>
public class UpdateAIConversationDto
{
    [Range(1, int.MaxValue)]
    public int AIConversationId { get; set; }

    [Required]
    [StringLength(40)]
    public string ConversationReference { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Title { get; set; }

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Web|Mobile|Voice)$", ErrorMessage = "Channel must be one of: Web, Mobile, Voice.")]
    public string Channel { get; set; } = "Web";

    [Required]
    [StringLength(10)]
    public string Language { get; set; } = "en";

    [StringLength(1000)]
    public string? Summary { get; set; }

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Active|Closed|Archived|Escalated)$", ErrorMessage = "Status must be one of: Active, Closed, Archived, Escalated.")]
    public string Status { get; set; } = "Active";

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int? CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? EscalatedToEmployeeId { get; set; }
}
