using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.CardDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Card DTOs to and from the Card domain entity.</summary>
public sealed class CardMappingProfile : Profile
{
    public CardMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateCardDto, Card>()
            .ForMember(dest => dest.CardId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.CardTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateCardDto, Card>()
            .ForMember(dest => dest.CardId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.CardTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Card, CardDto>();
    }
}
