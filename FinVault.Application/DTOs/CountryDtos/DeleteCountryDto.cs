using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CountryDtos;

/// <summary>Input for deleting a Country.</summary>
public class DeleteCountryDto
{
    [Range(1, int.MaxValue)]
    public int CountryId { get; set; }
}
