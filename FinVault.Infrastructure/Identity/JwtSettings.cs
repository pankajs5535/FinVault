using System.ComponentModel.DataAnnotations;
using System.Text;
using FinVault.Application.Interfaces.Services;

namespace FinVault.Infrastructure.Identity;

public sealed class JwtSettings : IValidatableObject, IAuthPolicy
{
    public const string SectionName = "JwtSettings";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Range(1, 60)]
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    [Range(1, 90)]
    public int RefreshTokenExpirationDays { get; set; } = 7;

    [Range(0, 60)]
    public int ClockSkewSeconds { get; set; } = 30;

    [Range(3, 20)]
    public int MaxFailedAccessAttempts { get; set; } = 5;

    [Range(1, 1440)]
    public int LockoutMinutes { get; set; } = 15;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Issuer))
            yield return new ValidationResult("JWT issuer is required.", [nameof(Issuer)]);

        if (string.IsNullOrWhiteSpace(Audience))
            yield return new ValidationResult("JWT audience is required.", [nameof(Audience)]);

        if (Encoding.UTF8.GetByteCount(SecretKey) < 32)
            yield return new ValidationResult("JWT secret key must contain at least 32 UTF-8 bytes.", [nameof(SecretKey)]);
    }
}
