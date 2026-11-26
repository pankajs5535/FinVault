using FinVault.Application.DTOs.DisputeDtos;
using FinVault.Domain.Entities;

namespace FinVault.Application.Interfaces.Services
{
    public interface IDisputeService
    {
        // 1. DISPUTE IDENTIFICATION
        Task<IEnumerable<DisputeDto>> GetAllAsync();
        Task<DisputeDto?> GetByIdAsync(int disputeId);
        Task<DisputeDto?> GetByDisputeNumberAsync(string disputeNumber);
        Task<DisputeDto?> GetDisputeDetailsAsync(int disputeId);
        Task<bool> ExistsByDisputeNumberAsync(string disputeNumber);

        // 2. DISPUTE CRUD
        Task<DisputeDto> CreateAsync(CreateDisputeDto dto);
        Task<DisputeDto?> UpdateAsync(UpdateDisputeDto dto);
        Task<bool> DeleteAsync(DeleteDisputeDto dto);

        // 3. TRANSACTION-BASED DISPUTES
        Task<IEnumerable<DisputeDto>> GetByTransactionIdAsync(int transactionId);
        Task<IEnumerable<DisputeDto>> GetByCardTransactionIdAsync(int cardTransactionId);

        // 4. CUSTOMER DISPUTES
        Task<IEnumerable<DisputeDto>> GetByCustomerIdAsync(int customerId);
        Task<int> CountDisputesByCustomerIdAsync(int customerId);

        // 5. STATUS AND PRIORITY
        Task<IEnumerable<DisputeDto>> GetByStatusAsync(string status);
        Task<IEnumerable<DisputeDto>> GetByPriorityAsync(string priority);

        // 6. EMPLOYEE ASSIGNMENT
        Task<IEnumerable<DisputeDto>> GetByAssignedEmployeeIdAsync(int employeeId);
        Task<IEnumerable<DisputeDto>> GetUnassignedDisputesAsync();

        // 7. INVESTIGATION QUEUES
        Task<IEnumerable<DisputeDto>> GetOpenDisputesAsync();
        Task<IEnumerable<DisputeDto>> GetOverdueDisputesAsync(DateTime currentDate);
        Task<IEnumerable<DisputeDto>> GetDisputesDueWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 8. DISPUTE TIMELINE
        Task<IEnumerable<DisputeDto>> GetDisputesRaisedWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);
        Task<IEnumerable<DisputeDto>> GetResolvedDisputesWithinRangeAsync(
            DateTime startDate,
            DateTime endDate);

        // 9. REFUND MANAGEMENT
        Task<IEnumerable<DisputeDto>> GetDisputesWithRefundsAsync();

        // 10. SEARCH
        Task<IEnumerable<DisputeDto>> SearchDisputesAsync(string searchTerm);
    }
}

//Banking rules:
//-Dispute numbers must be unique.
//- A dispute must reference a valid transaction and customer.
//- Card transaction reference is optional.
//- Only authorized employees should investigate assigned disputes.
//- Validate status, priority, and resolution against the DTO's allowed values.
//- Use RowVersion for optimistic concurrency during updates.
//- Preserve investigation and resolution history; do not silently overwrite outcomes.