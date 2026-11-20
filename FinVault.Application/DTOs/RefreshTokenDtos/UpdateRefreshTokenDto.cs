using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RefreshTokenDtos;

/// <summary>Input for updating a RefreshToken.</summary>
public class UpdateRefreshTokenDto
{
    [Range(1, int.MaxValue)]
    public int RefreshTokenId { get; set; }

    [Required]
    [StringLength(64)]
    public string JwtId { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    [Required]
    [StringLength(45)]
    public string CreatedByIp { get; set; } = string.Empty;

    [StringLength(512)]
    public string? UserAgent { get; set; }

    [StringLength(100)]
    public string? DeviceName { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}
