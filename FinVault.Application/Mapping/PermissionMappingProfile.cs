using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.PermissionDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Permission DTOs to and from the Permission domain entity.</summary>
public sealed class PermissionMappingProfile : Profile
{
    public PermissionMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreatePermissionDto, Permission>()
            .ForMember(dest => dest.PermissionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdatePermissionDto, Permission>()
            .ForMember(dest => dest.PermissionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Permission, PermissionDto>();
    }
}
