
using FinVault.Application.DTOs.CardTransactionDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface ICardTransactionService
    {

        // 1. CRUD AND TRANSACTION IDENTIFICATION


        Task<IEnumerable<CardTransactionDto>> GetAllAsync();

        Task<CardTransactionDto?> GetByIdAsync(int cardTransactionId);

        Task<CardTransactionDto?> GetByRetrievalReferenceNumberAsync(string retrievalReferenceNumber);

        Task<CardTransactionDto?> GetByAuthorizationCodeAsync(string authorizationCode);

        Task<CardTransactionDto?> GetCardTransactionDetailsAsync(int cardTransactionId);

        Task<CardTransactionDto> CreateAsync(CreateCardTransactionDto dto);

        Task<CardTransactionDto?> UpdateAsync(UpdateCardTransactionDto dto);

        Task<bool> DeleteAsync(int cardTransactionId);



        // 2. CARD AND ACCOUNT TRANSACTION HISTORY


        Task<IEnumerable<CardTransactionDto>> GetByCardIdAsync(int cardId);

        Task<IEnumerable<CardTransactionDto>> GetByTransactionIdAsync(int transactionId);

        Task<IEnumerable<CardTransactionDto>> GetRecentTransactionsAsync(int cardId, int count);

        Task<IEnumerable<CardTransactionDto>> GetTransactionsByCustomerAsync(int customerId);



        // 3. AUTHORIZATION STATUS AND CHANNEL


        Task<IEnumerable<CardTransactionDto>> GetByAuthorizationStatusAsync(string authorizationStatus);

        Task<IEnumerable<CardTransactionDto>> GetDeclinedTransactionsAsync();

        Task<IEnumerable<CardTransactionDto>> GetByChannelAsync(string channel);

        Task<IEnumerable<CardTransactionDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<bool> UpdateAuthorizationStatusAsync(int cardTransactionId, string authorizationStatus);

        Task<bool> ReverseTransactionAsync(int cardTransactionId, string reason);



        // 4. MERCHANT AND CURRENCY ANALYSIS


        Task<IEnumerable<CardTransactionDto>> GetByMerchantCategoryCodeAsync(string merchantCategoryCode);

        Task<IEnumerable<CardTransactionDto>> GetByMerchantNameAsync(string merchantName);

        Task<IEnumerable<CardTransactionDto>> GetByCurrencyIdAsync(int currencyId);



        // 5. SETTLEMENT MANAGEMENT


        Task<IEnumerable<CardTransactionDto>> GetUnsettledTransactionsAsync();

        Task<IEnumerable<CardTransactionDto>> GetSettledTransactionsAsync(DateTime startDate, DateTime endDate);

        Task<bool> SettleTransactionAsync(int cardTransactionId, decimal settlementAmount, DateTime settledAt);

        Task<decimal> GetTotalSettlementAmountByCardIdAsync(int cardId, DateTime startDate, DateTime endDate);



        // 6. DISPUTE AND INVESTIGATION


        Task<IEnumerable<CardTransactionDto>> GetTransactionsWithDisputesAsync(int cardId);

        Task<bool> HasActiveDisputeAsync(int cardTransactionId);

        Task<bool> IsTransactionDisputableAsync(int cardTransactionId);



        // 7. SEARCH AND FILTER


        Task<IEnumerable<CardTransactionDto>> SearchAsync(
            int? cardId,
            string? searchTerm,
            string? authorizationStatus,
            string? channel,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber,
            int pageSize);



        // 8. VALIDATION AND EXISTENCE


        Task<bool> IsRetrievalReferenceNumberExistsAsync(string retrievalReferenceNumber);

        Task<bool> IsCardTransactionValidAsync(int cardTransactionId);

        Task<bool> IsTransactionAlreadySettledAsync(int cardTransactionId);

        Task<bool> CanTransactionBeReversedAsync(int cardTransactionId);
    }
}
