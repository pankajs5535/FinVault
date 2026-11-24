using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.CardDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Card banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface ICardService
{
        Task<CardDto?> GetByIdAsync(int cardId);
        Task<IReadOnlyList<CardDto>> GetByCustomerIdAsync(int customerId);
        Task<CardDto> IssueAsync(CreateCardDto dto);
        Task<CardDto?> UpdateAsync(int cardId, UpdateCardDto dto);
        Task<bool> BlockAsync(int cardId, string reason);
        Task<bool> UnblockAsync(int cardId);
}
