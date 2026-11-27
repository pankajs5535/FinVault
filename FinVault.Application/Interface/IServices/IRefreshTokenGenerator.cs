namespace FinVault.Application.Interfaces.Services;

public interface IRefreshTokenGenerator
{
    string GenerateToken();

    string HashToken(string token);

    DateTime GetExpiryUtc();
}

