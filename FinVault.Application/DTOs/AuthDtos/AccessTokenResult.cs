namespace FinVault.Application.DTOs.AuthDtos;

public sealed class AccessTokenResult
{
    public string AccessToken { get; init; } = string.Empty;

    public string JwtId { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }
}
