using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.BeneficiaryDtos;

/// <summary>Input for updating a Beneficiary.</summary>
public class UpdateBeneficiaryDto
{
    [Range(1, int.MaxValue)]
    public int BeneficiaryId { get; set; }

    [Required]
    [StringLength(150)]
    public string BeneficiaryName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Nickname { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(IntraBank|InterBank|UPI)$", ErrorMessage = "BeneficiaryType must be one of: IntraBank, InterBank, UPI.")]
    public string BeneficiaryType { get; set; } = "InterBank";

    [StringLength(20)]
    public string? AccountNumber { get; set; }

    [StringLength(11)]
    [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code.")]
    public string? IfscCode { get; set; }

    [StringLength(150)]
    public string? BankName { get; set; }

    [StringLength(150)]
    public string? BranchName { get; set; }

    [StringLength(100)]
    public string? UpiVpa { get; set; }

    [StringLength(15)]
    [Phone]
    public string? MobileNumber { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Pending|Active|Blocked|Removed)$", ErrorMessage = "Status must be one of: Pending, Active, Blocked, Removed.")]
    public string Status { get; set; } = "Pending";

    public DateTime? ActivatedAt { get; set; }

    public DateTime? CoolingPeriodEndsAt { get; set; }

    public bool IsFavorite { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? PerTransactionLimit { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? InternalAccountId { get; set; }

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
