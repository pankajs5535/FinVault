using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AuthDtos;

public sealed class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required, StringLength(128, MinimumLength = 12)]
    public string NewPassword { get; set; } = string.Empty;
}
