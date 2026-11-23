using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.InterestCalculationDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps InterestCalculation DTOs to and from the InterestCalculation domain entity.</summary>
public sealed class InterestCalculationMappingProfile : Profile
{
    public InterestCalculationMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateInterestCalculationDto, InterestCalculation>()
            .ForMember(dest => dest.InterestCalculationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.InterestRate, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateInterestCalculationDto, InterestCalculation>()
            .ForMember(dest => dest.InterestCalculationId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.InterestRate, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<InterestCalculation, InterestCalculationDto>();
    }
}
