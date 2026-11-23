using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.ComplianceRecordDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps ComplianceRecord DTOs to and from the ComplianceRecord domain entity.</summary>
public sealed class ComplianceRecordMappingProfile : Profile
{
    public ComplianceRecordMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateComplianceRecordDto, ComplianceRecord>()
            .ForMember(dest => dest.ComplianceRecordId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateComplianceRecordDto, ComplianceRecord>()
            .ForMember(dest => dest.ComplianceRecordId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<ComplianceRecord, ComplianceRecordDto>();
    }
}
