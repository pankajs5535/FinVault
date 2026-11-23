using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.AIChatMessageDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps AIChatMessage DTOs to and from the AIChatMessage domain entity.</summary>
public sealed class AIChatMessageMappingProfile : Profile
{
    public AIChatMessageMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateAIChatMessageDto, AIChatMessage>()
            .ForMember(dest => dest.AIChatMessageId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AIConversation, opt => opt.Ignore())
            .ForMember(dest => dest.AIUsageLogs, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateAIChatMessageDto, AIChatMessage>()
            .ForMember(dest => dest.AIChatMessageId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AIConversation, opt => opt.Ignore())
            .ForMember(dest => dest.AIUsageLogs, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<AIChatMessage, AIChatMessageDto>();
    }
}
