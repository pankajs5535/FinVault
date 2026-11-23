using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.BeneficiaryDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Beneficiary DTOs to and from the Beneficiary domain entity.</summary>
public sealed class BeneficiaryMappingProfile : Profile
{
    public BeneficiaryMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateBeneficiaryDto, Beneficiary>()
            .ForMember(dest => dest.BeneficiaryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateBeneficiaryDto, Beneficiary>()
            .ForMember(dest => dest.BeneficiaryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Beneficiary, BeneficiaryDto>();
    }
}
