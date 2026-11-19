using System;
using System.Collections.Generic;

namespace FinVault.Domain.Entities;

/*
BUSINESS PURPOSE
Server-side record of an issued JWT refresh token. Only a hash of the token is stored so
that a database leak cannot be replayed.
Supports expiry, revocation and rotation (a token replaced by a newer one).

RELATIONSHIP
- Belongs to User through UserId.
- Optionally belongs to RefreshToken through ReplacedByTokenId.
- Referenced by RefreshToken through ReplacedByTokenId.
*/

public class RefreshToken
{
    // =========================
    // Primary Key
    // =========================

    public int RefreshTokenId { get; set; }

    // =========================
    // Token
    // =========================

    // SHA-256 hash of the opaque refresh token. The raw token is never stored.
    public string TokenHash { get; set; } = null!;

    // The jti claim of the access token this refresh token was issued with.
    public string JwtId { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    // =========================
    // Revocation
    // =========================

    public DateTime? RevokedAt { get; set; }

    public string? RevocationReason { get; set; }

    public string? RevokedByIp { get; set; }

    // =========================
    // Client Information
    // =========================

    public string CreatedByIp { get; set; } = null!;

    public string? UserAgent { get; set; }

    public string? DeviceName { get; set; }

    // =========================
    // Foreign Keys
    // =========================

    public int UserId { get; set; }

    public int? ReplacedByTokenId { get; set; }

    // =========================
    // Audit
    // =========================

    public DateTime CreatedAt { get; set; }

    // =========================
    // Navigation Properties
    // =========================

    public virtual User User { get; set; } = null!;

    public virtual RefreshToken? ReplacedByToken { get; set; }

    public virtual ICollection<RefreshToken> ReplacedTokens { get; set; }
        = new List<RefreshToken>();
}
