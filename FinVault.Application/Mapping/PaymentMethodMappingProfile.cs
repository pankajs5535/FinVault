using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.PaymentMethodDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps PaymentMethod DTOs to and from the PaymentMethod domain entity.</summary>
public sealed class PaymentMethodMappingProfile : Profile
{
    public PaymentMethodMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreatePaymentMethodDto, PaymentMethod>()
            .ForMember(dest => dest.PaymentMethodId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Fees, opt => opt.Ignore())
            .ForMember(dest => dest.ReconciliationBatches, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdatePaymentMethodDto, PaymentMethod>()
            .ForMember(dest => dest.PaymentMethodId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequests, opt => opt.Ignore())
            .ForMember(dest => dest.Fees, opt => opt.Ignore())
            .ForMember(dest => dest.ReconciliationBatches, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<PaymentMethod, PaymentMethodDto>();
    }
}
