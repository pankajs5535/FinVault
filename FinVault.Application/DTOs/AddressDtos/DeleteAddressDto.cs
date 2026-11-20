using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.AddressDtos;

/// <summary>Input for deleting an Address.</summary>
public class DeleteAddressDto
{
    [Range(1, int.MaxValue)]
    public int AddressId { get; set; }
}
