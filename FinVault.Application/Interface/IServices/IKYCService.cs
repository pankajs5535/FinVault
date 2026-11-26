using FinVault.Application.DTOs.KYCDetailsDtos;

namespace FinVault.Application.Interfaces.Services
{
    public interface IKYCService
    {
        // 1. KYC IDENTIFICATION AND CRUD
        Task<IEnumerable<KYCDetailsDto>> GetAllAsync();
        Task<KYCDetailsDto?> GetByIdAsync(int kycDetailsId);
        Task<KYCDetailsDto?> GetKYCDetailsAsync(int kycDetailsId);
        Task<KYCDetailsDto?> GetCurrentKYCByCustomerIdAsync(int customerId);
        Task<IEnumerable<KYCDetailsDto>> GetKYCHistoryByCustomerIdAsync(int customerId);
        Task<KYCDetailsDto> CreateAsync(CreateKYCDetailsDto dto);
        Task<KYCDetailsDto?> UpdateAsync(UpdateKYCDetailsDto dto);
        Task<bool> DeleteAsync(DeleteKYCDetailsDto dto);
        Task<bool> HasCurrentKYCRecordAsync(int customerId);

        // 2. KYC LEVEL AND VERIFICATION STATUS
        Task<IEnumerable<KYCDetailsDto>> GetByKycLevelAsync(string kycLevel);
        Task<IEnumerable<KYCDetailsDto>> GetByVerificationStatusAsync(string verificationStatus);
        Task<IEnumerable<KYCDetailsDto>> GetPendingKYCRecordsAsync();
        Task<IEnumerable<KYCDetailsDto>> GetInProgressKYCRecordsAsync();
        Task<IEnumerable<KYCDetailsDto>> GetVerifiedKYCRecordsAsync();
        Task<IEnumerable<KYCDetailsDto>> GetRejectedKYCRecordsAsync();
        Task<IEnumerable<KYCDetailsDto>> GetExpiredKYCRecordsAsync();

        // 3. PAN VERIFICATION AND DUPLICATE DETECTION
        Task<KYCDetailsDto?> GetCurrentByPanAsync(string pan);
        Task<bool> ExistsCurrentByPanAsync(string pan);
        Task<IEnumerable<KYCDetailsDto>> GetPanVerifiedRecordsAsync();
        Task<IEnumerable<KYCDetailsDto>> GetPanPendingVerificationRecordsAsync();
        Task<bool> VerifyPanAsync(int kycDetailsId);

        // 4. AADHAAR AND CKYC MANAGEMENT
        Task<IEnumerable<KYCDetailsDto>> GetAadhaarVerifiedRecordsAsync();
        Task<IEnumerable<KYCDetailsDto>> GetByAadhaarVerificationMethodAsync(string verificationMethod);
        Task<KYCDetailsDto?> GetByCkycNumberAsync(string ckycNumber);
        Task<IEnumerable<KYCDetailsDto>> GetByCkycStatusAsync(string ckycStatus);
        Task<bool> UpdateCkycStatusAsync(int kycDetailsId, string ckycStatus);

        // 5. RE-KYC AND VERIFICATION DEADLINES
        Task<IEnumerable<KYCDetailsDto>> GetOverdueReKYCRecordsAsync(DateTime currentDate);
        Task<IEnumerable<KYCDetailsDto>> GetReKYCRecordsDueWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<bool> UpdateReKycDueDateAsync(int kycDetailsId, DateTime nextReKycDueDate);

        // 6. EMPLOYEE VERIFICATION AND DOCUMENTS
        Task<IEnumerable<KYCDetailsDto>> GetByVerifiedByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<KYCDetailsDto>> GetKYCRecordsWithDocumentsAsync(int customerId);
        Task<bool> VerifyKYCAsync(int kycDetailsId, int employeeId);
        Task<bool> RejectKYCAsync(int kycDetailsId, int employeeId, string rejectionReason);

        // 7. DATE, SEARCH AND REPORTING
        Task<IEnumerable<KYCDetailsDto>> GetVerifiedWithinRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> CountCurrentRecordsByStatusAsync(string verificationStatus);
    }
}


//Banking security rules:
//-Encrypt PAN using a managed key and store a keyed hash for duplicate detection.
//- Never store the full Aadhaar number; retain only the vault reference and last four digits.
//- Only one KYC record per customer can be current.
//- Restrict verification and rejection operations to authorized employees.
//- Preserve KYC history and verification audit records.