using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.KYCDetailsDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the KYCDetails banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IKYCDetailsService
{
        Task<KYCDetailsDto?> GetByCustomerIdAsync(int customerId);
        Task<KYCDetailsDto> SubmitAsync(CreateKYCDetailsDto dto);
        Task<KYCDetailsDto?> UpdateAsync(int kycDetailsId, UpdateKYCDetailsDto dto);
        Task<KYCDetailsDto?> ReviewAsync(int kycDetailsId, string decision, string? remarks);
}
