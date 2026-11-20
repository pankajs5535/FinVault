using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CardTransactionDtos;

/// <summary>Input for creating a CardTransaction.</summary>
public class CreateCardTransactionDto
{
    [StringLength(20)]
    public string? AuthorizationCode { get; set; }

    [StringLength(12)]
    public string? RetrievalReferenceNumber { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Authorized|Declined|Reversed|Settled|Expired)$", ErrorMessage = "AuthorizationStatus must be one of: Authorized, Declined, Reversed, Settled, Expired.")]
    public string AuthorizationStatus { get; set; } = "Authorized";

    [StringLength(100)]
    public string? DeclineReason { get; set; }

    public DateTime AuthorizedAt { get; set; }

    [Required]
    [StringLength(150)]
    public string MerchantName { get; set; } = string.Empty;

    [Required]
    [StringLength(4)]
    public string MerchantCategoryCode { get; set; } = string.Empty;

    [StringLength(100)]
    public string? MerchantCity { get; set; }

    [StringLength(20)]
    public string? TerminalId { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(POS|ATM|Ecommerce|Contactless|Recurring)$", ErrorMessage = "Channel must be one of: POS, ATM, Ecommerce, Contactless, Recurring.")]
    public string Channel { get; set; } = "POS";

    [Range(0, double.MaxValue)]
    public decimal Amount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SettlementAmount { get; set; }

    public DateTime? SettledAt { get; set; }

    [Range(1, int.MaxValue)]
    public int CardId { get; set; }

    [Range(1, int.MaxValue)]
    public int? TransactionId { get; set; }

    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }
}
