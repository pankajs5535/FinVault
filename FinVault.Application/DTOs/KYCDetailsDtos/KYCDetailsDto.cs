using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.KYCDetailsDtos;

/// <summary>Returned to clients for Get All and Get By Id of KYCDetails.</summary>
public class KYCDetailsDto
{
    public int KYCDetailsId { get; set; }

    public string KycLevel { get; set; } = string.Empty;

    public string VerificationStatus { get; set; } = string.Empty;

    public bool IsCurrent { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime? NextReKycDueDate { get; set; }

    public string? PanMasked { get; set; }

    public bool IsPanVerified { get; set; }

    public DateTime? PanVerifiedAt { get; set; }

    public bool IsForm60Submitted { get; set; }

    public string? AadhaarLast4 { get; set; }

    public string? AadhaarVerificationMethod { get; set; }

    public bool IsAadhaarVerified { get; set; }

    public DateTime? AadhaarVerifiedAt { get; set; }

    public string? CkycNumber { get; set; }

    public string CkycStatus { get; set; } = string.Empty;

    public DateTime? CkycVerifiedAt { get; set; }

    public int CustomerId { get; set; }

    public int? VerifiedByEmployeeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }
}
