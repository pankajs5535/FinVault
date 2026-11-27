using AutoMapper;
using FinVault.Application.DTOs.CardTransactionDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class CardTransactionService : ICardTransactionService
{
    private static readonly string[] ValidAuthorizationStatuses =
    {
        "Authorized", "Declined", "Reversed", "Settled", "Expired"
    };

    private static readonly string[] ValidChannels =
    {
        "POS", "ATM", "Ecommerce", "Contactless", "Recurring"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CardTransactionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. TRANSACTION IDENTIFICATION

    public async Task<IEnumerable<CardTransactionDto>> GetAllAsync()
    {
        var transactions = await _unitOfWork.CardTransaction.GetAllAsync();
        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<CardTransactionDto?> GetByIdAsync(int cardTransactionId)
    {
        ValidateId(cardTransactionId, nameof(cardTransactionId));

        var transaction = await _unitOfWork.CardTransaction
            .GetByIdAsync(cardTransactionId);

        return _mapper.Map<CardTransactionDto?>(transaction);
    }

    public async Task<CardTransactionDto?> GetByRetrievalReferenceNumberAsync(
        string retrievalReferenceNumber)
    {
        ValidateRequiredString(
            retrievalReferenceNumber,
            nameof(retrievalReferenceNumber));

        var transaction = await _unitOfWork.CardTransaction
            .GetByRetrievalReferenceNumberAsync(
                retrievalReferenceNumber.Trim());

        return _mapper.Map<CardTransactionDto?>(transaction);
    }

    public async Task<CardTransactionDto?> GetByAuthorizationCodeAsync(
        string authorizationCode)
    {
        ValidateRequiredString(authorizationCode, nameof(authorizationCode));

        var transaction = await _unitOfWork.CardTransaction
            .GetByAuthorizationCodeAsync(authorizationCode.Trim());

        return _mapper.Map<CardTransactionDto?>(transaction);
    }

    public async Task<CardTransactionDto?> GetCardTransactionDetailsAsync(
        int cardTransactionId)
    {
        ValidateId(cardTransactionId, nameof(cardTransactionId));

        var transaction = await _unitOfWork.CardTransaction
            .GetCardTransactionDetailsAsync(cardTransactionId);

        return _mapper.Map<CardTransactionDto?>(transaction);
    }

    public async Task<CardTransactionDto> CreateAsync(
        CreateCardTransactionDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.CardId, nameof(dto.CardId));
        ValidateId(dto.CurrencyId, nameof(dto.CurrencyId));

        if (dto.TransactionId.HasValue)
            ValidateId(dto.TransactionId.Value, nameof(dto.TransactionId));

        ValidateRequiredString(dto.MerchantName, nameof(dto.MerchantName));
        ValidateRequiredString(
            dto.MerchantCategoryCode,
            nameof(dto.MerchantCategoryCode));

        var authorizationStatus = NormalizeValue(
            dto.AuthorizationStatus,
            ValidAuthorizationStatuses,
            nameof(dto.AuthorizationStatus));

        var channel = NormalizeValue(
            dto.Channel,
            ValidChannels,
            nameof(dto.Channel));

        if (dto.Amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(dto.Amount),
                "Transaction amount must be greater than zero.");

        if (dto.SettlementAmount.HasValue &&
            dto.SettlementAmount.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dto.SettlementAmount),
                "Settlement amount must be greater than zero.");
        }

        if (dto.AuthorizedAt == default)
            throw new ArgumentException(
                "Authorization timestamp is required.",
                nameof(dto.AuthorizedAt));

        if (dto.AuthorizedAt > DateTime.UtcNow.AddMinutes(5))
            throw new ArgumentException(
                "Authorization timestamp cannot be in the future.",
                nameof(dto.AuthorizedAt));

        if (dto.MerchantCategoryCode.Length != 4 ||
            !dto.MerchantCategoryCode.All(char.IsDigit))
        {
            throw new ArgumentException(
                "Merchant Category Code must contain exactly four digits.",
                nameof(dto.MerchantCategoryCode));
        }

        if (authorizationStatus == "Declined" &&
            string.IsNullOrWhiteSpace(dto.DeclineReason))
        {
            throw new ArgumentException(
                "Decline reason is required for declined transactions.",
                nameof(dto.DeclineReason));
        }

        if (authorizationStatus != "Declined" &&
            !string.IsNullOrWhiteSpace(dto.DeclineReason))
        {
            throw new ArgumentException(
                "Decline reason can only be provided for declined transactions.",
                nameof(dto.DeclineReason));
        }

        var card = await _unitOfWork.Card.GetByIdAsync(dto.CardId);

        if (card == null)
            throw new InvalidOperationException("Card does not exist.");

        if (card.Status != "Active")
            throw new InvalidOperationException(
                "Only active cards can perform transactions.");

        if (card.ExpiryDate.Date < DateTime.UtcNow.Date)
            throw new InvalidOperationException("The card has expired.");

        var account = await _unitOfWork.Account.GetByIdAsync(card.AccountId);

        if (account == null || account.Status != "Active")
            throw new InvalidOperationException(
                "The card's linked account must be active.");

        var currency = await _unitOfWork.Currency.GetByIdAsync(dto.CurrencyId);

        if (currency == null || !currency.IsActive)
            throw new InvalidOperationException(
                "The selected transaction currency is not active.");

        if (channel == "ATM" && !card.IsAtmEnabled)
            throw new InvalidOperationException(
                "ATM transactions are disabled for this card.");

        if (channel == "POS" && !card.IsPosEnabled)
            throw new InvalidOperationException(
                "POS transactions are disabled for this card.");

        if (channel == "Ecommerce" && !card.IsOnlineEnabled)
            throw new InvalidOperationException(
                "Online transactions are disabled for this card.");

        if (channel == "Contactless" &&
            (!card.IsContactlessEnabled || !card.IsPosEnabled))
        {
            throw new InvalidOperationException(
                "Contactless transactions are disabled for this card.");
        }

        if (channel == "Recurring" && !card.IsOnlineEnabled)
            throw new InvalidOperationException(
                "Recurring transactions are disabled for this card.");

        if (dto.Amount > GetChannelLimit(card, channel))
            throw new InvalidOperationException(
                $"Transaction amount exceeds the configured {channel} limit.");

        if (!string.IsNullOrWhiteSpace(dto.RetrievalReferenceNumber))
        {
            var rrn = dto.RetrievalReferenceNumber.Trim();

            if (await _unitOfWork.CardTransaction
                    .ExistsByRetrievalReferenceNumberAsync(rrn))
            {
                throw new InvalidOperationException(
                    "Retrieval Reference Number already exists.");
            }
        }

        Transaction? postedTransaction = null;

        if (authorizationStatus == "Settled")
        {
            if (!dto.TransactionId.HasValue)
                throw new InvalidOperationException(
                    "A settled card transaction must reference its posted account transaction.");

            if (!dto.SettlementAmount.HasValue || !dto.SettledAt.HasValue)
                throw new InvalidOperationException(
                    "Settlement amount and settlement timestamp are required for settled transactions.");

            if (dto.SettledAt.Value < dto.AuthorizedAt)
                throw new InvalidOperationException(
                    "Settlement timestamp cannot precede authorization timestamp.");

            postedTransaction = await _unitOfWork.Transaction
                .GetByIdAsync(dto.TransactionId.Value);

            if (postedTransaction == null)
                throw new InvalidOperationException(
                    "The linked account transaction does not exist.");

            if (postedTransaction.Status != "Posted")
                throw new InvalidOperationException(
                    "Card settlement must link to a posted account transaction.");

            if (postedTransaction.CurrencyId != dto.CurrencyId)
                throw new InvalidOperationException(
                    "Card transaction currency must match the linked posted transaction currency.");

            if (postedTransaction.SourceAccountId != card.AccountId &&
                postedTransaction.DestinationAccountId != card.AccountId)
            {
                throw new InvalidOperationException(
                    "The linked posted transaction does not involve the card's account.");
            }

            if (postedTransaction.Amount != dto.SettlementAmount.Value)
                throw new InvalidOperationException(
                    "Settlement amount must match the linked posted transaction amount.");
        }
        else if (dto.TransactionId.HasValue ||
                 dto.SettlementAmount.HasValue ||
                 dto.SettledAt.HasValue)
        {
            throw new InvalidOperationException(
                "Transaction reference and settlement details can only be provided for settled card transactions.");
        }

        if (authorizationStatus == "Authorized" &&
            string.IsNullOrWhiteSpace(dto.AuthorizationCode))
        {
            throw new ArgumentException(
                "Authorization code is required for authorized transactions.",
                nameof(dto.AuthorizationCode));
        }

        var cardTransaction = _mapper.Map<CardTransaction>(dto);

        cardTransaction.AuthorizationStatus = authorizationStatus;
        cardTransaction.Channel = channel;
        cardTransaction.MerchantName = dto.MerchantName.Trim();
        cardTransaction.MerchantCategoryCode =
            dto.MerchantCategoryCode.Trim();
        cardTransaction.RetrievalReferenceNumber =
            string.IsNullOrWhiteSpace(dto.RetrievalReferenceNumber)
                ? null
                : dto.RetrievalReferenceNumber.Trim();
        cardTransaction.AuthorizationCode =
            string.IsNullOrWhiteSpace(dto.AuthorizationCode)
                ? null
                : dto.AuthorizationCode.Trim();
        cardTransaction.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.CardTransaction.AddAsync(cardTransaction);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CardTransactionDto>(cardTransaction);
    }

    // 2. CARD AND ACCOUNT TRANSACTION HISTORY

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsByCardIdAsync(int cardId)
    {
        ValidateId(cardId, nameof(cardId));

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsByCardIdAsync(cardId);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsByTransactionIdAsync(int transactionId)
    {
        ValidateId(transactionId, nameof(transactionId));

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsByTransactionIdAsync(transactionId);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetRecentCardTransactionsAsync(int cardId, int count)
    {
        ValidateId(cardId, nameof(cardId));

        if (count <= 0 || count > 200)
            throw new ArgumentOutOfRangeException(
                nameof(count),
                "Count must be between 1 and 200.");

        var transactions = await _unitOfWork.CardTransaction
            .GetRecentCardTransactionsAsync(cardId, count);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    // 3. AUTHORIZATION STATUS AND CHANNEL

    public async Task<IEnumerable<CardTransactionDto>>
        GetByAuthorizationStatusAsync(string authorizationStatus)
    {
        var status = NormalizeValue(
            authorizationStatus,
            ValidAuthorizationStatuses,
            nameof(authorizationStatus));

        var transactions = await _unitOfWork.CardTransaction
            .GetByAuthorizationStatusAsync(status);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetDeclinedCardTransactionsAsync()
    {
        var transactions = await _unitOfWork.CardTransaction
            .GetDeclinedCardTransactionsAsync();

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsByChannelAsync(string channel)
    {
        var normalizedChannel = NormalizeValue(
            channel,
            ValidChannels,
            nameof(channel));

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsByChannelAsync(normalizedChannel);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsByDateRangeAsync(
            DateTime startDate,
            DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsByDateRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    // 4. MERCHANT AND CURRENCY INFORMATION

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsByMerchantCategoryCodeAsync(
            string merchantCategoryCode)
    {
        ValidateRequiredString(
            merchantCategoryCode,
            nameof(merchantCategoryCode));

        if (merchantCategoryCode.Length != 4 ||
            !merchantCategoryCode.All(char.IsDigit))
        {
            throw new ArgumentException(
                "Merchant Category Code must contain exactly four digits.",
                nameof(merchantCategoryCode));
        }

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsByMerchantCategoryCodeAsync(
                merchantCategoryCode.Trim());

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsByMerchantNameAsync(string merchantName)
    {
        ValidateRequiredString(merchantName, nameof(merchantName));

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsByMerchantNameAsync(merchantName.Trim());

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsByCurrencyIdAsync(int currencyId)
    {
        ValidateId(currencyId, nameof(currencyId));

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsByCurrencyIdAsync(currencyId);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    // 5. SETTLEMENT MANAGEMENT

    public async Task<IEnumerable<CardTransactionDto>>
        GetUnsettledCardTransactionsAsync()
    {
        var transactions = await _unitOfWork.CardTransaction
            .GetUnsettledCardTransactionsAsync();

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<IEnumerable<CardTransactionDto>>
        GetSettledCardTransactionsAsync(
            DateTime startDate,
            DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var transactions = await _unitOfWork.CardTransaction
            .GetSettledCardTransactionsAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<decimal> GetTotalSettlementAmountByCardIdAsync(
        int cardId,
        DateTime startDate,
        DateTime endDate)
    {
        ValidateId(cardId, nameof(cardId));
        ValidateDateRange(startDate, endDate);

        return await _unitOfWork.CardTransaction
            .GetTotalSettlementAmountByCardIdAsync(
                cardId,
                startDate,
                endDate);
    }

    // 6. DISPUTES AND VALIDATION

    public async Task<IEnumerable<CardTransactionDto>>
        GetCardTransactionsWithDisputesAsync(int cardId)
    {
        ValidateId(cardId, nameof(cardId));

        var transactions = await _unitOfWork.CardTransaction
            .GetCardTransactionsWithDisputesAsync(cardId);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    public async Task<bool> ExistsByRetrievalReferenceNumberAsync(
        string retrievalReferenceNumber)
    {
        ValidateRequiredString(
            retrievalReferenceNumber,
            nameof(retrievalReferenceNumber));

        return await _unitOfWork.CardTransaction
            .ExistsByRetrievalReferenceNumberAsync(
                retrievalReferenceNumber.Trim());
    }

    // 7. SEARCH

    public async Task<IEnumerable<CardTransactionDto>>
        SearchCardTransactionsAsync(
            int? cardId,
            string? searchTerm,
            string? authorizationStatus,
            string? channel,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize)
    {
        if (cardId.HasValue)
            ValidateId(cardId.Value, nameof(cardId));

        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0 || pageSize > 200)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        var normalizedStatus = string.IsNullOrWhiteSpace(authorizationStatus)
            ? null
            : NormalizeValue(
                authorizationStatus,
                ValidAuthorizationStatuses,
                nameof(authorizationStatus));

        var normalizedChannel = string.IsNullOrWhiteSpace(channel)
            ? null
            : NormalizeValue(
                channel,
                ValidChannels,
                nameof(channel));

        if (startDate.HasValue != endDate.HasValue)
            throw new ArgumentException(
                "Both start date and end date must be provided together.");

        if (startDate.HasValue && endDate.HasValue)
            ValidateDateRange(startDate.Value, endDate.Value);

        var transactions = await _unitOfWork.CardTransaction
            .SearchCardTransactionsAsync(
                cardId,
                string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim(),
                normalizedStatus,
                normalizedChannel,
                startDate,
                endDate,
                pageNumber,
                pageSize);

        return _mapper.Map<IEnumerable<CardTransactionDto>>(transactions);
    }

    // PRIVATE VALIDATION METHODS

    private static decimal GetChannelLimit(Card card, string channel)
    {
        return channel switch
        {
            "ATM" => card.DailyAtmLimit,
            "POS" or "Contactless" => card.DailyPosLimit,
            "Ecommerce" or "Recurring" => card.DailyOnlineLimit,
            _ => 0
        };
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

    private static void ValidateRequiredString(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                $"{parameterName} is required.",
                parameterName);
    }

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException(
                "Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot precede start date.");
    }
}