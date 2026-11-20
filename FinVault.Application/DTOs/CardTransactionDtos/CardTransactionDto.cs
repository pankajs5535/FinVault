using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CardTransactionDtos;

/// <summary>Returned to clients for Get All and Get By Id of CardTransaction.</summary>
public class CardTransactionDto
{
    public int CardTransactionId { get; set; }

    public string? AuthorizationCode { get; set; }

    public string? RetrievalReferenceNumber { get; set; }

    public string AuthorizationStatus { get; set; } = string.Empty;

    public string? DeclineReason { get; set; }

    public DateTime AuthorizedAt { get; set; }

    public string MerchantName { get; set; } = string.Empty;

    public string MerchantCategoryCode { get; set; } = string.Empty;

    public string? MerchantCity { get; set; }

    public string? TerminalId { get; set; }

    public string Channel { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public decimal? SettlementAmount { get; set; }

    public DateTime? SettledAt { get; set; }

    public int CardId { get; set; }

    public int? TransactionId { get; set; }

    public int CurrencyId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
