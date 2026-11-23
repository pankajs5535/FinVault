using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.KYCDetailsDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps KYCDetails DTOs to and from the KYCDetails domain entity.</summary>
public sealed class KYCDetailsMappingProfile : Profile
{
    public KYCDetailsMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateKYCDetailsDto, KYCDetails>()
            .ForMember(dest => dest.KYCDetailsId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateKYCDetailsDto, KYCDetails>()
            .ForMember(dest => dest.KYCDetailsId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<KYCDetails, KYCDetailsDto>();
    }
}
