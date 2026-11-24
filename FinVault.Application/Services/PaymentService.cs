using AutoMapper;
using FinVault.Application.DTOs.PaymentRequestDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Manages payment instructions; actual bank-rail execution belongs to a payment-provider integration.</summary>
public sealed class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves one payment request with its related records.
    public async Task<PaymentRequestDto?> GetPaymentRequestAsync(int paymentRequestId)
    {
        if (paymentRequestId <= 0) throw new ArgumentOutOfRangeException(nameof(paymentRequestId));
        var request = await _unitOfWork.PaymentRequest.GetPaymentRequestDetailsAsync(paymentRequestId);
        return request is null ? null : _mapper.Map<PaymentRequestDto>(request);
    }

    // 2. Retrieves the payment history belonging to a customer.
    public async Task<IReadOnlyList<PaymentRequestDto>> GetPaymentRequestsAsync(int customerId)
    {
        if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));
        var requests = await _unitOfWork.PaymentRequest.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IReadOnlyList<PaymentRequestDto>>(requests);
    }

    // 3. Creates a payment instruction after checking amount, source-account ownership and duplicate submission key.
    public async Task<PaymentRequestDto> InitiatePaymentAsync(CreatePaymentRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.Amount <= 0) throw new ArgumentException("Payment amount must be greater than zero.");
        if (string.IsNullOrWhiteSpace(dto.IdempotencyKey)) throw new ArgumentException("An idempotency key is required.");
        if (await _unitOfWork.PaymentRequest.ExistsByIdempotencyKeyAsync(dto.IdempotencyKey.Trim()))
            throw new InvalidOperationException("A payment request with this idempotency key already exists.");

        var account = await _unitOfWork.Account.GetAccountDetailsAsync(dto.SourceAccountId);
        if (account is null || account.Status != "Active") throw new InvalidOperationException("The source account does not exist or is not active.");
        var customerAccounts = await _unitOfWork.Account.GetAccountsByCustomerIdAsync(dto.CustomerId);
        if (!customerAccounts.Any(x => x.AccountId == dto.SourceAccountId)) throw new InvalidOperationException("The source account does not belong to this customer.");
        if (account.CurrencyId != dto.CurrencyId) throw new InvalidOperationException("Payment currency must match the source account currency.");
        if (account.AvailableBalance < dto.Amount) throw new InvalidOperationException("Insufficient available balance.");

        var request = _mapper.Map<PaymentRequest>(dto);
        request.IdempotencyKey = dto.IdempotencyKey.Trim();
        request.RequestReference = string.IsNullOrWhiteSpace(dto.RequestReference) ? $"PAY-{Guid.NewGuid():N}" : dto.RequestReference.Trim();
        request.Status = "Created";
        request.RequestedAt = DateTime.UtcNow;
        request.CreatedAt = DateTime.UtcNow;
        request.UtrNumber = null;
        request.TransactionId = null;
        request.FailureCode = null;
        request.FailureReason = null;
        request.AuthorizedAt = null;
        request.CompletedAt = null;
        await _unitOfWork.PaymentRequest.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PaymentRequestDto>(request);
    }

    // 4. Updates customer-editable payee and scheduling information only while the request is still editable.
    public async Task<PaymentRequestDto?> UpdatePaymentRequestAsync(int paymentRequestId, UpdatePaymentRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (paymentRequestId <= 0 || dto.PaymentRequestId != paymentRequestId) throw new ArgumentException("Route payment ID and DTO payment ID must match.");
        var request = await _unitOfWork.PaymentRequest.GetByIdAsync(paymentRequestId);
        if (request is null) return null;
        if (request.Status is not ("Created" or "Pending")) throw new InvalidOperationException("Only a newly created or pending payment can be edited.");
        if (dto.Amount <= 0) throw new ArgumentException("Payment amount must be greater than zero.");
        if (dto.SourceAccountId != request.SourceAccountId || dto.CustomerId != request.CustomerId || dto.CurrencyId != request.CurrencyId)
            throw new InvalidOperationException("Customer, source account and currency cannot be changed after initiation.");

        request.PayeeName = dto.PayeeName;
        request.PayeeAccountNumber = dto.PayeeAccountNumber;
        request.PayeeIfsc = dto.PayeeIfsc;
        request.PayeeVpa = dto.PayeeVpa;
        request.Amount = dto.Amount;
        request.Purpose = dto.Purpose;
        request.Remarks = dto.Remarks;
        request.ScheduledFor = dto.ScheduledFor;
        request.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.PaymentRequest.Update(request);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PaymentRequestDto>(request);
    }

    // 5. Cancels a payment instruction that has not been submitted for processing.
    public async Task<bool> CancelPaymentAsync(int paymentRequestId)
    {
        if (paymentRequestId <= 0) throw new ArgumentOutOfRangeException(nameof(paymentRequestId));
        var request = await _unitOfWork.PaymentRequest.GetByIdAsync(paymentRequestId);
        if (request is null) return false;
        if (request.Status is "Completed" or "Failed" or "Cancelled" or "Processing" or "Submitted")
            throw new InvalidOperationException("This payment can no longer be cancelled through this operation.");
        request.Status = "Cancelled";
        request.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.PaymentRequest.Update(request);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 6. Marks an eligible request as processing; a configured provider adapter must perform the real network call and callback handling.
    public async Task<bool> ProcessPaymentAsync(int paymentRequestId)
    {
        if (paymentRequestId <= 0) throw new ArgumentOutOfRangeException(nameof(paymentRequestId));
        var request = await _unitOfWork.PaymentRequest.GetByIdAsync(paymentRequestId);
        if (request is null) return false;
        if (request.Status is not ("Created" or "Authorized" or "Pending")) throw new InvalidOperationException("Payment request is not eligible for processing.");
        if (request.ExpiresAt.HasValue && request.ExpiresAt.Value <= DateTime.UtcNow) throw new InvalidOperationException("Payment request has expired.");
        request.Status = "Processing";
        request.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.PaymentRequest.Update(request);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
