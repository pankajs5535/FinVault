using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Definition of a statement, regulatory, operational or analytics report: output format,
parameters, schedule and recipients.

RELATIONSHIP
- Optionally belongs to User through OwnerUserId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
*/

public class ReportConfiguration
{
    // =========================
    // Primary Key
    // =========================

    public int ReportConfigurationId { get; set; }

    // =========================
    // Report Information
    // =========================

    public string ReportCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    // Allowed values: Statement | Regulatory | Operational | Financial | Analytics
    public string ReportType { get; set; } = "Operational";

    // Allowed values: Pdf | Excel | Csv | Json
    public string OutputFormat { get; set; } = "Pdf";

    public bool IsRegulatory { get; set; }

    // =========================
    // Definition
    // =========================

    // Approved query or stored-procedure name; never free-form user SQL.
    public string? QueryDefinition { get; set; }

    public string? ParametersJson { get; set; }

    // =========================
    // Schedule
    // =========================

    public bool IsScheduled { get; set; }

    public string? ScheduleCron { get; set; }

    public DateTime? LastRunAt { get; set; }

    public DateTime? NextRunAt { get; set; }

    public string? Recipients { get; set; }

    public int RetentionDays { get; set; } = 365;

    // =========================
    // Status
    // =========================

    public bool IsActive { get; set; } = true;

    // =========================
    // Foreign Keys
    // =========================

    public int? OwnerUserId { get; set; }

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

    public virtual User? OwnerUser { get; set; }

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }
}
