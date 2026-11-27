namespace FinVault.Application.Interfaces.Services;

public interface ICurrentUserService
{
    bool IsAuthenticated { get; }

    int? UserId { get; }

    string? Username { get; }

    string? Email { get; }

    string? Role { get; }

    IReadOnlyCollection<string> Permissions { get; }
}

