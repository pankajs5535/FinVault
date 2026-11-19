using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Double-entry journal header. A Posted entry must have TotalDebit = TotalCredit (enforced
by a database check constraint and by the posting service which derives the totals from
the lines). Posted entries are immutable; corrections are made with a reversing entry.

RELATIONSHIP
- Belongs to Currency through CurrencyId.
- Optionally belongs to Transaction through TransactionId.
- Optionally belongs to User through PostedByUserId.
- Optionally belongs to JournalEntry through ReversalOfJournalEntryId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by JournalEntry through ReversalOfJournalEntryId.
- Referenced by JournalEntryLine through JournalEntryId.
- Referenced by InterestCalculation through JournalEntryId.
*/

public class JournalEntry
{
    // =========================
    // Primary Key
    // =========================

    public int JournalEntryId { get; set; }

    // =========================
    // Journal Information
    // =========================

    public string JournalNumber { get; set; } = null!;

    // Allowed values: Transaction | Interest | Fee | Loan | Adjustment | Reversal
    public string JournalType { get; set; } = "Transaction";

    public DateTime EntryDate { get; set; }

    public string Description { get; set; } = null!;

    // Allowed values: Draft | Posted | Reversed
    public string Status { get; set; } = "Draft";

    // =========================
    // Totals
    // =========================

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    // =========================
    // Posting
    // =========================

    public DateTime? PostedAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int CurrencyId { get; set; }

    public int? TransactionId { get; set; }

    public int? PostedByUserId { get; set; }

    public int? ReversalOfJournalEntryId { get; set; }

    // =========================
    // Concurrency
    // =========================

    public byte[] RowVersion { get; set; } = null!;

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual Currency Currency { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }

    public virtual User? PostedByUser { get; set; }

    public virtual JournalEntry? ReversalOfJournalEntry { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<JournalEntry> ReversalEntries { get; set; }
        = new List<JournalEntry>();

    public virtual ICollection<JournalEntryLine> Lines { get; set; }
        = new List<JournalEntryLine>();

    public virtual ICollection<InterestCalculation> InterestCalculations { get; set; }
        = new List<InterestCalculation>();
}
