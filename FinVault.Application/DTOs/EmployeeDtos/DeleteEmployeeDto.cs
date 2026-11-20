using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.EmployeeDtos;

/// <summary>Input for deleting an Employee.</summary>
public class DeleteEmployeeDto
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }
}
