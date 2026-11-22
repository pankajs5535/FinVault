using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    public class VerifyUpiVpaDto
    {
        // 1. UPI IDENTIFICATION
        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9._-]{2,256}@[A-Za-z0-9.-]{2,64}$")]
        public string UpiVpa { get; set; } = string.Empty;
    }
}