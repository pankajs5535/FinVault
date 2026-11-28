
using FinVault.Application.DTOs.CardTransactionDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CardTransactionController : ControllerBase
{
    private readonly ICardTransactionService _cardTransactionService;

    public CardTransactionController(ICardTransactionService cardTransactionService)
    {
        _cardTransactionService = cardTransactionService;
    }

    // ==================== TRANSACTION IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _cardTransactionService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("{cardTransactionId:int}")]
    public async Task<IActionResult> GetById(int cardTransactionId)
    {
        var data = await _cardTransactionService.GetByIdAsync(cardTransactionId);

        return data is null
            ? NotFound(ApiResponse<CardTransactionDto>.FailureResponse("Card transaction not found.", 404))
            : Ok(ApiResponse<CardTransactionDto>.SuccessResponse(data));
    }

    [HttpGet("retrieval-reference/{retrievalReferenceNumber}")]
    public async Task<IActionResult> GetByRetrievalReferenceNumber(string retrievalReferenceNumber)
    {
        var data = await _cardTransactionService.GetByRetrievalReferenceNumberAsync(
            retrievalReferenceNumber);

        return data is null
            ? NotFound(ApiResponse<CardTransactionDto>.FailureResponse("Card transaction not found.", 404))
            : Ok(ApiResponse<CardTransactionDto>.SuccessResponse(data));
    }

    [HttpGet("authorization-code/{authorizationCode}")]
    public async Task<IActionResult> GetByAuthorizationCode(string authorizationCode)
    {
        var data = await _cardTransactionService.GetByAuthorizationCodeAsync(authorizationCode);

        return data is null
            ? NotFound(ApiResponse<CardTransactionDto>.FailureResponse("Card transaction not found.", 404))
            : Ok(ApiResponse<CardTransactionDto>.SuccessResponse(data));
    }

    [HttpGet("{cardTransactionId:int}/details")]
    public async Task<IActionResult> GetCardTransactionDetails(int cardTransactionId)
    {
        var data = await _cardTransactionService.GetCardTransactionDetailsAsync(
            cardTransactionId);

        return data is null
            ? NotFound(ApiResponse<CardTransactionDto>.FailureResponse(
                "Card transaction details not found.", 404))
            : Ok(ApiResponse<CardTransactionDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardTransactionDto request)
    {
        var data = await _cardTransactionService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { cardTransactionId = data.CardTransactionId },
            ApiResponse<CardTransactionDto>.SuccessResponse(
                data, "Card transaction recorded successfully.", 201));
    }

    // ==================== CARD AND ACCOUNT TRANSACTION HISTORY ====================

    [HttpGet("card/{cardId:int}")]
    public async Task<IActionResult> GetCardTransactionsByCardId(int cardId)
    {
        var data = await _cardTransactionService.GetCardTransactionsByCardIdAsync(cardId);
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("transaction/{transactionId:int}")]
    public async Task<IActionResult> GetCardTransactionsByTransactionId(int transactionId)
    {
        var data = await _cardTransactionService.GetCardTransactionsByTransactionIdAsync(
            transactionId);

        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("card/{cardId:int}/recent")]
    public async Task<IActionResult> GetRecentCardTransactions(
        int cardId,
        [FromQuery] int count = 10)
    {
        if (count < 1 || count > 200)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be between 1 and 200.", 400));

        var data = await _cardTransactionService.GetRecentCardTransactionsAsync(cardId, count);
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    // ==================== AUTHORIZATION STATUS AND CHANNEL ====================

    [HttpGet("authorization-status/{authorizationStatus}")]
    public async Task<IActionResult> GetByAuthorizationStatus(string authorizationStatus)
    {
        var data = await _cardTransactionService.GetByAuthorizationStatusAsync(
            authorizationStatus);

        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("declined")]
    public async Task<IActionResult> GetDeclinedCardTransactions()
    {
        var data = await _cardTransactionService.GetDeclinedCardTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("channel/{channel}")]
    public async Task<IActionResult> GetCardTransactionsByChannel(string channel)
    {
        var data = await _cardTransactionService.GetCardTransactionsByChannelAsync(channel);
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("date-range")]
    public async Task<IActionResult> GetCardTransactionsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _cardTransactionService.GetCardTransactionsByDateRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    // ==================== MERCHANT AND CURRENCY INFORMATION ====================

    [HttpGet("merchant-category/{merchantCategoryCode}")]
    public async Task<IActionResult> GetCardTransactionsByMerchantCategoryCode(
        string merchantCategoryCode)
    {
        var data = await _cardTransactionService.GetCardTransactionsByMerchantCategoryCodeAsync(
            merchantCategoryCode);

        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("merchant/{merchantName}")]
    public async Task<IActionResult> GetCardTransactionsByMerchantName(string merchantName)
    {
        var data = await _cardTransactionService.GetCardTransactionsByMerchantNameAsync(
            merchantName);

        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("currency/{currencyId:int}")]
    public async Task<IActionResult> GetCardTransactionsByCurrencyId(int currencyId)
    {
        var data = await _cardTransactionService.GetCardTransactionsByCurrencyIdAsync(currencyId);
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    // ==================== SETTLEMENT MANAGEMENT ====================

    [HttpGet("unsettled")]
    public async Task<IActionResult> GetUnsettledCardTransactions()
    {
        var data = await _cardTransactionService.GetUnsettledCardTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("settled")]
    public async Task<IActionResult> GetSettledCardTransactions(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid settlement date range is required.", 400));

        var data = await _cardTransactionService.GetSettledCardTransactionsAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("card/{cardId:int}/settlement-total")]
    public async Task<IActionResult> GetTotalSettlementAmountByCardId(
        int cardId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid settlement date range is required.", 400));

        var total = await _cardTransactionService.GetTotalSettlementAmountByCardIdAsync(
            cardId, startDate, endDate);

        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    // ==================== DISPUTES AND VALIDATION ====================

    [HttpGet("card/{cardId:int}/disputes")]
    public async Task<IActionResult> GetCardTransactionsWithDisputes(int cardId)
    {
        var data = await _cardTransactionService.GetCardTransactionsWithDisputesAsync(cardId);
        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("exists/retrieval-reference/{retrievalReferenceNumber}")]
    public async Task<IActionResult> ExistsByRetrievalReferenceNumber(
        string retrievalReferenceNumber)
    {
        var result = await _cardTransactionService.ExistsByRetrievalReferenceNumberAsync(
            retrievalReferenceNumber);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== SEARCH ====================

    [HttpGet("search")]
    public async Task<IActionResult> SearchCardTransactions(
        [FromQuery] int? cardId,
        [FromQuery] string? searchTerm,
        [FromQuery] string? authorizationStatus,
        [FromQuery] string? channel,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 200)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 200.", 400));

        if (startDate.HasValue != endDate.HasValue)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Both start date and end date must be provided together.", 400));

        if (startDate.HasValue && endDate.HasValue && endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "End date cannot precede start date.", 400));

        var data = await _cardTransactionService.SearchCardTransactionsAsync(
            cardId,
            searchTerm,
            authorizationStatus,
            channel,
            startDate,
            endDate,
            pageNumber,
            pageSize);

        return Ok(ApiResponse<IEnumerable<CardTransactionDto>>.SuccessResponse(data));
    }
}
