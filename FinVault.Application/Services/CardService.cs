using AutoMapper;
using FinVault.Application.DTOs.CardDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Applies card ownership, lifecycle and status rules before persisting card changes.</summary>
public sealed class CardService : ICardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves a card with its related account and customer information.
    public async Task<CardDto?> GetByIdAsync(int cardId)
    {
        if (cardId <= 0) throw new ArgumentOutOfRangeException(nameof(cardId));
        var card = await _unitOfWork.Card.GetCardDetailsAsync(cardId);
        return card is null ? null : _mapper.Map<CardDto>(card);
    }

    // 2. Retrieves every card belonging to the specified customer.
    public async Task<IReadOnlyList<CardDto>> GetByCustomerIdAsync(int customerId)
    {
        if (customerId <= 0) throw new ArgumentOutOfRangeException(nameof(customerId));
        var cards = await _unitOfWork.Card.GetCardsByCustomerIdAsync(customerId);
        return _mapper.Map<IReadOnlyList<CardDto>>(cards);
    }

    // 3. Issues a card only when the customer owns an active account.
    public async Task<CardDto> IssueAsync(CreateCardDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.AccountId <= 0 || dto.CustomerId <= 0) throw new ArgumentException("A valid account and customer are required.");
        if (dto.ExpiryDate.Date <= DateTime.UtcNow.Date) throw new ArgumentException("Card expiry date must be in the future.");

        var account = await _unitOfWork.Account.GetAccountDetailsAsync(dto.AccountId);
        if (account is null || account.Status != "Active") throw new InvalidOperationException("The account does not exist or is not active.");
        var customerAccounts = await _unitOfWork.Account.GetAccountsByCustomerIdAsync(dto.CustomerId);
        if (!customerAccounts.Any(x => x.AccountId == dto.AccountId)) throw new InvalidOperationException("The account does not belong to this customer.");

        var card = _mapper.Map<Card>(dto);
        card.CardNumberToken = Guid.NewGuid().ToString("N"); // Internal placeholder token; integrate a PCI-compliant card issuer/token vault for real issuance.
        card.Status = "Pending";
        card.IssuedAt = DateTime.UtcNow;
        card.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Card.AddAsync(card);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CardDto>(card);
    }

    // 4. Updates editable card settings while preventing changes to ownership and lifecycle state.
    public async Task<CardDto?> UpdateAsync(int cardId, UpdateCardDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (cardId <= 0 || dto.CardId != cardId) throw new ArgumentException("Route card ID and DTO card ID must match.");
        var card = await _unitOfWork.Card.GetByIdAsync(cardId);
        if (card is null) return null;
        if (card.Status is "Blocked" or "Hotlisted" or "Cancelled" or "Expired") throw new InvalidOperationException("This card cannot be edited in its current status.");

        _mapper.Map(dto, card);
        card.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Card.Update(card);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CardDto>(card);
    }

    // 5. Blocks a card and records the reason for the security/audit trail.
    public async Task<bool> BlockAsync(int cardId, string reason)
    {
        if (cardId <= 0) throw new ArgumentOutOfRangeException(nameof(cardId));
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("A block reason is required.", nameof(reason));
        var card = await _unitOfWork.Card.GetByIdAsync(cardId);
        if (card is null) return false;
        if (card.Status is "Cancelled" or "Expired") throw new InvalidOperationException("Cancelled or expired cards cannot be blocked.");
        card.Status = "Blocked";
        card.BlockReason = reason.Trim();
        card.BlockedAt = DateTime.UtcNow;
        card.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Card.Update(card);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 6. Unblocks a blocked card; hotlisted, expired and cancelled cards remain restricted.
    public async Task<bool> UnblockAsync(int cardId)
    {
        if (cardId <= 0) throw new ArgumentOutOfRangeException(nameof(cardId));
        var card = await _unitOfWork.Card.GetByIdAsync(cardId);
        if (card is null) return false;
        if (card.Status != "Blocked") throw new InvalidOperationException("Only a blocked card can be unblocked.");
        if (card.ExpiryDate.Date <= DateTime.UtcNow.Date) throw new InvalidOperationException("An expired card cannot be unblocked.");
        card.Status = "Active";
        card.BlockReason = null;
        card.BlockedAt = null;
        card.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Card.Update(card);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
