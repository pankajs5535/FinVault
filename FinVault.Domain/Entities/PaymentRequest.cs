using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Customer-initiated payment instruction sent over a payment rail. Tracks authorisation,
submission to the network and final outcome. When completed it points to the resulting
Transaction.

RELATIONSHIP
- Belongs to PaymentMethod through PaymentMethodId.
- Belongs to Customer through CustomerId.
- Belongs to Account through SourceAccountId.
- Optionally belongs to Beneficiary through BeneficiaryId.
- Belongs to Currency through CurrencyId.
- Optionally belongs to Transaction through TransactionId (one-to-one).
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by PaymentGatewayLog through PaymentRequestId.
*/

public class PaymentRequest
{
    // =========================
    // Primary Key
    // =========================

    public int PaymentRequestId { get; set; }

    // =========================
    // Reference
    // =========================

    public string RequestReference { get; set; } = null!;

    public string IdempotencyKey { get; set; } = null!;

    // UTR/RRN returned by the payment network.
    public string? UtrNumber { get; set; }

    // =========================
    // Payee (when no saved Beneficiary)
    // =========================

    public string? PayeeName { get; set; }

    public string? PayeeAccountNumber { get; set; }

    public string? PayeeIfsc { get; set; }

    public string? PayeeVpa { get; set; }

    // =========================
    // Payment Details
    // =========================

    public decimal Amount { get; set; }

    public string? Purpose { get; set; }

    public string? Remarks { get; set; }

    // Allowed values: Branch | NetBanking | MobileApp | UPI | Api
    public string Channel { get; set; } = "MobileApp";

    // =========================
    // Lifecycle
    // =========================

    // Allowed values: Created | Authorized | Submitted | Processing | Completed | Failed | Rejected | Expired | Cancelled
    public string Status { get; set; } = "Created";

    public string? FailureCode { get; set; }

    public string? FailureReason { get; set; }

    public DateTime RequestedAt { get; set; }

    public DateTime? AuthorizedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? ScheduledFor { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int PaymentMethodId { get; set; }

    public int CustomerId { get; set; }

    public int SourceAccountId { get; set; }

    public int? BeneficiaryId { get; set; }

    public int CurrencyId { get; set; }

    public int? TransactionId { get; set; }

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

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Account SourceAccount { get; set; } = null!;

    public virtual Beneficiary? Beneficiary { get; set; }

    public virtual Currency Currency { get; set; } = null!;

    public virtual Transaction? Transaction { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<PaymentGatewayLog> GatewayLogs { get; set; }
        = new List<PaymentGatewayLog>();
}
