using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentMethodDtos;

/// <summary>Returned to clients for Get All and Get By Id of PaymentMethod.</summary>
public class PaymentMethodDto
{
    public int PaymentMethodId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Rail { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    public decimal? DailyLimit { get; set; }

    public bool Is24x7 { get; set; }

    public string SettlementType { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
