
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface ICardTransactionRepository : IGenericRepository<CardTransaction>
    {
        // =====================================================
        // 1. CARD TRANSACTION IDENTIFICATION
        // =====================================================

        // Finds a card transaction using its retrieval reference number.
        // Useful for tracing a card-network transaction using its RRN.
        Task<CardTransaction?> GetByRetrievalReferenceNumberAsync(string retrievalReferenceNumber);

        // Finds a card transaction using its authorization code.
        // Useful for investigating a specific card authorization.
        Task<CardTransaction?> GetByAuthorizationCodeAsync(string authorizationCode);

        // Retrieves a card transaction with its card, currency, posted transaction and disputes.
        // Useful for reviewing the complete card transaction record.
        Task<CardTransaction?> GetCardTransactionDetailsAsync(int cardTransactionId);


        // =====================================================
        // 2. CARD AND ACCOUNT TRANSACTION HISTORY
        // =====================================================

        // Retrieves card transactions belonging to a particular card.
        // Useful for displaying the cardholder's transaction history.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsByCardIdAsync(int cardId);

        // Retrieves card transactions linked to a posted account transaction.
        // Useful for tracing the relationship between card-network activity and ledger posting.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsByTransactionIdAsync(int transactionId);

        // Retrieves the latest card transactions for a particular card.
        // Useful for displaying recent card activity.
        Task<IEnumerable<CardTransaction>> GetRecentCardTransactionsAsync(int cardId, int count);


        // =====================================================
        // 3. AUTHORIZATION STATUS AND CHANNEL
        // =====================================================

        // Retrieves card transactions matching a particular authorization status.
        // Useful for monitoring authorized, declined, reversed, settled and expired transactions.
        Task<IEnumerable<CardTransaction>> GetByAuthorizationStatusAsync(string authorizationStatus);

        // Retrieves declined card transactions.
        // Useful for investigating unsuccessful card payment attempts.
        Task<IEnumerable<CardTransaction>> GetDeclinedCardTransactionsAsync();

        // Retrieves card transactions made through a particular channel.
        // Useful for separating POS, ATM, ecommerce, contactless and recurring activity.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsByChannelAsync(string channel);

        // Retrieves card transactions within a specified authorization date range.
        // Useful for transaction monitoring and operational reporting.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);


        // =====================================================
        // 4. MERCHANT AND CURRENCY INFORMATION
        // =====================================================

        // Retrieves card transactions associated with a merchant category code.
        // Useful for analyzing spending by merchant category.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsByMerchantCategoryCodeAsync(string merchantCategoryCode);

        // Retrieves card transactions associated with a merchant name.
        // Useful for investigating payments made to a particular merchant.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsByMerchantNameAsync(string merchantName);

        // Retrieves card transactions recorded in a particular currency.
        // Useful for reviewing card activity by transaction currency.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsByCurrencyIdAsync(int currencyId);


        // =====================================================
        // 5. SETTLEMENT MANAGEMENT
        // =====================================================

        // Retrieves authorized card transactions that have not yet been settled.
        // Useful for identifying transactions awaiting settlement processing.
        Task<IEnumerable<CardTransaction>> GetUnsettledCardTransactionsAsync();

        // Retrieves settled card transactions within a specified date range.
        // Useful for settlement reconciliation and financial reporting.
        Task<IEnumerable<CardTransaction>> GetSettledCardTransactionsAsync(DateTime startDate, DateTime endDate);

        // Calculates the total settlement amount for a card within a date range.
        // Useful for reviewing the card's settled transaction value.
        Task<decimal> GetTotalSettlementAmountByCardIdAsync(int cardId, DateTime startDate, DateTime endDate);


        // =====================================================
        // 6. DISPUTES AND VALIDATION
        // =====================================================

        // Retrieves card transactions that have associated disputes.
        // Useful for reviewing disputed card activity.
        Task<IEnumerable<CardTransaction>> GetCardTransactionsWithDisputesAsync(int cardId);

        // Checks whether a retrieval reference number already exists.
        // Useful for preventing duplicate card-network transaction references.
        Task<bool> ExistsByRetrievalReferenceNumberAsync(string retrievalReferenceNumber);

        // Searches card transactions by merchant, reference number, status and channel.
        // Pagination prevents loading large card transaction histories at once.
        Task<IEnumerable<CardTransaction>> SearchCardTransactionsAsync(int? cardId, string? searchTerm, string? authorizationStatus, string? channel, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize);
    }
}
