
using FinVault.Domain.Entities;

namespace FinVault.Application.Interface.IRepositories
{
    public interface IDisputeRepository : IGenericRepository<Dispute>
    {
        // 1. DISPUTE IDENTIFICATION
        // Finds a dispute using its unique dispute number.
        // Useful for customer support and tracking a complaint using its reference number.
        Task<Dispute?> GetByDisputeNumberAsync(string disputeNumber);

        // 2. DISPUTE DETAILS
        // Retrieves a dispute with its related transaction, customer, card transaction, employee and refunds.
        // Useful for displaying the complete dispute record during investigation.
        Task<Dispute?> GetDisputeDetailsAsync(int disputeId);

        // 3. TRANSACTION-BASED DISPUTES
        // Retrieves disputes raised against a particular banking transaction.
        // Useful when investigating failed, duplicate or unauthorized transactions.
        Task<IEnumerable<Dispute>> GetByTransactionIdAsync(int transactionId);

        // Retrieves disputes associated with a particular card transaction.
        // Useful for investigating card payments, ATM withdrawals and card-related complaints.
        Task<IEnumerable<Dispute>> GetByCardTransactionIdAsync(int cardTransactionId);

        // 4. CUSTOMER DISPUTES
        // Retrieves all disputes submitted by a customer.
        // Useful for reviewing a customer's complaint history.
        Task<IEnumerable<Dispute>> GetByCustomerIdAsync(int customerId);

        // 5. STATUS AND PRIORITY
        // Retrieves disputes matching a particular lifecycle status.
        // Useful for separating open, investigated, resolved and rejected complaints.
        Task<IEnumerable<Dispute>> GetByStatusAsync(string status);

        // Retrieves disputes matching a particular priority.
        // Useful for prioritizing high-impact and critical customer complaints.
        Task<IEnumerable<Dispute>> GetByPriorityAsync(string priority);

        // 6. EMPLOYEE ASSIGNMENT
        // Retrieves disputes assigned to a particular employee.
        // Useful for monitoring an employee's investigation workload.
        Task<IEnumerable<Dispute>> GetByAssignedEmployeeIdAsync(int employeeId);

        // Retrieves disputes that have not yet been assigned to an employee.
        // Useful for the dispute allocation queue.
        Task<IEnumerable<Dispute>> GetUnassignedDisputesAsync();

        // 7. INVESTIGATION QUEUES
        // Retrieves disputes that are currently open.
        // Useful for monitoring complaints that still require action.
        Task<IEnumerable<Dispute>> GetOpenDisputesAsync();

        // Retrieves disputes whose due date has passed and that are not resolved, rejected or closed.
        // Useful for identifying overdue complaints and supporting turnaround-time monitoring.
        Task<IEnumerable<Dispute>> GetOverdueDisputesAsync(DateTime currentDate);

        // Retrieves disputes with due dates inside the supplied date range.
        // Useful for planning investigation workloads and upcoming complaint deadlines.
        Task<IEnumerable<Dispute>> GetDisputesDueWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 8. DISPUTE TIMELINE
        // Retrieves disputes raised within the supplied date range.
        // Useful for complaint-volume reporting and operational monitoring.
        Task<IEnumerable<Dispute>> GetDisputesRaisedWithinRangeAsync(DateTime startDate, DateTime endDate);

        // Retrieves disputes resolved within the supplied date range.
        // Useful for measuring dispute resolution activity over time.
        Task<IEnumerable<Dispute>> GetResolvedDisputesWithinRangeAsync(DateTime startDate, DateTime endDate);

        // 9. REFUND MANAGEMENT
        // Retrieves disputes together with their associated refund records.
        // Useful for checking whether a complaint has resulted in a refund process.
        Task<IEnumerable<Dispute>> GetDisputesWithRefundsAsync();

        // 10. SEARCH AND VALIDATION
        // Searches disputes by dispute number, description or customer name.
        // Useful for customer service and operations teams locating complaints quickly.
        Task<IEnumerable<Dispute>> SearchDisputesAsync(string searchTerm);

        // Checks whether a dispute number already exists.
        // Useful for preventing duplicate dispute references before inserting a record.
        Task<bool> ExistsByDisputeNumberAsync(string disputeNumber);

        // Counts disputes submitted by a particular customer.
        // Useful for customer complaint history and operational reporting.
        Task<int> CountDisputesByCustomerIdAsync(int customerId);
    }
}
