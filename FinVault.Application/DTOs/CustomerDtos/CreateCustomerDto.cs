using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CustomerDtos;

/// <summary>Input for creating a Customer.</summary>
public class CreateCustomerDto
{
    [Required]
    [StringLength(20)]
    public string CustomerNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Individual|Minor|Business)$", ErrorMessage = "CustomerType must be one of: Individual, Minor, Business.")]
    public string CustomerType { get; set; } = "Individual";

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [StringLength(10)]
    [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be one of: Male, Female, Other.")]
    public string? Gender { get; set; }

    [StringLength(200)]
    public string? FatherOrSpouseName { get; set; }

    [Required]
    [StringLength(15)]
    [Phone]
    public string MobileNumber { get; set; } = string.Empty;

    [StringLength(15)]
    [Phone]
    public string? AlternateMobileNumber { get; set; }

    [StringLength(256)]
    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(100)]
    public string? Occupation { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? AnnualIncome { get; set; }

    [Required]
    [StringLength(10)]
    public string PreferredLanguage { get; set; } = "en";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Active|Inactive|Dormant|Blocked|Closed)$", ErrorMessage = "Status must be one of: Active, Inactive, Dormant, Blocked, Closed.")]
    public string Status { get; set; } = "Active";

    [Required]
    [StringLength(10)]
    [RegularExpression(@"^(Low|Medium|High)$", ErrorMessage = "RiskCategory must be one of: Low, Medium, High.")]
    public string RiskCategory { get; set; } = "Low";

    public DateTime OnboardedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    [Range(1, int.MaxValue)]
    public int? UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int HomeBranchId { get; set; }

    [Range(1, int.MaxValue)]
    public int? RelationshipManagerId { get; set; }

    [Range(1, int.MaxValue)]
    public int NationalityCountryId { get; set; }
}
