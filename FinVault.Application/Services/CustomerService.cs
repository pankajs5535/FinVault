using AutoMapper;
using FinVault.Application.DTOs.CustomerDtos;
using FinVault.Application.Interface.IRepositories;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class CustomerService : ICustomerService
{
    private static readonly string[] ValidCustomerTypes =
        { "Individual", "Minor", "Business" };

    private static readonly string[] ValidGenders =
        { "Male", "Female", "Other" };

    private static readonly string[] ValidStatuses =
        { "Active", "Inactive", "Dormant", "Blocked", "Closed" };

    private static readonly string[] ValidRiskCategories =
        { "Low", "Medium", "High" };

    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Employee> _employeeRepository;
    private readonly IMapper _mapper;

    public CustomerService(
        IUnitOfWork unitOfWork,
        IGenericRepository<Employee> employeeRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    // 1. CUSTOMER CRUD AND IDENTIFICATION

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _unitOfWork.Customer.GetAllAsync();
        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto?> GetByIdAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var customer = await _unitOfWork.Customer.GetByIdAsync(customerId);
        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<CustomerDto?> GetByCustomerNumberAsync(string customerNumber)
    {
        if (string.IsNullOrWhiteSpace(customerNumber))
            return null;

        var customer = await _unitOfWork.Customer
            .GetByCustomerNumberAsync(NormalizeCustomerNumber(customerNumber));

        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<CustomerDto?> GetByMobileNumberAsync(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
            return null;

        var customer = await _unitOfWork.Customer
            .GetByMobileNumberAsync(NormalizeMobileNumber(mobileNumber));

        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<CustomerDto?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var customer = await _unitOfWork.Customer
            .GetByEmailAsync(NormalizeEmail(email));

        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<CustomerDto?> GetCustomerDetailsAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var customer = await _unitOfWork.Customer
            .GetCustomerDetailsAsync(customerId);

        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var customerNumber = NormalizeCustomerNumber(dto.CustomerNumber);
        var mobileNumber = NormalizeMobileNumber(dto.MobileNumber);
        var email = NormalizeEmail(dto.Email);

        var customerType = NormalizeValue(
            dto.CustomerType, ValidCustomerTypes, nameof(dto.CustomerType));

        var gender = NormalizeOptionalValue(
            dto.Gender, ValidGenders, nameof(dto.Gender));

        var status = NormalizeValue(
            dto.Status, ValidStatuses, nameof(dto.Status));

        var riskCategory = NormalizeValue(
            dto.RiskCategory, ValidRiskCategories, nameof(dto.RiskCategory));

        ValidateCustomerDates(dto.DateOfBirth, dto.OnboardedAt, status, dto.ClosedAt);
        ValidateIncome(dto.AnnualIncome);

        await EnsureUniqueCustomerDetailsAsync(
            customerNumber, mobileNumber, email);

        await ValidateReferencesAsync(
            dto.HomeBranchId,
            dto.NationalityCountryId,
            dto.UserId,
            dto.RelationshipManagerId);

        var customer = _mapper.Map<Customer>(dto);

        customer.CustomerNumber = customerNumber;
        customer.MobileNumber = mobileNumber;
        customer.Email = email;
        customer.CustomerType = customerType;
        customer.Gender = gender;
        customer.Status = status;
        customer.RiskCategory = riskCategory;
        customer.FirstName = RequiredText(dto.FirstName, nameof(dto.FirstName));
        customer.MiddleName = NormalizeOptionalText(dto.MiddleName);
        customer.LastName = RequiredText(dto.LastName, nameof(dto.LastName));
        customer.AlternateMobileNumber =
            NormalizeOptionalMobileNumber(dto.AlternateMobileNumber);
        customer.Occupation = NormalizeOptionalText(dto.Occupation);
        customer.FatherOrSpouseName =
            NormalizeOptionalText(dto.FatherOrSpouseName);
        customer.PreferredLanguage =
            RequiredText(dto.PreferredLanguage, nameof(dto.PreferredLanguage));

        customer.OnboardedAt = dto.OnboardedAt == default
            ? DateTime.UtcNow
            : dto.OnboardedAt;

        customer.ClosedAt = status == "Closed"
            ? dto.ClosedAt ?? DateTime.UtcNow
            : null;

        customer.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Customer.AddAsync(customer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<CustomerDto?> UpdateAsync(UpdateCustomerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.CustomerId, nameof(dto.CustomerId));

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer == null)
            return null;

        if (dto.RowVersion == null ||
            customer.RowVersion == null ||
            !customer.RowVersion.SequenceEqual(dto.RowVersion))
        {
            throw new InvalidOperationException(
                "Customer record has been modified by another operation. Reload the customer and try again.");
        }

        var customerNumber = NormalizeCustomerNumber(dto.CustomerNumber);
        var mobileNumber = NormalizeMobileNumber(dto.MobileNumber);
        var email = NormalizeEmail(dto.Email);

        var customerType = NormalizeValue(
            dto.CustomerType, ValidCustomerTypes, nameof(dto.CustomerType));

        var gender = NormalizeOptionalValue(
            dto.Gender, ValidGenders, nameof(dto.Gender));

        var status = NormalizeValue(
            dto.Status, ValidStatuses, nameof(dto.Status));

        var riskCategory = NormalizeValue(
            dto.RiskCategory, ValidRiskCategories, nameof(dto.RiskCategory));

        ValidateCustomerDates(dto.DateOfBirth, dto.OnboardedAt, status, dto.ClosedAt);
        ValidateIncome(dto.AnnualIncome);

        await EnsureUniqueCustomerDetailsAsync(
            customerNumber, mobileNumber, email, dto.CustomerId);

        await ValidateReferencesAsync(
            dto.HomeBranchId,
            dto.NationalityCountryId,
            dto.UserId,
            dto.RelationshipManagerId);

        _mapper.Map(dto, customer);

        customer.CustomerNumber = customerNumber;
        customer.MobileNumber = mobileNumber;
        customer.Email = email;
        customer.CustomerType = customerType;
        customer.Gender = gender;
        customer.Status = status;
        customer.RiskCategory = riskCategory;
        customer.FirstName = RequiredText(dto.FirstName, nameof(dto.FirstName));
        customer.MiddleName = NormalizeOptionalText(dto.MiddleName);
        customer.LastName = RequiredText(dto.LastName, nameof(dto.LastName));
        customer.AlternateMobileNumber =
            NormalizeOptionalMobileNumber(dto.AlternateMobileNumber);
        customer.Occupation = NormalizeOptionalText(dto.Occupation);
        customer.FatherOrSpouseName =
            NormalizeOptionalText(dto.FatherOrSpouseName);
        customer.PreferredLanguage =
            RequiredText(dto.PreferredLanguage, nameof(dto.PreferredLanguage));

        customer.ClosedAt = status == "Closed"
            ? dto.ClosedAt ?? customer.ClosedAt ?? DateTime.UtcNow
            : null;

        customer.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Customer.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CustomerDto>(customer);
    }

    public async Task<bool> DeleteAsync(DeleteCustomerDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ValidateId(dto.CustomerId, nameof(dto.CustomerId));

        var customer = await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId);

        if (customer == null)
            return false;

        // Preserve banking history. Never physically delete customer records.
        if (customer.Status == "Closed")
            return true;

        customer.Status = "Closed";
        customer.ClosedAt = DateTime.UtcNow;
        customer.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Customer.Update(customer);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. CUSTOMER CLASSIFICATION

    public async Task<IEnumerable<CustomerDto>> GetCustomersByTypeAsync(string customerType)
    {
        customerType = NormalizeValue(
            customerType, ValidCustomerTypes, nameof(customerType));

        var customers = await _unitOfWork.Customer
            .GetCustomersByTypeAsync(customerType);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersByGenderAsync(string gender)
    {
        gender = NormalizeValue(gender, ValidGenders, nameof(gender));

        var customers = await _unitOfWork.Customer
            .GetCustomersByGenderAsync(gender);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersByRiskCategoryAsync(string riskCategory)
    {
        riskCategory = NormalizeValue(
            riskCategory, ValidRiskCategories, nameof(riskCategory));

        var customers = await _unitOfWork.Customer
            .GetCustomersByRiskCategoryAsync(riskCategory);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersByStatusAsync(string status)
    {
        status = NormalizeValue(status, ValidStatuses, nameof(status));

        var customers = await _unitOfWork.Customer
            .GetCustomersByStatusAsync(status);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    // 3. BRANCH AND RELATIONSHIP MANAGEMENT

    public async Task<IEnumerable<CustomerDto>> GetCustomersByHomeBranchIdAsync(int homeBranchId)
    {
        ValidateId(homeBranchId, nameof(homeBranchId));

        var customers = await _unitOfWork.Customer
            .GetCustomersByHomeBranchIdAsync(homeBranchId);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersByRelationshipManagerIdAsync(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));

        var customers = await _unitOfWork.Customer
            .GetCustomersByRelationshipManagerIdAsync(employeeId);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersByNationalityCountryIdAsync(int countryId)
    {
        ValidateId(countryId, nameof(countryId));

        var customers = await _unitOfWork.Customer
            .GetCustomersByNationalityCountryIdAsync(countryId);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<CustomerDto?> GetCustomerWithBranchAndManagerAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var customer = await _unitOfWork.Customer
            .GetCustomerWithBranchAndManagerAsync(customerId);

        return _mapper.Map<CustomerDto?>(customer);
    }

    // 4. ACCOUNT, KYC AND ADDRESS RELATIONSHIPS

    public async Task<CustomerDto?> GetCustomerWithAccountsAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var customer = await _unitOfWork.Customer
            .GetCustomerWithAccountsAsync(customerId);

        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<CustomerDto?> GetCustomerWithKycHistoryAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var customer = await _unitOfWork.Customer
            .GetCustomerWithKycHistoryAsync(customerId);

        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<CustomerDto?> GetCustomerWithAddressesAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        var customer = await _unitOfWork.Customer
            .GetCustomerWithAddressesAsync(customerId);

        return _mapper.Map<CustomerDto?>(customer);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersWithOnlineBankingAsync()
    {
        var customers = await _unitOfWork.Customer
            .GetCustomersWithOnlineBankingAsync();

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    // 5. ONBOARDING AND AUDIT HISTORY

    public async Task<IEnumerable<CustomerDto>> GetCustomersOnboardedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var customers = await _unitOfWork.Customer
            .GetCustomersOnboardedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<IEnumerable<CustomerDto>> GetRecentlyOnboardedCustomersAsync(int count)
    {
        if (count <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(count), "Customer count must be greater than zero.");

        var customers = await _unitOfWork.Customer
            .GetRecentlyOnboardedCustomersAsync(count);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<IEnumerable<CustomerDto>> GetCustomersUpdatedWithinRangeAsync(
        DateTime startDate,
        DateTime endDate)
    {
        ValidateDateRange(startDate, endDate);

        var customers = await _unitOfWork.Customer
            .GetCustomersUpdatedWithinRangeAsync(startDate, endDate);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    // 6. SEARCH AND VALIDATION

    public async Task<IEnumerable<CustomerDto>> SearchAsync(
        string? searchTerm,
        string? status,
        string? riskCategory,
        int? homeBranchId,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (homeBranchId.HasValue)
            ValidateId(homeBranchId.Value, nameof(homeBranchId));

        var normalizedStatus = string.IsNullOrWhiteSpace(status)
            ? null
            : NormalizeValue(status, ValidStatuses, nameof(status));

        var normalizedRiskCategory = string.IsNullOrWhiteSpace(riskCategory)
            ? null
            : NormalizeValue(riskCategory, ValidRiskCategories, nameof(riskCategory));

        var customers = await _unitOfWork.Customer.SearchCustomersAsync(
            searchTerm?.Trim(),
            normalizedStatus,
            normalizedRiskCategory,
            homeBranchId,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<CustomerDto>>(customers);
    }

    public async Task<bool> ExistsByCustomerNumberAsync(string customerNumber)
    {
        if (string.IsNullOrWhiteSpace(customerNumber))
            return false;

        return await _unitOfWork.Customer
            .ExistsByCustomerNumberAsync(NormalizeCustomerNumber(customerNumber));
    }

    public async Task<bool> ExistsByMobileNumberAsync(string mobileNumber)
    {
        if (string.IsNullOrWhiteSpace(mobileNumber))
            return false;

        return await _unitOfWork.Customer
            .ExistsByMobileNumberAsync(NormalizeMobileNumber(mobileNumber));
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _unitOfWork.Customer
            .ExistsByEmailAsync(NormalizeEmail(email)!);
    }

    public async Task<int> CountCustomerAccountsAsync(int customerId)
    {
        ValidateId(customerId, nameof(customerId));

        return await _unitOfWork.Customer
            .CountCustomerAccountsAsync(customerId);
    }

    // PRIVATE VALIDATION AND BUSINESS-RULE HELPERS

    private async Task EnsureUniqueCustomerDetailsAsync(
        string customerNumber,
        string mobileNumber,
        string? email,
        int? excludeCustomerId = null)
    {
        var existingByNumber = await _unitOfWork.Customer
            .GetByCustomerNumberAsync(customerNumber);

        if (existingByNumber != null &&
            existingByNumber.CustomerId != excludeCustomerId)
        {
            throw new InvalidOperationException(
                $"Customer number '{customerNumber}' already exists.");
        }

        var existingByMobile = await _unitOfWork.Customer
            .GetByMobileNumberAsync(mobileNumber);

        if (existingByMobile != null &&
            existingByMobile.CustomerId != excludeCustomerId)
        {
            throw new InvalidOperationException(
                $"Mobile number '{mobileNumber}' is already registered.");
        }

        if (email == null)
            return;

        var existingByEmail = await _unitOfWork.Customer.GetByEmailAsync(email);

        if (existingByEmail != null &&
            existingByEmail.CustomerId != excludeCustomerId)
        {
            throw new InvalidOperationException(
                $"Email address '{email}' is already registered.");
        }
    }

    private async Task ValidateReferencesAsync(
        int homeBranchId,
        int nationalityCountryId,
        int? userId,
        int? relationshipManagerId)
    {
        ValidateId(homeBranchId, nameof(homeBranchId));
        ValidateId(nationalityCountryId, nameof(nationalityCountryId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(homeBranchId);

        if (branch == null)
            throw new InvalidOperationException(
                "The selected home branch does not exist.");

        if (!branch.IsActive || branch.ClosedOn.HasValue)
            throw new InvalidOperationException(
                "The selected home branch is not active.");

        var country = await _unitOfWork.Country.GetByIdAsync(nationalityCountryId);

        if (country == null)
            throw new InvalidOperationException(
                "The selected nationality country does not exist.");

        if (!country.IsActive)
            throw new InvalidOperationException(
                "The selected nationality country is inactive.");

        if (userId.HasValue)
        {
            ValidateId(userId.Value, nameof(userId));

            var user = await _unitOfWork.User.GetByIdAsync(userId.Value);

            if (user == null)
                throw new InvalidOperationException(
                    "The selected online banking user does not exist.");
        }

        if (relationshipManagerId.HasValue)
        {
            ValidateId(relationshipManagerId.Value, nameof(relationshipManagerId));

            var employee = await _employeeRepository
                .GetByIdAsync(relationshipManagerId.Value);

            if (employee == null)
                throw new InvalidOperationException(
                    "The selected relationship manager does not exist.");

            if (!string.Equals(
                    employee.EmploymentStatus,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only an active employee can be assigned as a relationship manager.");
            }
        }
    }

    private static void ValidateCustomerDates(
        DateTime dateOfBirth,
        DateTime onboardedAt,
        string status,
        DateTime? closedAt)
    {
        if (dateOfBirth == default || dateOfBirth.Date > DateTime.UtcNow.Date)
            throw new ArgumentException(
                "Customer date of birth must be valid and cannot be in the future.");

        if (onboardedAt != default && onboardedAt > DateTime.UtcNow)
            throw new ArgumentException(
                "Customer onboarding date cannot be in the future.");

        if (closedAt.HasValue &&
            closedAt.Value < (onboardedAt == default ? DateTime.MinValue : onboardedAt))
        {
            throw new ArgumentException(
                "Customer closing date cannot be earlier than the onboarding date.");
        }

        if (status == "Closed" && closedAt.HasValue &&
            closedAt.Value > DateTime.UtcNow)
        {
            throw new ArgumentException(
                "Customer closing date cannot be in the future.");
        }

        if (status != "Closed" && closedAt.HasValue)
            throw new InvalidOperationException(
                "ClosedAt can only be provided when customer status is Closed.");
    }

    private static void ValidateIncome(decimal? annualIncome)
    {
        if (annualIncome < 0)
            throw new ArgumentOutOfRangeException(
                nameof(annualIncome), "Annual income cannot be negative.");
    }

    private static void ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate == default || endDate == default)
            throw new ArgumentException("Both start date and end date are required.");

        if (endDate < startDate)
            throw new ArgumentException(
                "End date cannot be earlier than start date.");
    }

    private static string NormalizeCustomerNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Customer number is required.", nameof(value));

        return value.Trim().ToUpperInvariant();
    }

    private static string NormalizeMobileNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Mobile number is required.", nameof(value));

        return value.Trim();
    }

    private static string? NormalizeEmail(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToLowerInvariant();
    }

    private static string? NormalizeOptionalMobileNumber(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);

        return value.Trim();
    }

    private static string? NormalizeOptionalText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string NormalizeValue(
        string value,
        string[] allowedValues,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A value is required.", parameterName);

        var normalized = allowedValues.FirstOrDefault(
            x => string.Equals(
                x, value.Trim(), StringComparison.OrdinalIgnoreCase));

        return normalized ?? throw new ArgumentException(
            $"Invalid value. Allowed values: {string.Join(", ", allowedValues)}.",
            parameterName);
    }

    private static string? NormalizeOptionalValue(
        string? value,
        string[] allowedValues,
        string parameterName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : NormalizeValue(value, allowedValues, parameterName);
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName, "ID must be greater than zero.");
    }
}