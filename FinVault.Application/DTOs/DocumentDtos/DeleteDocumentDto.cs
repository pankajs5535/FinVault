using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.DocumentDtos;

/// <summary>Input for deleting a Document.</summary>
public class DeleteDocumentDto
{
    [Range(1, int.MaxValue)]
    public int DocumentId { get; set; }
}
