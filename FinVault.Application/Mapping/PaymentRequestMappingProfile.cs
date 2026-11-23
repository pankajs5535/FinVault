using AutoMapper;
using FinVault.Domain.Entities;
using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Mapping;

/// <summary>Maps PaymentRequest DTOs to and from the PaymentRequest domain entity.</summary>
public sealed class PaymentRequestMappingProfile : Profile
{
    public PaymentRequestMappingProfile()
    {
        // Create: database keys, audit fields, concurrency tokens, and navigation graphs are managed by the application.
        CreateMap<CreatePaymentRequestDto, PaymentRequest>()
            .ForMember(dest => dest.PaymentRequestId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.SourceAccount, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.GatewayLogs, opt => opt.Ignore())
            .ForMember(dest => dest.RequestReference, opt => opt.Ignore())
            .ForMember(dest => dest.UtrNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Channel, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.FailureCode, opt => opt.Ignore())
            .ForMember(dest => dest.FailureReason, opt => opt.Ignore())
            .ForMember(dest => dest.RequestedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorizedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CurrencyId, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionId, opt => opt.Ignore());

        // Update: do not overwrite the entity key, audit metadata, concurrency token, or navigation graph.
        CreateMap<UpdatePaymentRequestDto, PaymentRequest>()
            .ForMember(dest => dest.PaymentRequestId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RowVersion, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.SourceAccount, opt => opt.Ignore())
            .ForMember(dest => dest.Currency, opt => opt.Ignore())
            .ForMember(dest => dest.GatewayLogs, opt => opt.Ignore())
            .ForMember(dest => dest.RequestReference, opt => opt.Ignore())
            .ForMember(dest => dest.UtrNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Channel, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.FailureCode, opt => opt.Ignore())
            .ForMember(dest => dest.FailureReason, opt => opt.Ignore())
            .ForMember(dest => dest.RequestedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AuthorizedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CompletedAt, opt => opt.Ignore())
            .ForMember(dest => dest.ExpiresAt, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
            .ForMember(dest => dest.CurrencyId, opt => opt.Ignore())
            .ForMember(dest => dest.TransactionId, opt => opt.Ignore());

        // Response: map matching entity and DTO properties by name.
        CreateMap<PaymentRequest, PaymentRequestDto>();
    }
}
