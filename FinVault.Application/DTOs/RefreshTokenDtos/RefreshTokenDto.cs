using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RefreshTokenDtos;

/// <summary>Returned to clients for Get All and Get By Id of RefreshToken.</summary>
public class RefreshTokenDto
{
    public int RefreshTokenId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevocationReason { get; set; }

    public string? UserAgent { get; set; }

    public string? DeviceName { get; set; }

    public int UserId { get; set; }

    public int? ReplacedByTokenId { get; set; }

    public DateTime CreatedAt { get; set; }
}
