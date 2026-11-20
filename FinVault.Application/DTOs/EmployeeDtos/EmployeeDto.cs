using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.EmployeeDtos;

/// <summary>Returned to clients for Get All and Get By Id of Employee.</summary>
public class EmployeeDto
{
    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public DateTime DateOfJoining { get; set; }

    public DateTime? DateOfLeaving { get; set; }

    public string EmploymentStatus { get; set; } = string.Empty;

    public decimal? ApprovalLimit { get; set; }

    public int UserId { get; set; }

    public int BranchId { get; set; }

    public int? ReportsToEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
