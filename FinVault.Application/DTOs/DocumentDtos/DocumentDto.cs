using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.DocumentDtos;

/// <summary>Returned to clients for Get All and Get By Id of Document.</summary>
public class DocumentDto
{
    public int DocumentId { get; set; }

    public string DocumentType { get; set; } = string.Empty;

    public string? DocumentNumberMasked { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public bool IsEncrypted { get; set; }

    public string VerificationStatus { get; set; } = string.Empty;

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public int CustomerId { get; set; }

    public int? KYCDetailsId { get; set; }

    public int? LoanId { get; set; }

    public int? VerifiedByEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
