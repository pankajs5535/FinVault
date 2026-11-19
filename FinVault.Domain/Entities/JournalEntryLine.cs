using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Single debit or credit leg of a JournalEntry against one Account. Each line carries
either a debit or a credit amount, never both. Lines are append-only.

RELATIONSHIP
- Belongs to JournalEntry through JournalEntryId.
- Belongs to Account through AccountId.
- CreatedBy optionally reference the User who performed the change.
*/

public class JournalEntryLine
{
    // =========================
    // Primary Key
    // =========================

    public int JournalEntryLineId { get; set; }

    // =========================
    // Line Details
    // =========================

    public int LineNumber { get; set; }

    public decimal DebitAmount { get; set; }

    public decimal CreditAmount { get; set; }

    public string? Narration { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int JournalEntryId { get; set; }

    public int AccountId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual JournalEntry JournalEntry { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }
}
