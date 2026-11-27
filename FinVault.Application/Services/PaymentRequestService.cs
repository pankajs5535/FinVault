using AutoMapper;
using FinVault.Application.DTOs.PaymentRequestDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class PaymentRequestService : IPaymentRequestService
{
    private static readonly string[] ValidStatuses =
    {
        "Created", "Authorized", "Submitted", "Processing",
        "Completed", "Failed", "Rejected", "Expired", "Cancelled"
    };

    private static readonly string[] ValidChannels =
    {
        "Branch", "NetBanking", "MobileApp", "UPI", "Api"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PaymentRequestService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. PAYMENT REQUEST IDENTIFICATION

    public async Task<IEnumerable<PaymentRequestDto>> GetAllAsync()
    {
        var requests = await _unitOfWork.PaymentRequest.GetAllAsync();
        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<PaymentRequestDto?> GetByIdAsync(int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var request = await _unitOfWork.PaymentRequest.GetByIdAsync(paymentRequestId);
        return _mapper.Map<PaymentRequestDto?>(request);
    }

    public async Task<PaymentRequestDto?> GetByRequestReferenceAsync(
        string requestReference)
    {
        ValidateRequiredString(requestReference, nameof(requestReference));

        var request = await _unitOfWork.PaymentRequest
            .GetByRequestReferenceAsync(requestReference.Trim());

        return _mapper.Map<PaymentRequestDto?>(request);
    }

    public async Task<PaymentRequestDto?> GetByIdempotencyKeyAsync(
        string idempotencyKey)
    {
        ValidateIdempotencyKey(idempotencyKey);

        var request = await _unitOfWork.PaymentRequest
            .GetByIdempotencyKeyAsync(idempotencyKey.Trim());

        return _mapper.Map<PaymentRequestDto?>(request);
    }

    public async Task<PaymentRequestDto?> GetByUtrNumberAsync(string utrNumber)
    {
        ValidateRequiredString(utrNumber, nameof(utrNumber));

        var request = await _unitOfWork.PaymentRequest
            .GetByUtrNumberAsync(utrNumber.Trim());

        return _mapper.Map<PaymentRequestDto?>(request);
    }

    public async Task<PaymentRequestDto?> GetPaymentRequestDetailsAsync(
        int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var request = await _unitOfWork.PaymentRequest
            .GetPaymentRequestDetailsAsync(paymentRequestId);

        return _mapper.Map<PaymentRequestDto?>(request);
    }

    public async Task<bool> ExistsByRequestReferenceAsync(string requestReference)
    {
        ValidateRequiredString(requestReference, nameof(requestReference));

        return await _unitOfWork.PaymentRequest
            .ExistsByRequestReferenceAsync(requestReference.Trim());
    }

    public async Task<bool> ExistsByIdempotencyKeyAsync(string idempotencyKey)
    {
        ValidateIdempotencyKey(idempotencyKey);

        return await _unitOfWork.PaymentRequest
            .ExistsByIdempotencyKeyAsync(idempotencyKey.Trim());
    }

    public async Task<PaymentRequestDto> CreateAsync(CreatePaymentRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.PaymentMethodId, nameof(dto.PaymentMethodId));
        ValidateId(dto.SourceAccountId, nameof(dto.SourceAccountId));
        ValidateIdempotencyKey(dto.IdempotencyKey);
        ValidateAmount(dto.Amount);

        // Return the original request for a legitimate retry.
        var existingRequest = await _unitOfWork.PaymentRequest
            .GetByIdempotencyKeyAsync(dto.IdempotencyKey.Trim());

        if (existingRequest != null)
        {
            if (existingRequest.SourceAccountId != dto.SourceAccountId ||
                existingRequest.PaymentMethodId != dto.PaymentMethodId ||
                existingRequest.Amount != dto.Amount ||
                existingRequest.BeneficiaryId != dto.BeneficiaryId)
            {
                throw new InvalidOperationException(
                    "The idempotency key has already been used with different payment details.");
            }

            return _mapper.Map<PaymentRequestDto>(existingRequest);
        }

        var account = await _unitOfWork.Account.GetByIdAsync(dto.SourceAccountId);

        if (account == null || account.Status != "Active")
            throw new InvalidOperationException(
                "The source account does not exist or is not active.");

        if (account.AvailableBalance < dto.Amount)
            throw new InvalidOperationException(
                "The source account does not have sufficient available balance.");

        // CustomerId is not supplied by the DTO.
        // Derive the customer from the active primary account holder.
        var primaryHolders = await _unitOfWork.AccountHolder.FindAsync(
            x => x.AccountId == dto.SourceAccountId &&
                 x.HolderType == "Primary" &&
                 x.IsActive &&
                 x.EffectiveFrom <= DateTime.UtcNow.Date &&
                 (!x.EffectiveTo.HasValue ||
                  x.EffectiveTo.Value >= DateTime.UtcNow.Date));

        var primaryHolder = primaryHolders.SingleOrDefault();

        if (primaryHolder == null)
            throw new InvalidOperationException(
                "The source account must have exactly one active primary holder.");

        var customer = await _unitOfWork.Customer
            .GetByIdAsync(primaryHolder.CustomerId);

        if (customer == null || customer.Status != "Active")
            throw new InvalidOperationException(
                "The primary account holder must be an active customer.");

        var paymentMethod = await _unitOfWork.PaymentMethod
            .GetByIdAsync(dto.PaymentMethodId);

        if (paymentMethod == null || !paymentMethod.IsActive)
            throw new InvalidOperationException(
                "The selected payment method does not exist or is inactive.");

        ValidatePaymentMethodLimits(paymentMethod, dto.Amount);

        var currency = await _unitOfWork.Currency
            .GetByIdAsync(account.CurrencyId);

        if (currency == null || !currency.IsActive)
            throw new InvalidOperationException(
                "The source account currency is unavailable.");

        Beneficiary? beneficiary = null;

        if (dto.BeneficiaryId.HasValue)
        {
            ValidateId(dto.BeneficiaryId.Value, nameof(dto.BeneficiaryId));

            beneficiary = await _unitOfWork.Beneficiary
                .GetByIdAsync(dto.BeneficiaryId.Value);

            if (beneficiary == null ||
                beneficiary.CustomerId != customer.CustomerId ||
                beneficiary.Status != "Active")
            {
                throw new InvalidOperationException(
                    "The selected beneficiary does not belong to the primary customer or is not active.");
            }

            if (beneficiary.PerTransactionLimit.HasValue &&
                dto.Amount > beneficiary.PerTransactionLimit.Value)
            {
                throw new InvalidOperationException(
                    "Payment amount exceeds the beneficiary's per-transaction limit.");
            }

            if (paymentMethod.Rail == "UPI" &&
                beneficiary.BeneficiaryType != "UPI")
            {
                throw new InvalidOperationException(
                    "UPI payments require an active UPI beneficiary.");
            }

            if (paymentMethod.Rail != "UPI" &&
                beneficiary.BeneficiaryType == "UPI")
            {
                throw new InvalidOperationException(
                    "A UPI beneficiary cannot be used with this payment rail.");
            }
        }
        else
        {
            ValidateManualPayeeDetails(paymentMethod.Rail, dto);
        }

        if (paymentMethod.Rail == "UPI" &&
            beneficiary == null &&
            string.IsNullOrWhiteSpace(dto.PayeeVpa))
        {
            throw new InvalidOperationException(
                "A UPI payment requires a saved UPI beneficiary or a payee VPA.");
        }

        if (dto.ScheduledFor.HasValue &&
            dto.ScheduledFor.Value <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "Scheduled payment time must be in the future.",
                nameof(dto.ScheduledFor));
        }

        await ValidateDailyPaymentLimitAsync(
            dto.SourceAccountId,
            paymentMethod,
            dto.Amount);

        // The application currently has no authenticated-user context.
        // API authorization must independently verify the caller's account access.

        var now = DateTime.UtcNow;

        var request = _mapper.Map<PaymentRequest>(dto);

        request.RequestReference = GenerateRequestReference();
        request.IdempotencyKey = dto.IdempotencyKey.Trim();
        request.CustomerId = customer.CustomerId;
        request.CurrencyId = account.CurrencyId;
        request.SourceAccountId = account.AccountId;
        request.PaymentMethodId = paymentMethod.PaymentMethodId;
        request.BeneficiaryId = beneficiary?.BeneficiaryId;

        request.PayeeName = beneficiary?.BeneficiaryName ??
                            NormalizeOptional(dto.PayeeName);

        request.PayeeAccountNumber = beneficiary?.AccountNumber ??
                                     NormalizeOptional(dto.PayeeAccountNumber);

        request.PayeeIfsc = beneficiary?.IfscCode ??
                            NormalizeOptional(dto.PayeeIfsc)?.ToUpperInvariant();

        request.PayeeVpa = beneficiary?.UpiVpa ??
                           NormalizeOptional(dto.PayeeVpa);

        request.Channel = paymentMethod.Rail == "UPI"
            ? "UPI"
            : "MobileApp";

        request.Status = "Created";
        request.RequestedAt = now;
        request.ExpiresAt = dto.ScheduledFor.HasValue
            ? dto.ScheduledFor.Value.AddHours(24)
            : now.AddHours(24);

        request.ScheduledFor = dto.ScheduledFor;
        request.TransactionId = null;
        request.UtrNumber = null;
        request.AuthorizedAt = null;
        request.CompletedAt = null;
        request.FailureCode = null;
        request.FailureReason = null;
        request.CreatedAt = now;
        request.UpdatedAt = null;

        await _unitOfWork.PaymentRequest.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PaymentRequestDto>(request);
    }

    public async Task<PaymentRequestDto?> UpdateAsync(UpdatePaymentRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.PaymentRequestId, nameof(dto.PaymentRequestId));
        ValidateAmount(dto.Amount);

        if (dto.RowVersion == null || dto.RowVersion.Length == 0)
            throw new ArgumentException(
                "RowVersion is required.",
                nameof(dto.RowVersion));

        var request = await _unitOfWork.PaymentRequest
            .GetByIdAsync(dto.PaymentRequestId);

        if (request == null)
            return null;

        if (request.RowVersion == null ||
            !request.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "Payment request has been modified by another operation. Reload it and try again.");
        }

        if (request.Status != "Created")
            throw new InvalidOperationException(
                "Payment requests can only be edited while their status is Created.");

        if (request.ExpiresAt.HasValue &&
            request.ExpiresAt.Value <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "An expired payment request cannot be updated.");
        }

        if (dto.ScheduledFor.HasValue &&
            dto.ScheduledFor.Value <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "Scheduled payment time must be in the future.",
                nameof(dto.ScheduledFor));
        }

        if (request.BeneficiaryId.HasValue)
        {
            var beneficiary = await _unitOfWork.Beneficiary
                .GetByIdAsync(request.BeneficiaryId.Value);

            if (beneficiary == null || beneficiary.Status != "Active")
                throw new InvalidOperationException(
                    "The saved beneficiary is no longer active.");
        }

        var account = await _unitOfWork.Account
            .GetByIdAsync(request.SourceAccountId);

        if (account == null || account.Status != "Active")
            throw new InvalidOperationException(
                "The source account is no longer active.");

        if (account.AvailableBalance < dto.Amount)
            throw new InvalidOperationException(
                "The source account does not have sufficient available balance.");

        var paymentMethod = await _unitOfWork.PaymentMethod
            .GetByIdAsync(request.PaymentMethodId);

        if (paymentMethod == null || !paymentMethod.IsActive)
            throw new InvalidOperationException(
                "The payment method is no longer active.");

        ValidatePaymentMethodLimits(paymentMethod, dto.Amount);

        await ValidateDailyPaymentLimitAsync(
            request.SourceAccountId,
            paymentMethod,
            dto.Amount,
            request.PaymentRequestId);

        var originalId = request.PaymentRequestId;
        var originalCustomerId = request.CustomerId;
        var originalAccountId = request.SourceAccountId;
        var originalPaymentMethodId = request.PaymentMethodId;
        var originalCurrencyId = request.CurrencyId;
        var originalBeneficiaryId = request.BeneficiaryId;
        var originalReference = request.RequestReference;
        var originalIdempotencyKey = request.IdempotencyKey;
        var originalStatus = request.Status;
        var originalChannel = request.Channel;
        var originalRequestedAt = request.RequestedAt;
        var originalExpiresAt = request.ExpiresAt;
        var originalCreatedAt = request.CreatedAt;
        var originalCreatedBy = request.CreatedBy;
        var originalRowVersion = request.RowVersion;

        _mapper.Map(dto, request);

        request.PaymentRequestId = originalId;
        request.CustomerId = originalCustomerId;
        request.SourceAccountId = originalAccountId;
        request.PaymentMethodId = originalPaymentMethodId;
        request.CurrencyId = originalCurrencyId;
        request.BeneficiaryId = originalBeneficiaryId;
        request.RequestReference = originalReference;
        request.IdempotencyKey = originalIdempotencyKey;
        request.Status = originalStatus;
        request.Channel = originalChannel;
        request.RequestedAt = originalRequestedAt;
        request.ExpiresAt = originalExpiresAt;
        request.CreatedAt = originalCreatedAt;
        request.CreatedBy = originalCreatedBy;
        request.RowVersion = originalRowVersion;
        request.UpdatedAt = DateTime.UtcNow;

        request.ExpiresAt = dto.ScheduledFor.HasValue
            ? dto.ScheduledFor.Value.AddHours(24)
            : DateTime.UtcNow.AddHours(24);

        _unitOfWork.PaymentRequest.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<PaymentRequestDto>(request);
    }

    public async Task<bool> DeleteAsync(DeletePaymentRequestDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.PaymentRequestId, nameof(dto.PaymentRequestId));

        var request = await _unitOfWork.PaymentRequest
            .GetByIdAsync(dto.PaymentRequestId);

        if (request == null)
            return false;

        if (request.Status == "Cancelled")
            return true;

        if (request.Status != "Created")
            throw new InvalidOperationException(
                "Only payment requests in Created status can be cancelled.");

        request.Status = "Cancelled";
        request.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.PaymentRequest.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CUSTOMER AND ACCOUNT PAYMENT HISTORY

    public async Task<IEnumerable<PaymentRequestDto>> GetByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var requests = await _unitOfWork.PaymentRequest
            .GetByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetBySourceAccountIdAsync(
        int sourceAccountId)
    {
        ValidateId(sourceAccountId, nameof(sourceAccountId));

        var requests = await _unitOfWork.PaymentRequest
            .GetBySourceAccountIdAsync(sourceAccountId);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetByBeneficiaryIdAsync(
        int beneficiaryId)
    {
        ValidateId(beneficiaryId, nameof(beneficiaryId));

        var requests = await _unitOfWork.PaymentRequest
            .GetByBeneficiaryIdAsync(beneficiaryId);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetByPaymentMethodIdAsync(
        int paymentMethodId)
    {
        ValidateId(paymentMethodId, nameof(paymentMethodId));

        var requests = await _unitOfWork.PaymentRequest
            .GetByPaymentMethodIdAsync(paymentMethodId);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetByCurrencyIdAsync(
        int currencyId)
    {
        ValidateId(currencyId, nameof(currencyId));

        var requests = await _unitOfWork.PaymentRequest
            .GetByCurrencyIdAsync(currencyId);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    // 3. PAYMENT LIFECYCLE AND STATUS

    public async Task<IEnumerable<PaymentRequestDto>> GetByStatusAsync(string status)
    {
        var normalizedStatus = NormalizeValue(
            status,
            ValidStatuses,
            nameof(status));

        var requests = await _unitOfWork.PaymentRequest
            .GetByStatusAsync(normalizedStatus);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetAuthorizedPaymentRequestsAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetAuthorizedPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetSubmittedPaymentRequestsAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetSubmittedPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetProcessingPaymentRequestsAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetProcessingPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetCompletedPaymentRequestsAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetCompletedPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetFailedPaymentRequestsAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetFailedPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetExpiredPaymentRequestsAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetExpiredPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetCancelledPaymentRequestsAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetCancelledPaymentRequestsAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    // 4. CHANNEL AND SCHEDULING

    public async Task<IEnumerable<PaymentRequestDto>> GetByChannelAsync(string channel)
    {
        var normalizedChannel = NormalizeValue(
            channel,
            ValidChannels,
            nameof(channel));

        var requests = await _unitOfWork.PaymentRequest
            .GetByChannelAsync(normalizedChannel);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>>
        GetScheduledPaymentsWithinRangeAsync(
            DateTime startDate,
            DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var requests = await _unitOfWork.PaymentRequest
            .GetScheduledPaymentsWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetScheduledPaymentsDueAsync(
        DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var requests = await _unitOfWork.PaymentRequest
            .GetScheduledPaymentsDueAsync(currentDateTime);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetExpiredPendingRequestsAsync(
        DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var requests = await _unitOfWork.PaymentRequest
            .GetExpiredPendingRequestsAsync(currentDateTime);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    // 5. DATE-BASED REPORTING

    public async Task<IEnumerable<PaymentRequestDto>> GetRequestedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var requests = await _unitOfWork.PaymentRequest
            .GetRequestedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetCompletedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var requests = await _unitOfWork.PaymentRequest
            .GetCompletedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetRecentRequestsByCustomerIdAsync(
        int customerId,
        int count)
    {
        ValidateId(customerId, nameof(customerId));

        if (count <= 0 || count > 200)
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Count must be between 1 and 200.");

        var requests = await _unitOfWork.PaymentRequest
            .GetRecentRequestsByCustomerIdAsync(customerId, count);

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    // 6. TRANSACTION AND GATEWAY TRACKING

    public async Task<PaymentRequestDto?> GetWithTransactionAsync(
        int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var request = await _unitOfWork.PaymentRequest
            .GetWithTransactionAsync(paymentRequestId);

        return _mapper.Map<PaymentRequestDto?>(request);
    }

    public async Task<PaymentRequestDto?> GetWithGatewayLogsAsync(
        int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var request = await _unitOfWork.PaymentRequest
            .GetWithGatewayLogsAsync(paymentRequestId);

        return _mapper.Map<PaymentRequestDto?>(request);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetRequestsWithoutTransactionAsync()
    {
        var requests = await _unitOfWork.PaymentRequest
            .GetRequestsWithoutTransactionAsync();

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<IEnumerable<PaymentRequestDto>> GetByFailureCodeAsync(
        string failureCode)
    {
        ValidateRequiredString(failureCode, nameof(failureCode));

        var requests = await _unitOfWork.PaymentRequest
            .GetByFailureCodeAsync(failureCode.Trim());

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    // 7. FINANCIAL REPORTING AND SEARCH

    public async Task<decimal> GetTotalCompletedAmountByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        return await _unitOfWork.PaymentRequest
            .GetTotalCompletedAmountByCustomerIdAsync(customerId);
    }

    public async Task<decimal> GetTotalCompletedAmountWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        return await _unitOfWork.PaymentRequest
            .GetTotalCompletedAmountWithinRangeAsync(startDate, endDate);
    }

    public async Task<IEnumerable<PaymentRequestDto>> SearchPaymentRequestsAsync(
        string searchTerm)
    {
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        var requests = await _unitOfWork.PaymentRequest
            .SearchPaymentRequestsAsync(searchTerm.Trim());

        return _mapper.Map<IEnumerable<PaymentRequestDto>>(requests);
    }

    public async Task<int> CountByCustomerAndStatusAsync(
        int customerId,
        string status)
    {
        ValidateId(customerId, nameof(customerId));

        var normalizedStatus = NormalizeValue(
            status,
            ValidStatuses,
            nameof(status));

        return await _unitOfWork.PaymentRequest
            .CountByCustomerAndStatusAsync(customerId, normalizedStatus);
    }

    // 8. UPI PAYMENT OPERATIONS

    public Task<UpiTransferResponseDto> InitiateUpiTransferAsync(
        InitiateUpiTransferDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.SourceAccountId, nameof(dto.SourceAccountId));
        ValidateId(dto.BeneficiaryId, nameof(dto.BeneficiaryId));
        ValidateIdempotencyKey(dto.IdempotencyKey);
        ValidateAmount(dto.Amount);

        throw new NotSupportedException(
            "UPI transfer initiation requires a configured UPI provider client, " +
            "provider credentials, secure request signing and response verification. " +
            "These are not present in the current solution.");
    }

    public Task<bool> ProcessUpiProviderCallbackAsync(UpiProviderCallbackDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateRequiredString(dto.ProviderEventId, nameof(dto.ProviderEventId));
        ValidateRequiredString(dto.ProviderReference, nameof(dto.ProviderReference));
        ValidateRequiredString(dto.RequestReference, nameof(dto.RequestReference));

        throw new NotSupportedException(
            "UPI callbacks cannot be processed safely because the current callback DTO " +
            "does not contain a verifiable signature and the solution has no provider " +
            "signature-validation contract or persistent provider-event deduplication.");
    }

    public async Task<UpiTransactionStatusDto?> GetUpiTransactionStatusAsync(
        int paymentRequestId)
    {
        ValidateId(paymentRequestId, nameof(paymentRequestId));

        var request = await _unitOfWork.PaymentRequest
            .GetPaymentRequestDetailsAsync(paymentRequestId);

        if (request == null)
            return null;

        if (request.Channel != "UPI")
            throw new InvalidOperationException(
                "The selected payment request is not a UPI payment.");

        return new UpiTransactionStatusDto
        {
            PaymentRequestId = request.PaymentRequestId,
            RequestReference = request.RequestReference,
            UtrNumber = request.UtrNumber,
            Amount = request.Amount,
            CurrencyCode = request.Currency?.CurrencyCode ?? "INR",
            Status = request.Status,
            FailureCode = request.FailureCode,
            FailureReason = request.FailureReason,
            RequestedAt = request.RequestedAt,
            CompletedAt = request.CompletedAt,
            LastCheckedAt = null
        };
    }

    public Task<UpiVpaVerificationResponseDto> VerifyUpiVpaAsync(
        VerifyUpiVpaDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateRequiredString(dto.UpiVpa, nameof(dto.UpiVpa));

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                dto.UpiVpa.Trim(),
                @"^[A-Za-z0-9._-]{2,256}@[A-Za-z0-9.-]{2,64}$"))
        {
            throw new ArgumentException(
                "Invalid UPI VPA format.",
                nameof(dto.UpiVpa));
        }

        throw new NotSupportedException(
            "UPI VPA verification requires a configured payment provider. " +
            "A syntactically valid VPA does not prove that the VPA exists or belongs to the intended payee.");
    }

    // PRIVATE BUSINESS-RULE METHODS

    private async Task ValidateDailyPaymentLimitAsync(
        int sourceAccountId,
        PaymentMethod paymentMethod,
        decimal requestedAmount,
        int? excludePaymentRequestId = null)
    {
        if (!paymentMethod.DailyLimit.HasValue)
            return;

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var existingRequests = await _unitOfWork.PaymentRequest
            .GetBySourceAccountIdAsync(sourceAccountId);

        var dailyTotal = existingRequests
            .Where(x =>
                x.RequestedAt >= today &&
                x.RequestedAt < tomorrow &&
                x.PaymentMethodId == paymentMethod.PaymentMethodId &&
                x.PaymentRequestId != excludePaymentRequestId &&
                x.Status != "Failed" &&
                x.Status != "Rejected" &&
                x.Status != "Expired" &&
                x.Status != "Cancelled")
            .Sum(x => x.Amount);

        if (dailyTotal + requestedAmount > paymentMethod.DailyLimit.Value)
            throw new InvalidOperationException(
                $"Payment exceeds the daily limit of {paymentMethod.DailyLimit.Value} for this payment method.");
    }

    private static void ValidatePaymentMethodLimits(
        PaymentMethod paymentMethod,
        decimal amount)
    {
        if (amount < paymentMethod.MinAmount)
            throw new InvalidOperationException(
                $"Payment amount is below the minimum allowed amount of {paymentMethod.MinAmount}.");

        if (paymentMethod.MaxAmount.HasValue &&
            amount > paymentMethod.MaxAmount.Value)
        {
            throw new InvalidOperationException(
                $"Payment amount exceeds the maximum allowed amount of {paymentMethod.MaxAmount.Value}.");
        }

        if (paymentMethod.DailyLimit.HasValue &&
            amount > paymentMethod.DailyLimit.Value)
        {
            throw new InvalidOperationException(
                "Payment amount exceeds the configured daily payment limit.");
        }
    }

    private static void ValidateManualPayeeDetails(
        string rail,
        CreatePaymentRequestDto dto)
    {
        if (rail == "UPI")
        {
            ValidateRequiredString(dto.PayeeVpa, nameof(dto.PayeeVpa));

            if (!string.IsNullOrWhiteSpace(dto.PayeeAccountNumber) ||
                !string.IsNullOrWhiteSpace(dto.PayeeIfsc))
            {
                throw new InvalidOperationException(
                    "UPI payments must use a VPA rather than manually entered bank account details.");
            }

            return;
        }

        if (rail is "NEFT" or "RTGS" or "IMPS")
        {
            ValidateRequiredString(
                dto.PayeeAccountNumber,
                nameof(dto.PayeeAccountNumber));

            ValidateRequiredString(dto.PayeeIfsc, nameof(dto.PayeeIfsc));

            if (!System.Text.RegularExpressions.Regex.IsMatch(
                    dto.PayeeIfsc.Trim().ToUpperInvariant(),
                    @"^[A-Z]{4}0[A-Z0-9]{6}$"))
            {
                throw new ArgumentException(
                    "Invalid beneficiary IFSC code.",
                    nameof(dto.PayeeIfsc));
            }

            if (!string.IsNullOrWhiteSpace(dto.PayeeVpa))
                throw new InvalidOperationException(
                    "Bank transfer payee details must not contain a UPI VPA.");

            return;
        }

        throw new InvalidOperationException(
            $"Manual payee details are not supported for the '{rail}' payment rail. Use a supported saved beneficiary.");
    }

    private static string GenerateRequestReference()
    {
        return $"PR{Guid.NewGuid():N}".ToUpperInvariant();
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static string NormalizeValue(
        string value,
        IEnumerable<string> allowedValues,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);

        var normalized = allowedValues.FirstOrDefault(
            x => string.Equals(
                x,
                value.Trim(),
                StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid {parameterName}. Allowed values: {string.Join(", ", allowedValues)}.",
            parameterName);
    }

    private static void ValidateIdempotencyKey(string idempotencyKey)
    {
        ValidateRequiredString(idempotencyKey, nameof(idempotencyKey));

        if (idempotencyKey.Trim().Length is < 16 or > 64)
            throw new ArgumentException(
                "Idempotency key must contain between 16 and 64 characters.",
                nameof(idempotencyKey));
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Payment amount must be greater than zero.");
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }

    private static void ValidateRequiredString(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static void ValidateDate(DateTime date, string parameterName)
    {
        if (date == default)
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot precede start date.");
    }
}