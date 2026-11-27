using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinVault.Application.DTOs.AuthDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FinVault.Infrastructure.Identity;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly SigningCredentials _signingCredentials;

    public JwtTokenService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public AccessTokenResult CreateAccessToken(User user, Role role)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_settings.AccessTokenExpirationMinutes);
        var jwtId = Guid.NewGuid().ToString("N");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, jwtId),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, role.Name),
            new("user_type", user.UserType),
            new("security_stamp", user.SecurityStamp)
        };

        foreach (var permission in role.Permissions.Where(x => x.IsActive).Select(x => x.Code).Distinct(StringComparer.Ordinal))
            claims.Add(new Claim("permission", permission));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: _signingCredentials);

        return new AccessTokenResult
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            JwtId = jwtId,
            ExpiresAtUtc = expires
        };
    }
}
