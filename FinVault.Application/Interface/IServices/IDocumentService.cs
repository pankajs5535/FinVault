using FinVault.Application.DTOs.DocumentDtos;
using static System.Net.WebRequestMethods;

namespace FinVault.Application.Interfaces.Services
{
    public interface IDocumentService
    {
        // 1. DOCUMENT IDENTIFICATION AND CRUD
        Task<IEnumerable<DocumentDto>> GetAllAsync();
        Task<DocumentDto?> GetByIdAsync(int documentId);
        Task<DocumentDto?> GetDocumentDetailsAsync(int documentId);
        Task<DocumentDto?> GetByContentHashAsync(string contentHash);
        Task<DocumentDto> CreateAsync(CreateDocumentDto dto);
        Task<DocumentDto?> UpdateAsync(UpdateDocumentDto dto);
        Task<bool> DeleteAsync(DeleteDocumentDto dto);

        // 2. CUSTOMER DOCUMENT MANAGEMENT
        Task<IEnumerable<DocumentDto>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<DocumentDto>> GetByCustomerAndDocumentTypeAsync(
            int customerId,
            string documentType);
        Task<IEnumerable<DocumentDto>> GetCustomerDocumentsWithVerificationDetailsAsync(
            int customerId);
        Task<int> CountDocumentsByCustomerIdAsync(int customerId);

        // 3. KYC AND LOAN DOCUMENT MANAGEMENT
        Task<IEnumerable<DocumentDto>> GetByKYCDetailsIdAsync(int kycDetailsId);
        Task<IEnumerable<DocumentDto>> GetByLoanIdAsync(int loanId);
        Task<IEnumerable<DocumentDto>> GetByVerifiedByEmployeeIdAsync(int employeeId);

        // 4. DOCUMENT VERIFICATION
        Task<IEnumerable<DocumentDto>> GetByVerificationStatusAsync(string verificationStatus);
        Task<IEnumerable<DocumentDto>> GetPendingVerificationDocumentsAsync();
        Task<IEnumerable<DocumentDto>> GetRejectedDocumentsAsync();
        Task<IEnumerable<DocumentDto>> GetVerifiedDocumentsByCustomerIdAsync(int customerId);

        // 5. DOCUMENT EXPIRY MANAGEMENT
        Task<IEnumerable<DocumentDto>> GetExpiredDocumentsAsync(DateTime currentDate);
        Task<IEnumerable<DocumentDto>> GetDocumentsExpiringWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 6. FILE STORAGE AND SECURITY
        Task<IEnumerable<DocumentDto>> GetUnencryptedDocumentsAsync();
        Task<bool> ExistsByContentHashAsync(string contentHash);

        // 7. SEARCH AND REPORTING
        Task<IEnumerable<DocumentDto>> SearchDocumentsAsync(string searchTerm);
    }
}


 
//Banking security rules:
//-Store files in encrypted object storage; keep only the storage path and SHA-256 hash in the database.
//- Validate file type and size before storage.
//- Never expose the internal storage path to unauthorized clients.
//- Preserve verification status and employee audit details.
//- Restrict document access according to customer ownership and employee permissions.