using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.ReconciliationRecordDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps ReconciliationRecord DTOs to and from the ReconciliationRecord domain entity.</summary>
public sealed class ReconciliationRecordMappingProfile : Profile
{
    public ReconciliationRecordMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateReconciliationRecordDto, ReconciliationRecord>()
            .ForMember(dest => dest.ReconciliationRecordId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ReconciliationBatch, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateReconciliationRecordDto, ReconciliationRecord>()
            .ForMember(dest => dest.ReconciliationRecordId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ReconciliationBatch, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<ReconciliationRecord, ReconciliationRecordDto>();
    }
}
