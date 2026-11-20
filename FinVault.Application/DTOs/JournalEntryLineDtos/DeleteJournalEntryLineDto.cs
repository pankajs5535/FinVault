using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.JournalEntryLineDtos;

/// <summary>Input for deleting a JournalEntryLine.</summary>
public class DeleteJournalEntryLineDto
{
    [Range(1, int.MaxValue)]
    public int JournalEntryLineId { get; set; }
}
