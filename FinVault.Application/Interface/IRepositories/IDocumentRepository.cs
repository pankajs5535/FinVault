
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        // =========================================================
        // 1. DOCUMENT IDENTIFICATION
        // =========================================================

        // Retrieves a document using its unique database identifier.
        // Useful for opening a specific document record during KYC or loan processing.
        Task<Document?> GetDocumentDetailsAsync(int documentId);

        // Retrieves documents using their SHA-256 content hash.
        // Useful for detecting identical uploaded files and preventing unnecessary duplicate storage.
        Task<Document?> GetByContentHashAsync(string contentHash);

        // =========================================================
        // 2. CUSTOMER DOCUMENTS
        // =========================================================

        // Retrieves all documents belonging to a customer.
        // Useful for reviewing the customer's complete document history.
        Task<IEnumerable<Document>> GetByCustomerIdAsync(int customerId);

        // Retrieves documents of a particular type belonging to a customer.
        // Useful for locating specific KYC documents such as PAN or address proof.
        Task<IEnumerable<Document>> GetByCustomerAndDocumentTypeAsync(int customerId, string documentType);

        // Retrieves all documents belonging to a customer with their related verification employee.
        // Useful for customer service and compliance teams reviewing document ownership and verification.
        Task<IEnumerable<Document>> GetCustomerDocumentsWithVerificationDetailsAsync(int customerId);

        // =========================================================
        // 3. KYC AND LOAN DOCUMENTS
        // =========================================================

        // Retrieves documents associated with a particular KYC record.
        // Useful for checking whether the required KYC evidence has been submitted.
        Task<IEnumerable<Document>> GetByKYCDetailsIdAsync(int kycDetailsId);

        // Retrieves documents associated with a particular loan.
        // Useful for reviewing loan agreements, income proof and supporting documents.
        Task<IEnumerable<Document>> GetByLoanIdAsync(int loanId);

        // Retrieves documents verified by a particular employee.
        // Useful for reviewing an employee's document verification activity.
        Task<IEnumerable<Document>> GetByVerifiedByEmployeeIdAsync(int employeeId);

        // =========================================================
        // 4. VERIFICATION MANAGEMENT
        // =========================================================

        // Retrieves documents matching a particular verification status.
        // Useful for managing pending, verified, rejected and expired document queues.
        Task<IEnumerable<Document>> GetByVerificationStatusAsync(string verificationStatus);

        // Retrieves documents awaiting verification.
        // Useful for the KYC and compliance verification work queue.
        Task<IEnumerable<Document>> GetPendingVerificationDocumentsAsync();

        // Retrieves documents that have been rejected.
        // Useful for identifying documents requiring customer resubmission.
        Task<IEnumerable<Document>> GetRejectedDocumentsAsync();

        // Retrieves verified documents belonging to a customer.
        // Useful for checking which customer documents have passed verification.
        Task<IEnumerable<Document>> GetVerifiedDocumentsByCustomerIdAsync(int customerId);

        // =========================================================
        // 5. DOCUMENT EXPIRY MANAGEMENT
        // =========================================================

        // Retrieves documents whose expiry date has passed.
        // Useful for identifying expired identity and supporting documents.
        Task<IEnumerable<Document>> GetExpiredDocumentsAsync(DateTime currentDate);

        // Retrieves documents expiring within the supplied date range.
        // Useful for scheduling KYC refresh requests before documents expire.
        Task<IEnumerable<Document>> GetDocumentsExpiringWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 6. FILE AND STORAGE VALIDATION
        // =========================================================

        // Retrieves documents that are not marked as encrypted.
        // Useful for identifying records that require storage-security review.
        Task<IEnumerable<Document>> GetUnencryptedDocumentsAsync();

        // Checks whether a document with the supplied content hash already exists.
        // Useful for checking file duplication before saving a new document record.
        Task<bool> ExistsByContentHashAsync(string contentHash);

        // =========================================================
        // 7. SEARCH AND REPORTING
        // =========================================================

        // Searches documents by file name, masked document number or customer name.
        // Useful for authorized operations staff locating a document record.
        Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm);

        // Counts documents belonging to a particular customer.
        // Useful for monitoring document submission completeness.
        Task<int> CountDocumentsByCustomerIdAsync(int customerId);
    }
}
