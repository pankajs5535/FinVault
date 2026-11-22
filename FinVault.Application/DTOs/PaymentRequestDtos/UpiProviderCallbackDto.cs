using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    public class UpiProviderCallbackDto
    {
        // 1. PROVIDER EVENT IDENTIFICATION
        [Required]
        [StringLength(100)]
        public string ProviderEventId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ProviderReference { get; set; } = string.Empty;

        // 2. FINVAULT PAYMENT IDENTIFICATION
        [Required]
        [StringLength(40)]
        public string RequestReference { get; set; } = string.Empty;

        // 3. PAYMENT RESULT
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        [StringLength(30)]
        public string? UtrNumber { get; set; }

        [StringLength(30)]
        public string? ResponseCode { get; set; }

        [StringLength(250)]
        public string? ResponseMessage { get; set; }

        // 4. PROVIDER EVENT TIMESTAMP
        public DateTime OccurredAt { get; set; }
    }
}