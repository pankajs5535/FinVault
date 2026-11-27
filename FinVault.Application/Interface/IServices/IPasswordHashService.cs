using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services;

public interface IPasswordHashService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string password);
}

