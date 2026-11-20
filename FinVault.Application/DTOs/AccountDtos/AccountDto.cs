using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AccountDtos;

/// <summary>Returned to clients for Get All and Get By Id of Account.</summary>
public class AccountDto
{
    public int AccountId { get; set; }

    public string AccountNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? StatusReason { get; set; }

    public string ModeOfOperation { get; set; } = string.Empty;

    public decimal Balance { get; set; }

    public decimal AvailableBalance { get; set; }

    public decimal HoldAmount { get; set; }

    public decimal OverdraftLimit { get; set; }

    public DateTime OpenedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    public DateTime? LastTransactionAt { get; set; }

    public string? NomineeName { get; set; }

    public string? NomineeRelationship { get; set; }

    public decimal? PrincipalAmount { get; set; }

    public decimal? MaturityAmount { get; set; }

    public decimal? ContractedInterestRate { get; set; }

    public int? TenureMonths { get; set; }

    public DateTime? MaturityDate { get; set; }

    public bool AutoRenew { get; set; }

    public int AccountTypeId { get; set; }

    public int BranchId { get; set; }

    public int CurrencyId { get; set; }

    public int? PayoutAccountId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
