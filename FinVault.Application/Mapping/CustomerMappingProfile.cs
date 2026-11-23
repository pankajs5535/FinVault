using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.CustomerDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Customer DTOs to and from the Customer domain entity.</summary>
public sealed class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateCustomerDto, Customer>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.HomeBranch, opt => opt.Ignore())
            .ForMember(dest => dest.NationalityCountry, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.KYCDetailsHistory, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.AccountHolders, opt => opt.Ignore())
            .ForMember(dest => dest.Beneficiaries, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Cards, opt => opt.Ignore())
            .ForMember(dest => dest.Loans, opt => opt.Ignore())
            .ForMember(dest => dest.Disputes, opt => opt.Ignore())
            .ForMember(dest => dest.RiskAssessments, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore())
            .ForMember(dest => dest.AIConversations, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateCustomerDto, Customer>()
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.HomeBranch, opt => opt.Ignore())
            .ForMember(dest => dest.NationalityCountry, opt => opt.Ignore())
            .ForMember(dest => dest.Addresses, opt => opt.Ignore())
            .ForMember(dest => dest.KYCDetailsHistory, opt => opt.Ignore())
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.AccountHolders, opt => opt.Ignore())
            .ForMember(dest => dest.Beneficiaries, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Cards, opt => opt.Ignore())
            .ForMember(dest => dest.Loans, opt => opt.Ignore())
            .ForMember(dest => dest.Disputes, opt => opt.Ignore())
            .ForMember(dest => dest.RiskAssessments, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.Notifications, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore())
            .ForMember(dest => dest.AIConversations, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Customer, CustomerDto>();
    }
}
