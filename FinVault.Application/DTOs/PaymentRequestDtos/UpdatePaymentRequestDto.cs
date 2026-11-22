using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    /// <summary>
    /// Accepts permitted changes to a payment request before processing.
    /// Payment lifecycle information remains controlled by the backend.
    /// </summary>
    public class UpdatePaymentRequestDto
    {
        // 1. PAYMENT IDENTIFICATION
        [Range(1, int.MaxValue)]
        public int PaymentRequestId { get; set; }

        // 2. PAYEE INFORMATION
        // Allows correction of payee details before payment submission.
        [StringLength(150)]
        public string? PayeeName { get; set; }

        [StringLength(20)]
        public string? PayeeAccountNumber { get; set; }

        [StringLength(11)]
        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code.")]
        public string? PayeeIfsc { get; set; }

        [StringLength(100)]
        public string? PayeeVpa { get; set; }

        // 3. PAYMENT INFORMATION
        // Allows modification of amount while the request is still editable.
        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal Amount { get; set; }

        [StringLength(250)]
        public string? Purpose { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        // 4. PAYMENT SCHEDULING
        public DateTime? ScheduledFor { get; set; }

        // 5. CONCURRENCY CONTROL
        // Prevents overwriting another update made to the same payment request.
        [Required]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}