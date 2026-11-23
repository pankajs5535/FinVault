using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.PaymentGatewayLogDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps PaymentGatewayLog DTOs to and from the PaymentGatewayLog domain entity.</summary>
public sealed class PaymentGatewayLogMappingProfile : Profile
{
    public PaymentGatewayLogMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreatePaymentGatewayLogDto, PaymentGatewayLog>()
            .ForMember(dest => dest.PaymentGatewayLogId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequest, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdatePaymentGatewayLogDto, PaymentGatewayLog>()
            .ForMember(dest => dest.PaymentGatewayLogId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentRequest, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<PaymentGatewayLog, PaymentGatewayLogDto>();
    }
}
