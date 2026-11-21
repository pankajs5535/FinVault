
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class CardTransactionRepository : GenericRepository<CardTransaction>, ICardTransactionRepository
    {
        public CardTransactionRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. CARD TRANSACTION IDENTIFICATION
        // =====================================================

        // Finds a card transaction using its retrieval reference number.
        // Useful for tracing a card-network transaction using its RRN.
        public async Task<CardTransaction?> GetByRetrievalReferenceNumberAsync(string retrievalReferenceNumber)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.RetrievalReferenceNumber == retrievalReferenceNumber);
        }

        // Finds a card transaction using its authorization code.
        // Useful for investigating a specific card authorization.
        public async Task<CardTransaction?> GetByAuthorizationCodeAsync(string authorizationCode)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.AuthorizationCode == authorizationCode);
        }

        // Retrieves a card transaction with its card, currency, posted transaction and disputes.
        // Useful for reviewing the complete card transaction record.
        public async Task<CardTransaction?> GetCardTransactionDetailsAsync(int cardTransactionId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Card)
                .Include(x => x.Transaction)
                .Include(x => x.Currency)
                .Include(x => x.Disputes)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.CardTransactionId == cardTransactionId);
        }


        // =====================================================
        // 2. CARD AND ACCOUNT TRANSACTION HISTORY
        // =====================================================

        // Retrieves card transactions belonging to a particular card.
        // Useful for displaying the cardholder's transaction history.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsByCardIdAsync(int cardId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CardId == cardId)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves card transactions linked to a posted account transaction.
        // Useful for tracing the relationship between card-network activity and ledger posting.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsByTransactionIdAsync(int transactionId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.TransactionId == transactionId)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves the latest card transactions for a particular card.
        // Useful for displaying recent card activity.
        public async Task<IEnumerable<CardTransaction>> GetRecentCardTransactionsAsync(int cardId, int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Transaction count must be greater than zero.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.CardId == cardId)
                .OrderByDescending(x => x.AuthorizedAt)
                .Take(count)
                .ToListAsync();
        }


        // =====================================================
        // 3. AUTHORIZATION STATUS AND CHANNEL
        // =====================================================

        // Retrieves card transactions matching a particular authorization status.
        // Useful for monitoring authorized, declined, reversed, settled and expired transactions.
        public async Task<IEnumerable<CardTransaction>> GetByAuthorizationStatusAsync(string authorizationStatus)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AuthorizationStatus == authorizationStatus)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves declined card transactions.
        // Useful for investigating unsuccessful card payment attempts.
        public async Task<IEnumerable<CardTransaction>> GetDeclinedCardTransactionsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AuthorizationStatus == "Declined")
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves card transactions made through a particular channel.
        // Useful for separating POS, ATM, ecommerce, contactless and recurring activity.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsByChannelAsync(string channel)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Channel == channel)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves card transactions within a specified authorization date range.
        // Useful for transaction monitoring and operational reporting.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.AuthorizedAt >= startDate && x.AuthorizedAt <= endDate)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }


        // =====================================================
        // 4. MERCHANT AND CURRENCY INFORMATION
        // =====================================================

        // Retrieves card transactions associated with a merchant category code.
        // Useful for analyzing spending by merchant category.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsByMerchantCategoryCodeAsync(string merchantCategoryCode)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.MerchantCategoryCode == merchantCategoryCode)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves card transactions associated with a merchant name.
        // Useful for investigating payments made to a particular merchant.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsByMerchantNameAsync(string merchantName)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.MerchantName == merchantName)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves card transactions recorded in a particular currency.
        // Useful for reviewing card activity by transaction currency.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsByCurrencyIdAsync(int currencyId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CurrencyId == currencyId)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }


        // =====================================================
        // 5. SETTLEMENT MANAGEMENT
        // =====================================================

        // Retrieves authorized card transactions that have not yet been settled.
        // Useful for identifying transactions awaiting settlement processing.
        public async Task<IEnumerable<CardTransaction>> GetUnsettledCardTransactionsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AuthorizationStatus == "Authorized" && x.SettledAt == null)
                .OrderBy(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Retrieves settled card transactions within a specified date range.
        // Useful for settlement reconciliation and financial reporting.
        public async Task<IEnumerable<CardTransaction>> GetSettledCardTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.AuthorizationStatus == "Settled" &&
                    x.SettledAt >= startDate &&
                    x.SettledAt <= endDate)
                .OrderByDescending(x => x.SettledAt)
                .ToListAsync();
        }

        // Calculates the total settlement amount for a card within a date range.
        // Useful for reviewing the card's settled transaction value.
        public async Task<decimal> GetTotalSettlementAmountByCardIdAsync(int cardId, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet
                .Where(x =>
                    x.CardId == cardId &&
                    x.AuthorizationStatus == "Settled" &&
                    x.SettledAt >= startDate &&
                    x.SettledAt <= endDate)
                .SumAsync(x => (decimal?)x.SettlementAmount) ?? 0m;
        }


        // =====================================================
        // 6. DISPUTES AND VALIDATION
        // =====================================================

        // Retrieves card transactions that have associated disputes.
        // Useful for reviewing disputed card activity.
        public async Task<IEnumerable<CardTransaction>> GetCardTransactionsWithDisputesAsync(int cardId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CardId == cardId && x.Disputes.Any())
                .Include(x => x.Disputes)
                .OrderByDescending(x => x.AuthorizedAt)
                .ToListAsync();
        }

        // Checks whether a retrieval reference number already exists.
        // Useful for preventing duplicate card-network transaction references.
        public async Task<bool> ExistsByRetrievalReferenceNumberAsync(string retrievalReferenceNumber)
        {
            return await _dbSet.AnyAsync(x => x.RetrievalReferenceNumber == retrievalReferenceNumber);
        }

        // Searches card transactions by merchant, reference number, status and channel.
        // Pagination prevents loading large card transaction histories at once.
        public async Task<IEnumerable<CardTransaction>> SearchCardTransactionsAsync(int? cardId, string? searchTerm, string? authorizationStatus, string? channel, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            if (startDate.HasValue && endDate.HasValue && endDate.Value < startDate.Value)
                throw new ArgumentException("End date cannot be earlier than start date.");

            var query = _dbSet.AsNoTracking().AsQueryable();

            if (cardId.HasValue)
                query = query.Where(x => x.CardId == cardId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(x =>
                    x.MerchantName.Contains(searchTerm) ||
                    (x.AuthorizationCode != null && x.AuthorizationCode.Contains(searchTerm)) ||
                    (x.RetrievalReferenceNumber != null && x.RetrievalReferenceNumber.Contains(searchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(authorizationStatus))
                query = query.Where(x => x.AuthorizationStatus == authorizationStatus.Trim());

            if (!string.IsNullOrWhiteSpace(channel))
                query = query.Where(x => x.Channel == channel.Trim());

            if (startDate.HasValue)
                query = query.Where(x => x.AuthorizedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(x => x.AuthorizedAt <= endDate.Value);

            return await query.OrderByDescending(x => x.AuthorizedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
