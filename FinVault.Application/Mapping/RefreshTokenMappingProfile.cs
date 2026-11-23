using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.RefreshTokenDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps RefreshToken DTOs to and from the RefreshToken domain entity.</summary>
public sealed class RefreshTokenMappingProfile : Profile
{
    public RefreshTokenMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateRefreshTokenDto, RefreshToken>()
            .ForMember(dest => dest.RefreshTokenId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ReplacedTokens, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateRefreshTokenDto, RefreshToken>()
            .ForMember(dest => dest.RefreshTokenId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.ReplacedTokens, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<RefreshToken, RefreshTokenDto>();
    }
}
