using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Authentication identity for every person or system that signs in to FinVault (customers,
bank employees, administrators and system jobs).
Only password hashes, never plain-text passwords, are stored. Lockout, verification and
multi-factor data support security policies.

RELATIONSHIP
- Belongs to Role through RoleId.
- CreatedBy / UpdatedBy optionally reference the User who performed the change.
- Referenced by RefreshToken through UserId.
- Referenced by Customer through UserId.
- Referenced by Employee through UserId.
- Referenced by Transaction through InitiatedByUserId.
- Referenced by Notification through UserId.
- Referenced by AuditLog through UserId.
- Referenced by ReportConfiguration through OwnerUserId.
- Referenced by AIConversation through UserId.
- Referenced by AIUsageLog through UserId.
- Customer and Employee reference a User through a one-to-one relationship; those
  reverse navigations are exposed here.
- All other entities reference User only through the CreatedBy / UpdatedBy audit
  columns (no inverse collection).
*/

public class User
{
    // =========================
    // Primary Key
    // =========================

    public int UserId { get; set; }

    // =========================
    // Login Credentials
    // =========================

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    // Salted Argon2id/bcrypt hash only. Plain-text passwords are never stored.
    public string PasswordHash { get; set; } = null!;

    public DateTime? PasswordChangedAt { get; set; }

    // Random value rotated on credential or role change so that issued tokens can be
    // invalidated.
    public string SecurityStamp { get; set; } = null!;

    // =========================
    // Account Status
    // =========================

    // Allowed values: Customer | Employee | Admin | System
    public string UserType { get; set; } = "Customer";

    public bool IsActive { get; set; } = true;

    public bool IsEmailVerified { get; set; }

    public bool IsPhoneVerified { get; set; }

    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? LockoutEndAt { get; set; }

    // =========================
    // Multi-Factor Authentication
    // =========================

    public bool IsTwoFactorEnabled { get; set; }

    // Encrypted TOTP secret. Never stored in plain text.
    public string? TwoFactorSecretEncrypted { get; set; }

    // =========================
    // Login Tracking
    // =========================

    public DateTime? LastLoginAt { get; set; }

    public string? LastLoginIp { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int RoleId { get; set; }

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

    public virtual Role Role { get; set; } = null!;

    public virtual User? CreatedByUser { get; set; }

    public virtual User? UpdatedByUser { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        = new List<RefreshToken>();

    public virtual ICollection<Transaction> InitiatedTransactions { get; set; }
        = new List<Transaction>();

    public virtual ICollection<Notification> Notifications { get; set; }
        = new List<Notification>();

    public virtual ICollection<AuditLog> AuditLogs { get; set; }
        = new List<AuditLog>();

    public virtual ICollection<ReportConfiguration> OwnedReportConfigurations { get; set; }
        = new List<ReportConfiguration>();

    public virtual ICollection<AIConversation> AIConversations { get; set; }
        = new List<AIConversation>();

    public virtual ICollection<AIUsageLog> AIUsageLogs { get; set; }
        = new List<AIUsageLog>();
}
