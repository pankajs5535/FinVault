using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.JournalEntryDtos;

/// <summary>Input for updating a JournalEntry.</summary>
public class UpdateJournalEntryDto
{
    [Range(1, int.MaxValue)]
    public int JournalEntryId { get; set; }

    [Required]
    [StringLength(30)]
    public string JournalNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Transaction|Interest|Fee|Loan|Adjustment|Reversal)$", ErrorMessage = "JournalType must be one of: Transaction, Interest, Fee, Loan, Adjustment, Reversal.")]
    public string JournalType { get; set; } = "Transaction";

    public DateTime EntryDate { get; set; }

    [Required]
    [StringLength(250)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Draft|Posted|Reversed)$", ErrorMessage = "Status must be one of: Draft, Posted, Reversed.")]
    public string Status { get; set; } = "Draft";

    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int? PostedByUserId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ReversalOfJournalEntryId { get; set; }

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
