using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Technical log of each call to or callback from a payment switch / gateway for a
PaymentRequest. Payloads must be masked before storage; no PAN, CVV, PIN or Aadhaar
values may be logged.

RELATIONSHIP
- Belongs to PaymentRequest through PaymentRequestId.
*/

public class PaymentGatewayLog
{
    // =========================
    // Primary Key
    // =========================

    public int PaymentGatewayLogId { get; set; }

    // =========================
    // Call Details
    // =========================

    public string GatewayName { get; set; } = null!;

    // Allowed values: Initiate | StatusCheck | Callback | Reversal | Refund
    public string Operation { get; set; } = null!;

    public string CorrelationId { get; set; } = null!;

    public int AttemptNumber { get; set; } = 1;

    // =========================
    // Payload (masked)
    // =========================

    // Sanitised request payload.
    public string? RequestPayload { get; set; }

    // Sanitised response payload.
    public string? ResponsePayload { get; set; }

    // =========================
    // Outcome
    // =========================

    public bool IsSuccess { get; set; }

    public int? HttpStatusCode { get; set; }

    public string? ResponseCode { get; set; }

    public string? ResponseMessage { get; set; }

    public int? DurationMs { get; set; }

    public string? ErrorMessage { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int PaymentRequestId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual PaymentRequest PaymentRequest { get; set; } = null!;
}
