using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Postal address of a customer (residential, permanent, correspondence, office or
registered) using Indian address conventions (state, district, 6-digit PIN code).

RELATIONSHIP
- Belongs to Customer through CustomerId.
- Belongs to Country through CountryId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class Address
{
    // =========================
    // Primary Key
    // =========================

    public int AddressId { get; set; }

    // =========================
    // Address Details
    // =========================

    // Allowed values: Residential | Permanent | Correspondence | Office | Registered
    public string AddressType { get; set; } = "Residential";

    public string AddressLine1 { get; set; } = null!;

    public string? AddressLine2 { get; set; }

    public string? Landmark { get; set; }

    public string City { get; set; } = null!;

    public string? District { get; set; }

    public string State { get; set; } = null!;

    public string PinCode { get; set; } = null!;

    // =========================
    // Verification
    // =========================

    public bool IsPrimary { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerifiedAt { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int CustomerId { get; set; }

    public int CountryId { get; set; }

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

    public virtual Customer Customer { get; set; } = null!;

    public virtual Country Country { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
