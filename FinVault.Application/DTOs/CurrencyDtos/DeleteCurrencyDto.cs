using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CurrencyDtos;

/// <summary>Input for deleting a Currency.</summary>
public class DeleteCurrencyDto
{
    [Range(1, int.MaxValue)]
    public int CurrencyId { get; set; }
}
