using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.RiskAssessmentDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps RiskAssessment DTOs to and from the RiskAssessment domain entity.</summary>
public sealed class RiskAssessmentMappingProfile : Profile
{
    public RiskAssessmentMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateRiskAssessmentDto, RiskAssessment>()
            .ForMember(dest => dest.RiskAssessmentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateRiskAssessmentDto, RiskAssessment>()
            .ForMember(dest => dest.RiskAssessmentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<RiskAssessment, RiskAssessmentDto>();
    }
}
