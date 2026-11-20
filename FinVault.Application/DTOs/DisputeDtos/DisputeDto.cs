using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.DisputeDtos;

/// <summary>Returned to clients for Get All and Get By Id of Dispute.</summary>
public class DisputeDto
{
    public int DisputeId { get; set; }

    public string DisputeNumber { get; set; } = string.Empty;

    public string DisputeType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal DisputedAmount { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public DateTime RaisedAt { get; set; }

    public DateTime? DueBy { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? Resolution { get; set; }

    public string? ResolutionNotes { get; set; }

    public int TransactionId { get; set; }

    public int? CardTransactionId { get; set; }

    public int CustomerId { get; set; }

    public int? AssignedToEmployeeId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
