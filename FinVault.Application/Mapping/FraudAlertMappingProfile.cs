using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.FraudAlertDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps FraudAlert DTOs to and from the FraudAlert domain entity.</summary>
public sealed class FraudAlertMappingProfile : Profile
{
    public FraudAlertMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateFraudAlertDto, FraudAlert>()
            .ForMember(dest => dest.FraudAlertId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FraudRule, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateFraudAlertDto, FraudAlert>()
            .ForMember(dest => dest.FraudAlertId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.FraudRule, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<FraudAlert, FraudAlertDto>();
    }
}
