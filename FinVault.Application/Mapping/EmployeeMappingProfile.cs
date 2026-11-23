using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.EmployeeDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Employee DTOs to and from the Employee domain entity.</summary>
public sealed class EmployeeMappingProfile : Profile
{
    public EmployeeMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateEmployeeDto, Employee>()
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.ManagedCustomers, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedKYCRecords, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedDocuments, opt => opt.Ignore())
            .ForMember(dest => dest.ManagedBranches, opt => opt.Ignore())
            .ForMember(dest => dest.DirectReports, opt => opt.Ignore())
            .ForMember(dest => dest.ApprovedLoans, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedDisputes, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewedFraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.FiledComplianceRecords, opt => opt.Ignore())
            .ForMember(dest => dest.EscalatedConversations, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateEmployeeDto, Employee>()
            .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.ManagedCustomers, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedKYCRecords, opt => opt.Ignore())
            .ForMember(dest => dest.VerifiedDocuments, opt => opt.Ignore())
            .ForMember(dest => dest.ManagedBranches, opt => opt.Ignore())
            .ForMember(dest => dest.DirectReports, opt => opt.Ignore())
            .ForMember(dest => dest.ApprovedLoans, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedDisputes, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewedFraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.FiledComplianceRecords, opt => opt.Ignore())
            .ForMember(dest => dest.EscalatedConversations, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Employee, EmployeeDto>();
    }
}
