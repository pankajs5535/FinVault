
using FinVault.Application.DTOs.AIChatMessageDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AIChatMessageController : ControllerBase
{
    private readonly IAIChatMessageService _messageService;

    public AIChatMessageController(IAIChatMessageService messageService)
    {
        _messageService = messageService;
    }

    // 1. MESSAGE CRUD AND IDENTIFICATION

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(
            await _messageService.GetAllAsync()));

    [HttpGet("{messageId:int}")]
    public async Task<IActionResult> GetById(int messageId)
    {
        var result = await _messageService.GetByIdAsync(messageId);

        return result == null
            ? NotFound(ApiResponse<AIChatMessageDto>.FailureResponse("Message not found.", 404))
            : Ok(ApiResponse<AIChatMessageDto>.SuccessResponse(result));
    }

    [HttpGet("conversation/{conversationId:int}/sequence/{sequenceNumber:int}")]
    public async Task<IActionResult> GetByConversationAndSequence(
        int conversationId,
        int sequenceNumber)
    {
        var result = await _messageService.GetByConversationAndSequenceAsync(
            conversationId, sequenceNumber);

        return result == null
            ? NotFound(ApiResponse<AIChatMessageDto>.FailureResponse("Message not found.", 404))
            : Ok(ApiResponse<AIChatMessageDto>.SuccessResponse(result));
    }

    [HttpGet("{messageId:int}/details")]
    public async Task<IActionResult> GetMessageDetails(int messageId)
    {
        var result = await _messageService.GetMessageDetailsAsync(messageId);

        return result == null
            ? NotFound(ApiResponse<AIChatMessageDto>.FailureResponse("Message not found.", 404))
            : Ok(ApiResponse<AIChatMessageDto>.SuccessResponse(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAIChatMessageDto dto)
    {
        try
        {
            var result = await _messageService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { messageId = result.AIChatMessageId },
                ApiResponse<AIChatMessageDto>.SuccessResponse(
                    result, "Message created successfully.", 201));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AIChatMessageDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<AIChatMessageDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateAIChatMessageDto dto)
    {
        try
        {
            var result = await _messageService.UpdateAsync(dto);

            return result == null
                ? NotFound(ApiResponse<AIChatMessageDto>.FailureResponse("Message not found.", 404))
                : Ok(ApiResponse<AIChatMessageDto>.SuccessResponse(
                    result, "Message updated successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<AIChatMessageDto>.FailureResponse(ex.Message, 400));
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ApiResponse<AIChatMessageDto>.FailureResponse(ex.Message, 409));
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteAIChatMessageDto dto)
    {
        var result = await _messageService.DeleteAsync(dto);

        return result
            ? Ok(ApiResponse<bool>.SuccessResponse(true, "Message deleted successfully."))
            : NotFound(ApiResponse<bool>.FailureResponse("Message not found.", 404));
    }

    // 2. CONVERSATION MESSAGE HISTORY

    [HttpGet("conversation/{conversationId:int}")]
    public async Task<IActionResult> GetMessagesByConversationId(int conversationId)
        => Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(
            await _messageService.GetMessagesByConversationIdAsync(conversationId)));

    [HttpGet("conversation/{conversationId:int}/latest")]
    public async Task<IActionResult> GetLatestMessages(
        int conversationId,
        [FromQuery] int count = 10)
    {
        if (count <= 0 || count > 200)
            return BadRequest(ApiResponse<IEnumerable<AIChatMessageDto>>.FailureResponse(
                "Count must be between 1 and 200.", 400));

        return Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(
            await _messageService.GetLatestMessagesAsync(conversationId, count)));
    }

    [HttpGet("conversation/{conversationId:int}/latest-assistant")]
    public async Task<IActionResult> GetLatestAssistantMessage(int conversationId)
    {
        var result = await _messageService.GetLatestAssistantMessageAsync(conversationId);

        return result == null
            ? NotFound(ApiResponse<AIChatMessageDto>.FailureResponse(
                "No assistant message found for this conversation.", 404))
            : Ok(ApiResponse<AIChatMessageDto>.SuccessResponse(result));
    }

    [HttpGet("conversation/{conversationId:int}/date-range")]
    public async Task<IActionResult> GetMessagesByDateRange(
        int conversationId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var result = await _messageService.GetMessagesByDateRangeAsync(
                conversationId, startDate, endDate);

            return Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIChatMessageDto>>.FailureResponse(ex.Message, 400));
        }
    }

    // 3. SENDER ROLE AND MODERATION

    [HttpGet("conversation/{conversationId:int}/sender-role/{senderRole}")]
    public async Task<IActionResult> GetMessagesBySenderRole(
        int conversationId,
        string senderRole)
    {
        try
        {
            var result = await _messageService.GetMessagesBySenderRoleAsync(
                conversationId, senderRole);

            return Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIChatMessageDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("flagged")]
    public async Task<IActionResult> GetFlaggedMessages()
        => Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(
            await _messageService.GetFlaggedMessagesAsync()));

    [HttpGet("conversation/{conversationId:int}/flagged")]
    public async Task<IActionResult> GetFlaggedMessagesByConversationId(int conversationId)
        => Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(
            await _messageService.GetFlaggedMessagesByConversationIdAsync(conversationId)));

    // 4. MODEL AND USAGE INFORMATION

    [HttpGet("model/{modelName}")]
    public async Task<IActionResult> GetMessagesByModelName(string modelName)
    {
        try
        {
            var result = await _messageService.GetMessagesByModelNameAsync(modelName);

            return Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIChatMessageDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("conversation/{conversationId:int}/usage-logs")]
    public async Task<IActionResult> GetMessagesWithUsageLogs(int conversationId)
        => Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(
            await _messageService.GetMessagesWithUsageLogsAsync(conversationId)));

    // 5. MESSAGE SEARCH AND PAGINATION

    [HttpGet("conversation/{conversationId:int}/search")]
    public async Task<IActionResult> SearchMessages(
        int conversationId,
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await _messageService.SearchMessagesAsync(
                conversationId, searchTerm, pageNumber, pageSize);

            return Ok(ApiResponse<IEnumerable<AIChatMessageDto>>.SuccessResponse(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<AIChatMessageDto>>.FailureResponse(ex.Message, 400));
        }
    }

    [HttpGet("conversation/{conversationId:int}/count")]
    public async Task<IActionResult> CountMessagesByConversationId(int conversationId)
        => Ok(ApiResponse<int>.SuccessResponse(
            await _messageService.CountMessagesByConversationIdAsync(conversationId)));
}
