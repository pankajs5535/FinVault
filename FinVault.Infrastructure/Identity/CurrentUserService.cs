using System.Security.Claims;
using FinVault.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace FinVault.Infrastructure.Identity;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    public int? UserId
    {
        get
        {
            var value = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? Principal?.FindFirst("sub")?.Value;
            return int.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? Username => Principal?.FindFirst(ClaimTypes.Name)?.Value;

    public string? Email => Principal?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Role => Principal?.FindFirst(ClaimTypes.Role)?.Value;

    public IReadOnlyCollection<string> Permissions =>
        Principal?.FindAll("permission").Select(x => x.Value).Distinct(StringComparer.Ordinal).ToArray()
        ?? Array.Empty<string>();
}
