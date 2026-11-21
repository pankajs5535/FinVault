
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IKYCDetailsRepository : IGenericRepository<KYCDetails>
    {
        // =========================================================
        // 1. KYC IDENTIFICATION
        // =========================================================

        // Retrieves a KYC record with its customer, verifier and documents.
        // Useful for reviewing a customer's complete KYC verification record.
        Task<KYCDetails?> GetKYCDetailsAsync(int kycDetailsId);

        // Retrieves the current KYC record belonging to a customer.
        // Useful for checking the customer's latest KYC verification status.
        Task<KYCDetails?> GetCurrentKYCByCustomerIdAsync(int customerId);

        // Retrieves the complete KYC history of a customer.
        // Useful for reviewing previous KYC submissions and verification changes.
        Task<IEnumerable<KYCDetails>> GetKYCHistoryByCustomerIdAsync(int customerId);

        // =========================================================
        // 2. KYC LEVEL AND VERIFICATION STATUS
        // =========================================================

        // Retrieves KYC records matching a particular KYC level.
        // Useful for separating Minimum, Full, Video and EKyc verification records.
        Task<IEnumerable<KYCDetails>> GetByKycLevelAsync(string kycLevel);

        // Retrieves KYC records matching a particular verification status.
        // Useful for managing pending, in-progress, verified, rejected and expired records.
        Task<IEnumerable<KYCDetails>> GetByVerificationStatusAsync(string verificationStatus);

        // Retrieves current KYC records awaiting verification.
        // Useful for managing the bank's KYC verification queue.
        Task<IEnumerable<KYCDetails>> GetPendingKYCRecordsAsync();

        // Retrieves current KYC records currently being verified.
        // Useful for monitoring ongoing KYC investigations.
        Task<IEnumerable<KYCDetails>> GetInProgressKYCRecordsAsync();

        // Retrieves current KYC records that have been verified.
        // Useful for identifying customers whose current KYC record is verified.
        Task<IEnumerable<KYCDetails>> GetVerifiedKYCRecordsAsync();

        // Retrieves current KYC records that have been rejected.
        // Useful for identifying customers who may need to resubmit their KYC documents.
        Task<IEnumerable<KYCDetails>> GetRejectedKYCRecordsAsync();

        // Retrieves current KYC records marked as expired.
        // Useful for identifying customers requiring KYC renewal.
        Task<IEnumerable<KYCDetails>> GetExpiredKYCRecordsAsync();

        // =========================================================
        // 3. PAN VERIFICATION AND DUPLICATE DETECTION
        // =========================================================

        // Retrieves a current KYC record using the keyed PAN hash.
        // Useful for detecting duplicate PAN registrations without querying plaintext PAN values.
        Task<KYCDetails?> GetCurrentByPanHashAsync(string panHash);

        // Checks whether a current KYC record already uses the supplied PAN hash.
        // Useful for preventing duplicate PAN registration during customer onboarding.
        Task<bool> ExistsCurrentByPanHashAsync(string panHash);

        // Retrieves current KYC records with PAN verification completed.
        // Useful for monitoring PAN verification completion.
        Task<IEnumerable<KYCDetails>> GetPanVerifiedRecordsAsync();

        // Retrieves current KYC records where PAN verification is still pending.
        // Useful for identifying customers whose PAN verification requires action.
        Task<IEnumerable<KYCDetails>> GetPanPendingVerificationRecordsAsync();

        // =========================================================
        // 4. AADHAAR AND CKYC MANAGEMENT
        // =========================================================

        // Retrieves current KYC records with Aadhaar verification completed.
        // Useful for monitoring Aadhaar verification completion without retrieving Aadhaar numbers.
        Task<IEnumerable<KYCDetails>> GetAadhaarVerifiedRecordsAsync();

        // Retrieves current KYC records matching an Aadhaar verification method.
        // Useful for reporting OTP, biometric, offline XML and Virtual ID verification activity.
        Task<IEnumerable<KYCDetails>> GetByAadhaarVerificationMethodAsync(string verificationMethod);

        // Retrieves a KYC record using its CKYC number.
        // Useful for locating a customer's Central KYC Registry record.
        Task<KYCDetails?> GetByCkycNumberAsync(string ckycNumber);

        // Retrieves current KYC records matching a CKYC processing status.
        // Useful for monitoring CKYC upload and verification workflows.
        Task<IEnumerable<KYCDetails>> GetByCkycStatusAsync(string ckycStatus);

        // =========================================================
        // 5. RE-KYC AND VERIFICATION DEADLINES
        // =========================================================

        // Retrieves current KYC records whose re-KYC deadline has passed.
        // Useful for identifying customers requiring periodic KYC updates.
        Task<IEnumerable<KYCDetails>> GetOverdueReKYCRecordsAsync(DateTime currentDate);

        // Retrieves current KYC records with re-KYC deadlines inside the supplied date range.
        // Useful for scheduling KYC renewal notifications before deadlines.
        Task<IEnumerable<KYCDetails>> GetReKYCRecordsDueWithinRangeAsync(DateTime startDate, DateTime endDate);

        // =========================================================
        // 6. EMPLOYEE VERIFICATION AND DOCUMENTS
        // =========================================================

        // Retrieves KYC records verified by a particular employee.
        // Useful for reviewing employee verification workload and audit history.
        Task<IEnumerable<KYCDetails>> GetByVerifiedByEmployeeIdAsync(int employeeId);

        // Retrieves KYC records with their associated supporting documents.
        // Useful for reviewing evidence submitted during KYC verification.
        Task<IEnumerable<KYCDetails>> GetKYCRecordsWithDocumentsAsync(int customerId);

        // =========================================================
        // 7. DATE, SEARCH AND REPORTING
        // =========================================================

        // Retrieves KYC records verified within the supplied date range.
        // Useful for monitoring completed KYC verification activity.
        Task<IEnumerable<KYCDetails>> GetVerifiedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Counts current KYC records belonging to a particular verification status.
        // Useful for KYC operational dashboards and compliance reporting.
        Task<int> CountCurrentRecordsByStatusAsync(string verificationStatus);

        // Retrieves current KYC records belonging to customers with a particular customer identifier.
        // Useful for customer-specific KYC reporting.
        Task<bool> HasCurrentKYCRecordAsync(int customerId);
    }
}
