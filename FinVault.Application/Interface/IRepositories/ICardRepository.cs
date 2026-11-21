
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface ICardRepository : IGenericRepository<Card>
    {
        // =====================================================
        // 1. CARD IDENTIFICATION
        // =====================================================

        // Finds a card using its unique ID and customer ownership.
        // Useful for ensuring customers can access only their own cards.
        Task<Card?> GetByIdAndCustomerIdAsync(int cardId, int customerId);

        // Retrieves a card with its account, customer and transaction details.
        // Useful for reviewing complete card information.
        Task<Card?> GetCardDetailsAsync(int cardId);

        // Finds a card using its masked card number and customer ownership.
        // Useful for locating a card without exposing its full card number or vault token.
        Task<Card?> GetByMaskedNumberAsync(int customerId, string cardNumberMasked);


        // =====================================================
        // 2. CUSTOMER AND ACCOUNT CARD MANAGEMENT
        // =====================================================

        // Retrieves all cards belonging to a customer.
        // Useful for displaying the customer's complete card portfolio.
        Task<IEnumerable<Card>> GetCardsByCustomerIdAsync(int customerId);

        // Retrieves all cards linked to a particular account.
        // Useful for reviewing cards associated with a bank account.
        Task<IEnumerable<Card>> GetCardsByAccountIdAsync(int accountId);

        // Retrieves active cards belonging to a customer.
        // Useful for displaying cards currently available for transactions.
        Task<IEnumerable<Card>> GetActiveCardsByCustomerIdAsync(int customerId);

        // Retrieves cards matching a particular status.
        // Useful for monitoring pending, blocked, hotlisted, expired and cancelled cards.
        Task<IEnumerable<Card>> GetCardsByStatusAsync(string status);


        // =====================================================
        // 3. CARD TYPE AND NETWORK
        // =====================================================

        // Retrieves cards belonging to a particular card type.
        // Useful for separating debit, credit and prepaid cards.
        Task<IEnumerable<Card>> GetCardsByTypeAsync(int customerId, string cardType);

        // Retrieves cards issued on a particular payment network.
        // Useful for monitoring RuPay, Visa and Mastercard card portfolios.
        Task<IEnumerable<Card>> GetCardsByNetworkAsync(string network);

        // Retrieves credit cards belonging to a customer.
        // Useful for reviewing the customer's credit card facilities.
        Task<IEnumerable<Card>> GetCreditCardsByCustomerIdAsync(int customerId);


        // =====================================================
        // 4. CARD EXPIRY AND LIFECYCLE
        // =====================================================

        // Retrieves cards expiring within a specified date range.
        // Useful for notifying customers about upcoming card renewals.
        Task<IEnumerable<Card>> GetCardsExpiringWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves cards whose expiry date has passed.
        // Useful for identifying cards requiring expiry processing.
        Task<IEnumerable<Card>> GetExpiredCardsAsync(DateTime currentDate);

        // Retrieves cards that are pending activation.
        // Useful for monitoring newly issued cards awaiting activation.
        Task<IEnumerable<Card>> GetPendingCardsAsync();


        // =====================================================
        // 5. CARD CONTROLS AND SECURITY
        // =====================================================

        // Retrieves cards with PIN verification temporarily blocked.
        // Useful for monitoring cards affected by PIN retry restrictions.
        Task<IEnumerable<Card>> GetCardsWithBlockedPinAsync(DateTime currentDateTime);

        // Retrieves cards with at least one disabled transaction channel.
        // Useful for reviewing cards with restricted ATM, POS or online usage.
        Task<IEnumerable<Card>> GetCardsWithRestrictedChannelsAsync(int customerId);

        // Retrieves cards whose status is blocked or hotlisted.
        // Useful for monitoring cards requiring security or customer-support attention.
        Task<IEnumerable<Card>> GetBlockedOrHotlistedCardsAsync(int customerId);


        // =====================================================
        // 6. CREDIT CARD INFORMATION
        // =====================================================

        // Retrieves credit cards with available credit below a specified threshold.
        // Useful for identifying credit cards approaching their spending limit.
        Task<IEnumerable<Card>> GetCreditCardsWithLowAvailableCreditAsync(decimal threshold);


        // =====================================================
        // 7. CARD SEARCH AND VALIDATION
        // =====================================================

        // Searches cards by cardholder name or masked card number.
        // Pagination prevents loading large card lists at once.
        Task<IEnumerable<Card>> SearchCardsAsync(int customerId, string searchTerm, int pageNumber, int pageSize);

        // Checks whether a customer has an active card linked to an account.
        // Useful for validating card availability for account-related operations.
        Task<bool> HasActiveCardForAccountAsync(int customerId, int accountId);
    }
}
