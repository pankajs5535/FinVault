using AutoMapper;
using FinVault.Application.DTOs.CustomerDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Handles customer onboarding, profile maintenance and customer status changes.</summary>
public sealed class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Finds a customer by ID and returns null when the record does not exist.
    public async Task<CustomerDto?> GetByIdAsync(int customerId)
    {
        var customer = await _unitOfWork.Customer.GetCustomerDetailsAsync(customerId);
        return customer is null ? null : _mapper.Map<CustomerDto>(customer);
    }

    // 2. Returns the customer list for administrative screens.
    public async Task<IReadOnlyList<CustomerDto>> GetAllAsync()
    {
        var customers = await _unitOfWork.Customer.GetAllAsync();
        return _mapper.Map<IReadOnlyList<CustomerDto>>(customers);
    }

    // 3. Checks unique customer identifiers and required master records before onboarding.
    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var customerNumber = dto.CustomerNumber.Trim();
        var mobile = dto.MobileNumber.Trim();
        var email = dto.Email?.Trim();

        if (await _unitOfWork.Customer.ExistsByCustomerNumberAsync(customerNumber))
            throw new InvalidOperationException("Customer number is already registered.");
        if (await _unitOfWork.Customer.ExistsByMobileNumberAsync(mobile))
            throw new InvalidOperationException("Mobile number is already registered.");
        if (!string.IsNullOrWhiteSpace(email) && await _unitOfWork.Customer.ExistsByEmailAsync(email))
            throw new InvalidOperationException("Email address is already registered.");
        if (await _unitOfWork.Branch.GetByIdAsync(dto.HomeBranchId) is null)
            throw new InvalidOperationException("The selected home branch does not exist.");
        if (await _unitOfWork.Country.GetByIdAsync(dto.NationalityCountryId) is null)
            throw new InvalidOperationException("The selected nationality country does not exist.");

        var customer = _mapper.Map<Customer>(dto);
        customer.CustomerNumber = customerNumber;
        customer.MobileNumber = mobile;
        customer.Email = email;
        customer.OnboardedAt = dto.OnboardedAt == default ? DateTime.UtcNow : dto.OnboardedAt;
        customer.CreatedAt = DateTime.UtcNow;
        customer.Status = "Active";

        await _unitOfWork.Customer.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CustomerDto>(customer);
    }

    // 4. Updates an existing customer and prevents duplicate contact information.
    public async Task<CustomerDto?> UpdateAsync(int customerId, UpdateCustomerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.CustomerId != customerId)
            throw new ArgumentException("The route customer ID and DTO customer ID do not match.", nameof(dto));

        var customer = await _unitOfWork.Customer.GetByIdAsync(customerId);
        if (customer is null) return null;

        var numberMatch = await _unitOfWork.Customer.GetByCustomerNumberAsync(dto.CustomerNumber.Trim());
        var mobileMatch = await _unitOfWork.Customer.GetByMobileNumberAsync(dto.MobileNumber.Trim());
        var emailMatch = string.IsNullOrWhiteSpace(dto.Email) ? null : await _unitOfWork.Customer.GetByEmailAsync(dto.Email.Trim());
        if ((numberMatch is not null && numberMatch.CustomerId != customerId) ||
            (mobileMatch is not null && mobileMatch.CustomerId != customerId) ||
            (emailMatch is not null && emailMatch.CustomerId != customerId))
            throw new InvalidOperationException("Another customer already uses the supplied customer number or contact information.");
        if (await _unitOfWork.Branch.GetByIdAsync(dto.HomeBranchId) is null ||
            await _unitOfWork.Country.GetByIdAsync(dto.NationalityCountryId) is null)
            throw new InvalidOperationException("The selected branch or nationality country does not exist.");

        _mapper.Map(dto, customer);
        customer.CustomerNumber = dto.CustomerNumber.Trim();
        customer.MobileNumber = dto.MobileNumber.Trim();
        customer.Email = dto.Email?.Trim();
        customer.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Customer.Update(customer);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CustomerDto>(customer);
    }

    // 5. Deactivates rather than physically deleting a customer with banking history.
    public async Task<bool> DeactivateAsync(int customerId)
    {
        var customer = await _unitOfWork.Customer.GetByIdAsync(customerId);
        if (customer is null) return false;
        if (customer.Status == "Closed") return false;
        customer.Status = "Inactive";
        customer.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Customer.Update(customer);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
