using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.CustomerDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Customer banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface ICustomerService
{
        Task<CustomerDto?> GetByIdAsync(int customerId);
        Task<IReadOnlyList<CustomerDto>> GetAllAsync();
        Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerDto?> UpdateAsync(int customerId, UpdateCustomerDto dto);
        Task<bool> DeactivateAsync(int customerId);
}
