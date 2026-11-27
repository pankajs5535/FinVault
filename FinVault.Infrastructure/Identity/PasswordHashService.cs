using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FinVault.Infrastructure.Identity;

public sealed class PasswordHashService : IPasswordHashService
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordHashService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash) || string.IsNullOrEmpty(password))
            return false;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
