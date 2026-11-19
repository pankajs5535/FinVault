using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Card authorisation/settlement record from the card network (POS, ATM, e-commerce,
contactless). Linked to the posted account Transaction once settled.

RELATIONSHIP
- Belongs to Card through CardId.
- Optionally belongs to Transaction through TransactionId.
- Belongs to Currency through CurrencyId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Dispute through CardTransactionId.
*/

public class CardTransaction
{
    // =========================
    // Primary Key
    // =========================

    public int CardTransactionId { get; set; }

    // =========================
    // Authorisation
    // =========================

    public string? AuthorizationCode { get; set; }

    // RRN assigned by the card network.
    public string? RetrievalReferenceNumber { get; set; }

    // Allowed values: Authorized | Declined | Reversed | Settled | Expired
    public string AuthorizationStatus { get; set; } = "Authorized";

    public string? DeclineReason { get; set; }

    public DateTime AuthorizedAt { get; set; }

    // =========================
    // Merchant
    // =========================

    public string MerchantName { get; set; } = null!;

    public string MerchantCategoryCode { get; set; } = null!;

    public string? MerchantCity { get; set; }

    public string? TerminalId { get; set; }

    // Allowed values: POS | ATM | Ecommerce | Contactless | Recurring
    public string Channel { get; set; } = "POS";

    // =========================
    // Amounts
    // =========================

    public decimal Amount { get; set; }

    public decimal? SettlementAmount { get; set; }

    public DateTime? SettledAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int CardId { get; set; }

    public int? TransactionId { get; set; }

    public int CurrencyId { get; set; }

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

    public virtual Card Card { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<Dispute> Disputes { get; set; }
        = new List<Dispute>();
}
