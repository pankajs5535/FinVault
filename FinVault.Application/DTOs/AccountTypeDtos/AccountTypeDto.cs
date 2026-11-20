using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountTypeDtos;

/// <summary>Returned to clients for Get All and Get By Id of AccountType.</summary>
public class AccountTypeDto
{
    public int AccountTypeId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal MinimumBalance { get; set; }

    public decimal MinimumAverageBalance { get; set; }

    public decimal? DailyTransactionLimit { get; set; }

    public bool AllowsOverdraft { get; set; }

    public bool IsInterestBearing { get; set; }

    public string? InterestPayoutFrequency { get; set; }

    public int? MinTenureMonths { get; set; }

    public int? MaxTenureMonths { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
