using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    /// <summary>
    /// Accepts customer input to initiate a payment request.
    /// Backend controls payment lifecycle and transaction information.
    /// </summary>
    public class CreatePaymentRequestDto
    {
        // 1. IDEMPOTENCY
        // Prevents duplicate payment requests when a customer retries.
        [Required]
        [StringLength(64, MinimumLength = 16)]
        public string IdempotencyKey { get; set; } = string.Empty;

        // 2. PAYEE INFORMATION
        // Supports a saved beneficiary or manually entered payee details.
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
        // Amount must be positive and within database precision.
        [Range(typeof(decimal), "0.01", "9999999999999999.99")]
        public decimal Amount { get; set; }

        [StringLength(250)]
        public string? Purpose { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        // 4. PAYMENT SCHEDULING
        // Allows the customer to request a future payment.
        public DateTime? ScheduledFor { get; set; }

        // 5. PAYMENT METHOD
        // Identifies the selected payment method.
        [Range(1, int.MaxValue)]
        public int PaymentMethodId { get; set; }

        // 6. SOURCE ACCOUNT
        // Identifies the customer's account to debit.
        [Range(1, int.MaxValue)]
        public int SourceAccountId { get; set; }

        // 7. BENEFICIARY
        // Optional when payee details are entered manually.
        [Range(1, int.MaxValue)]
        public int? BeneficiaryId { get; set; }
    }
}