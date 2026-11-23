using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.LoanDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Loan DTOs to and from the Loan domain entity.</summary>
public sealed class LoanMappingProfile : Profile
{
    public LoanMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateLoanDto, Loan>()
            .ForMember(dest => dest.LoanId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.LoanSchedules, opt => opt.Ignore())
            .ForMember(dest => dest.LoanRepayments, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateLoanDto, Loan>()
            .ForMember(dest => dest.LoanId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.LoanSchedules, opt => opt.Ignore())
            .ForMember(dest => dest.LoanRepayments, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Loan, LoanDto>();
    }
}
