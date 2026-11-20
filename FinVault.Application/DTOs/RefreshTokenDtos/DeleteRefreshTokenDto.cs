using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.RefreshTokenDtos;

/// <summary>Input for deleting a RefreshToken.</summary>
public class DeleteRefreshTokenDto
{
    [Range(1, int.MaxValue)]
    public int RefreshTokenId { get; set; }
}
