using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.CountryDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Country DTOs to and from the Country domain entity.</summary>
public sealed class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateCountryDto, Country>()
            .ForMember(dest => dest.CountryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customers, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.Branches, opt => opt.Ignore())
            .ForMember(dest => dest.Currencies, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateCountryDto, Country>()
            .ForMember(dest => dest.CountryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customers, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.Branches, opt => opt.Ignore())
            .ForMember(dest => dest.Currencies, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Country, CountryDto>();

        // Usage counts require the related collections to be loaded; use database projection for large reports.
        CreateMap<Country, CountryUsageSummaryDto>()
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TotalCustomers, opt => opt.MapFrom(src => src.Customers.Count))
            .ForMember(dest => dest.TotalBranches, opt => opt.MapFrom(src => src.Branches.Count))
            .ForMember(dest => dest.TotalCurrencies, opt => opt.MapFrom(src => src.Currencies.Count));
    }
}
