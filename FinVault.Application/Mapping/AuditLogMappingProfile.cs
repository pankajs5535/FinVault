using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.AuditLogDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps AuditLog DTOs to and from the AuditLog domain entity.</summary>
public sealed class AuditLogMappingProfile : Profile
{
    public AuditLogMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateAuditLogDto, AuditLog>()
            .ForMember(dest => dest.AuditLogId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateAuditLogDto, AuditLog>()
            .ForMember(dest => dest.AuditLogId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<AuditLog, AuditLogDto>();
    }
}
