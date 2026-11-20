using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.DocumentDtos;

/// <summary>Input for updating a Document.</summary>
public class UpdateDocumentDto
{
    [Range(1, int.MaxValue)]
    public int DocumentId { get; set; }

    [Required]
    [StringLength(30)]
    [RegularExpression(@"^(Pan|Aadhaar|Passport|VoterId|DrivingLicence|Photo|Signature|AddressProof|IncomeProof|Form60|LoanAgreement|Other)$", ErrorMessage = "DocumentType must be one of: Pan, Aadhaar, Passport, VoterId, DrivingLicence, Photo, Signature, AddressProof, IncomeProof, Form60, LoanAgreement, Other.")]
    public string DocumentType { get; set; } = "Other";

    [StringLength(30)]
    public string? DocumentNumberMasked { get; set; }

    public DateTime? IssueDate { get; set; }

    public DateTime? ExpiryDate { get; set; }

    [Required]
    [StringLength(260)]
    public string FileName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ContentType { get; set; } = string.Empty;

    [Range(0, long.MaxValue)]
    public long FileSizeBytes { get; set; }

    public bool IsEncrypted { get; set; } = true;

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Pending|Verified|Rejected|Expired)$", ErrorMessage = "VerificationStatus must be one of: Pending, Verified, Rejected, Expired.")]
    public string VerificationStatus { get; set; } = "Pending";

    public DateTime? VerifiedAt { get; set; }

    [StringLength(500)]
    public string? RejectionReason { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? KYCDetailsId { get; set; }

    [Range(1, int.MaxValue)]
    public int? LoanId { get; set; }

    [Range(1, int.MaxValue)]
    public int? VerifiedByEmployeeId { get; set; }
}
