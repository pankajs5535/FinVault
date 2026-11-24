using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.AIConversationDtos;
using FinVault.Application.DTOs.AIChatMessageDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Assistant banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IAssistantService
{
        Task<AIConversationDto> StartConversationAsync(CreateAIConversationDto dto);
        Task<AIConversationDto?> GetConversationAsync(int conversationId);
        Task<AIChatMessageDto> SendMessageAsync(CreateAIChatMessageDto dto);
        Task<IReadOnlyList<AIChatMessageDto>> GetMessagesAsync(int conversationId);
}
