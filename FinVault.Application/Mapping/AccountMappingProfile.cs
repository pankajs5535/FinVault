using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.AccountDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Account DTOs to and from the Account domain entity.</summary>
public sealed class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateAccountDto, Account>()
            .ForMember(dest => dest.AccountId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.AccountType, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedDepositAccounts, opt => opt.Ignore())
            .ForMember(dest => dest.AccountHolders, opt => opt.Ignore())
            .ForMember(dest => dest.BeneficiaryReferences, opt => opt.Ignore())
            .ForMember(dest => dest.OutgoingTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.IncomingTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.JournalEntryLines, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Cards, opt => opt.Ignore())
            .ForMember(dest => dest.DisbursementLoans, opt => opt.Ignore())
            .ForMember(dest => dest.RepaymentLoans, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore())
            .ForMember(dest => dest.RiskAssessments, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateAccountDto, Account>()
            .ForMember(dest => dest.AccountId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.AccountType, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedDepositAccounts, opt => opt.Ignore())
            .ForMember(dest => dest.AccountHolders, opt => opt.Ignore())
            .ForMember(dest => dest.BeneficiaryReferences, opt => opt.Ignore())
            .ForMember(dest => dest.OutgoingTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.IncomingTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.JournalEntryLines, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Cards, opt => opt.Ignore())
            .ForMember(dest => dest.DisbursementLoans, opt => opt.Ignore())
            .ForMember(dest => dest.RepaymentLoans, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore())
            .ForMember(dest => dest.RiskAssessments, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Account, AccountDto>();
    }
}
