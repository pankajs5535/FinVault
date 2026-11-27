using AutoMapper;
using FinVault.Application.DTOs.CardDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class CardService : ICardService
{
    private static readonly string[] ValidCardTypes =
    {
        "Debit", "Credit", "Prepaid"
    };

    private static readonly string[] ValidNetworks =
    {
        "RuPay", "Visa", "Mastercard"
    };

    private static readonly string[] ValidStatuses =
    {
        "Pending", "Active", "Blocked", "Hotlisted", "Expired", "Cancelled"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CARD IDENTIFICATION

    public async Task<IEnumerable<CardDto>> GetAllAsync()
    {
        var cards = await _unitOfWork.Card.GetAllAsync();
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<CardDto?> GetByIdAsync(int cardId)
    {
        ValidateId(cardId, nameof(cardId));

        var card = await _unitOfWork.Card.GetByIdAsync(cardId);
        return _mapper.Map<CardDto?>(card);
    }

    public async Task<CardDto?> GetByIdAndCustomerIdAsync(
        int cardId,
        int customerId)
    {
        ValidateId(cardId, nameof(cardId));
        ValidateId(customerId, nameof(customerId));

        var card = await _unitOfWork.Card
            .GetByIdAndCustomerIdAsync(cardId, customerId);

        return _mapper.Map<CardDto?>(card);
    }

    public async Task<CardDto?> GetCardDetailsAsync(int cardId)
    {
        ValidateId(cardId, nameof(cardId));

        var card = await _unitOfWork.Card.GetCardDetailsAsync(cardId);
        return _mapper.Map<CardDto?>(card);
    }

    public async Task<CardDto?> GetByMaskedNumberAsync(
        int customerId,
        string cardNumberMasked)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateRequiredString(cardNumberMasked, nameof(cardNumberMasked));

        var card = await _unitOfWork.Card
            .GetByMaskedNumberAsync(customerId, cardNumberMasked.Trim());

        return _mapper.Map<CardDto?>(card);
    }

    public async Task<CardDto> CreateAsync(CreateCardDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.CustomerId, nameof(dto.CustomerId));
        ValidateId(dto.AccountId, nameof(dto.AccountId));

        ValidateCardDetails(
            dto.CardNumberMasked,
            dto.CardHolderName,
            dto.CardType,
            dto.Network,
            dto.ExpiryDate,
            dto.DailyAtmLimit,
            dto.DailyPosLimit,
            dto.DailyOnlineLimit,
            dto.CreditLimit,
            dto.AvailableCredit,
            dto.OutstandingAmount,
            dto.BillingCycleDay);

        if (!string.Equals(dto.Status, "Pending", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                "A newly created card must begin with Pending status.");

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer == null || customer.Status != "Active")
            throw new InvalidOperationException(
                "An active customer is required to issue a card.");

        var account = await _unitOfWork.Account.GetByIdAsync(dto.AccountId);

        if (account == null || account.Status != "Active")
            throw new InvalidOperationException(
                "An active account is required to issue a card.");

        var customerOwnsAccount = await _unitOfWork.AccountHolder.ExistsAsync(
            x => x.CustomerId == dto.CustomerId &&
                 x.AccountId == dto.AccountId &&
                 x.IsActive &&
                 x.EffectiveFrom <= DateTime.UtcNow.Date &&
                 (!x.EffectiveTo.HasValue ||
                  x.EffectiveTo.Value >= DateTime.UtcNow.Date));

        if (!customerOwnsAccount)
            throw new InvalidOperationException(
                "The customer does not have an active holder relationship with this account.");

        ValidateCardTypeTerms(
            dto.CardType,
            dto.CreditLimit,
            dto.AvailableCredit,
            dto.OutstandingAmount,
            dto.BillingCycleDay);

        throw new NotSupportedException(
            "Card creation requires a secure card-vault/tokenization integration. " +
            "The current CreateCardDto does not provide a vault token, and the application " +
            "must never generate or store a substitute token.");
    }

    public async Task<CardDto?> UpdateAsync(UpdateCardDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.CardId, nameof(dto.CardId));
        ValidateId(dto.CustomerId, nameof(dto.CustomerId));
        ValidateId(dto.AccountId, nameof(dto.AccountId));

        var card = await _unitOfWork.Card
            .GetByIdAndCustomerIdAsync(dto.CardId, dto.CustomerId);

        if (card == null)
            return null;

        if (dto.RowVersion == null ||
            dto.RowVersion.Length == 0 ||
            card.RowVersion == null ||
            !card.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "Card data has been modified by another operation. Reload the card and try again.");
        }

        ValidateCardDetails(
            dto.CardNumberMasked,
            dto.CardHolderName,
            dto.CardType,
            dto.Network,
            dto.ExpiryDate,
            dto.DailyAtmLimit,
            dto.DailyPosLimit,
            dto.DailyOnlineLimit,
            dto.CreditLimit,
            dto.AvailableCredit,
            dto.OutstandingAmount,
            dto.BillingCycleDay);

        if (card.CustomerId != dto.CustomerId ||
            card.AccountId != dto.AccountId)
        {
            throw new InvalidOperationException(
                "A card cannot be transferred to another customer or account through an update.");
        }

        if (card.Status is "Cancelled" or "Expired" or "Hotlisted")
            throw new InvalidOperationException(
                "A cancelled, expired or hotlisted card cannot be updated.");

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer == null || customer.Status != "Active")
            throw new InvalidOperationException(
                "The card's customer must be active.");

        ValidateStatusTransition(card.Status, dto.Status);

        ValidateCardTypeTerms(
            dto.CardType,
            dto.CreditLimit,
            dto.AvailableCredit,
            dto.OutstandingAmount,
            dto.BillingCycleDay);

        if (dto.ExpiryDate.Date <= DateTime.UtcNow.Date &&
            dto.Status == "Active")
        {
            throw new InvalidOperationException(
                "An expired card cannot have Active status.");
        }

        if (dto.Status == "Active" &&
            !await _unitOfWork.AccountHolder.ExistsAsync(
                x => x.CustomerId == dto.CustomerId &&
                     x.AccountId == dto.AccountId &&
                     x.IsActive &&
                     x.EffectiveFrom <= DateTime.UtcNow.Date &&
                     (!x.EffectiveTo.HasValue ||
                      x.EffectiveTo.Value >= DateTime.UtcNow.Date)))
        {
            throw new InvalidOperationException(
                "The customer must have an active holder relationship with the linked account.");
        }

        var originalCardId = card.CardId;
        var originalCustomerId = card.CustomerId;
        var originalAccountId = card.AccountId;
        var originalToken = card.CardNumberToken;
        var originalRowVersion = card.RowVersion;
        var originalCreatedAt = card.CreatedAt;
        var originalCreatedBy = card.CreatedBy;
        var originalIssuedAt = card.IssuedAt;
        var originalActivatedAt = card.ActivatedAt;
        var originalBlockedAt = card.BlockedAt;
        var originalPinLastSetAt = card.PinLastSetAt;
        var originalPinRetryCount = card.PinRetryCount;
        var originalPinBlockedUntil = card.PinBlockedUntil;

        _mapper.Map(dto, card);

        // Preserve security-sensitive and database-managed properties.
        card.CardId = originalCardId;
        card.CustomerId = originalCustomerId;
        card.AccountId = originalAccountId;
        card.CardNumberToken = originalToken;
        card.RowVersion = originalRowVersion;
        card.CreatedAt = originalCreatedAt;
        card.CreatedBy = originalCreatedBy;
        card.IssuedAt = originalIssuedAt;
        card.ActivatedAt = originalActivatedAt;
        card.BlockedAt = originalBlockedAt;
        card.PinLastSetAt = originalPinLastSetAt;
        card.PinRetryCount = originalPinRetryCount;
        card.PinBlockedUntil = originalPinBlockedUntil;
        card.UpdatedAt = DateTime.UtcNow;

        if (dto.Status == "Blocked" && card.BlockedAt == null)
            card.BlockedAt = DateTime.UtcNow;

        if (dto.Status != "Blocked" && dto.Status != "Hotlisted")
            card.BlockedAt = null;

        _unitOfWork.Card.Update(card);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CardDto>(card);
    }

    public async Task<bool> DeleteAsync(DeleteCardDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.CardId, nameof(dto.CardId));

        var card = await _unitOfWork.Card.GetByIdAsync(dto.CardId);

        if (card == null)
            return false;

        if (card.Status == "Cancelled")
            return true;

        if (card.Status == "Hotlisted")
            throw new InvalidOperationException(
                "A hotlisted card must be reviewed through the bank's security process before cancellation.");

        card.Status = "Cancelled";
        card.BlockedAt = DateTime.UtcNow;
        card.BlockReason = "Card cancelled.";
        card.IsContactlessEnabled = false;
        card.IsOnlineEnabled = false;
        card.IsAtmEnabled = false;
        card.IsPosEnabled = false;
        card.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Card.Update(card);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CUSTOMER AND ACCOUNT CARD MANAGEMENT

    public async Task<IEnumerable<CardDto>> GetCardsByCustomerIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var cards = await _unitOfWork.Card
            .GetCardsByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsByAccountIdAsync(int accountId)
    {
        ValidateId(accountId, nameof(accountId));

        var cards = await _unitOfWork.Card
            .GetCardsByAccountIdAsync(accountId);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetActiveCardsByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var cards = await _unitOfWork.Card
            .GetActiveCardsByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsByStatusAsync(string status)
    {
        var normalizedStatus = NormalizeValue(
            status,
            ValidStatuses,
            nameof(status));

        var cards = await _unitOfWork.Card
            .GetCardsByStatusAsync(normalizedStatus);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    // 3. CARD TYPE AND NETWORK

    public async Task<IEnumerable<CardDto>> GetCardsByTypeAsync(
        int customerId,
        string cardType)
    {
        ValidateId(customerId, nameof(customerId));

        var normalizedType = NormalizeValue(
            cardType,
            ValidCardTypes,
            nameof(cardType));

        var cards = await _unitOfWork.Card
            .GetCardsByTypeAsync(customerId, normalizedType);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsByNetworkAsync(string network)
    {
        var normalizedNetwork = NormalizeValue(
            network,
            ValidNetworks,
            nameof(network));

        var cards = await _unitOfWork.Card
            .GetCardsByNetworkAsync(normalizedNetwork);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCreditCardsByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var cards = await _unitOfWork.Card
            .GetCreditCardsByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    // 4. CARD EXPIRY AND LIFECYCLE

    public async Task<IEnumerable<CardDto>> GetCardsExpiringWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var cards = await _unitOfWork.Card
            .GetCardsExpiringWithinRangeAsync(startDate.Date, endDate.Date);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetExpiredCardsAsync(DateTime currentDate)
    {
        ValidateDate(currentDate, nameof(currentDate));

        var cards = await _unitOfWork.Card.GetExpiredCardsAsync(currentDate.Date);
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetPendingCardsAsync()
    {
        var cards = await _unitOfWork.Card.GetPendingCardsAsync();
        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    // 5. CARD CONTROLS AND SECURITY

    public async Task<IEnumerable<CardDto>> GetCardsWithBlockedPinAsync(
        DateTime currentDateTime)
    {
        ValidateDate(currentDateTime, nameof(currentDateTime));

        var cards = await _unitOfWork.Card
            .GetCardsWithBlockedPinAsync(currentDateTime);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetCardsWithRestrictedChannelsAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var cards = await _unitOfWork.Card
            .GetCardsWithRestrictedChannelsAsync(customerId);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<IEnumerable<CardDto>> GetBlockedOrHotlistedCardsAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var cards = await _unitOfWork.Card
            .GetBlockedOrHotlistedCardsAsync(customerId);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    // 6. CREDIT CARD INFORMATION

    public async Task<IEnumerable<CardDto>> GetCreditCardsWithLowAvailableCreditAsync(
        decimal threshold)
    {
        if (threshold < 0)
            throw new ArgumentOutOfRangeException(
                nameof(threshold),
                "Threshold cannot be negative.");

        var cards = await _unitOfWork.Card
            .GetCreditCardsWithLowAvailableCreditAsync(threshold);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    // 7. CARD SEARCH AND VALIDATION

    public async Task<IEnumerable<CardDto>> SearchCardsAsync(
        int customerId,
        string searchTerm,
        int pageNumber,
        int pageSize)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateRequiredString(searchTerm, nameof(searchTerm));

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(pageNumber),
                "Page number must be greater than zero.");

        if (pageSize <= 0 || pageSize > 200)
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                "Page size must be between 1 and 200.");

        var cards = await _unitOfWork.Card.SearchCardsAsync(
            customerId,
            searchTerm.Trim(),
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<CardDto>>(cards);
    }

    public async Task<bool> HasActiveCardForAccountAsync(
        int customerId,
        int accountId)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateId(accountId, nameof(accountId));

        return await _unitOfWork.Card
            .HasActiveCardForAccountAsync(customerId, accountId);
    }

    // PRIVATE VALIDATION METHODS

    private static void ValidateCardDetails(
        string cardNumberMasked,
        string cardHolderName,
        string cardType,
        string network,
        DateTime expiryDate,
        decimal dailyAtmLimit,
        decimal dailyPosLimit,
        decimal dailyOnlineLimit,
        decimal? creditLimit,
        decimal? availableCredit,
        decimal? outstandingAmount,
        int? billingCycleDay)
    {
        ValidateRequiredString(cardNumberMasked, nameof(cardNumberMasked));
        ValidateRequiredString(cardHolderName, nameof(cardHolderName));

        if (cardNumberMasked.Length > 19)
            throw new ArgumentException(
                "Masked card number cannot exceed 19 characters.",
                nameof(cardNumberMasked));

        if (cardHolderName.Length > 100)
            throw new ArgumentException(
                "Cardholder name cannot exceed 100 characters.",
                nameof(cardHolderName));

        NormalizeValue(cardType, ValidCardTypes, nameof(cardType));
        NormalizeValue(network, ValidNetworks, nameof(network));

        if (expiryDate == default)
            throw new ArgumentException(
                "Card expiry date is required.",
                nameof(expiryDate));

        if (dailyAtmLimit < 0 ||
            dailyPosLimit < 0 ||
            dailyOnlineLimit < 0)
        {
            throw new ArgumentOutOfRangeException(
                "Daily card limits cannot be negative.");
        }

        if (creditLimit < 0 ||
            availableCredit < 0 ||
            outstandingAmount < 0)
        {
            throw new ArgumentOutOfRangeException(
                "Credit card amounts cannot be negative.");
        }

        if (billingCycleDay.HasValue &&
            (billingCycleDay.Value < 1 || billingCycleDay.Value > 31))
        {
            throw new ArgumentOutOfRangeException(
                nameof(billingCycleDay),
                "Billing cycle day must be between 1 and 31.");
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(
                cardNumberMasked,
                @"^[0-9Xx ]{8,19}$"))
        {
            throw new ArgumentException(
                "Card number must contain only digits, X characters and spaces in masked format.",
                nameof(cardNumberMasked));
        }

        if (cardNumberMasked.Any(char.IsDigit) &&
            !System.Text.RegularExpressions.Regex.IsMatch(
                cardNumberMasked,
                @"X{4,}"))
        {
            throw new ArgumentException(
                "The card number must be masked. A full card number must never be stored or accepted here.",
                nameof(cardNumberMasked));
        }
    }

    private static void ValidateCardTypeTerms(
        string cardType,
        decimal? creditLimit,
        decimal? availableCredit,
        decimal? outstandingAmount,
        int? billingCycleDay)
    {
        if (cardType == "Credit")
        {
            if (!creditLimit.HasValue ||
                !availableCredit.HasValue ||
                !outstandingAmount.HasValue ||
                !billingCycleDay.HasValue)
            {
                throw new InvalidOperationException(
                    "Credit cards require CreditLimit, AvailableCredit, OutstandingAmount and BillingCycleDay.");
            }

            if (availableCredit.Value > creditLimit.Value)
                throw new InvalidOperationException(
                    "Available credit cannot exceed the credit limit.");

            if (outstandingAmount.Value > creditLimit.Value)
                throw new InvalidOperationException(
                    "Outstanding amount cannot exceed the credit limit.");
        }
        else if (creditLimit.HasValue ||
                 availableCredit.HasValue ||
                 outstandingAmount.HasValue ||
                 billingCycleDay.HasValue)
        {
            throw new InvalidOperationException(
                "Credit card terms can only be assigned to Credit cards.");
        }
    }

    private static void ValidateStatusTransition(string currentStatus, string newStatus)
    {
        NormalizeValue(newStatus, ValidStatuses, nameof(newStatus));

        if (currentStatus == newStatus)
            return;

        var allowed = currentStatus switch
        {
            "Pending" => new[] { "Active", "Blocked", "Cancelled" },
            "Active" => new[] { "Blocked", "Hotlisted", "Expired", "Cancelled" },
            "Blocked" => new[] { "Active", "Hotlisted", "Cancelled", "Expired" },
            "Hotlisted" => new[] { "Cancelled" },
            "Expired" => new[] { "Cancelled" },
            "Cancelled" => Array.Empty<string>(),
            _ => Array.Empty<string>()
        };

        if (!allowed.Contains(newStatus, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Card status transition from '{currentStatus}' to '{newStatus}' is not permitted.");
        }
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

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }

    private static void ValidateRequiredString(string? value, string parameterName)
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

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        ValidateDate(startDate, nameof(startDate));
        ValidateDate(endDate, nameof(endDate));

        if (endDate.Date < startDate.Date)
            throw new ArgumentException(
                "End date cannot precede start date.");
    }
}