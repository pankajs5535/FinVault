using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.JournalEntryDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps JournalEntry DTOs to and from the JournalEntry domain entity.</summary>
public sealed class JournalEntryMappingProfile : Profile
{
    public JournalEntryMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateJournalEntryDto, JournalEntry>()
            .ForMember(dest => dest.JournalEntryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.ReversalEntries, opt => opt.Ignore())
            .ForMember(dest => dest.Lines, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateJournalEntryDto, JournalEntry>()
            .ForMember(dest => dest.JournalEntryId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.ReversalEntries, opt => opt.Ignore())
            .ForMember(dest => dest.Lines, opt => opt.Ignore())
            .ForMember(dest => dest.InterestCalculations, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<JournalEntry, JournalEntryDto>();
    }
}
