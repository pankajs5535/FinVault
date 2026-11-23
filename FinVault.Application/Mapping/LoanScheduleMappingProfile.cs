using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.LoanScheduleDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps LoanSchedule DTOs to and from the LoanSchedule domain entity.</summary>
public sealed class LoanScheduleMappingProfile : Profile
{
    public LoanScheduleMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateLoanScheduleDto, LoanSchedule>()
            .ForMember(dest => dest.LoanScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Loan, opt => opt.Ignore())
            .ForMember(dest => dest.LoanRepayments, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateLoanScheduleDto, LoanSchedule>()
            .ForMember(dest => dest.LoanScheduleId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Loan, opt => opt.Ignore())
            .ForMember(dest => dest.LoanRepayments, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<LoanSchedule, LoanScheduleDto>();
    }
}
