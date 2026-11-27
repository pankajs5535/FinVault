namespace FinVault.Application.Interfaces.Services;

public interface IAuthPolicy
{
    int MaxFailedAccessAttempts { get; }

    int LockoutMinutes { get; }
}

