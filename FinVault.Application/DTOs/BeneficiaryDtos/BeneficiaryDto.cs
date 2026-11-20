using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.BeneficiaryDtos;

/// <summary>Returned to clients for Get All and Get By Id of Beneficiary.</summary>
public class BeneficiaryDto
{
    public int BeneficiaryId { get; set; }

    public string BeneficiaryName { get; set; } = string.Empty;

    public string? Nickname { get; set; }

    public string BeneficiaryType { get; set; } = string.Empty;

    public string? AccountNumber { get; set; }

    public string? IfscCode { get; set; }

    public string? BankName { get; set; }

    public string? BranchName { get; set; }

    public string? UpiVpa { get; set; }

    public string? MobileNumber { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime? ActivatedAt { get; set; }

    public DateTime? CoolingPeriodEndsAt { get; set; }

    public bool IsFavorite { get; set; }

    public decimal? PerTransactionLimit { get; set; }

    public int CustomerId { get; set; }

    public int? InternalAccountId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
