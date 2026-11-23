using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.BranchDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Branch DTOs to and from the Branch domain entity.</summary>
public sealed class BranchMappingProfile : Profile
{
    public BranchMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateBranchDto, Branch>()
            .ForMember(dest => dest.BranchId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Country, opt => opt.Ignore())
            .ForMember(dest => dest.Customers, opt => opt.Ignore())
            .ForMember(dest => dest.ChildBranches, opt => opt.Ignore())
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.Accounts, opt => opt.Ignore())
            .ForMember(dest => dest.Loans, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateBranchDto, Branch>()
            .ForMember(dest => dest.BranchId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Country, opt => opt.Ignore())
            .ForMember(dest => dest.Customers, opt => opt.Ignore())
            .ForMember(dest => dest.ChildBranches, opt => opt.Ignore())
            .ForMember(dest => dest.Employees, opt => opt.Ignore())
            .ForMember(dest => dest.Accounts, opt => opt.Ignore())
            .ForMember(dest => dest.Loans, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Branch, BranchDto>();
    }
}
