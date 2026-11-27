namespace FinVault.Application.DTOs.AuthDtos;

public sealed class AuthResponseDto
{
    public string TokenType { get; init; } = "Bearer";

    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public DateTime AccessTokenExpiresAtUtc { get; init; }

    public DateTime RefreshTokenExpiresAtUtc { get; init; }

    public int UserId { get; init; }

    public string Username { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;
}
