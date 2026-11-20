using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.JournalEntryLineDtos;

/// <summary>Input for creating a JournalEntryLine.</summary>
public class CreateJournalEntryLineDto
{
    [Range(0, int.MaxValue)]
    public int LineNumber { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DebitAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CreditAmount { get; set; }

    [StringLength(250)]
    public string? Narration { get; set; }

    [Range(1, int.MaxValue)]
    public int JournalEntryId { get; set; }

    [Range(1, int.MaxValue)]
    public int AccountId { get; set; }
}
