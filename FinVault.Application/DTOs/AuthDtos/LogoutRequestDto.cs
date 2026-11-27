using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuthDtos;

public sealed class LogoutRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
