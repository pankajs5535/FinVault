using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.LoanRepaymentDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps LoanRepayment DTOs to and from the LoanRepayment domain entity.</summary>
public sealed class LoanRepaymentMappingProfile : Profile
{
    public LoanRepaymentMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateLoanRepaymentDto, LoanRepayment>()
            .ForMember(dest => dest.LoanRepaymentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Loan, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateLoanRepaymentDto, LoanRepayment>()
            .ForMember(dest => dest.LoanRepaymentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Loan, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<LoanRepayment, LoanRepaymentDto>();
    }
}
