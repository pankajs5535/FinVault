
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class KYCDetailsRepository : GenericRepository<KYCDetails>, IKYCDetailsRepository
    {
        public KYCDetailsRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. KYC IDENTIFICATION
        // =========================================================

        // Retrieves a KYC record with its customer, verifier and documents.
        // Useful for reviewing a customer's complete KYC verification record.
        public async Task<KYCDetails?> GetKYCDetailsAsync(int kycDetailsId)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Include(x => x.VerifiedByEmployee)
                .Include(x => x.Documents)
                .Include(x => x.CreatedByUser)
                .Include(x => x.UpdatedByUser)
                .FirstOrDefaultAsync(x => x.KYCDetailsId == kycDetailsId);
        }

        // Retrieves the current KYC record belonging to a customer.
        // Useful for checking the customer's latest KYC verification status.
        public async Task<KYCDetails?> GetCurrentKYCByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsCurrent);
        }

        // Retrieves the complete KYC history of a customer.
        // Useful for reviewing previous KYC submissions and verification changes.
        public async Task<IEnumerable<KYCDetails>> GetKYCHistoryByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 2. KYC LEVEL AND VERIFICATION STATUS
        // =========================================================

        // Retrieves KYC records matching a particular KYC level.
        // Useful for separating Minimum, Full, Video and EKyc verification records.
        public async Task<IEnumerable<KYCDetails>> GetByKycLevelAsync(string kycLevel)
        {
            return await _dbSet
                .Where(x => x.KycLevel == kycLevel)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves KYC records matching a particular verification status.
        // Useful for managing pending, in-progress, verified, rejected and expired records.
        public async Task<IEnumerable<KYCDetails>> GetByVerificationStatusAsync(string verificationStatus)
        {
            return await _dbSet
                .Where(x => x.VerificationStatus == verificationStatus)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves current KYC records awaiting verification.
        // Useful for managing the bank's KYC verification queue.
        public async Task<IEnumerable<KYCDetails>> GetPendingKYCRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.VerificationStatus == "Pending")
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves current KYC records currently being verified.
        // Useful for monitoring ongoing KYC investigations.
        public async Task<IEnumerable<KYCDetails>> GetInProgressKYCRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.VerificationStatus == "InProgress")
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Retrieves current KYC records that have been verified.
        // Useful for identifying customers whose current KYC record is verified.
        public async Task<IEnumerable<KYCDetails>> GetVerifiedKYCRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.VerificationStatus == "Verified")
                .OrderByDescending(x => x.VerifiedAt)
                .ToListAsync();
        }

        // Retrieves current KYC records that have been rejected.
        // Useful for identifying customers who may need to resubmit their KYC documents.
        public async Task<IEnumerable<KYCDetails>> GetRejectedKYCRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.VerificationStatus == "Rejected")
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        // Retrieves current KYC records marked as expired.
        // Useful for identifying customers requiring KYC renewal.
        public async Task<IEnumerable<KYCDetails>> GetExpiredKYCRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.VerificationStatus == "Expired")
                .OrderBy(x => x.NextReKycDueDate)
                .ToListAsync();
        }

        // =========================================================
        // 3. PAN VERIFICATION AND DUPLICATE DETECTION
        // =========================================================

        // Retrieves a current KYC record using the keyed PAN hash.
        // Useful for detecting duplicate PAN registrations without querying plaintext PAN values.
        public async Task<KYCDetails?> GetCurrentByPanHashAsync(string panHash)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.IsCurrent && x.PanHash == panHash);
        }

        // Checks whether a current KYC record already uses the supplied PAN hash.
        // Useful for preventing duplicate PAN registration during customer onboarding.
        public async Task<bool> ExistsCurrentByPanHashAsync(string panHash)
        {
            return await _dbSet.AnyAsync(x => x.IsCurrent && x.PanHash == panHash);
        }

        // Retrieves current KYC records with PAN verification completed.
        // Useful for monitoring PAN verification completion.
        public async Task<IEnumerable<KYCDetails>> GetPanVerifiedRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.IsPanVerified)
                .OrderByDescending(x => x.PanVerifiedAt)
                .ToListAsync();
        }

        // Retrieves current KYC records where PAN verification is still pending.
        // Useful for identifying customers whose PAN verification requires action.
        public async Task<IEnumerable<KYCDetails>> GetPanPendingVerificationRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && !x.IsPanVerified && !x.IsForm60Submitted)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. AADHAAR AND CKYC MANAGEMENT
        // =========================================================

        // Retrieves current KYC records with Aadhaar verification completed.
        // Useful for monitoring Aadhaar verification completion without retrieving Aadhaar numbers.
        public async Task<IEnumerable<KYCDetails>> GetAadhaarVerifiedRecordsAsync()
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.IsAadhaarVerified)
                .OrderByDescending(x => x.AadhaarVerifiedAt)
                .ToListAsync();
        }

        // Retrieves current KYC records matching an Aadhaar verification method.
        // Useful for reporting OTP, biometric, offline XML and Virtual ID verification activity.
        public async Task<IEnumerable<KYCDetails>> GetByAadhaarVerificationMethodAsync(string verificationMethod)
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.AadhaarVerificationMethod == verificationMethod)
                .OrderByDescending(x => x.AadhaarVerifiedAt)
                .ToListAsync();
        }

        // Retrieves a KYC record using its CKYC number.
        // Useful for locating a customer's Central KYC Registry record.
        public async Task<KYCDetails?> GetByCkycNumberAsync(string ckycNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.CkycNumber == ckycNumber);
        }

        // Retrieves current KYC records matching a CKYC processing status.
        // Useful for monitoring CKYC upload and verification workflows.
        public async Task<IEnumerable<KYCDetails>> GetByCkycStatusAsync(string ckycStatus)
        {
            return await _dbSet
                .Where(x => x.IsCurrent && x.CkycStatus == ckycStatus)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. RE-KYC AND VERIFICATION DEADLINES
        // =========================================================

        // Retrieves current KYC records whose re-KYC deadline has passed.
        // Useful for identifying customers requiring periodic KYC updates.
        public async Task<IEnumerable<KYCDetails>> GetOverdueReKYCRecordsAsync(DateTime currentDate)
        {
            return await _dbSet
                .Where(x => x.IsCurrent &&
                            x.NextReKycDueDate.HasValue &&
                            x.NextReKycDueDate.Value < currentDate)
                .OrderBy(x => x.NextReKycDueDate)
                .ToListAsync();
        }

        // Retrieves current KYC records with re-KYC deadlines inside the supplied date range.
        // Useful for scheduling KYC renewal notifications before deadlines.
        public async Task<IEnumerable<KYCDetails>> GetReKYCRecordsDueWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.IsCurrent &&
                            x.NextReKycDueDate.HasValue &&
                            x.NextReKycDueDate.Value >= startDate &&
                            x.NextReKycDueDate.Value <= endDate)
                .OrderBy(x => x.NextReKycDueDate)
                .ToListAsync();
        }

        // =========================================================
        // 6. EMPLOYEE VERIFICATION AND DOCUMENTS
        // =========================================================

        // Retrieves KYC records verified by a particular employee.
        // Useful for reviewing employee verification workload and audit history.
        public async Task<IEnumerable<KYCDetails>> GetByVerifiedByEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(x => x.VerifiedByEmployeeId == employeeId)
                .OrderByDescending(x => x.VerifiedAt)
                .ToListAsync();
        }

        // Retrieves KYC records with their associated supporting documents.
        // Useful for reviewing evidence submitted during KYC verification.
        public async Task<IEnumerable<KYCDetails>> GetKYCRecordsWithDocumentsAsync(int customerId)
        {
            return await _dbSet
                .Include(x => x.Documents)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =========================================================
        // 7. DATE, SEARCH AND REPORTING
        // =========================================================

        // Retrieves KYC records verified within the supplied date range.
        // Useful for monitoring completed KYC verification activity.
        public async Task<IEnumerable<KYCDetails>> GetVerifiedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.VerifiedAt.HasValue &&
                            x.VerifiedAt.Value >= startDate &&
                            x.VerifiedAt.Value <= endDate)
                .OrderByDescending(x => x.VerifiedAt)
                .ToListAsync();
        }

        // Counts current KYC records belonging to a particular verification status.
        // Useful for KYC operational dashboards and compliance reporting.
        public async Task<int> CountCurrentRecordsByStatusAsync(string verificationStatus)
        {
            return await _dbSet.CountAsync(x =>
                x.IsCurrent && x.VerificationStatus == verificationStatus);
        }

        // Checks whether a customer has a current KYC record.
        // Useful for validating customer onboarding and KYC workflow prerequisites.
        public async Task<bool> HasCurrentKYCRecordAsync(int customerId)
        {
            return await _dbSet.AnyAsync(x =>
                x.CustomerId == customerId && x.IsCurrent);
        }
    }
}
