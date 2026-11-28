
using FinVault.Application.DTOs.JournalEntryDtos;
using FinVault.Application.DTOs.JournalEntryLineDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class JournalEntryController : ControllerBase
{
    private readonly IJournalEntryService _journalEntryService;

    public JournalEntryController(IJournalEntryService journalEntryService)
    {
        _journalEntryService = journalEntryService;
    }

    // ==================== JOURNAL ENTRY IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _journalEntryService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("{journalEntryId:int}")]
    public async Task<IActionResult> GetById(int journalEntryId)
    {
        var data = await _journalEntryService.GetByIdAsync(journalEntryId);

        return data is null
            ? NotFound(ApiResponse<JournalEntryDto>.FailureResponse("Journal entry not found.", 404))
            : Ok(ApiResponse<JournalEntryDto>.SuccessResponse(data));
    }

    [HttpGet("number/{journalNumber}")]
    public async Task<IActionResult> GetByJournalNumber(string journalNumber)
    {
        var data = await _journalEntryService.GetByJournalNumberAsync(journalNumber);

        return data is null
            ? NotFound(ApiResponse<JournalEntryDto>.FailureResponse("Journal entry not found.", 404))
            : Ok(ApiResponse<JournalEntryDto>.SuccessResponse(data));
    }

    [HttpGet("{journalEntryId:int}/details")]
    public async Task<IActionResult> GetJournalEntryDetails(int journalEntryId)
    {
        var data = await _journalEntryService.GetJournalEntryDetailsAsync(journalEntryId);

        return data is null
            ? NotFound(ApiResponse<JournalEntryDto>.FailureResponse("Journal entry details not found.", 404))
            : Ok(ApiResponse<JournalEntryDto>.SuccessResponse(data));
    }

    [HttpGet("exists/number/{journalNumber}")]
    public async Task<IActionResult> ExistsByJournalNumber(string journalNumber)
    {
        var result = await _journalEntryService.ExistsByJournalNumberAsync(journalNumber);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJournalEntryDto request)
    {
        var data = await _journalEntryService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { journalEntryId = data.JournalEntryId },
            ApiResponse<JournalEntryDto>.SuccessResponse(
                data, "Journal entry created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateJournalEntryDto request)
    {
        var data = await _journalEntryService.UpdateAsync(request);

        return data is null
            ? Conflict(ApiResponse<JournalEntryDto>.FailureResponse(
                "Journal entry could not be updated. It may be posted, reversed, or its RowVersion may be outdated.", 409))
            : Ok(ApiResponse<JournalEntryDto>.SuccessResponse(
                data, "Journal entry updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteJournalEntryDto request)
    {
        var result = await _journalEntryService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Journal entry deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Journal entry cannot be deleted. It may contain financial records or be posted.", 409));
    }

    // ==================== JOURNAL CLASSIFICATION AND STATUS ====================

    [HttpGet("type/{journalType}")]
    public async Task<IActionResult> GetByJournalType(string journalType)
    {
        var data = await _journalEntryService.GetByJournalTypeAsync(journalType);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _journalEntryService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("draft")]
    public async Task<IActionResult> GetDraftEntries()
    {
        var data = await _journalEntryService.GetDraftEntriesAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("posted")]
    public async Task<IActionResult> GetPostedEntries()
    {
        var data = await _journalEntryService.GetPostedEntriesAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("reversed")]
    public async Task<IActionResult> GetReversedEntries()
    {
        var data = await _journalEntryService.GetReversedEntriesAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    // ==================== TRANSACTION AND CURRENCY ASSOCIATIONS ====================

    [HttpGet("transaction/{transactionId:int}")]
    public async Task<IActionResult> GetByTransactionId(int transactionId)
    {
        var data = await _journalEntryService.GetByTransactionIdAsync(transactionId);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("currency/{currencyId:int}")]
    public async Task<IActionResult> GetByCurrencyId(int currencyId)
    {
        var data = await _journalEntryService.GetByCurrencyIdAsync(currencyId);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("posted-by/{userId:int}")]
    public async Task<IActionResult> GetByPostedByUserId(int userId)
    {
        var data = await _journalEntryService.GetByPostedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    // ==================== REVERSAL MANAGEMENT ====================

    [HttpGet("{reversalEntryId:int}/original-entry")]
    public async Task<IActionResult> GetOriginalEntry(int reversalEntryId)
    {
        var data = await _journalEntryService.GetOriginalEntryAsync(reversalEntryId);

        return data is null
            ? NotFound(ApiResponse<JournalEntryDto>.FailureResponse("Original journal entry not found.", 404))
            : Ok(ApiResponse<JournalEntryDto>.SuccessResponse(data));
    }

    [HttpGet("{originalJournalEntryId:int}/reversals")]
    public async Task<IActionResult> GetReversalEntries(int originalJournalEntryId)
    {
        var data = await _journalEntryService.GetReversalEntriesAsync(originalJournalEntryId);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("with-reversals")]
    public async Task<IActionResult> GetEntriesWithReversals()
    {
        var data = await _journalEntryService.GetEntriesWithReversalsAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    // ==================== JOURNAL DATE AND PERIOD MANAGEMENT ====================

    [HttpGet("entry-date-range")]
    public async Task<IActionResult> GetByEntryDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid entry date range is required.", 400));

        var data = await _journalEntryService.GetByEntryDateRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("posted/date-range")]
    public async Task<IActionResult> GetPostedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid posted date range is required.", 400));

        var data = await _journalEntryService.GetPostedWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    // ==================== JOURNAL LINE MANAGEMENT ====================

    [HttpGet("{journalEntryId:int}/lines")]
    public async Task<IActionResult> GetLinesByJournalEntryId(int journalEntryId)
    {
        var data = await _journalEntryService.GetLinesByJournalEntryIdAsync(journalEntryId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    [HttpGet("lines/{journalEntryLineId:int}")]
    public async Task<IActionResult> GetJournalEntryLineDetails(int journalEntryLineId)
    {
        var data = await _journalEntryService.GetJournalEntryLineDetailsAsync(journalEntryLineId);

        return data is null
            ? NotFound(ApiResponse<JournalEntryLineDto>.FailureResponse("Journal entry line not found.", 404))
            : Ok(ApiResponse<JournalEntryLineDto>.SuccessResponse(data));
    }

    [HttpGet("{journalEntryId:int}/lines/number/{lineNumber:int}")]
    public async Task<IActionResult> GetLineByJournalEntryAndLineNumber(
        int journalEntryId,
        int lineNumber)
    {
        var data = await _journalEntryService.GetLineByJournalEntryAndLineNumberAsync(
            journalEntryId, lineNumber);

        return data is null
            ? NotFound(ApiResponse<JournalEntryLineDto>.FailureResponse("Journal entry line not found.", 404))
            : Ok(ApiResponse<JournalEntryLineDto>.SuccessResponse(data));
    }

    [HttpPost("lines")]
    public async Task<IActionResult> CreateLine([FromBody] CreateJournalEntryLineDto request)
    {
        var data = await _journalEntryService.CreateLineAsync(request);

        return CreatedAtAction(
            nameof(GetJournalEntryLineDetails),
            new { journalEntryLineId = data.JournalEntryLineId },
            ApiResponse<JournalEntryLineDto>.SuccessResponse(
                data, "Journal entry line created successfully.", 201));
    }

    [HttpGet("{journalEntryId:int}/lines/exists/{lineNumber:int}")]
    public async Task<IActionResult> ExistsByJournalEntryAndLineNumber(
        int journalEntryId,
        int lineNumber)
    {
        var result = await _journalEntryService.ExistsByJournalEntryAndLineNumberAsync(
            journalEntryId, lineNumber);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{journalEntryId:int}/lines/debit")]
    public async Task<IActionResult> GetDebitLinesByJournalEntryId(int journalEntryId)
    {
        var data = await _journalEntryService.GetDebitLinesByJournalEntryIdAsync(journalEntryId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    [HttpGet("{journalEntryId:int}/lines/credit")]
    public async Task<IActionResult> GetCreditLinesByJournalEntryId(int journalEntryId)
    {
        var data = await _journalEntryService.GetCreditLinesByJournalEntryIdAsync(journalEntryId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    [HttpGet("{journalEntryId:int}/lines/with-accounts")]
    public async Task<IActionResult> GetJournalLinesWithAccounts(int journalEntryId)
    {
        var data = await _journalEntryService.GetJournalLinesWithAccountsAsync(journalEntryId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    // ==================== ACCOUNT ASSOCIATIONS ====================

    [HttpGet("account/{accountId:int}/lines")]
    public async Task<IActionResult> GetLinesByAccountId(int accountId)
    {
        var data = await _journalEntryService.GetLinesByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/lines/debit")]
    public async Task<IActionResult> GetDebitLinesByAccountId(int accountId)
    {
        var data = await _journalEntryService.GetDebitLinesByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/lines/credit")]
    public async Task<IActionResult> GetCreditLinesByAccountId(int accountId)
    {
        var data = await _journalEntryService.GetCreditLinesByAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/lines/date-range")]
    public async Task<IActionResult> GetAccountLinesByDateRange(
        int accountId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid account journal date range is required.", 400));

        var data = await _journalEntryService.GetAccountLinesByDateRangeAsync(
            accountId, startDate, endDate);

        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    // ==================== ACCOUNTING VALIDATION ====================

    [HttpGet("{journalEntryId:int}/total-debit")]
    public async Task<IActionResult> GetTotalDebitByJournalEntryId(int journalEntryId)
    {
        var total = await _journalEntryService.GetTotalDebitByJournalEntryIdAsync(journalEntryId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("{journalEntryId:int}/total-credit")]
    public async Task<IActionResult> GetTotalCreditByJournalEntryId(int journalEntryId)
    {
        var total = await _journalEntryService.GetTotalCreditByJournalEntryIdAsync(journalEntryId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("{journalEntryId:int}/is-balanced")]
    public async Task<IActionResult> IsJournalEntryBalanced(int journalEntryId)
    {
        var result = await _journalEntryService.IsJournalEntryBalancedAsync(journalEntryId);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("{journalEntryId:int}/lines/count")]
    public async Task<IActionResult> CountLinesByJournalEntryId(int journalEntryId)
    {
        var count = await _journalEntryService.CountLinesByJournalEntryIdAsync(journalEntryId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("unbalanced-drafts")]
    public async Task<IActionResult> GetUnbalancedDraftEntries()
    {
        var data = await _journalEntryService.GetUnbalancedDraftEntriesAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("without-lines")]
    public async Task<IActionResult> GetEntriesWithoutLines()
    {
        var data = await _journalEntryService.GetEntriesWithoutLinesAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    // ==================== INTEREST AND FINANCIAL REPORTING ====================

    [HttpGet("with-interest-calculations")]
    public async Task<IActionResult> GetEntriesWithInterestCalculations()
    {
        var data = await _journalEntryService.GetEntriesWithInterestCalculationsAsync();
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("currency/{currencyId:int}/total-posted-debits")]
    public async Task<IActionResult> GetTotalPostedDebits(
        int currencyId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid reporting date range is required.", 400));

        var total = await _journalEntryService.GetTotalPostedDebitsAsync(
            currencyId, startDate, endDate);

        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("currency/{currencyId:int}/total-posted-credits")]
    public async Task<IActionResult> GetTotalPostedCredits(
        int currencyId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid reporting date range is required.", 400));

        var total = await _journalEntryService.GetTotalPostedCreditsAsync(
            currencyId, startDate, endDate);

        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    // ==================== AUDIT AND SEARCH ====================

    [HttpGet("lines/created-by/{userId:int}")]
    public async Task<IActionResult> GetLinesByCreatedByUserId(int userId)
    {
        var data = await _journalEntryService.GetLinesByCreatedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchJournalEntries([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse("Search term is required.", 400));

        var data = await _journalEntryService.SearchJournalEntriesAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<JournalEntryDto>>.SuccessResponse(data));
    }

    [HttpGet("lines/search")]
    public async Task<IActionResult> SearchJournalEntryLines([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse("Search term is required.", 400));

        var data = await _journalEntryService.SearchJournalEntryLinesAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<JournalEntryLineDto>>.SuccessResponse(data));
    }
}
