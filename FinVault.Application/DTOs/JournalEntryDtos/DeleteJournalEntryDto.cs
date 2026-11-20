using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.JournalEntryDtos;

/// <summary>Input for deleting a JournalEntry.</summary>
public class DeleteJournalEntryDto
{
    [Range(1, int.MaxValue)]
    public int JournalEntryId { get; set; }
}
