namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    /// <summary>
    /// Returns the result of a UPI bank-to-bank transfer.
    /// </summary>
    public class UpiTransferResponseDto
    {
        // 1. PAYMENT IDENTIFICATION
        // Identifies the payment request created by FinVault.
        public int PaymentRequestId { get; set; }
        public string RequestReference { get; set; } = string.Empty;

        // 2. TRANSACTION IDENTIFICATION
        // Identifies the completed or attempted banking transaction.
        public int? TransactionId { get; set; }
        public string? UtrNumber { get; set; }
        public string? ExternalReference { get; set; }

        // 3. TRANSFER INFORMATION
        // Shows the amount and purpose of the UPI transfer.
        public decimal Amount { get; set; }
        public string? Purpose { get; set; }
        public string? Remarks { get; set; }

        // 4. TRANSFER STATUS
        // Indicates whether the transfer is pending, successful or failed.
        public string Status { get; set; } = string.Empty;
        public string? FailureCode { get; set; }
        public string? FailureReason { get; set; }

        // 5. TRANSFER TIMESTAMPS
        // Helps customers track when the transfer was requested and completed.
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}