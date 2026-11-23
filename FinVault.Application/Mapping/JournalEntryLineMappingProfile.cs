using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.JournalEntryLineDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps JournalEntryLine DTOs to and from the JournalEntryLine domain entity.</summary>
public sealed class JournalEntryLineMappingProfile : Profile
{
    public JournalEntryLineMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateJournalEntryLineDto, JournalEntryLine>()
            .ForMember(dest => dest.JournalEntryLineId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.JournalEntry, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateJournalEntryLineDto, JournalEntryLine>()
            .ForMember(dest => dest.JournalEntryLineId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.JournalEntry, opt => opt.Ignore())
            .ForMember(dest => dest.Account, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<JournalEntryLine, JournalEntryLineDto>();
    }
}
