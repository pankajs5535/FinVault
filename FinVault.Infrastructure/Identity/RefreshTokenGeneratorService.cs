using System.Security.Cryptography;
using System.Text;
using FinVault.Application.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace FinVault.Infrastructure.Identity;

public sealed class RefreshTokenGeneratorService : IRefreshTokenGenerator
{
    private readonly JwtSettings _settings;

    public RefreshTokenGeneratorService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string HashToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }

    public DateTime GetExpiryUtc()
    {
        return DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
    }
}
