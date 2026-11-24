using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.BeneficiaryDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Beneficiary banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IBeneficiaryService
{
        Task<BeneficiaryDto?> GetByIdAsync(int beneficiaryId);
        Task<IReadOnlyList<BeneficiaryDto>> GetByCustomerIdAsync(int customerId);
        Task<BeneficiaryDto> AddAsync(CreateBeneficiaryDto dto);
        Task<BeneficiaryDto?> UpdateAsync(int beneficiaryId, UpdateBeneficiaryDto dto);
        Task<bool> RemoveAsync(int beneficiaryId);
        Task<bool> ActivateAfterCoolingPeriodAsync(int beneficiaryId);
}
