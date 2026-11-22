using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    /// <summary>
    /// Input for initiating a UPI bank-to-bank transfer.
    /// </summary>
    public class InitiateUpiTransferDto
    {
        // 1. SOURCE BANK ACCOUNT
        // Identifies the account from which money will be debited.
        [Range(1, int.MaxValue)]
        public int SourceAccountId { get; set; }

        // 2. BENEFICIARY INFORMATION
        // Identifies the saved beneficiary receiving the UPI payment.
        [Range(1, int.MaxValue)]
        public int BeneficiaryId { get; set; }

        // 3. TRANSFER INFORMATION
        // Amount to transfer. The service must enforce UPI and account limits.
        [Range(typeof(decimal), "0.01", "100000000")]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? Purpose { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        // 4. IDEMPOTENCY
        // Prevents duplicate transfers when the client retries a request.
        [Required]
        [StringLength(64, MinimumLength = 16)]
        public string IdempotencyKey { get; set; } = string.Empty;
    }
}