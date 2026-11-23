using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.FeeDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Fee DTOs to and from the Fee domain entity.</summary>
public sealed class FeeMappingProfile : Profile
{
    public FeeMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateFeeDto, Fee>()
            .ForMember(dest => dest.FeeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ChargedTransactions, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateFeeDto, Fee>()
            .ForMember(dest => dest.FeeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ChargedTransactions, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Fee, FeeDto>();
    }
}
