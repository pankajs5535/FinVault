using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.BranchDtos;

/// <summary>Returned to clients for Get All and Get By Id of Branch.</summary>
public class BranchDto
{
    public int BranchId { get; set; }

    public string BranchCode { get; set; } = string.Empty;

    public string BranchName { get; set; } = string.Empty;

    public string IfscCode { get; set; } = string.Empty;

    public string? MicrCode { get; set; }

    public string BranchType { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string District { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PinCode { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; }

    public DateTime OpenedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    public int CountryId { get; set; }

    public int? ParentBranchId { get; set; }

    public int? ManagerEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
