using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.RefundDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps Refund DTOs to and from the Refund domain entity.</summary>
public sealed class RefundMappingProfile : Profile
{
    public RefundMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreateRefundDto, Refund>()
            .ForMember(dest => dest.RefundId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.OriginalTransaction, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdateRefundDto, Refund>()
            .ForMember(dest => dest.RefundId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.OriginalTransaction, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<Refund, RefundDto>();
    }
}
