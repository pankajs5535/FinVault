
using FinVault.Application.Interface.IRepositories;
using FinVault.Domain.Entities;
using FinVault.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace FinVault.Persistence.Repositories
{
    public class DisputeRepository : GenericRepository<Dispute>, IDisputeRepository
    {
        public DisputeRepository(FinVaultDbContext context) : base(context)
        {
        }

        // =========================================================
        // 1. DISPUTE IDENTIFICATION
        // =========================================================

        // Finds a dispute using its unique dispute number.
        // Useful for customer support and tracking a complaint using its reference number.
        public async Task<Dispute?> GetByDisputeNumberAsync(string disputeNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.DisputeNumber == disputeNumber);
        }

        // =========================================================
        // 2. DISPUTE DETAILS
        // =========================================================

        // Retrieves a dispute with its related transaction, customer, card transaction, employee and refunds.
        // Useful for displaying the complete dispute record during investigation.
        public async Task<Dispute?> GetDisputeDetailsAsync(int disputeId)
        {
            return await _dbSet
                .Include(x => x.Transaction)
                .Include(x => x.CardTransaction)
                .Include(x => x.Customer)
                .Include(x => x.AssignedToEmployee)
                .Include(x => x.Refunds)
                .FirstOrDefaultAsync(x => x.DisputeId == disputeId);
        }

        // =========================================================
        // 3. TRANSACTION-BASED DISPUTES
        // =========================================================

        // Retrieves disputes raised against a particular banking transaction.
        // Useful when investigating failed, duplicate or unauthorized transactions.
        public async Task<IEnumerable<Dispute>> GetByTransactionIdAsync(int transactionId)
        {
            return await _dbSet
                .Where(x => x.TransactionId == transactionId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves disputes associated with a particular card transaction.
        // Useful for investigating card payments, ATM withdrawals and card-related complaints.
        public async Task<IEnumerable<Dispute>> GetByCardTransactionIdAsync(int cardTransactionId)
        {
            return await _dbSet
                .Where(x => x.CardTransactionId == cardTransactionId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 4. CUSTOMER DISPUTES
        // =========================================================

        // Retrieves all disputes submitted by a customer.
        // Useful for reviewing a customer's complaint history.
        public async Task<IEnumerable<Dispute>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 5. STATUS AND PRIORITY
        // =========================================================

        // Retrieves disputes matching a particular lifecycle status.
        // Useful for separating open, investigated, resolved and rejected complaints.
        public async Task<IEnumerable<Dispute>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Where(x => x.Status == status)
                .OrderBy(x => x.DueBy)
                .ToListAsync();
        }

        // Retrieves disputes matching a particular priority.
        // Useful for prioritizing high-impact and critical customer complaints.
        public async Task<IEnumerable<Dispute>> GetByPriorityAsync(string priority)
        {
            return await _dbSet
                .Where(x => x.Priority == priority)
                .OrderBy(x => x.DueBy)
                .ToListAsync();
        }

        // =========================================================
        // 6. EMPLOYEE ASSIGNMENT
        // =========================================================

        // Retrieves disputes assigned to a particular employee.
        // Useful for monitoring an employee's investigation workload.
        public async Task<IEnumerable<Dispute>> GetByAssignedEmployeeIdAsync(int employeeId)
        {
            return await _dbSet
                .Where(x => x.AssignedToEmployeeId == employeeId)
                .OrderBy(x => x.DueBy)
                .ToListAsync();
        }

        // Retrieves disputes that have not yet been assigned to an employee.
        // Useful for the dispute allocation queue.
        public async Task<IEnumerable<Dispute>> GetUnassignedDisputesAsync()
        {
            return await _dbSet
                .Where(x => x.AssignedToEmployeeId == null &&
                            (x.Status == "Open" || x.Status == "UnderInvestigation"))
                .OrderBy(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 7. INVESTIGATION QUEUES
        // =========================================================

        // Retrieves disputes that are currently open.
        // Useful for monitoring complaints that still require action.
        public async Task<IEnumerable<Dispute>> GetOpenDisputesAsync()
        {
            return await _dbSet
                .Where(x => x.Status == "Open" || x.Status == "UnderInvestigation" ||
                            x.Status == "ProvisionalCreditGiven" || x.Status == "Escalated")
                .OrderBy(x => x.DueBy)
                .ToListAsync();
        }

        // Retrieves disputes whose due date has passed and that are not resolved, rejected or closed.
        // Useful for identifying overdue complaints and supporting turnaround-time monitoring.
        public async Task<IEnumerable<Dispute>> GetOverdueDisputesAsync(DateTime currentDate)
        {
            return await _dbSet
                .Where(x => x.DueBy.HasValue &&
                            x.DueBy.Value < currentDate &&
                            (x.Status == "Open" || x.Status == "UnderInvestigation" ||
                             x.Status == "ProvisionalCreditGiven" || x.Status == "Escalated"))
                .OrderBy(x => x.DueBy)
                .ToListAsync();
        }

        // Retrieves disputes with due dates inside the supplied date range.
        // Useful for planning investigation workloads and upcoming complaint deadlines.
        public async Task<IEnumerable<Dispute>> GetDisputesDueWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.DueBy.HasValue && x.DueBy.Value >= startDate && x.DueBy.Value <= endDate)
                .OrderBy(x => x.DueBy)
                .ToListAsync();
        }

        // =========================================================
        // 8. DISPUTE TIMELINE
        // =========================================================

        // Retrieves disputes raised within the supplied date range.
        // Useful for complaint-volume reporting and operational monitoring.
        public async Task<IEnumerable<Dispute>> GetDisputesRaisedWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.RaisedAt >= startDate && x.RaisedAt <= endDate)
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Retrieves disputes resolved within the supplied date range.
        // Useful for measuring dispute resolution activity over time.
        public async Task<IEnumerable<Dispute>> GetResolvedDisputesWithinRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(x => x.ResolvedAt.HasValue &&
                            x.ResolvedAt.Value >= startDate &&
                            x.ResolvedAt.Value <= endDate)
                .OrderByDescending(x => x.ResolvedAt)
                .ToListAsync();
        }

        // =========================================================
        // 9. REFUND MANAGEMENT
        // =========================================================

        // Retrieves disputes together with their associated refund records.
        // Useful for checking whether a complaint has resulted in a refund process.
        public async Task<IEnumerable<Dispute>> GetDisputesWithRefundsAsync()
        {
            return await _dbSet
                .Include(x => x.Refunds)
                .Where(x => x.Refunds.Any())
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // =========================================================
        // 10. SEARCH AND VALIDATION
        // =========================================================

        // Searches disputes by dispute number, description or customer name.
        // Useful for customer service and operations teams locating complaints quickly.
        public async Task<IEnumerable<Dispute>> SearchDisputesAsync(string searchTerm)
        {
            return await _dbSet
                .Include(x => x.Customer)
                .Where(x => x.DisputeNumber.Contains(searchTerm) ||
                            x.Description.Contains(searchTerm) ||
                            x.Customer.FirstName.Contains(searchTerm) ||
                            x.Customer.LastName.Contains(searchTerm))
                .OrderByDescending(x => x.RaisedAt)
                .ToListAsync();
        }

        // Checks whether a dispute number already exists.
        // Useful for preventing duplicate dispute references before inserting a record.
        public async Task<bool> ExistsByDisputeNumberAsync(string disputeNumber)
        {
            return await _dbSet.AnyAsync(x => x.DisputeNumber == disputeNumber);
        }

        // Counts disputes submitted by a particular customer.
        // Useful for customer complaint history and operational reporting.
        public async Task<int> CountDisputesByCustomerIdAsync(int customerId)
        {
            return await _dbSet.CountAsync(x => x.CustomerId == customerId);
        }
    }
}
