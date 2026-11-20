using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIConversationDtos;

/// <summary>Input for deleting an AIConversation.</summary>
public class DeleteAIConversationDto
{
    [Range(1, int.MaxValue)]
    public int AIConversationId { get; set; }
}
