using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.DocumentDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Document DTOs to and from the Document domain entity.</summary>
public sealed class DocumentMappingProfile : Profile
{
    public DocumentMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateDocumentDto, Document>()
            .ForMember(dest => dest.DocumentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateDocumentDto, Document>()
            .ForMember(dest => dest.DocumentId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Document, DocumentDto>();
    }
}
