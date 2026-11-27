using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuthDtos;

public sealed class RefreshTokenRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
