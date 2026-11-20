using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.TransactionTypeDtos;

/// <summary>Returned to clients for Get All and Get By Id of TransactionType.</summary>
public class TransactionTypeDto
{
    public int TransactionTypeId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool RequiresApproval { get; set; }

    public decimal? ApprovalThreshold { get; set; }

    public decimal? DailyLimit { get; set; }

    public bool IsReversible { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
