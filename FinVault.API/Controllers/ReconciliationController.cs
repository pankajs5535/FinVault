
using FinVault.Application.DTOs.ReconciliationBatchDtos;
using FinVault.Application.DTOs.ReconciliationRecordDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReconciliationController : ControllerBase
{
    private readonly IReconciliationService _service;

    public ReconciliationController(IReconciliationService service)
    {
        _service = service;
    }

    // ==================== BATCH IDENTIFICATION AND CRUD (10) ====================

    [HttpGet("batches")]
    public async Task<IActionResult> GetAllBatches()
    {
        var data = await _service.GetAllBatchesAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}")]
    public async Task<IActionResult> GetBatchById(int reconciliationBatchId) =>
        BatchResult(await _service.GetBatchByIdAsync(reconciliationBatchId));

    [HttpGet("batches/number/{batchNumber}")]
    public async Task<IActionResult> GetByBatchNumber(string batchNumber) =>
        BatchResult(await _service.GetByBatchNumberAsync(batchNumber));

    [HttpGet("batches/file-hash/{fileHash}")]
    public async Task<IActionResult> GetByFileHash(string fileHash) =>
        BatchResult(await _service.GetByFileHashAsync(fileHash));

    [HttpGet("batches/{reconciliationBatchId:int}/details")]
    public async Task<IActionResult> GetBatchDetails(int reconciliationBatchId) =>
        BatchResult(await _service.GetBatchDetailsAsync(reconciliationBatchId));

    [HttpGet("batches/exists/number/{batchNumber}")]
    public async Task<IActionResult> ExistsByBatchNumber(string batchNumber) =>
        Ok(ApiResponse<bool>.SuccessResponse(await _service.ExistsByBatchNumberAsync(batchNumber)));

    [HttpGet("batches/exists/file-hash/{fileHash}")]
    public async Task<IActionResult> ExistsByFileHash(string fileHash) =>
        Ok(ApiResponse<bool>.SuccessResponse(await _service.ExistsByFileHashAsync(fileHash)));

    [HttpPost("batches")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateReconciliationBatchDto request)
    {
        try
        {
            var data = await _service.CreateBatchAsync(request);

            return CreatedAtAction(
                nameof(GetBatchById),
                new { reconciliationBatchId = data.ReconciliationBatchId },
                ApiResponse<ReconciliationBatchDto>.SuccessResponse(
                    data, "Reconciliation batch created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut("batches")]
    public async Task<IActionResult> UpdateBatch([FromBody] UpdateReconciliationBatchDto request)
    {
        try
        {
            var data = await _service.UpdateBatchAsync(request);

            return data is null
                ? Conflict(ApiResponse<ReconciliationBatchDto>.FailureResponse(
                    "Batch not found or concurrency conflict detected.", 409))
                : Ok(ApiResponse<ReconciliationBatchDto>.SuccessResponse(
                    data, "Reconciliation batch updated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete("batches")]
    public async Task<IActionResult> DeleteBatch([FromBody] DeleteReconciliationBatchDto request)
    {
        try
        {
            var result = await _service.DeleteBatchAsync(request);

            return result
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Reconciliation batch deleted successfully."))
                : Conflict(ApiResponse<object>.FailureResponse(
                    "Batch not found or cannot be deleted.", 409));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    // ==================== BATCH TYPE AND BUSINESS DATE (4) ====================

    [HttpGet("batches/type/{reconciliationType}")]
    public async Task<IActionResult> GetBatchesByReconciliationType(string reconciliationType)
    {
        var data = await _service.GetBatchesByReconciliationTypeAsync(reconciliationType);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/business-date")]
    public async Task<IActionResult> GetBatchesByBusinessDate([FromQuery] DateTime businessDate)
    {
        var data = await _service.GetBatchesByBusinessDateAsync(businessDate);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/type/{reconciliationType}/business-date")]
    public async Task<IActionResult> GetBatchesByTypeAndBusinessDate(
        string reconciliationType,
        [FromQuery] DateTime businessDate)
    {
        var data = await _service.GetBatchesByTypeAndBusinessDateAsync(
            reconciliationType, businessDate);

        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/business-date/range")]
    public async Task<IActionResult> GetBatchesByBusinessDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        var data = await _service.GetBatchesByBusinessDateRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    // ==================== BATCH STATUS (6) ====================

    [HttpGet("batches/status/{status}")]
    public async Task<IActionResult> GetBatchesByStatus(string status)
    {
        var data = await _service.GetBatchesByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/pending")]
    public async Task<IActionResult> GetPendingBatches()
    {
        var data = await _service.GetPendingBatchesAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/in-progress")]
    public async Task<IActionResult> GetInProgressBatches()
    {
        var data = await _service.GetInProgressBatchesAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/completed")]
    public async Task<IActionResult> GetCompletedBatches()
    {
        var data = await _service.GetCompletedBatchesAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/completed-with-exceptions")]
    public async Task<IActionResult> GetCompletedWithExceptions()
    {
        var data = await _service.GetCompletedWithExceptionsAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/failed")]
    public async Task<IActionResult> GetFailedBatches()
    {
        var data = await _service.GetFailedBatchesAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    // ==================== BATCH INITIATION AND RESULTS (5) ====================

    [HttpGet("batches/payment-method/{paymentMethodId:int}")]
    public async Task<IActionResult> GetBatchesByPaymentMethodId(int paymentMethodId)
    {
        var data = await _service.GetBatchesByPaymentMethodIdAsync(paymentMethodId);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/initiated-by/{userId:int}")]
    public async Task<IActionResult> GetBatchesByInitiatedByUserId(int userId)
    {
        var data = await _service.GetBatchesByInitiatedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/with-differences")]
    public async Task<IActionResult> GetBatchesWithDifferences()
    {
        var data = await _service.GetBatchesWithDifferencesAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/with-records")]
    public async Task<IActionResult> GetBatchesWithRecords()
    {
        var data = await _service.GetBatchesWithRecordsAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/completed/date-range")]
    public async Task<IActionResult> GetCompletedBatchesWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        var data = await _service.GetCompletedBatchesWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    // ==================== RECORD IDENTIFICATION AND CRUD (8) ====================

    [HttpGet("records")]
    public async Task<IActionResult> GetAllRecords()
    {
        var data = await _service.GetAllRecordsAsync();
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("records/{reconciliationRecordId:int}")]
    public async Task<IActionResult> GetRecordById(int reconciliationRecordId) =>
        RecordResult(await _service.GetRecordByIdAsync(reconciliationRecordId));

    [HttpGet("records/{reconciliationRecordId:int}/details")]
    public async Task<IActionResult> GetRecordDetails(int reconciliationRecordId) =>
        RecordResult(await _service.GetRecordDetailsAsync(reconciliationRecordId));

    [HttpGet("records/external-reference/{externalReference}")]
    public async Task<IActionResult> GetByExternalReference(string externalReference)
    {
        var data = await _service.GetByExternalReferenceAsync(externalReference);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("records/exists/external-reference/{externalReference}")]
    public async Task<IActionResult> ExistsByExternalReference(string externalReference)
    {
        var result = await _service.ExistsByExternalReferenceAsync(externalReference);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    [HttpPost("records")]
    public async Task<IActionResult> CreateRecord([FromBody] CreateReconciliationRecordDto request)
    {
        try
        {
            var data = await _service.CreateRecordAsync(request);

            return CreatedAtAction(
                nameof(GetRecordById),
                new { reconciliationRecordId = data.ReconciliationRecordId },
                ApiResponse<ReconciliationRecordDto>.SuccessResponse(
                    data, "Reconciliation record created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut("records")]
    public async Task<IActionResult> UpdateRecord([FromBody] UpdateReconciliationRecordDto request)
    {
        try
        {
            var data = await _service.UpdateRecordAsync(request);

            return data is null
                ? NotFound(ApiResponse<ReconciliationRecordDto>.FailureResponse(
                    "Reconciliation record not found.", 404))
                : Ok(ApiResponse<ReconciliationRecordDto>.SuccessResponse(
                    data, "Reconciliation record updated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete("records")]
    public async Task<IActionResult> DeleteRecord([FromBody] DeleteReconciliationRecordDto request)
    {
        try
        {
            var result = await _service.DeleteRecordAsync(request);

            return result
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Reconciliation record deleted successfully."))
                : NotFound(ApiResponse<object>.FailureResponse(
                    "Reconciliation record not found or could not be deleted.", 404));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<object>.FailureResponse(ex.Message, 409));
        }
    }

    // ==================== RECORD BATCH AND TRANSACTION RELATIONSHIPS (4) ====================

    [HttpGet("batches/{reconciliationBatchId:int}/records")]
    public async Task<IActionResult> GetRecordsByBatchId(int reconciliationBatchId)
    {
        var data = await _service.GetRecordsByBatchIdAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/with-transactions")]
    public async Task<IActionResult> GetBatchRecordsWithTransactions(int reconciliationBatchId)
    {
        var data = await _service.GetBatchRecordsWithTransactionsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("records/transaction/{transactionId:int}")]
    public async Task<IActionResult> GetRecordsByTransactionId(int transactionId)
    {
        var data = await _service.GetRecordsByTransactionIdAsync(transactionId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/without-transaction")]
    public async Task<IActionResult> GetRecordsWithoutTransaction(int reconciliationBatchId)
    {
        var data = await _service.GetRecordsWithoutTransactionAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    // ==================== RECORD MATCH STATUS (8) ====================

    [HttpGet("records/match-status/{matchStatus}")]
    public async Task<IActionResult> GetRecordsByMatchStatus(string matchStatus)
    {
        var data = await _service.GetRecordsByMatchStatusAsync(matchStatus);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/matched")]
    public async Task<IActionResult> GetMatchedRecords(int reconciliationBatchId)
    {
        var data = await _service.GetMatchedRecordsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/unmatched")]
    public async Task<IActionResult> GetUnmatchedRecords(int reconciliationBatchId)
    {
        var data = await _service.GetUnmatchedRecordsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/amount-mismatch")]
    public async Task<IActionResult> GetAmountMismatchRecords(int reconciliationBatchId)
    {
        var data = await _service.GetAmountMismatchRecordsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/missing-in-bank")]
    public async Task<IActionResult> GetMissingInBankRecords(int reconciliationBatchId)
    {
        var data = await _service.GetMissingInBankRecordsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/missing-in-external")]
    public async Task<IActionResult> GetMissingInExternalRecords(int reconciliationBatchId)
    {
        var data = await _service.GetMissingInExternalRecordsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/manually-resolved")]
    public async Task<IActionResult> GetManuallyResolvedRecords(int reconciliationBatchId)
    {
        var data = await _service.GetManuallyResolvedRecordsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/with-differences")]
    public async Task<IActionResult> GetRecordsWithDifferences(int reconciliationBatchId)
    {
        var data = await _service.GetRecordsWithDifferencesAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    // ==================== RECORD AMOUNT AND DATE ANALYSIS (2) ====================

    [HttpGet("records/external-date-range")]
    public async Task<IActionResult> GetRecordsByExternalDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        var data = await _service.GetRecordsByExternalDateRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("records/external-amount-range")]
    public async Task<IActionResult> GetRecordsByExternalAmountRange(
        [FromQuery] decimal minimumAmount,
        [FromQuery] decimal maximumAmount)
    {
        if (minimumAmount < 0 || maximumAmount < minimumAmount)
            return BadRequest(ApiResponse<object>.FailureResponse(
                "A valid external amount range is required.", 400));

        var data = await _service.GetRecordsByExternalAmountRangeAsync(
            minimumAmount, maximumAmount);

        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    // ==================== EXCEPTION RESOLUTION AND AUDIT (3) ====================

    [HttpGet("records/resolved-by/{userId:int}")]
    public async Task<IActionResult> GetRecordsByResolvedByUserId(int userId)
    {
        var data = await _service.GetRecordsByResolvedByUserIdAsync(userId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("records/resolved/date-range")]
    public async Task<IActionResult> GetRecordsResolvedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (!ValidRange(startDate, endDate))
            return InvalidRange();

        var data = await _service.GetRecordsResolvedWithinRangeAsync(startDate, endDate);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("batches/{reconciliationBatchId:int}/records/unresolved")]
    public async Task<IActionResult> GetUnresolvedRecords(int reconciliationBatchId)
    {
        var data = await _service.GetUnresolvedRecordsAsync(reconciliationBatchId);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH AND REPORTING (7) ====================

    [HttpGet("batches/search")]
    public async Task<IActionResult> SearchBatches([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse("Search term is required.", 400));

        var data = await _service.SearchBatchesAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<ReconciliationBatchDto>>.SuccessResponse(data));
    }

    [HttpGet("records/search")]
    public async Task<IActionResult> SearchRecords([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(ApiResponse<object>.FailureResponse("Search term is required.", 400));

        var data = await _service.SearchRecordsAsync(searchTerm);
        return Ok(ApiResponse<IEnumerable<ReconciliationRecordDto>>.SuccessResponse(data));
    }

    [HttpGet("reports/batches/count")]
    public async Task<IActionResult> CountBatchesByStatusAndBusinessDate(
        [FromQuery] string status,
        [FromQuery] DateTime businessDate)
    {
        var count = await _service.CountBatchesByStatusAndBusinessDateAsync(status, businessDate);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("reports/batches/{reconciliationBatchId:int}/records/count")]
    public async Task<IActionResult> CountRecordsByBatchAndMatchStatus(
        int reconciliationBatchId,
        [FromQuery] string matchStatus)
    {
        var count = await _service.CountRecordsByBatchAndMatchStatusAsync(
            reconciliationBatchId, matchStatus);

        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    [HttpGet("reports/batches/{reconciliationBatchId:int}/total-external-amount")]
    public async Task<IActionResult> GetTotalExternalAmountByBatch(int reconciliationBatchId)
    {
        var total = await _service.GetTotalExternalAmountByBatchAsync(reconciliationBatchId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("reports/batches/{reconciliationBatchId:int}/total-internal-amount")]
    public async Task<IActionResult> GetTotalInternalAmountByBatch(int reconciliationBatchId)
    {
        var total = await _service.GetTotalInternalAmountByBatchAsync(reconciliationBatchId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    [HttpGet("reports/batches/{reconciliationBatchId:int}/total-difference-amount")]
    public async Task<IActionResult> GetTotalDifferenceAmountByBatch(int reconciliationBatchId)
    {
        var total = await _service.GetTotalDifferenceAmountByBatchAsync(reconciliationBatchId);
        return Ok(ApiResponse<decimal>.SuccessResponse(total));
    }

    // ==================== RESPONSE HELPERS ====================

    private IActionResult BatchResult(ReconciliationBatchDto? data) =>
        data is null
            ? NotFound(ApiResponse<ReconciliationBatchDto>.FailureResponse(
                "Reconciliation batch not found.", 404))
            : Ok(ApiResponse<ReconciliationBatchDto>.SuccessResponse(data));

    private IActionResult RecordResult(ReconciliationRecordDto? data) =>
        data is null
            ? NotFound(ApiResponse<ReconciliationRecordDto>.FailureResponse(
                "Reconciliation record not found.", 404))
            : Ok(ApiResponse<ReconciliationRecordDto>.SuccessResponse(data));

    private static bool ValidRange(DateTime startDate, DateTime endDate) =>
        startDate != default &&
        endDate != default &&
        endDate >= startDate;

    private IActionResult InvalidRange() =>
        BadRequest(ApiResponse<object>.FailureResponse(
            "A valid date range is required.", 400));
}
