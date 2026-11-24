using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.AddressDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Address banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IAddressService
{
        Task<AddressDto?> GetByIdAsync(int addressId);
        Task<IReadOnlyList<AddressDto>> GetByCustomerIdAsync(int customerId);
        Task<AddressDto> CreateAsync(CreateAddressDto dto);
        Task<AddressDto?> UpdateAsync(int addressId, UpdateAddressDto dto);
        Task<bool> DeleteAsync(int addressId);
}
