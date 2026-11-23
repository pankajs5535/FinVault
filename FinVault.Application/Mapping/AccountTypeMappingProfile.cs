using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.AccountTypeDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps AccountType DTOs to and from the AccountType domain entity.</summary>
public sealed class AccountTypeMappingProfile : Profile
{
    public AccountTypeMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateAccountTypeDto, AccountType>()
            .ForMember(dest => dest.AccountTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Accounts, opt => opt.Ignore())
            .ForMember(dest => dest.InterestRates, opt => opt.Ignore())
            .ForMember(dest => dest.Fees, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateAccountTypeDto, AccountType>()
            .ForMember(dest => dest.AccountTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Accounts, opt => opt.Ignore())
            .ForMember(dest => dest.InterestRates, opt => opt.Ignore())
            .ForMember(dest => dest.Fees, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<AccountType, AccountTypeDto>();
    }
}
