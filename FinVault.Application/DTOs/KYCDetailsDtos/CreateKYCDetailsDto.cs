using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.KYCDetailsDtos;

/// <summary>Input for creating a KYCDetails.</summary>
public class CreateKYCDetailsDto
{
    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Minimum|Full|Video|EKyc)$", ErrorMessage = "KycLevel must be one of: Minimum, Full, Video, EKyc.")]
    public string KycLevel { get; set; } = "Minimum";

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(Pending|InProgress|Verified|Rejected|Expired)$", ErrorMessage = "VerificationStatus must be one of: Pending, InProgress, Verified, Rejected, Expired.")]
    public string VerificationStatus { get; set; } = "Pending";

    public bool IsCurrent { get; set; } = true;

    public DateTime? VerifiedAt { get; set; }

    [StringLength(500)]
    public string? RejectionReason { get; set; }

    public DateTime? NextReKycDueDate { get; set; }

    [StringLength(10)]
    public string? PanMasked { get; set; }

    public bool IsPanVerified { get; set; }

    public DateTime? PanVerifiedAt { get; set; }

    public bool IsForm60Submitted { get; set; }

    [StringLength(4)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Must be exactly 4 digits.")]
    public string? AadhaarLast4 { get; set; }

    [StringLength(20)]
    [RegularExpression(@"^(OtpEkyc|BiometricEkyc|OfflineXml|VirtualId)$", ErrorMessage = "AadhaarVerificationMethod must be one of: OtpEkyc, BiometricEkyc, OfflineXml, VirtualId.")]
    public string? AadhaarVerificationMethod { get; set; }

    public bool IsAadhaarVerified { get; set; }

    public DateTime? AadhaarVerifiedAt { get; set; }

    [StringLength(14)]
    public string? CkycNumber { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^(NotInitiated|Pending|Uploaded|Verified|Rejected)$", ErrorMessage = "CkycStatus must be one of: NotInitiated, Pending, Uploaded, Verified, Rejected.")]
    public string CkycStatus { get; set; } = "NotInitiated";

    public DateTime? CkycVerifiedAt { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Range(1, int.MaxValue)]
    public int? VerifiedByEmployeeId { get; set; }

    /// <summary>Plain PAN supplied once; the service layer encrypts it into PanEncrypted/PanHash. Never returned.</summary>
    [StringLength(10)]
    [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]$", ErrorMessage = "PAN must be in the format ABCDE1234F.")]
    public string? Pan { get; set; }
}
