using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.LoanDtos;

/// <summary>Input for updating a Loan.</summary>
public class UpdateLoanDto
{
    [Range(1, int.MaxValue)]
    public int LoanId { get; set; }

    [Required]
    [StringLength(20)]
    public string LoanNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Personal|Home|Vehicle|Education|Business|Gold|AgainstDeposit)$", ErrorMessage = "LoanType must be one of: Personal, Home, Vehicle, Education, Business, Gold, AgainstDeposit.")]
    public string LoanType { get; set; } = "Personal";

    [StringLength(250)]
    public string? Purpose { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Applied|Approved|Rejected|Disbursed|Active|Closed|WrittenOff)$", ErrorMessage = "Status must be one of: Applied, Approved, Rejected, Disbursed, Active, Closed, WrittenOff.")]
    public string Status { get; set; } = "Applied";

    [Range(0, double.MaxValue)]
    public decimal RequestedAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SanctionedAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? EmiAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AnnualInterestRate { get; set; }

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Fixed|Floating)$", ErrorMessage = "InterestType must be one of: Fixed, Floating.")]
    public string InterestType { get; set; } = "Fixed";

    [Range(0, int.MaxValue)]
    public int TenureMonths { get; set; }

    public DateTime ApplicationDate { get; set; }

    public DateTime? SanctionDate { get; set; }

    public DateTime? DisbursementDate { get; set; }

    public DateTime? FirstEmiDate { get; set; }

    public DateTime? MaturityDate { get; set; }

    public DateTime? ClosedOn { get; set; }

    [Required]
    [StringLength(15)]
    [RegularExpression(@"^(Standard|SMA0|SMA1|SMA2|SubStandard|Doubtful|Loss)$", ErrorMessage = "AssetClassification must be one of: Standard, SMA0, SMA1, SMA2, SubStandard, Doubtful, Loss.")]
    public string AssetClassification { get; set; } = "Standard";

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int BranchId { get; set; }

    [Range(1, int.MaxValue)]
    public int? DisbursementAccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? RepaymentAccountId { get; set; }

    [Range(1, int.MaxValue)]
    public int? InterestRateId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ApprovedByEmployeeId { get; set; }

    /// <summary>Concurrency token returned by the last read.</summary>
    [Required]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
