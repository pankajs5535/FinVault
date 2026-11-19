using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Debit, credit or prepaid card linked to an account and a customer. The full card number
is never stored - only a network/vault token and a masked value. CVV and PIN are never
stored by the application (PIN verification is delegated to an HSM).

RELATIONSHIP
- Belongs to Account through AccountId.
- Belongs to Customer through CustomerId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by CardTransaction through CardId.
- Referenced by FraudAlert through CardId.
*/

public class Card
{
    // =========================
    // Primary Key
    // =========================

    public int CardId { get; set; }

    // =========================
    // Card Identification
    // =========================

    // Vault/network token that represents the PAN.
    public string CardNumberToken { get; set; } = null!;

    // Display value, e.g. 4XXX XXXX XXXX 1234.
    public string CardNumberMasked { get; set; } = null!;

    public string CardHolderName { get; set; } = null!;

    // Allowed values: Debit | Credit | Prepaid
    public string CardType { get; set; } = "Debit";

    // Allowed values: RuPay | Visa | Mastercard
    public string Network { get; set; } = "RuPay";

    public DateTime ExpiryDate { get; set; }

    // =========================
    // Status
    // =========================

    // Allowed values: Pending | Active | Blocked | Hotlisted | Expired | Cancelled
    public string Status { get; set; } = "Pending";

    public DateTime? IssuedAt { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public DateTime? BlockedAt { get; set; }

    public string? BlockReason { get; set; }

    // =========================
    // Controls
    // =========================

    public bool IsContactlessEnabled { get; set; } = true;

    public bool IsOnlineEnabled { get; set; } = true;

    public bool IsAtmEnabled { get; set; } = true;

    public bool IsPosEnabled { get; set; } = true;

    public decimal DailyAtmLimit { get; set; }

    public decimal DailyPosLimit { get; set; }

    public decimal DailyOnlineLimit { get; set; }

    // =========================
    // PIN Security (no PIN stored)
    // =========================

    public DateTime? PinLastSetAt { get; set; }

    public int PinRetryCount { get; set; } = 0;

    public DateTime? PinBlockedUntil { get; set; }

    // =========================
    // Credit Card Terms
    // =========================

    public decimal? CreditLimit { get; set; }

    public decimal? AvailableCredit { get; set; }

    public decimal? OutstandingAmount { get; set; }

    public int? BillingCycleDay { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int AccountId { get; set; }

    public int CustomerId { get; set; }

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

    public virtual Account Account { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<CardTransaction> CardTransactions { get; set; }
        = new List<CardTransaction>();

    public virtual ICollection<FraudAlert> FraudAlerts { get; set; }
        = new List<FraudAlert>();
}
