using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.EmployeeDtos;

/// <summary>Input for creating an Employee.</summary>
public class CreateEmployeeDto
{
    [Required]
    [StringLength(20)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Designation { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [StringLength(15)]
    [Phone]
    public string? PhoneNumber { get; set; }

    public DateTime DateOfJoining { get; set; }

    public DateTime? DateOfLeaving { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Active|OnLeave|Suspended|Resigned|Terminated)$", ErrorMessage = "EmploymentStatus must be one of: Active, OnLeave, Suspended, Resigned, Terminated.")]
    public string EmploymentStatus { get; set; } = "Active";

    [Range(0, double.MaxValue)]
    public decimal? ApprovalLimit { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int BranchId { get; set; }

    [Range(1, int.MaxValue)]
    public int? ReportsToEmployeeId { get; set; }
}
