
using FinVault.Application.DTOs.CardDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    // ==================== CARD IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _cardService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetById(int cardId)
    {
        var data = await _cardService.GetByIdAsync(cardId);

        return data is null
            ? NotFound(ApiResponse<CardDto>.FailureResponse("Card not found.", 404))
            : Ok(ApiResponse<CardDto>.SuccessResponse(data));
    }

    [HttpGet("{cardId:int}/customer/{customerId:int}")]
    public async Task<IActionResult> GetByIdAndCustomerId(int cardId, int customerId)
    {
        var data = await _cardService.GetByIdAndCustomerIdAsync(cardId, customerId);

        return data is null
            ? NotFound(ApiResponse<CardDto>.FailureResponse(
                "Card not found for the specified customer.", 404))
            : Ok(ApiResponse<CardDto>.SuccessResponse(data));
    }

    [HttpGet("{cardId:int}/details")]
    public async Task<IActionResult> GetCardDetails(int cardId)
    {
        var data = await _cardService.GetCardDetailsAsync(cardId);

        return data is null
            ? NotFound(ApiResponse<CardDto>.FailureResponse("Card details not found.", 404))
            : Ok(ApiResponse<CardDto>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/masked-number/{cardNumberMasked}")]
    public async Task<IActionResult> GetByMaskedNumber(
        int customerId,
        string cardNumberMasked)
    {
        var data = await _cardService.GetByMaskedNumberAsync(
            customerId, cardNumberMasked);

        return data is null
            ? NotFound(ApiResponse<CardDto>.FailureResponse("Card not found.", 404))
            : Ok(ApiResponse<CardDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCardDto request)
    {
        var data = await _cardService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { cardId = data.CardId },
            ApiResponse<CardDto>.SuccessResponse(
                data, "Card created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateCardDto request)
    {
        var data = await _cardService.UpdateAsync(request);

        return data is null
            ? Conflict(ApiResponse<CardDto>.FailureResponse(
                "Card could not be updated. It may not exist or its RowVersion may be outdated.", 409))
            : Ok(ApiResponse<CardDto>.SuccessResponse(
                data, "Card updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteCardDto request)
    {
        var result = await _cardService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(
                null, "Card deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Card could not be deleted.", 409));
    }

    // ==================== CUSTOMER AND ACCOUNT CARD MANAGEMENT ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetCardsByCustomerId(int customerId)
    {
        var data = await _cardService.GetCardsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}")]
    public async Task<IActionResult> GetCardsByAccountId(int accountId)
    {
        var data = await _cardService.GetCardsByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/active")]
    public async Task<IActionResult> GetActiveCardsByCustomerId(int customerId)
    {
        var data = await _cardService.GetActiveCardsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetCardsByStatus(string status)
    {
        var data = await _cardService.GetCardsByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    // ==================== CARD TYPE AND NETWORK ====================

    [HttpGet("customer/{customerId:int}/type/{cardType}")]
    public async Task<IActionResult> GetCardsByType(int customerId, string cardType)
    {
        var data = await _cardService.GetCardsByTypeAsync(customerId, cardType);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("network/{network}")]
    public async Task<IActionResult> GetCardsByNetwork(string network)
    {
        var data = await _cardService.GetCardsByNetworkAsync(network);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/credit")]
    public async Task<IActionResult> GetCreditCardsByCustomerId(int customerId)
    {
        var data = await _cardService.GetCreditCardsByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    // ==================== CARD EXPIRY AND LIFECYCLE ====================

    [HttpGet("expiring")]
    public async Task<IActionResult> GetCardsExpiringWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default ||
            startDate.Date > endDate.Date)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid expiry date range is required.", 400));
        }

        var data = await _cardService.GetCardsExpiringWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredCards(
        [FromQuery] DateTime currentDate)
    {
        if (currentDate == default)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid current date is required.", 400));

        var data = await _cardService.GetExpiredCardsAsync(currentDate);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingCards()
    {
        var data = await _cardService.GetPendingCardsAsync();
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    // ==================== CARD CONTROLS AND SECURITY ====================

    [HttpGet("pin-blocked")]
    public async Task<IActionResult> GetCardsWithBlockedPin(
        [FromQuery] DateTime currentDateTime)
    {
        if (currentDateTime == default)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid current date and time is required.", 400));

        var data = await _cardService.GetCardsWithBlockedPinAsync(currentDateTime);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/restricted-channels")]
    public async Task<IActionResult> GetCardsWithRestrictedChannels(int customerId)
    {
        var data = await _cardService.GetCardsWithRestrictedChannelsAsync(customerId);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/blocked-or-hotlisted")]
    public async Task<IActionResult> GetBlockedOrHotlistedCards(int customerId)
    {
        var data = await _cardService.GetBlockedOrHotlistedCardsAsync(customerId);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    // ==================== CREDIT CARD INFORMATION ====================

    [HttpGet("credit/low-available-credit")]
    public async Task<IActionResult> GetCreditCardsWithLowAvailableCredit(
        [FromQuery] decimal threshold)
    {
        if (threshold < 0)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Threshold cannot be negative.", 400));

        var data = await _cardService.GetCreditCardsWithLowAvailableCreditAsync(threshold);
        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    // ==================== CARD SEARCH AND VALIDATION ====================

    [HttpGet("customer/{customerId:int}/search")]
    public async Task<IActionResult> SearchCards(
        int customerId,
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        if (pageNumber < 1 || pageSize < 1 || pageSize > 200)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Page number must be positive and page size must be between 1 and 200.", 400));

        var data = await _cardService.SearchCardsAsync(
            customerId, searchTerm, pageNumber, pageSize);

        return Ok(ApiResponse<IEnumerable<CardDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/account/{accountId:int}/has-active")]
    public async Task<IActionResult> HasActiveCardForAccount(
        int customerId,
        int accountId)
    {
        var result = await _cardService.HasActiveCardForAccountAsync(
            customerId, accountId);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }
}
