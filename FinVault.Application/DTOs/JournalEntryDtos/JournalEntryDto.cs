using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.JournalEntryDtos;

/// <summary>Returned to clients for Get All and Get By Id of JournalEntry.</summary>
public class JournalEntryDto
{
    public int JournalEntryId { get; set; }

    public string JournalNumber { get; set; } = string.Empty;

    public string JournalType { get; set; } = string.Empty;

    public DateTime EntryDate { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public DateTime? PostedAt { get; set; }

    public int CurrencyId { get; set; }

    public int? TransactionId { get; set; }

    public int? PostedByUserId { get; set; }

    public int? ReversalOfJournalEntryId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
