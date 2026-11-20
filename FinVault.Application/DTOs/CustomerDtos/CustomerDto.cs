using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CustomerDtos;

/// <summary>Returned to clients for Get All and Get By Id of Customer.</summary>
public class CustomerDto
{
    public int CustomerId { get; set; }

    public string CustomerNumber { get; set; } = string.Empty;

    public string CustomerType { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? FatherOrSpouseName { get; set; }

    public string MobileNumber { get; set; } = string.Empty;

    public string? AlternateMobileNumber { get; set; }

    public string? Email { get; set; }

    public string? Occupation { get; set; }

    public decimal? AnnualIncome { get; set; }

    public string PreferredLanguage { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string RiskCategory { get; set; } = string.Empty;

    public DateTime OnboardedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public int? UserId { get; set; }

    public int HomeBranchId { get; set; }

    public int? RelationshipManagerId { get; set; }

    public int NationalityCountryId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
