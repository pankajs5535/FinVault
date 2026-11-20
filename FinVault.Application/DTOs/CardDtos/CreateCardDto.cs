using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CardDtos;

/// <summary>Input for creating a Card.</summary>
public class CreateCardDto
{
    [Required]
    [StringLength(19)]
    public string CardNumberMasked { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string CardHolderName { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Debit|Credit|Prepaid)$", ErrorMessage = "CardType must be one of: Debit, Credit, Prepaid.")]
    public string CardType { get; set; } = "Debit";

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(RuPay|Visa|Mastercard)$", ErrorMessage = "Network must be one of: RuPay, Visa, Mastercard.")]
    public string Network { get; set; } = "RuPay";

    public DateTime ExpiryDate { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Pending|Active|Blocked|Hotlisted|Expired|Cancelled)$", ErrorMessage = "Status must be one of: Pending, Active, Blocked, Hotlisted, Expired, Cancelled.")]
    public string Status { get; set; } = "Pending";

    public DateTime? IssuedAt { get; set; }

    public DateTime? ActivatedAt { get; set; }

    [StringLength(250)]
    public string? BlockReason { get; set; }

    public bool IsContactlessEnabled { get; set; } = true;

    public bool IsOnlineEnabled { get; set; } = true;

    public bool IsAtmEnabled { get; set; } = true;

    public bool IsPosEnabled { get; set; } = true;

    [Range(0, double.MaxValue)]
    public decimal DailyAtmLimit { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DailyPosLimit { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DailyOnlineLimit { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? CreditLimit { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? AvailableCredit { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? OutstandingAmount { get; set; }

    [Range(0, int.MaxValue)]
    public int? BillingCycleDay { get; set; }

    [Range(1, int.MaxValue)]
    public int AccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }
}
