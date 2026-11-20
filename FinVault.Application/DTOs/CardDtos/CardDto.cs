using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CardDtos;

/// <summary>Returned to clients for Get All and Get By Id of Card.</summary>
public class CardDto
{
    public int CardId { get; set; }

    public string CardNumberMasked { get; set; } = string.Empty;

    public string CardHolderName { get; set; } = string.Empty;

    public string CardType { get; set; } = string.Empty;

    public string Network { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? IssuedAt { get; set; }

    public DateTime? ActivatedAt { get; set; }

    public DateTime? BlockedAt { get; set; }

    public string? BlockReason { get; set; }

    public bool IsContactlessEnabled { get; set; }

    public bool IsOnlineEnabled { get; set; }

    public bool IsAtmEnabled { get; set; }

    public bool IsPosEnabled { get; set; }

    public decimal DailyAtmLimit { get; set; }

    public decimal DailyPosLimit { get; set; }

    public decimal DailyOnlineLimit { get; set; }

    public DateTime? PinLastSetAt { get; set; }

    public int PinRetryCount { get; set; }

    public DateTime? PinBlockedUntil { get; set; }

    public decimal? CreditLimit { get; set; }

    public decimal? AvailableCredit { get; set; }

    public decimal? OutstandingAmount { get; set; }

    public int? BillingCycleDay { get; set; }

    public int AccountId { get; set; }

    public int CustomerId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
