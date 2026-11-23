using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.CurrencyDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Currency DTOs to and from the Currency domain entity.</summary>
public sealed class CurrencyMappingProfile : Profile
{
    public CurrencyMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateCurrencyDto, Currency>()
            .ForMember(dest => dest.CurrencyId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Accounts, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateCurrencyDto, Currency>()
            .ForMember(dest => dest.CurrencyId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Accounts, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Currency, CurrencyDto>();
    }
}
