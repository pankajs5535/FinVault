using System.Text;
using FinVault.Application.Interfaces.Services;
using Microsoft.Extensions.Options;
using FinVault.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace FinVault.Infrastructure.Identity;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddFinVaultIdentity(this IServiceCollection services,IConfiguration configuration)
    {
        var section = configuration.GetSection(JwtSettings.SectionName);

        services.AddOptions<JwtSettings>()
            .Bind(section)
            .ValidateDataAnnotations()
            .Validate(settings => settings.SecretKey is not null &&
                                  Encoding.UTF8.GetByteCount(settings.SecretKey) >= 32,
                "JWT secret key must contain at least 32 UTF-8 bytes.")
            .ValidateOnStart();

        services.AddHttpContextAccessor();
        services.AddSingleton<IAuthPolicy>(serviceProvider =>
            serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value);
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<IRefreshTokenGenerator, RefreshTokenGeneratorService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = section["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = section["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(section["SecretKey"] ?? string.Empty)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(
                        int.TryParse(section["ClockSkewSeconds"], out var skew) ? skew : 30),
                    NameClaimType = System.Security.Claims.ClaimTypes.Name,
                    RoleClaimType = System.Security.Claims.ClaimTypes.Role
                };
            });

        services.AddAuthorization();
        return services;
    }
}
