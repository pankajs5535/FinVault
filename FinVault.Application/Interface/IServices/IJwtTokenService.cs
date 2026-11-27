using FinVault.Application.DTOs.AuthDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services;

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(User user, Role role);
}

