using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Payment rail definition: UPI, NEFT, RTGS, IMPS, NACH, cheque, card, with amount limits
and settlement behaviour.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by PaymentRequest through PaymentMethodId.
- Referenced by Fee through PaymentMethodId.
- Referenced by ReconciliationBatch through PaymentMethodId.
*/

public class PaymentMethod
{
    // =========================
    // Primary Key
    // =========================

    public int PaymentMethodId { get; set; }

    // =========================
    // Method Information
    // =========================

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    // Allowed values: UPI | NEFT | RTGS | IMPS | NACH | Cheque | Card | Internal
    public string Rail { get; set; } = null!;

    public string? Description { get; set; }

    // =========================
    // Limits and Settlement
    // =========================

    public decimal MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public decimal? DailyLimit { get; set; }

    public bool Is24x7 { get; set; }

    // Allowed values: RealTime | DeferredNet | Batch
    public string SettlementType { get; set; } = "RealTime";

    public bool IsActive { get; set; } = true;

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

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual ICollection<PaymentRequest> PaymentRequests { get; set; }
        = new List<PaymentRequest>();

    public virtual ICollection<Fee> Fees { get; set; }
        = new List<Fee>();

    public virtual ICollection<ReconciliationBatch> ReconciliationBatches { get; set; }
        = new List<ReconciliationBatch>();
}
