
using FinVault.Application.DTOs.AIConversationDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIConversationController : ControllerBase
{
    private readonly IAIConversationService _conversationService;

    public AIConversationController(IAIConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    // 1. CONVERSATION CRUD

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetAllAsync()));

    [HttpGet("{conversationId:int}")]
    public async Task<IActionResult> GetById(int conversationId)
    {
        var result = await _conversationService.GetByIdAsync(conversationId);
        return result == null
            ? NotFound(ApiResponse<AIConversationDto>.FailureResponse("Conversation not found.", 404))
            : Ok(ApiResponse<AIConversationDto>.SuccessResponse(result));
    }

    [HttpGet("reference/{conversationReference}")]
    public async Task<IActionResult> GetByConversationReference(string conversationReference)
    {
        var result = await _conversationService.GetByConversationReferenceAsync(conversationReference);
        return result == null
            ? NotFound(ApiResponse<AIConversationDto>.FailureResponse("Conversation not found.", 404))
            : Ok(ApiResponse<AIConversationDto>.SuccessResponse(result));
    }

    [HttpGet("{conversationId:int}/details")]
    public async Task<IActionResult> GetConversationDetails(int conversationId)
    {
        var result = await _conversationService.GetConversationDetailsAsync(conversationId);
        return result == null
            ? NotFound(ApiResponse<AIConversationDto>.FailureResponse("Conversation not found.", 404))
            : Ok(ApiResponse<AIConversationDto>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAIConversationDto dto)
    {
        try
        {
            var result = await _conversationService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { conversationId = result.AIConversationId },
                ApiResponse<AIConversationDto>.SuccessResponse(
                    result, "Conversation created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AIConversationDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<AIConversationDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAIConversationDto dto)
    {
        try
        {
            var result = await _conversationService.UpdateAsync(dto);

            return result == null
                ? NotFound(ApiResponse<AIConversationDto>.FailureResponse("Conversation not found.", 404))
                : Ok(ApiResponse<AIConversationDto>.SuccessResponse(
                    result, "Conversation updated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AIConversationDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<AIConversationDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAIConversationDto dto)
    {
        var result = await _conversationService.DeleteAsync(dto);

        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Conversation deleted successfully."))
            : NotFound(ApiResponse<bool>.FailureResponse("Conversation not found or could not be deleted.", 404));
    }

    // 2. USER AND CUSTOMER CONVERSATIONS

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetConversationsByUserId(int userId)
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetConversationsByUserIdAsync(userId)));

    [HttpGet("customer/{customerId:int}")]
    public async Task<IActionResult> GetConversationsByCustomerId(int customerId)
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetConversationsByCustomerIdAsync(customerId)));

    [HttpGet("user/{userId:int}/recent")]
    public async Task<IActionResult> GetRecentConversationsByUserId(
        int userId,
        [FromQuery] int count = 10)
    {
        if (count < 1)
            return BadRequest(ApiResponse<IEnumerable<AIConversationDto>>.FailureResponse(
                "Count must be greater than zero.", 400));

        return Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetRecentConversationsByUserIdAsync(userId, count)));
    }

    [HttpGet("user/{userId:int}/active")]
    public async Task<IActionResult> GetActiveConversationsByUserId(int userId)
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetActiveConversationsByUserIdAsync(userId)));

    [HttpGet("user/{userId:int}/has-active")]
    public async Task<IActionResult> HasActiveConversation(int userId)
        => Ok(ApiResponse<bool>.SuccessResponse(
            await _conversationService.HasActiveConversationAsync(userId)));

    // 3. CONVERSATION STATUS AND CHANNEL

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetConversationsByStatus(string status)
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetConversationsByStatusAsync(status)));

    [HttpGet("channel/{channel}")]
    public async Task<IActionResult> GetConversationsByChannel(string channel)
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetConversationsByChannelAsync(channel)));

    [HttpGet("date-range")]
    public async Task<IActionResult> GetConversationsByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest(ApiResponse<IEnumerable<AIConversationDto>>.FailureResponse(
                "Start date cannot be later than end date.", 400));

        return Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetConversationsByDateRangeAsync(startDate, endDate)));
    }

    // 4. HUMAN SUPPORT ESCALATION

    [HttpGet("escalated")]
    public async Task<IActionResult> GetEscalatedConversations()
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetEscalatedConversationsAsync()));

    [HttpGet("escalated/employee/{employeeId:int}")]
    public async Task<IActionResult> GetEscalatedConversationsByEmployeeId(int employeeId)
        => Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.GetEscalatedConversationsByEmployeeIdAsync(employeeId)));

    // 5. MESSAGE AND USAGE INFORMATION

    [HttpGet("{conversationId:int}/messages")]
    public async Task<IActionResult> GetConversationWithMessages(int conversationId)
    {
        var result = await _conversationService.GetConversationWithMessagesAsync(conversationId);

        return result == null
            ? NotFound(ApiResponse<AIConversationDto>.FailureResponse("Conversation not found.", 404))
            : Ok(ApiResponse<AIConversationDto>.SuccessResponse(result));
    }

    [HttpGet("{conversationId:int}/usage-logs")]
    public async Task<IActionResult> GetConversationWithUsageLogs(int conversationId)
    {
        var result = await _conversationService.GetConversationWithUsageLogsAsync(conversationId);

        return result == null
            ? NotFound(ApiResponse<AIConversationDto>.FailureResponse("Conversation not found.", 404))
            : Ok(ApiResponse<AIConversationDto>.SuccessResponse(result));
    }

    // 6. SEARCH

    [HttpGet("search")]
    public async Task<IActionResult> SearchConversations(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest(ApiResponse<IEnumerable<AIConversationDto>>.FailureResponse(
                "Invalid pagination values. Page number must be positive and page size must be between 1 and 100.", 400));

        return Ok(ApiResponse<IEnumerable<AIConversationDto>>.SuccessResponse(
            await _conversationService.SearchConversationsAsync(searchTerm, pageNumber, pageSize)));
    }
}
