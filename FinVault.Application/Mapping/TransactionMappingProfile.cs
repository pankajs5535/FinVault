using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.TransactionDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Transaction DTOs to and from the Transaction domain entity.</summary>
public sealed class TransactionMappingProfile : Profile
{
    public TransactionMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateTransactionDto, Transaction>()
            .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionType, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.ReversalTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.JournalEntries, opt => opt.Ignore())
            .ForMember(dest => dest.CardTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.LoanRepayments, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore())
            .ForMember(dest => dest.Refunds, opt => opt.Ignore())
            .ForMember(dest => dest.Disputes, opt => opt.Ignore())
            .ForMember(dest => dest.ReconciliationRecords, opt => opt.Ignore())
            .ForMember(dest => dest.RiskAssessments, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateTransactionDto, Transaction>()
            .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionType, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.ReversalTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.JournalEntries, opt => opt.Ignore())
            .ForMember(dest => dest.CardTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.LoanRepayments, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore())
            .ForMember(dest => dest.Refunds, opt => opt.Ignore())
            .ForMember(dest => dest.Disputes, opt => opt.Ignore())
            .ForMember(dest => dest.ReconciliationRecords, opt => opt.Ignore())
            .ForMember(dest => dest.RiskAssessments, opt => opt.Ignore())
            .ForMember(dest => dest.FraudAlerts, opt => opt.Ignore())
            .ForMember(dest => dest.ComplianceRecords, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Transaction, TransactionDto>();
    }
}
