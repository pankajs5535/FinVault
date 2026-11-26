using FinVault.Application.DTOs.CardTransactionDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICardTransactionService
    {
        // 1. TRANSACTION IDENTIFICATION
        Task<IEnumerable<CardTransactionDto>> GetAllAsync();
        Task<CardTransactionDto?> GetByIdAsync(int cardTransactionId);
        Task<CardTransactionDto?> GetByRetrievalReferenceNumberAsync(string retrievalReferenceNumber);
        Task<CardTransactionDto?> GetByAuthorizationCodeAsync(string authorizationCode);
        Task<CardTransactionDto?> GetCardTransactionDetailsAsync(int cardTransactionId);
        Task<CardTransactionDto> CreateAsync(CreateCardTransactionDto dto);

        // 2. CARD AND ACCOUNT TRANSACTION HISTORY
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsByCardIdAsync(int cardId);
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsByTransactionIdAsync(int transactionId);
        Task<IEnumerable<CardTransactionDto>> GetRecentCardTransactionsAsync(int cardId, int count);

        // 3. AUTHORIZATION STATUS AND CHANNEL
        Task<IEnumerable<CardTransactionDto>> GetByAuthorizationStatusAsync(string authorizationStatus);
        Task<IEnumerable<CardTransactionDto>> GetDeclinedCardTransactionsAsync();
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsByChannelAsync(string channel);
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);

        // 4. MERCHANT AND CURRENCY INFORMATION
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsByMerchantCategoryCodeAsync(string merchantCategoryCode);
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsByMerchantNameAsync(string merchantName);
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsByCurrencyIdAsync(int currencyId);

        // 5. SETTLEMENT MANAGEMENT
        Task<IEnumerable<CardTransactionDto>> GetUnsettledCardTransactionsAsync();
        Task<IEnumerable<CardTransactionDto>> GetSettledCardTransactionsAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalSettlementAmountByCardIdAsync(int cardId, DateTime startDate, DateTime endDate);

        // 6. DISPUTES AND VALIDATION
        Task<IEnumerable<CardTransactionDto>> GetCardTransactionsWithDisputesAsync(int cardId);
        Task<bool> ExistsByRetrievalReferenceNumberAsync(string retrievalReferenceNumber);

        // 7. SEARCH
        Task<IEnumerable<CardTransactionDto>> SearchCardTransactionsAsync(
            int? cardId,
            string? searchTerm,
            string? authorizationStatus,
            string? channel,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);
    }
}

//Banking rules:
//-Treat card transaction records as immutable financial events; do not expose ordinary update or delete operations.
//- Validate RRN uniqueness when an RRN is provided.
//- Link settled card activity to its posted account transaction.
//- Preserve authorization and settlement timestamps for audit and reconciliation.
//- Validate card ownership and currency before recording a transaction.