
using FinVault.Application.DTOs.CardDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICardService
    {

        // 1. CRUD AND CARD IDENTIFICATION


        Task<IEnumerable<CardDto>> GetAllAsync();

        Task<CardDto?> GetByIdAsync(int cardId);

        Task<CardDto?> GetByIdAndCustomerIdAsync(int cardId, int customerId);

        Task<CardDto?> GetByMaskedNumberAsync(int customerId, string maskedCardNumber);

        Task<CardDto?> GetCardDetailsAsync(int cardId);

        Task<CardDto> CreateAsync(CreateCardDto dto);

        Task<CardDto?> UpdateAsync(UpdateCardDto dto);

        Task<bool> DeleteAsync(int cardId);



        // 2. CUSTOMER AND ACCOUNT CARD MANAGEMENT


        Task<IEnumerable<CardDto>> GetCardsByCustomerIdAsync(int customerId);

        Task<IEnumerable<CardDto>> GetCardsByAccountIdAsync(int accountId);

        Task<IEnumerable<CardDto>> GetActiveCardsByCustomerIdAsync(int customerId);

        Task<IEnumerable<CardDto>> GetCardsByStatusAsync(string status);

        Task<bool> HasActiveCardForAccountAsync(int customerId, int accountId);



        // 3. CARD TYPE AND NETWORK


        Task<IEnumerable<CardDto>> GetCardsByTypeAsync(int customerId, string cardType);

        Task<IEnumerable<CardDto>> GetCardsByNetworkAsync(string network);

        Task<IEnumerable<CardDto>> GetCreditCardsByCustomerIdAsync(int customerId);



        // 4. CARD ISSUANCE AND LIFECYCLE


        Task<bool> ActivateCardAsync(int cardId, int customerId);

        Task<bool> BlockCardAsync(int cardId, int customerId, string reason);

        Task<bool> UnblockCardAsync(int cardId, int customerId);

        Task<bool> HotlistCardAsync(int cardId, int customerId, string reason);

        Task<bool> CancelCardAsync(int cardId, int customerId, string reason);

        Task<bool> RenewCardAsync(int cardId, DateTime newExpiryDate);

        Task<IEnumerable<CardDto>> GetPendingCardsAsync();

        Task<IEnumerable<CardDto>> GetCardsExpiringWithinRangeAsync(DateTime startDate, DateTime endDate);

        Task<IEnumerable<CardDto>> GetExpiredCardsAsync(DateTime currentDate);



        // 5. CARD TRANSACTION CHANNEL CONTROLS


        Task<bool> UpdateContactlessStatusAsync(int cardId, int customerId, bool isEnabled);

        Task<bool> UpdateOnlineStatusAsync(int cardId, int customerId, bool isEnabled);

        Task<bool> UpdateAtmStatusAsync(int cardId, int customerId, bool isEnabled);

        Task<bool> UpdatePosStatusAsync(int cardId, int customerId, bool isEnabled);

        Task<IEnumerable<CardDto>> GetCardsWithRestrictedChannelsAsync(int customerId);

        Task<IEnumerable<CardDto>> GetBlockedOrHotlistedCardsAsync(int customerId);



        // 6. SPENDING LIMIT MANAGEMENT


        Task<bool> UpdateDailyAtmLimitAsync(int cardId, decimal limit);

        Task<bool> UpdateDailyPosLimitAsync(int cardId, decimal limit);

        Task<bool> UpdateDailyOnlineLimitAsync(int cardId, decimal limit);

        Task<bool> IsWithinDailyAtmLimitAsync(int cardId, decimal amount);

        Task<bool> IsWithinDailyPosLimitAsync(int cardId, decimal amount);

        Task<bool> IsWithinDailyOnlineLimitAsync(int cardId, decimal amount);



        // 7. CREDIT CARD MANAGEMENT


        Task<bool> UpdateCreditLimitAsync(int cardId, decimal creditLimit);

        Task<decimal?> GetAvailableCreditAsync(int cardId);

        Task<decimal?> GetOutstandingAmountAsync(int cardId);

        Task<IEnumerable<CardDto>> GetCreditCardsWithLowAvailableCreditAsync(decimal threshold);

        Task<bool> HasSufficientAvailableCreditAsync(int cardId, decimal amount);



        // 8. PIN SECURITY


        Task<bool> RecordPinFailureAsync(int cardId);

        Task<bool> ResetPinRetryCountAsync(int cardId);

        Task<bool> IsPinBlockedAsync(int cardId, DateTime currentDateTime);

        Task<IEnumerable<CardDto>> GetCardsWithBlockedPinAsync(DateTime currentDateTime);

        Task<bool> UpdatePinLastSetAtAsync(int cardId, DateTime pinLastSetAt);



        // 9. SEARCH AND FILTER


        Task<IEnumerable<CardDto>> SearchCardsAsync(int customerId, string searchTerm, int pageNumber, int pageSize);



        // 10. VALIDATION AND ELIGIBILITY


        Task<bool> IsCardActiveAsync(int cardId);

        Task<bool> IsCardOwnedByCustomerAsync(int cardId, int customerId);

        Task<bool> IsCardEligibleForTransactionsAsync(int cardId);

        Task<bool> CanCardBeDeletedAsync(int cardId);
    }
}
