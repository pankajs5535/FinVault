using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.JournalEntryLineDtos;

/// <summary>Returned to clients for Get All and Get By Id of JournalEntryLine.</summary>
public class JournalEntryLineDto
{
    public int JournalEntryLineId { get; set; }

    public int LineNumber { get; set; }

    public decimal DebitAmount { get; set; }

    public decimal CreditAmount { get; set; }

    public string? Narration { get; set; }

    public int JournalEntryId { get; set; }

    public int AccountId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }
}
