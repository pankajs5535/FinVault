using FinVault.Application.DTOs.CardDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICardService
    {
        // 1. CARD IDENTIFICATION
        Task<IEnumerable<CardDto>> GetAllAsync();
        Task<CardDto?> GetByIdAsync(int cardId);
        Task<CardDto?> GetByIdAndCustomerIdAsync(int cardId, int customerId);
        Task<CardDto?> GetCardDetailsAsync(int cardId);
        Task<CardDto?> GetByMaskedNumberAsync(int customerId, string cardNumberMasked);
        Task<CardDto> CreateAsync(CreateCardDto dto);
        Task<CardDto?> UpdateAsync(UpdateCardDto dto);
        Task<bool> DeleteAsync(DeleteCardDto dto);

        // 2. CUSTOMER AND ACCOUNT CARD MANAGEMENT
        Task<IEnumerable<CardDto>> GetCardsByCustomerIdAsync(int customerId);
        Task<IEnumerable<CardDto>> GetCardsByAccountIdAsync(int accountId);
        Task<IEnumerable<CardDto>> GetActiveCardsByCustomerIdAsync(int customerId);
        Task<IEnumerable<CardDto>> GetCardsByStatusAsync(string status);

        // 3. CARD TYPE AND NETWORK
        Task<IEnumerable<CardDto>> GetCardsByTypeAsync(int customerId, string cardType);
        Task<IEnumerable<CardDto>> GetCardsByNetworkAsync(string network);
        Task<IEnumerable<CardDto>> GetCreditCardsByCustomerIdAsync(int customerId);

        // 4. CARD EXPIRY AND LIFECYCLE
        Task<IEnumerable<CardDto>> GetCardsExpiringWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<CardDto>> GetExpiredCardsAsync(DateTime currentDate);
        Task<IEnumerable<CardDto>> GetPendingCardsAsync();

        // 5. CARD CONTROLS AND SECURITY
        Task<IEnumerable<CardDto>> GetCardsWithBlockedPinAsync(DateTime currentDateTime);
        Task<IEnumerable<CardDto>> GetCardsWithRestrictedChannelsAsync(int customerId);
        Task<IEnumerable<CardDto>> GetBlockedOrHotlistedCardsAsync(int customerId);

        // 6. CREDIT CARD INFORMATION
        Task<IEnumerable<CardDto>> GetCreditCardsWithLowAvailableCreditAsync(decimal threshold);

        // 7. CARD SEARCH AND VALIDATION
        Task<IEnumerable<CardDto>> SearchCardsAsync(
            int customerId,
            string searchTerm,
            int pageNumber,
            int pageSize);

        Task<bool> HasActiveCardForAccountAsync(int customerId, int accountId);
    }
}


//Banking security rules:
//-Never expose the full card number, CVV, PIN, or CardNumberToken in DTOs.
//- Store only the masked card number and secure vault/network token.
//- Enforce customer ownership before card access or modification.
//- Use RowVersion for concurrency checks during updates.
//- Validate card status, expiry, and transaction-channel restrictions.