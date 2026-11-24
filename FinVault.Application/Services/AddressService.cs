using AutoMapper;
using FinVault.Application.DTOs.AddressDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Maintains customer addresses and enforces primary-address rules.</summary>
public sealed class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddressService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Returns one address with its related details, or null if it is missing.
    public async Task<AddressDto?> GetByIdAsync(int addressId)
    {
        var address = await _unitOfWork.Address.GetAddressDetailsAsync(addressId);
        return address is null ? null : _mapper.Map<AddressDto>(address);
    }

    // 2. Lists the addresses registered by one customer.
    public async Task<IReadOnlyList<AddressDto>> GetByCustomerIdAsync(int customerId)
    {
        var addresses = await _unitOfWork.Address.GetAddressesByCustomerIdAsync(customerId);
        return _mapper.Map<IReadOnlyList<AddressDto>>(addresses);
    }

    // 3. Validates customer and country, then creates the address.
    public async Task<AddressDto> CreateAsync(CreateAddressDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId) is null)
            throw new InvalidOperationException("The selected customer does not exist.");
        if (await _unitOfWork.Country.GetByIdAsync(dto.CountryId) is null)
            throw new InvalidOperationException("The selected country does not exist.");
        if (dto.IsPrimary)
        {
            var currentPrimary = await _unitOfWork.Address.GetPrimaryAddressByCustomerIdAsync(dto.CustomerId);
            if (currentPrimary is not null)
            {
                currentPrimary.IsPrimary = false;
                currentPrimary.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Address.Update(currentPrimary);
            }
        }

        var address = _mapper.Map<Address>(dto);
        address.AddressLine1 = dto.AddressLine1.Trim();
        address.City = dto.City.Trim();
        address.State = dto.State.Trim();
        address.PinCode = dto.PinCode.Trim();
        address.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Address.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AddressDto>(address);
    }

    // 4. Updates an existing address and maintains only one primary address per customer.
    public async Task<AddressDto?> UpdateAsync(int addressId, UpdateAddressDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.AddressId != addressId)
            throw new ArgumentException("The route address ID and DTO address ID do not match.", nameof(dto));
        var address = await _unitOfWork.Address.GetByIdAsync(addressId);
        if (address is null) return null;
        if (await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId) is null ||
            await _unitOfWork.Country.GetByIdAsync(dto.CountryId) is null)
            throw new InvalidOperationException("The selected customer or country does not exist.");

        if (dto.IsPrimary)
        {
            var currentPrimary = await _unitOfWork.Address.GetPrimaryAddressByCustomerIdAsync(dto.CustomerId);
            if (currentPrimary is not null && currentPrimary.AddressId != addressId)
            {
                currentPrimary.IsPrimary = false;
                currentPrimary.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Address.Update(currentPrimary);
            }
        }

        _mapper.Map(dto, address);
        address.AddressLine1 = dto.AddressLine1.Trim();
        address.City = dto.City.Trim();
        address.State = dto.State.Trim();
        address.PinCode = dto.PinCode.Trim();
        address.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Address.Update(address);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AddressDto>(address);
    }

    // 5. Deletes the selected address; returns false when it does not exist.
    public async Task<bool> DeleteAsync(int addressId)
    {
        var address = await _unitOfWork.Address.GetByIdAsync(addressId);
        if (address is null) return false;
        _unitOfWork.Address.Delete(address);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
