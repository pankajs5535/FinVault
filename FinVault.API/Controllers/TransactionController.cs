
using FinVault.Application.DTOs.TransactionDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    // ==================== CRUD AND TRANSACTION IDENTIFICATION ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _transactionService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("{transactionId:int}")]
    public async Task<IActionResult> GetById(int transactionId)
    {
        var data = await _transactionService.GetByIdAsync(transactionId);

        return data is null
            ? NotFound(ApiResponse<TransactionDto>.FailureResponse("Transaction not found.", 404))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(data));
    }

    [HttpGet("reference/{transactionReference}")]
    public async Task<IActionResult> GetByTransactionReference(string transactionReference)
    {
        var data = await _transactionService.GetByTransactionReferenceAsync(transactionReference);

        return data is null
            ? NotFound(ApiResponse<TransactionDto>.FailureResponse("Transaction not found.", 404))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(data));
    }

    [HttpGet("idempotency/{idempotencyKey}")]
    public async Task<IActionResult> GetByIdempotencyKey(string idempotencyKey)
    {
        var data = await _transactionService.GetByIdempotencyKeyAsync(idempotencyKey);

        return data is null
            ? NotFound(ApiResponse<TransactionDto>.FailureResponse("Transaction not found.", 404))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(data));
    }

    [HttpGet("external-reference/{externalReference}")]
    public async Task<IActionResult> GetByExternalReference(string externalReference)
    {
        var data = await _transactionService.GetByExternalReferenceAsync(externalReference);

        return data is null
            ? NotFound(ApiResponse<TransactionDto>.FailureResponse("Transaction not found.", 404))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(data));
    }

    [HttpGet("{transactionId:int}/details")]
    public async Task<IActionResult> GetTransactionDetails(int transactionId)
    {
        var data = await _transactionService.GetTransactionDetailsAsync(transactionId);

        return data is null
            ? NotFound(ApiResponse<TransactionDto>.FailureResponse("Transaction details not found.", 404))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto request)
    {
        var data = await _transactionService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { transactionId = data.TransactionId },
            ApiResponse<TransactionDto>.SuccessResponse(
                data, "Transaction created successfully.", 201));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateTransactionDto request)
    {
        var data = await _transactionService.UpdateAsync(request);

        return data is null
            ? Conflict(ApiResponse<TransactionDto>.FailureResponse(
                "Transaction could not be updated. It may not exist or its RowVersion may be outdated.", 409))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(
                data, "Transaction updated successfully."));
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteTransactionDto request)
    {
        var result = await _transactionService.DeleteAsync(request);

        return result
            ? Ok(ApiResponse<object>.SuccessResponse(null, "Transaction deleted successfully."))
            : Conflict(ApiResponse<object>.FailureResponse(
                "Transaction could not be deleted.", 409));
    }

    [HttpGet("exists/reference/{transactionReference}")]
    public async Task<IActionResult> ExistsByTransactionReference(string transactionReference)
    {
        var result = await _transactionService.ExistsByTransactionReferenceAsync(
            transactionReference);

        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpGet("exists/idempotency/{idempotencyKey}")]
    public async Task<IActionResult> ExistsByIdempotencyKey(string idempotencyKey)
    {
        var result = await _transactionService.ExistsByIdempotencyKeyAsync(idempotencyKey);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== ACCOUNT TRANSACTION HISTORY ====================

    [HttpGet("account/{accountId:int}/source")]
    public async Task<IActionResult> GetBySourceAccountId(int accountId)
    {
        var data = await _transactionService.GetBySourceAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/destination")]
    public async Task<IActionResult> GetByDestinationAccountId(int accountId)
    {
        var data = await _transactionService.GetByDestinationAccountIdAsync(accountId);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/history")]
    public async Task<IActionResult> GetAccountTransactionHistory(int accountId)
    {
        var data = await _transactionService.GetAccountTransactionHistoryAsync(accountId);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("type/{transactionTypeId:int}")]
    public async Task<IActionResult> GetByTransactionTypeId(int transactionTypeId)
    {
        var data = await _transactionService.GetByTransactionTypeIdAsync(transactionTypeId);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("currency/{currencyId:int}")]
    public async Task<IActionResult> GetByCurrencyId(int currencyId)
    {
        var data = await _transactionService.GetByCurrencyIdAsync(currencyId);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    // ==================== TRANSACTION STATUS AND CHANNEL ====================

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _transactionService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("channel/{channel}")]
    public async Task<IActionResult> GetByChannel(string channel)
    {
        var data = await _transactionService.GetByChannelAsync(channel);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("posted")]
    public async Task<IActionResult> GetPostedTransactions()
    {
        var data = await _transactionService.GetPostedTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingTransactions()
    {
        var data = await _transactionService.GetPendingTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("on-hold")]
    public async Task<IActionResult> GetOnHoldTransactions()
    {
        var data = await _transactionService.GetOnHoldTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("failed")]
    public async Task<IActionResult> GetFailedTransactions()
    {
        var data = await _transactionService.GetFailedTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    // ==================== DATE-BASED TRANSACTION HISTORY ====================

    [HttpGet("date-range")]
    public async Task<IActionResult> GetTransactionsWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _transactionService.GetTransactionsWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("posted/date-range")]
    public async Task<IActionResult> GetPostedTransactionsWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var data = await _transactionService.GetPostedTransactionsWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("value-date")]
    public async Task<IActionResult> GetTransactionsByValueDate(
        [FromQuery] DateTime valueDate)
    {
        if (valueDate == default)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid value date is required.", 400));

        var data = await _transactionService.GetTransactionsByValueDateAsync(valueDate);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/recent")]
    public async Task<IActionResult> GetRecentTransactionsByAccountId(
        int accountId,
        [FromQuery] int count = 10)
    {
        if (count < 1 || count > 500)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Count must be between 1 and 500.", 400));

        var data = await _transactionService.GetRecentTransactionsByAccountIdAsync(
            accountId, count);

        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    // ==================== APPROVAL AND REVERSAL TRACKING ====================

    [HttpGet("initiated-by/{userId:int}")]
    public async Task<IActionResult> GetByInitiatedByUserId(int userId)
    {
        var data = await _transactionService.GetByInitiatedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("approved-by/{userId:int}")]
    public async Task<IActionResult> GetByApprovedByUserId(int userId)
    {
        var data = await _transactionService.GetByApprovedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("{originalTransactionId:int}/reversals")]
    public async Task<IActionResult> GetReversalTransactions(int originalTransactionId)
    {
        var data = await _transactionService.GetReversalTransactionsAsync(
            originalTransactionId);

        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("with-original-transaction")]
    public async Task<IActionResult> GetTransactionsWithOriginalTransaction()
    {
        var data = await _transactionService.GetTransactionsWithOriginalTransactionAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    // ==================== JOURNAL AND RELATED BANKING RECORDS ====================

    [HttpGet("{transactionId:int}/journal-entries")]
    public async Task<IActionResult> GetWithJournalEntries(int transactionId)
    {
        var data = await _transactionService.GetWithJournalEntriesAsync(transactionId);

        return data is null
            ? NotFound(ApiResponse<TransactionDto>.FailureResponse(
                "Transaction or its journal entries were not found.", 404))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(data));
    }

    [HttpGet("{transactionId:int}/payment-request")]
    public async Task<IActionResult> GetWithPaymentRequest(int transactionId)
    {
        var data = await _transactionService.GetWithPaymentRequestAsync(transactionId);

        return data is null
            ? NotFound(ApiResponse<TransactionDto>.FailureResponse(
                "Transaction or its payment request was not found.", 404))
            : Ok(ApiResponse<TransactionDto>.SuccessResponse(data));
    }

    [HttpGet("with-card-transactions")]
    public async Task<IActionResult> GetTransactionsWithCardTransactions()
    {
        var data = await _transactionService.GetTransactionsWithCardTransactionsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("with-loan-repayments")]
    public async Task<IActionResult> GetTransactionsWithLoanRepayments()
    {
        var data = await _transactionService.GetTransactionsWithLoanRepaymentsAsync();
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    // ==================== FINANCIAL REPORTING AND SEARCH ====================

    [HttpGet("account/{accountId:int}/total-posted-outgoing")]
    public async Task<IActionResult> GetTotalPostedOutgoingAmount(int accountId)
    {
        var total = await _transactionService.GetTotalPostedOutgoingAmountAsync(accountId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("account/{accountId:int}/total-posted-incoming")]
    public async Task<IActionResult> GetTotalPostedIncomingAmount(int accountId)
    {
        var total = await _transactionService.GetTotalPostedIncomingAmountAsync(accountId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("total-posted/date-range")]
    public async Task<IActionResult> GetTotalPostedAmountWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate == default || endDate == default || endDate < startDate)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid date range is required.", 400));

        var total = await _transactionService.GetTotalPostedAmountWithinRangeAsync(
            startDate, endDate);

        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTransactions(
        [FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Search term is required.", 400));

        var data = await _transactionService.SearchTransactionsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<TransactionDto>>.SuccessResponse(data));
    }

    [HttpGet("account/{accountId:int}/count-by-status")]
    public async Task<IActionResult> CountAccountTransactionsByStatus(
        int accountId,
        [FromQuery] string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return BadRequest(ApiResponse<object>.FailureResponse(
                "Transaction status is required.", 400));

        var count = await _transactionService.CountAccountTransactionsByStatusAsync(
            accountId, status);

        return Ok(ApiResponse<int>.SuccessResponse(count));
    }
}
