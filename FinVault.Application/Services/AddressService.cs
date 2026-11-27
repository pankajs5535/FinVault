using AutoMapper;
using FinVault.Application.DTOs.AddressDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class AddressService : IAddressService
{
    private static readonly string[] ValidAddressTypes =
    {
        "Residential",
        "Permanent",
        "Correspondence",
        "Office",
        "Registered"
    };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AddressService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<AddressDto>> GetAllAsync()
    {
        var addresses = await _unitOfWork.Address.GetAllAsync();
        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<AddressDto?> GetByIdAsync(int addressId)
    {
        ValidateId(addressId, nameof(addressId));

        var address = await _unitOfWork.Address.GetByIdAsync(addressId);
        return _mapper.Map<AddressDto?>(address);
    }

    public async Task<AddressDto?> GetAddressDetailsAsync(int addressId)
    {
        ValidateId(addressId, nameof(addressId));

        var address = await _unitOfWork.Address.GetAddressDetailsAsync(addressId);
        return _mapper.Map<AddressDto?>(address);
    }

    public async Task<AddressDto> CreateAsync(CreateAddressDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var addressType = NormalizeAddressType(dto.AddressType);

        ValidateAddressFields(
            dto.AddressLine1,
            dto.City,
            dto.State,
            dto.PinCode);

        await ValidateCustomerAndCountryAsync(
            dto.CustomerId,
            dto.CountryId);

        var existingAddresses =
            await _unitOfWork.Address.GetAddressesByCustomerIdAsync(dto.CustomerId);

        if (dto.IsPrimary)
        {
            await ClearPrimaryAddressesAsync(dto.CustomerId);
        }
        else if (!existingAddresses.Any(x => x.IsPrimary))
        {
            // The first address of a customer becomes the primary address.
            dto.IsPrimary = true;
        }

        var address = _mapper.Map<Address>(dto);

        address.AddressType = addressType;
        address.AddressLine1 = dto.AddressLine1.Trim();
        address.AddressLine2 = NormalizeOptionalText(dto.AddressLine2);
        address.Landmark = NormalizeOptionalText(dto.Landmark);
        address.City = dto.City.Trim();
        address.District = NormalizeOptionalText(dto.District);
        address.State = dto.State.Trim();
        address.PinCode = dto.PinCode.Trim();

        // Address verification must be performed through VerifyAddressAsync.
        address.IsVerified = false;
        address.VerifiedAt = null;

        address.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Address.AddAsync(address);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AddressDto>(address);
    }

    public async Task<AddressDto?> UpdateAsync(UpdateAddressDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AddressId, nameof(dto.AddressId));

        var address = await _unitOfWork.Address.GetByIdAsync(dto.AddressId);

        if (address == null)
            return null;

        if (address.CustomerId != dto.CustomerId)
            throw new InvalidOperationException(
                "An address cannot be transferred to another customer through an update operation.");

        var addressType = NormalizeAddressType(dto.AddressType);

        ValidateAddressFields(
            dto.AddressLine1,
            dto.City,
            dto.State,
            dto.PinCode);

        await ValidateCustomerAndCountryAsync(
            dto.CustomerId,
            dto.CountryId);

        var existingAddresses =
            await _unitOfWork.Address.GetAddressesByCustomerIdAsync(dto.CustomerId);

        if (dto.IsPrimary)
        {
            await ClearPrimaryAddressesAsync(dto.CustomerId, dto.AddressId);
        }
        else if (address.IsPrimary &&
                 !existingAddresses.Any(x =>
                     x.AddressId != dto.AddressId && x.IsPrimary))
        {
            // Do not leave the customer without a primary address.
            throw new InvalidOperationException(
                "A customer must have one primary address. Set another address as primary first.");
        }

        // Preserve verification history. Clients cannot verify an address through UpdateAsync.
        var wasVerified = address.IsVerified;
        var verifiedAt = address.VerifiedAt;

        _mapper.Map(dto, address);

        address.AddressType = addressType;
        address.AddressLine1 = dto.AddressLine1.Trim();
        address.AddressLine2 = NormalizeOptionalText(dto.AddressLine2);
        address.Landmark = NormalizeOptionalText(dto.Landmark);
        address.City = dto.City.Trim();
        address.District = NormalizeOptionalText(dto.District);
        address.State = dto.State.Trim();
        address.PinCode = dto.PinCode.Trim();

        address.IsVerified = wasVerified;
        address.VerifiedAt = verifiedAt;
        address.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Address.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<AddressDto>(address);
    }

    public async Task<bool> DeleteAsync(DeleteAddressDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.AddressId, nameof(dto.AddressId));

        var address = await _unitOfWork.Address.GetByIdAsync(dto.AddressId);

        if (address == null)
            return false;

        if (!await CanAddressBeDeletedAsync(dto.AddressId))
            return false;

        _unitOfWork.Address.Delete(address);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CUSTOMER ADDRESS MANAGEMENT

    public async Task<IEnumerable<AddressDto>> GetAddressesByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var addresses = await _unitOfWork.Address
            .GetAddressesByCustomerIdAsync(customerId);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<AddressDto?> GetPrimaryAddressByCustomerIdAsync(
        int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var address = await _unitOfWork.Address
            .GetPrimaryAddressByCustomerIdAsync(customerId);

        return _mapper.Map<AddressDto?>(address);
    }

    public async Task<IEnumerable<AddressDto>> GetByCustomerAndAddressTypeAsync(
        int customerId,
        string addressType)
    {
        ValidateId(customerId, nameof(customerId));
        addressType = NormalizeAddressType(addressType);

        var addresses = await _unitOfWork.Address
            .GetByCustomerAndAddressTypeAsync(customerId, addressType);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<bool> SetPrimaryAddressAsync(int addressId)
    {
        ValidateId(addressId, nameof(addressId));

        var address = await _unitOfWork.Address.GetByIdAsync(addressId);

        if (address == null)
            return false;

        var customer = await _unitOfWork.Customer
            .GetByIdAsync(address.CustomerId);

        if (customer == null)
            throw new InvalidOperationException(
                "The customer associated with this address does not exist.");

        if (address.IsPrimary)
            return true;

        await ClearPrimaryAddressesAsync(
            address.CustomerId,
            address.AddressId);

        address.IsPrimary = true;
        address.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Address.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> HasAddressOfTypeAsync(
        int customerId,
        string addressType)
    {
        ValidateId(customerId, nameof(customerId));
        addressType = NormalizeAddressType(addressType);

        return await _unitOfWork.Address
            .HasAddressOfTypeAsync(customerId, addressType);
    }

    public async Task<bool> HasVerifiedPrimaryAddressAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        return await _unitOfWork.Address
            .HasVerifiedPrimaryAddressAsync(customerId);
    }

    // 3. COUNTRY AND LOCATION

    public async Task<IEnumerable<AddressDto>> GetAddressesByCountryIdAsync(
        int countryId)
    {
        ValidateId(countryId, nameof(countryId));

        var addresses = await _unitOfWork.Address
            .GetAddressesByCountryIdAsync(countryId);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesByLocationAsync(
        string state,
        string city)
    {
        state = RequiredText(state, nameof(state));
        city = RequiredText(city, nameof(city));

        var addresses = await _unitOfWork.Address
            .GetAddressesByLocationAsync(state, city);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesByDistrictAsync(
        string district)
    {
        district = RequiredText(district, nameof(district));

        var addresses = await _unitOfWork.Address
            .GetAddressesByDistrictAsync(district);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesByPinCodeAsync(
        string pinCode)
    {
        pinCode = NormalizePinCode(pinCode);

        var addresses = await _unitOfWork.Address
            .GetAddressesByPinCodeAsync(pinCode);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    // 4. ADDRESS TYPE AND VERIFICATION

    public async Task<IEnumerable<AddressDto>> GetAddressesByTypeAsync(
        string addressType)
    {
        addressType = NormalizeAddressType(addressType);

        var addresses = await _unitOfWork.Address
            .GetAddressesByTypeAsync(addressType);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<IEnumerable<AddressDto>> GetVerifiedAddressesAsync()
    {
        var addresses = await _unitOfWork.Address.GetVerifiedAddressesAsync();
        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<IEnumerable<AddressDto>> GetUnverifiedAddressesAsync()
    {
        var addresses = await _unitOfWork.Address.GetUnverifiedAddressesAsync();
        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<IEnumerable<AddressDto>> GetAddressesVerifiedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var addresses = await _unitOfWork.Address
            .GetAddressesVerifiedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    public async Task<bool> VerifyAddressAsync(int addressId)
    {
        ValidateId(addressId, nameof(addressId));

        var address = await _unitOfWork.Address.GetByIdAsync(addressId);

        if (address == null)
            return false;

        if (address.IsVerified)
            return true;

        address.IsVerified = true;
        address.VerifiedAt = DateTime.UtcNow;
        address.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Address.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkAddressAsUnverifiedAsync(int addressId)
    {
        ValidateId(addressId, nameof(addressId));

        var address = await _unitOfWork.Address.GetByIdAsync(addressId);

        if (address == null)
            return false;

        if (!address.IsVerified)
            return true;

        address.IsVerified = false;
        address.VerifiedAt = null;
        address.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Address.Update(address);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 5. SEARCH

    public async Task<IEnumerable<AddressDto>> SearchAsync(
        string? searchTerm,
        int? customerId,
        int? countryId,
        string? addressType,
        bool? isVerified,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (customerId.HasValue)
            ValidateId(customerId.Value, nameof(customerId));

        if (countryId.HasValue)
            ValidateId(countryId.Value, nameof(countryId));

        var normalizedAddressType = string.IsNullOrWhiteSpace(addressType)
            ? null
            : NormalizeAddressType(addressType);

        var addresses = await _unitOfWork.Address.SearchAddressesAsync(
            searchTerm?.Trim(),
            customerId,
            countryId,
            normalizedAddressType,
            isVerified,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<AddressDto>>(addresses);
    }

    // 6. VALIDATION

    public async Task<bool> CanAddressBeDeletedAsync(int addressId)
    {
        ValidateId(addressId, nameof(addressId));

        var address = await _unitOfWork.Address.GetByIdAsync(addressId);

        if (address == null)
            return false;

        // Preserve verified addresses as KYC/compliance evidence.
        if (address.IsVerified || address.VerifiedAt.HasValue)
            return false;

        // A primary address cannot be removed until another address is made primary.
        if (address.IsPrimary)
            return false;

        return true;
    }

    // PRIVATE BUSINESS-RULE HELPERS

    private async Task ValidateCustomerAndCountryAsync(
        int customerId,
        int countryId)
    {
        ValidateId(customerId, nameof(customerId));
        ValidateId(countryId, nameof(countryId));

        var customer = await _unitOfWork.Customer.GetByIdAsync(customerId);

        if (customer == null)
            throw new InvalidOperationException(
                "The selected customer does not exist.");

        var country = await _unitOfWork.Country.GetByIdAsync(countryId);

        if (country == null)
            throw new InvalidOperationException(
                "The selected country does not exist.");

        if (!country.IsActive)
            throw new InvalidOperationException(
                "An address cannot be assigned to an inactive country.");
    }

    private async Task ClearPrimaryAddressesAsync(
        int customerId,
        int? exceptAddressId = null)
    {
        var addresses = await _unitOfWork.Address
            .GetAddressesByCustomerIdAsync(customerId);

        foreach (var existingAddress in addresses.Where(x =>
                     x.IsPrimary &&
                     x.AddressId != exceptAddressId))
        {
            existingAddress.IsPrimary = false;
            existingAddress.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Address.Update(existingAddress);
        }
    }

    private static string NormalizeAddressType(string addressType)
    {
        if (string.IsNullOrWhiteSpace(addressType))
            throw new ArgumentException(
                "Address type is required.",
                nameof(addressType));

        var normalized = ValidAddressTypes.FirstOrDefault(x =>
            string.Equals(
                x,
                addressType.Trim(),
                StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid address type. Allowed values: {string.Join(", ", ValidAddressTypes)}.",
            nameof(addressType));
    }

    private static void ValidateAddressFields(
        string addressLine1,
        string city,
        string state,
        string pinCode)
    {
        RequiredText(addressLine1, nameof(addressLine1));
        RequiredText(city, nameof(city));
        RequiredText(state, nameof(state));
        NormalizePinCode(pinCode);
    }

    private static string NormalizePinCode(string pinCode)
    {
        if (string.IsNullOrWhiteSpace(pinCode) ||
            pinCode.Trim().Length != 6 ||
            !pinCode.Trim().All(char.IsDigit))
        {
            throw new ArgumentException(
                "PIN code must contain exactly six digits.",
                nameof(pinCode));
        }

        return pinCode.Trim();
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "A value is required.",
                parameterName);

        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }

    private static void ValidateDateRange(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException(
                "Start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }
}