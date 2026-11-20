using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.CardDtos;

/// <summary>Input for deleting a Card.</summary>
public class DeleteCardDto
{
    [Range(1, int.MaxValue)]
    public int CardId { get; set; }
}
