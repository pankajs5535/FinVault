using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.AIConversationDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps AIConversation DTOs to and from the AIConversation domain entity.</summary>
public sealed class AIConversationMappingProfile : Profile
{
    public AIConversationMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateAIConversationDto, AIConversation>()
            .ForMember(dest => dest.AIConversationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AIChatMessages, opt => opt.Ignore())
            .ForMember(dest => dest.AIUsageLogs, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateAIConversationDto, AIConversation>()
            .ForMember(dest => dest.AIConversationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.AIChatMessages, opt => opt.Ignore())
            .ForMember(dest => dest.AIUsageLogs, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<AIConversation, AIConversationDto>();
    }
}
