
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. DOCUMENT IDENTIFICATION
        // =========================================================

        // Retrieves a document using its unique database identifier.
        // Useful for opening a specific document record during KYC or loan processing.
        public async Task<Document?> GetDocumentDetailsAsync(int documentId)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.KYCDetails)
                .Include(x => x.Loan)
                .Include(x => x.VerifiedByEmployee)
                .FirstOrDefaultAsync(x => x.DocumentId == documentId);
        }

        // Retrieves a document using its SHA-256 content hash.
        // Useful for detecting identical uploaded files and preventing unnecessary duplicate storage.
        public async Task<Document?> GetByContentHashAsync(string contentHash)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.ContentHash == contentHash);
        }

        // =========================================================
        // 2. CUSTOMER DOCUMENTS
        // =========================================================

        // Retrieves all documents belonging to a customer.
        // Useful for reviewing the customer's complete document history.
        public async Task<IEnumerable<Document>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves documents of a particular type belonging to a customer.
        // Useful for locating specific KYC documents such as PAN or address proof.
        public async Task<IEnumerable<Document>> GetByCustomerAndDocumentTypeAsync(int customerId, string documentType)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId && x.DocumentType == documentType)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves all documents belonging to a customer with their related verification employee.
        // Useful for customer service and compliance teams reviewing document ownership and verification.
        public async Task<IEnumerable<Document>> GetCustomerDocumentsWithVerificationDetailsAsync(int customerId)
        {
            return await _dbSet
                .Include(x => x.VerifiedByEmployee)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 3. KYC AND LOAN DOCUMENTS
        // =========================================================

        // Retrieves documents associated with a particular KYC record.
        // Useful for checking whether the required KYC evidence has been submitted.
        public async Task<IEnumerable<Document>> GetByKYCDetailsIdAsync(int kycDetailsId)
        {
            return await _dbSet
                .Where(x => x.KYCDetailsId == kycDetailsId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves documents associated with a particular loan.
        // Useful for reviewing loan agreements, income proof and supporting documents.
        public async Task<IEnumerable<Document>> GetByLoanIdAsync(int loanId)
        {
            return await _dbSet
                .Where(x => x.LoanId == loanId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves documents verified by a particular employee.
        // Useful for reviewing an employee's document verification activity.
        public async Task<IEnumerable<Document>> GetByVerifiedByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(x => x.VerifiedByEmployeeId == employeeId)
                .OrderByDescending(x => x.VerifiedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. VERIFICATION MANAGEMENT
        // =========================================================

        // Retrieves documents matching a particular verification status.
        // Useful for managing pending, verified, rejected and expired document queues.
        public async Task<IEnumerable<Document>> GetByVerificationStatusAsync(string verificationStatus)
        {
            return await _dbSet
                .Where(x => x.VerificationStatus == verificationStatus)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves documents awaiting verification.
        // Useful for the KYC and compliance verification work queue.
        public async Task<IEnumerable<Document>> GetPendingVerificationDocumentsAsync()
        {
            return await _dbSet
                .Where(x => x.VerificationStatus == "Pending")
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves documents that have been rejected.
        // Useful for identifying documents requiring customer resubmission.
        public async Task<IEnumerable<Document>> GetRejectedDocumentsAsync()
        {
            return await _dbSet
                .Where(x => x.VerificationStatus == "Rejected")
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // Retrieves verified documents belonging to a customer.
        // Useful for checking which customer documents have passed verification.
        public async Task<IEnumerable<Document>> GetVerifiedDocumentsByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId && x.VerificationStatus == "Verified")
                .OrderByDescending(x => x.VerifiedAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. DOCUMENT EXPIRY MANAGEMENT
        // =========================================================

        // Retrieves documents whose expiry date has passed.
        // Useful for identifying expired identity and supporting documents.
        public async Task<IEnumerable<Document>> GetExpiredDocumentsAsync(DateTime currentDate)
        {
            return await _dbSet
                .Where(x => x.ExpiryDate.HasValue &&
                            x.ExpiryDate.Value < currentDate &&
                            x.VerificationStatus != "Expired")
                .OrderBy(x => x.ExpiryDate)
                .ToListAsync();
        }

        // Retrieves documents expiring within the supplied date range.
        // Useful for scheduling KYC refresh requests before documents expire.
        public async Task<IEnumerable<Document>> GetDocumentsExpiringWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ExpiryDate.HasValue &&
                            x.ExpiryDate.Value >= startDate &&
                            x.ExpiryDate.Value <= endDate)
                .OrderBy(x => x.ExpiryDate)
                .ToListAsync();
        }

        // =========================================================
        // 6. FILE AND STORAGE VALIDATION
        // =========================================================

        // Retrieves documents that are not marked as encrypted.
        // Useful for identifying records that require storage-security review.
        public async Task<IEnumerable<Document>> GetUnencryptedDocumentsAsync()
        {
            return await _dbSet
                .Where(x => !x.IsEncrypted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Checks whether a document with the supplied content hash already exists.
        // Useful for checking file duplication before saving a new document record.
        public async Task<bool> ExistsByContentHashAsync(string contentHash)
        {
            return await _dbSet.AnyAsync(x => x.ContentHash == contentHash);
        }

        // =========================================================
        // 7. SEARCH AND REPORTING
        // =========================================================

        // Searches documents by file name, masked document number or customer name.
        // Useful for authorized operations staff locating a document record.
        public async Task<IEnumerable<Document>> SearchDocumentsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Where(x => x.FileName.Contains(searchTerm) ||
                            (x.DocumentNumberMasked != null && x.DocumentNumberMasked.Contains(searchTerm)) ||
                            x.Customer.FirstName.Contains(searchTerm) ||
                            x.Customer.LastName.Contains(searchTerm))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Counts documents belonging to a particular customer.
        // Useful for monitoring document submission completeness.
        public async Task<int> CountDocumentsByCustomerIdAsync(int customerId)
        {
            return await _dbSet.CountAsync(x => x.CustomerId == customerId);
        }
    }
}
