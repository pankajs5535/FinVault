using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.PaymentRequestDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Payment banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IPaymentService
{
        Task<PaymentRequestDto?> GetPaymentRequestAsync(int paymentRequestId);
        Task<IReadOnlyList<PaymentRequestDto>> GetPaymentRequestsAsync(int customerId);
        Task<PaymentRequestDto> InitiatePaymentAsync(CreatePaymentRequestDto dto);
        Task<PaymentRequestDto?> UpdatePaymentRequestAsync(int paymentRequestId, UpdatePaymentRequestDto dto);
        Task<bool> CancelPaymentAsync(int paymentRequestId);
        Task<bool> ProcessPaymentAsync(int paymentRequestId);
}
