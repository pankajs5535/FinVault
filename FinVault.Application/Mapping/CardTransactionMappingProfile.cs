using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.CardTransactionDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps CardTransaction DTOs to and from the CardTransaction domain entity.</summary>
public sealed class CardTransactionMappingProfile : Profile
{
    public CardTransactionMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateCardTransactionDto, CardTransaction>()
            .ForMember(dest => dest.CardTransactionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Card, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.Disputes, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateCardTransactionDto, CardTransaction>()
            .ForMember(dest => dest.CardTransactionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Card, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.Disputes, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<CardTransaction, CardTransactionDto>();
    }
}
