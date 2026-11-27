using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuthDtos;

public sealed class LoginRequestDto
{
    [Required, StringLength(256, MinimumLength = 3)]
    public string Identifier { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}
