using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AIChatMessageDtos;

/// <summary>Input for deleting an AIChatMessage.</summary>
public class DeleteAIChatMessageDto
{
    [Range(1, int.MaxValue)]
    public int AIChatMessageId { get; set; }
}
