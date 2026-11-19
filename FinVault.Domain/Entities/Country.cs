using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
ISO 3166 country reference data (India is the primary value). Used for nationality and
address country.

RELATIONSHIP
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by Customer through NationalityCountryId.
- Referenced by Address through CountryId.
- Referenced by Branch through CountryId.
- Referenced by Currency through CountryId.
*/

public class Country
{
    // =========================
    // Primary Key
    // =========================

    public int CountryId { get; set; }

    // =========================
    // Country Information
    // =========================

    public string IsoAlpha2Code { get; set; } = null!;

    public string IsoAlpha3Code { get; set; } = null!;

    public string NumericCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? PhoneCode { get; set; }

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

    public virtual ICollection<Customer> Customers { get; set; }
        = new List<Customer>();

    public virtual ICollection<Address> Addresses { get; set; }
        = new List<Address>();

    public virtual ICollection<Branch> Branches { get; set; }
        = new List<Branch>();

    public virtual ICollection<Currency> Currencies { get; set; }
        = new List<Currency>();
}
