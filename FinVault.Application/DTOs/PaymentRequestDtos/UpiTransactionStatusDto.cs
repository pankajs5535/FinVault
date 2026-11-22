namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    public class UpiTransactionStatusDto
    {
        // 1. TRANSACTION IDENTIFICATION
        public int PaymentRequestId { get; set; }
        public string RequestReference { get; set; } = string.Empty;
        public string? UtrNumber { get; set; }

        // 2. TRANSFER INFORMATION
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "INR";

        // 3. CURRENT STATUS
        public string Status { get; set; } = string.Empty;
        public string? FailureCode { get; set; }
        public string? FailureReason { get; set; }

        // 4. STATUS TIMESTAMPS
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? LastCheckedAt { get; set; }
    }
}