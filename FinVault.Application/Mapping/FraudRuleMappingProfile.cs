using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.FraudRuleDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps FraudRule DTOs to and from the FraudRule domain entity.</summary>
public sealed class FraudRuleMappingProfile : Profile
{
    public FraudRuleMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateFraudRuleDto, FraudRule>()
            .ForMember(dest => dest.FraudRuleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateFraudRuleDto, FraudRule>()
            .ForMember(dest => dest.FraudRuleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<FraudRule, FraudRuleDto>();
    }
}
