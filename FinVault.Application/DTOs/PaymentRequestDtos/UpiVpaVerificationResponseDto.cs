namespace FinVault.Application.DTOs.PaymentRequestDtos
{
    public class UpiVpaVerificationResponseDto
    {
        // 1. UPI IDENTIFICATION
        public string UpiVpa { get; set; } = string.Empty;

        // 2. VERIFICATION RESULT
        public bool IsValid { get; set; }
        public string? BeneficiaryName { get; set; }

        // 3. PROVIDER INFORMATION
        public string? VerificationReference { get; set; }
        public string? Message { get; set; }

        // 4. VERIFICATION TIMESTAMP
        public DateTime? VerifiedAt { get; set; }
    }
}