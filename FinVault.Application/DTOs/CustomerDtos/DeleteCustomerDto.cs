using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CustomerDtos;

/// <summary>Input for deleting a Customer.</summary>
public class DeleteCustomerDto
{
    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }
}
