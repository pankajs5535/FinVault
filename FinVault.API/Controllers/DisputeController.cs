
using FinVault.Application.DTOs.DisputeDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DisputeController : ControllerBase
{
    private readonly IDisputeService _disputeService;

    public DisputeController(IDisputeService disputeService)
    {
        _disputeService = disputeService;
    }

    // ==================== DISPUTE IDENTIFICATION (5) ====================

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _disputeService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    [HttpGet("{disputeId:int}")]
    public async Task<IActionResult> GetById(int disputeId) =>
        Result(await _disputeService.GetByIdAsync(disputeId));

    [HttpGet("number/{disputeNumber}")]
    public async Task<IActionResult> GetByDisputeNumber(string disputeNumber) =>
        Result(await _disputeService.GetByDisputeNumberAsync(disputeNumber));

    [HttpGet("{disputeId:int}/details")]
    public async Task<IActionResult> GetDisputeDetails(int disputeId) =>
        Result(await _disputeService.GetDisputeDetailsAsync(disputeId));

    [HttpGet("exists/number/{disputeNumber}")]
    public async Task<IActionResult> ExistsByDisputeNumber(string disputeNumber)
    {
        var result = await _disputeService.ExistsByDisputeNumberAsync(disputeNumber);
        return Ok(ApiResponse<bool>.SuccessResponse(result));
    }

    // ==================== DISPUTE CRUD (3) ====================

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDisputeDto request)
    {
        try
        {
            var data = await _disputeService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { disputeId = data.DisputeId },
                ApiResponse<DisputeDto>.SuccessResponse(
                    data, "Dispute created successfully.", 201));
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

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateDisputeDto request)
    {
        try
        {
            var data = await _disputeService.UpdateAsync(request);

            return data is null
                ? NotFound(ApiResponse<DisputeDto>.FailureResponse("Dispute not found.", 404))
                : Ok(ApiResponse<DisputeDto>.SuccessResponse(
                    data, "Dispute updated successfully."));
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

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteDisputeDto request)
    {
        try
        {
            var result = await _disputeService.DeleteAsync(request);

            return result
                ? Ok(ApiResponse<object>.SuccessResponse(null, "Dispute deleted successfully."))
                : NotFound(ApiResponse<object>.FailureResponse("Dispute not found.", 404));
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

    // ==================== TRANSACTION-BASED DISPUTES (2) ====================

    [HttpGet("transaction/{transactionId:int}")]
    public async Task<IActionResult> GetByTransactionId(int transactionId)
    {
        var data = await _disputeService.GetByTransactionIdAsync(transactionId);
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    [HttpGet("card-transaction/{cardTransactionId:int}")]
    public async Task<IActionResult> GetByCardTransactionId(int cardTransactionId)
    {
        var data = await _disputeService.GetByCardTransactionIdAsync(cardTransactionId);
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    // ==================== CUSTOMER DISPUTES (2) ====================

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var data = await _disputeService.GetByCustomerIdAsync(customerId);
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    [HttpGet("customer/{customerId:int}/count")]
    public async Task<IActionResult> CountDisputesByCustomerId(int customerId)
    {
        var count = await _disputeService.CountDisputesByCustomerIdAsync(customerId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    // ==================== STATUS AND PRIORITY (2) ====================

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(string status)
    {
        var data = await _disputeService.GetByStatusAsync(status);
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    [HttpGet("priority/{priority}")]
    public async Task<IActionResult> GetByPriority(string priority)
    {
        var data = await _disputeService.GetByPriorityAsync(priority);
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    // ==================== EMPLOYEE ASSIGNMENT (2) ====================

    [HttpGet("assigned-employee/{employeeId:int}")]
    public async Task<IActionResult> GetByAssignedEmployeeId(int employeeId)
    {
        var data = await _disputeService.GetByAssignedEmployeeIdAsync(employeeId);
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    [HttpGet("unassigned")]
    public async Task<IActionResult> GetUnassignedDisputes()
    {
        var data = await _disputeService.GetUnassignedDisputesAsync();
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    // ==================== INVESTIGATION QUEUES (3) ====================

    [HttpGet("open")]
    public async Task<IActionResult> GetOpenDisputes()
    {
        var data = await _disputeService.GetOpenDisputesAsync();
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdueDisputes([FromQuery] DateTime currentDate)
    {
        try
        {
            var data = await _disputeService.GetOverdueDisputesAsync(currentDate);
            return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("due/date-range")]
    public async Task<IActionResult> GetDisputesDueWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var data = await _disputeService.GetDisputesDueWithinRangeAsync(startDate, endDate);
            return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    // ==================== DISPUTE TIMELINE (2) ====================

    [HttpGet("raised/date-range")]
    public async Task<IActionResult> GetDisputesRaisedWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var data = await _disputeService.GetDisputesRaisedWithinRangeAsync(startDate, endDate);
            return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("resolved/date-range")]
    public async Task<IActionResult> GetResolvedDisputesWithinRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var data = await _disputeService.GetResolvedDisputesWithinRangeAsync(startDate, endDate);
            return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    // ==================== REFUND MANAGEMENT (1) ====================

    [HttpGet("with-refunds")]
    public async Task<IActionResult> GetDisputesWithRefunds()
    {
        var data = await _disputeService.GetDisputesWithRefundsAsync();
        return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
    }

    // ==================== SEARCH (1) ====================

    [HttpGet("search")]
    public async Task<IActionResult> SearchDisputes([FromQuery] string searchTerm)
    {
        try
        {
            var data = await _disputeService.SearchDisputesAsync(searchTerm);
            return Ok(ApiResponse<IEnumerable<DisputeDto>>.SuccessResponse(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(ex.Message, 400));
        }
    }

    // ==================== RESPONSE HELPER ====================

    private IActionResult Result(DisputeDto? data) =>
        data is null
            ? NotFound(ApiResponse<DisputeDto>.FailureResponse("Dispute not found.", 404))
            : Ok(ApiResponse<DisputeDto>.SuccessResponse(data));
}
