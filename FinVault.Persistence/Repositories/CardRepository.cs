
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class CardRepository : GenericRepository<Card>, ICardRepository
    {
        public CardRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =====================================================
        // 1. CARD IDENTIFICATION
        // =====================================================

        // Finds a card using its unique ID and customer ownership.
        // Useful for ensuring customers can access only their own cards.
        public async Task<Card?> GetByIdAndCustomerIdAsync(int cardId, int customerId)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CardId == cardId && x.CustomerId == customerId);
        }

        // Retrieves a card with its account, customer and transaction details.
        // Useful for reviewing complete card information.
        public async Task<Card?> GetCardDetailsAsync(int cardId)
        {
            return await _dbSet.AsNoTracking()
                .Include(x => x.Account)
                .Include(x => x.Customer)
                .Include(x => x.CardTransactions)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.CardId == cardId);
        }

        // Finds a card using its masked card number and customer ownership.
        // Useful for locating a card without exposing its full card number or vault token.
        public async Task<Card?> GetByMaskedNumberAsync(int customerId, string cardNumberMasked)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.CardNumberMasked == cardNumberMasked);
        }


        // =====================================================
        // 2. CUSTOMER AND ACCOUNT CARD MANAGEMENT
        // =====================================================

        // Retrieves all cards belonging to a customer.
        // Useful for displaying the customer's complete card portfolio.
        public async Task<IEnumerable<Card>> GetCardsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves all cards linked to a particular account.
        // Useful for reviewing cards associated with a bank account.
        public async Task<IEnumerable<Card>> GetCardsByAccountIdAsync(int accountId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves active cards belonging to a customer.
        // Useful for displaying cards currently available for transactions.
        public async Task<IEnumerable<Card>> GetActiveCardsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.Status == "Active")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves cards matching a particular status.
        // Useful for monitoring pending, blocked, hotlisted, expired and cancelled cards.
        public async Task<IEnumerable<Card>> GetCardsByStatusAsync(string status)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 3. CARD TYPE AND NETWORK
        // =====================================================

        // Retrieves cards belonging to a particular card type.
        // Useful for separating debit, credit and prepaid cards.
        public async Task<IEnumerable<Card>> GetCardsByTypeAsync(int customerId, string cardType)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.CardType == cardType)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves cards issued on a particular payment network.
        // Useful for monitoring RuPay, Visa and Mastercard card portfolios.
        public async Task<IEnumerable<Card>> GetCardsByNetworkAsync(string network)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Network == network)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves credit cards belonging to a customer.
        // Useful for reviewing the customer's credit card facilities.
        public async Task<IEnumerable<Card>> GetCreditCardsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.CardType == "Credit")
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }


        // =====================================================
        // 4. CARD EXPIRY AND LIFECYCLE
        // =====================================================

        // Retrieves cards expiring within a specified date range.
        // Useful for notifying customers about upcoming card renewals.
        public async Task<IEnumerable<Card>> GetCardsExpiringWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be earlier than start date.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.ExpiryDate >= startDate && x.ExpiryDate <= endDate && x.Status == "Active")
                .OrderBy(x => x.ExpiryDate)
                .ToListAsync();
        }

        // Retrieves cards whose expiry date has passed.
        // Useful for identifying cards requiring expiry processing.
        public async Task<IEnumerable<Card>> GetExpiredCardsAsync(DateTime currentDate)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.ExpiryDate < currentDate && x.Status != "Cancelled" && x.Status != "Expired")
                .OrderBy(x => x.ExpiryDate)
                .ToListAsync();
        }

        // Retrieves cards that are pending activation.
        // Useful for monitoring newly issued cards awaiting activation.
        public async Task<IEnumerable<Card>> GetPendingCardsAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.Status == "Pending")
                .OrderBy(x => x.IssuedAt)
                .ToListAsync();
        }


        // =====================================================
        // 5. CARD CONTROLS AND SECURITY
        // =====================================================

        // Retrieves cards with PIN verification temporarily blocked.
        // Useful for monitoring cards affected by PIN retry restrictions.
        public async Task<IEnumerable<Card>> GetCardsWithBlockedPinAsync(DateTime currentDateTime)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.PinBlockedUntil.HasValue && x.PinBlockedUntil.Value > currentDateTime)
                .OrderBy(x => x.PinBlockedUntil)
                .ToListAsync();
        }

        // Retrieves cards with at least one disabled transaction channel.
        // Useful for reviewing cards with restricted ATM, POS or online usage.
        public async Task<IEnumerable<Card>> GetCardsWithRestrictedChannelsAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.CustomerId == customerId &&
                    x.Status == "Active" &&
                    (!x.IsContactlessEnabled || !x.IsOnlineEnabled || !x.IsAtmEnabled || !x.IsPosEnabled))
                .OrderBy(x => x.CardHolderName)
                .ToListAsync();
        }

        // Retrieves cards whose status is blocked or hotlisted.
        // Useful for monitoring cards requiring security or customer-support attention.
        public async Task<IEnumerable<Card>> GetBlockedOrHotlistedCardsAsync(int customerId)
        {
            return await _dbSet.AsNoTracking()
                .Where(x => x.CustomerId == customerId && (x.Status == "Blocked" || x.Status == "Hotlisted"))
                .OrderByDescending(x => x.BlockedAt)
                .ToListAsync();
        }


        // =====================================================
        // 6. CREDIT CARD INFORMATION
        // =====================================================

        // Retrieves credit cards with available credit below a specified threshold.
        // Useful for identifying credit cards approaching their spending limit.
        public async Task<IEnumerable<Card>> GetCreditCardsWithLowAvailableCreditAsync(decimal threshold)
        {
            if (threshold < 0)
                throw new ArgumentOutOfRangeException(nameof(threshold), "Threshold cannot be negative.");

            return await _dbSet.AsNoTracking()
                .Where(x => x.CardType == "Credit" && x.Status == "Active" && x.AvailableCredit.HasValue && x.AvailableCredit.Value <= threshold)
                .OrderBy(x => x.AvailableCredit)
                .ToListAsync();
        }


        // =====================================================
        // 7. CARD SEARCH AND VALIDATION
        // =====================================================

        // Searches cards by cardholder name or masked card number.
        // Pagination prevents loading large card lists at once.
        public async Task<IEnumerable<Card>> SearchCardsAsync(int customerId, string searchTerm, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty.", nameof(searchTerm));

            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number and page size must be greater than zero.");

            searchTerm = searchTerm.Trim();

            return await _dbSet.AsNoTracking()
                .Where(x =>
                    x.CustomerId == customerId &&
                    (x.CardHolderName.Contains(searchTerm) || x.CardNumberMasked.Contains(searchTerm)))
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Checks whether a customer has an active card linked to an account.
        // Useful for validating card availability for account-related operations.
        public async Task<bool> HasActiveCardForAccountAsync(int customerId, int accountId)
        {
            return await _dbSet.AnyAsync(x =>
                x.CustomerId == customerId &&
                x.AccountId == accountId &&
                x.Status == "Active");
        }
    }
}
