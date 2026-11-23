using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.AIUsageLogDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps AIUsageLog DTOs to and from the AIUsageLog domain entity.</summary>
public sealed class AIUsageLogMappingProfile : Profile
{
    public AIUsageLogMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateAIUsageLogDto, AIUsageLog>()
            .ForMember(dest => dest.AIUsageLogId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateAIUsageLogDto, AIUsageLog>()
            .ForMember(dest => dest.AIUsageLogId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<AIUsageLog, AIUsageLogDto>();
    }
}
