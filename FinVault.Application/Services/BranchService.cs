using System.Text.RegularExpressions;
using AutoMapper;
using FinVault.Application.DTOs.BranchDtos;
using FinVault.Application.Interface.IRepositories;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public class BranchService : IBranchService
{
    private static readonly Regex IfscRegex = new(
        "^[A-Z]{4}0[A-Z0-9]{6}$",
        RegexOptions.Compiled);

    private static readonly Regex MicrRegex = new(
        "^\\d{9}$",
        RegexOptions.Compiled);

    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public BranchService(
        IUnitOfWork unitOfWork,
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    // 1. CRUD AND IDENTIFICATION

    public async Task<IEnumerable<BranchDto>> GetAllAsync()
    {
        var branches = await _unitOfWork.Branch.GetAllAsync();
        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<BranchDto?> GetByIdAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));
        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);
        return _mapper.Map<BranchDto?>(branch);
    }

    public async Task<BranchDto?> GetByBranchCodeAsync(string branchCode)
    {
        if (string.IsNullOrWhiteSpace(branchCode))
            return null;

        var branch = await _unitOfWork.Branch.GetByBranchCodeAsync(
            NormalizeCode(branchCode));

        return _mapper.Map<BranchDto?>(branch);
    }

    public async Task<BranchDto?> GetByIfscCodeAsync(string ifscCode)
    {
        if (string.IsNullOrWhiteSpace(ifscCode))
            return null;

        var branch = await _unitOfWork.Branch.GetByIfscCodeAsync(
            NormalizeCode(ifscCode));

        return _mapper.Map<BranchDto?>(branch);
    }

    public async Task<BranchDto?> GetByMicrCodeAsync(string micrCode)
    {
        if (string.IsNullOrWhiteSpace(micrCode))
            return null;

        var branch = await _unitOfWork.Branch.GetByMicrCodeAsync(
            micrCode.Trim());

        return _mapper.Map<BranchDto?>(branch);
    }

    public async Task<BranchDto?> GetByBranchNameAsync(string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            return null;

        var branches = await _unitOfWork.Branch.FindAsync(
            x => x.BranchName == branchName.Trim());

        var branch = branches
            .OrderBy(x => x.BranchCode)
            .FirstOrDefault();

        return _mapper.Map<BranchDto?>(branch);
    }

    public async Task<BranchDto?> GetBranchDetailsAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch.GetBranchDetailsAsync(branchId);

        return _mapper.Map<BranchDto?>(branch);
    }

    public async Task<BranchDto> CreateAsync(CreateBranchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var branchCode = NormalizeCode(dto.BranchCode);
        var ifscCode = NormalizeCode(dto.IfscCode);
        var micrCode = NormalizeOptionalCode(dto.MicrCode);

        ValidateIfsc(ifscCode);
        ValidateMicr(micrCode);
        ValidateDates(dto.OpenedOn, dto.ClosedOn, dto.IsActive);

        await EnsureUniqueCodesAsync(
            branchCode,
            ifscCode,
            micrCode);

        await ValidateCountryAsync(dto.CountryId);

        await ValidateParentBranchAsync(
            dto.ParentBranchId,
            null,
            dto.CountryId);

        await ValidateManagerAsync(dto.ManagerEmployeeId);

        var branch = _mapper.Map<Branch>(dto);

        branch.BranchCode = branchCode;
        branch.IfscCode = ifscCode;
        branch.MicrCode = micrCode;
        branch.BranchName = RequiredText(
            dto.BranchName,
            nameof(dto.BranchName));

        branch.State = RequiredText(dto.State, nameof(dto.State));
        branch.City = RequiredText(dto.City, nameof(dto.City));
        branch.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Branch.AddAsync(branch);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BranchDto>(branch);
    }

    public async Task<BranchDto?> UpdateAsync(UpdateBranchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.BranchId, nameof(dto.BranchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(dto.BranchId);

        if (branch == null)
            return null;

        var branchCode = NormalizeCode(dto.BranchCode);
        var ifscCode = NormalizeCode(dto.IfscCode);
        var micrCode = NormalizeOptionalCode(dto.MicrCode);

        ValidateIfsc(ifscCode);
        ValidateMicr(micrCode);
        ValidateDates(dto.OpenedOn, dto.ClosedOn, dto.IsActive);

        await EnsureUniqueCodesAsync(
            branchCode,
            ifscCode,
            micrCode,
            dto.BranchId);

        await ValidateCountryAsync(dto.CountryId);

        await ValidateParentBranchAsync(
            dto.ParentBranchId,
            dto.BranchId,
            dto.CountryId);

        await ValidateManagerAsync(dto.ManagerEmployeeId);

        _mapper.Map(dto, branch);

        branch.BranchCode = branchCode;
        branch.IfscCode = ifscCode;
        branch.MicrCode = micrCode;
        branch.BranchName = RequiredText(
            dto.BranchName,
            nameof(dto.BranchName));

        branch.State = RequiredText(dto.State, nameof(dto.State));
        branch.City = RequiredText(dto.City, nameof(dto.City));
        branch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Branch.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<BranchDto>(branch);
    }

    public async Task<bool> DeleteAsync(DeleteBranchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        ValidateId(dto.BranchId, nameof(dto.BranchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(dto.BranchId);

        if (branch == null ||
            !await CanBranchBeDeletedAsync(dto.BranchId))
            return false;

        _unitOfWork.Branch.Delete(branch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 2. STATUS AND LOCATION

    public async Task<IEnumerable<BranchDto>> GetActiveBranchesAsync()
    {
        var branches = await _unitOfWork.Branch.GetActiveBranchesAsync();
        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetBranchesByCountryIdAsync(int countryId)
    {
        ValidateId(countryId, nameof(countryId));

        var branches = await _unitOfWork.Branch
            .GetBranchesByCountryIdAsync(countryId);

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetBranchesByLocationAsync(
        string state,
        string city)
    {
        state = RequiredText(state, nameof(state));
        city = RequiredText(city, nameof(city));

        var branches = await _unitOfWork.Branch
            .GetBranchesByLocationAsync(state, city);

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetBranchesByTypeAsync(string branchType)
    {
        branchType = RequiredText(branchType, nameof(branchType));

        ValidateBranchType(branchType);

        var branches = await _unitOfWork.Branch
            .GetBranchesByTypeAsync(branchType);

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetRecentlyOpenedBranchesAsync(int count)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        var branches = await _unitOfWork.Branch
            .GetRecentlyOpenedBranchesAsync(count);

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<bool> ActivateAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);

        if (branch == null || branch.ClosedOn.HasValue)
            return false;

        if (branch.IsActive)
            return true;

        var country = await _unitOfWork.Country.GetByIdAsync(branch.CountryId);

        if (country == null || !country.IsActive)
            return false;

        branch.IsActive = true;
        branch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Branch.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);

        if (branch == null || branch.ClosedOn.HasValue)
            return false;

        if (!branch.IsActive)
            return true;

        branch.IsActive = false;
        branch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Branch.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CloseBranchAsync(int branchId, DateTime closedOn)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);

        if (branch == null ||
            branch.ClosedOn.HasValue ||
            closedOn.Date < branch.OpenedOn.Date)
            return false;

        if (await IsBranchInUseAsync(branchId))
            return false;

        branch.IsActive = false;
        branch.ClosedOn = closedOn;
        branch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Branch.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 3. BRANCH HIERARCHY AND MANAGEMENT

    public async Task<IEnumerable<BranchDto>> GetChildBranchesAsync(int parentBranchId)
    {
        ValidateId(parentBranchId, nameof(parentBranchId));

        var branches = await _unitOfWork.Branch
            .GetChildBranchesAsync(parentBranchId);

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetRootBranchesAsync()
    {
        var branches = await _unitOfWork.Branch.GetRootBranchesAsync();
        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetBranchesByManagerIdAsync(
        int managerEmployeeId)
    {
        ValidateId(managerEmployeeId, nameof(managerEmployeeId));

        var branches = await _unitOfWork.Branch
            .GetBranchesByManagerIdAsync(managerEmployeeId);

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<BranchDto?> GetBranchHierarchyAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch
            .GetBranchHierarchyAsync(branchId);

        return _mapper.Map<BranchDto?>(branch);
    }

    public async Task<bool> AssignManagerAsync(
        int branchId,
        int managerEmployeeId)
    {
        ValidateId(branchId, nameof(branchId));
        ValidateId(managerEmployeeId, nameof(managerEmployeeId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);

        if (branch == null)
            return false;

        await ValidateManagerAsync(managerEmployeeId);

        branch.ManagerEmployeeId = managerEmployeeId;
        branch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Branch.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveManagerAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);

        if (branch == null)
            return false;

        if (!branch.ManagerEmployeeId.HasValue)
            return true;

        branch.ManagerEmployeeId = null;
        branch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Branch.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangeParentBranchAsync(
        int branchId,
        int? parentBranchId)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);

        if (branch == null)
            return false;

        await ValidateParentBranchAsync(
            parentBranchId,
            branchId,
            branch.CountryId);

        branch.ParentBranchId = parentBranchId;
        branch.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Branch.Update(branch);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    // 4. BUSINESS RELATIONSHIPS

    public async Task<IEnumerable<BranchDto>> GetBranchesWithEmployeesAsync()
    {
        var branches = await _unitOfWork.Branch
            .GetBranchesWithEmployeesAsync();

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetBranchesWithAccountsAsync()
    {
        var branches = await _unitOfWork.Branch
            .GetBranchesWithAccountsAsync();

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<IEnumerable<BranchDto>> GetBranchesWithLoansAsync()
    {
        var branches = await _unitOfWork.Branch
            .GetBranchesWithLoansAsync();

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    // 5. SEARCH AND VALIDATION

    public async Task<IEnumerable<BranchDto>> SearchAsync(
        string? searchTerm,
        int? countryId,
        string? state,
        string? city,
        string? branchType,
        bool? isActive,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));

        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (countryId.HasValue)
            ValidateId(countryId.Value, nameof(countryId));

        if (!string.IsNullOrWhiteSpace(branchType))
            ValidateBranchType(branchType.Trim());

        var branches = await _unitOfWork.Branch.SearchBranchesAsync(
            searchTerm?.Trim(),
            countryId,
            state?.Trim(),
            city?.Trim(),
            branchType?.Trim(),
            isActive,
            pageNumber,
            pageSize);

        return _mapper.Map<IEnumerable<BranchDto>>(branches);
    }

    public async Task<bool> IsBranchCodeExistsAsync(string branchCode)
    {
        if (string.IsNullOrWhiteSpace(branchCode))
            return false;

        var matches = await _unitOfWork.Branch.FindAsync(
            x => x.BranchCode == NormalizeCode(branchCode));

        return matches.Any();
    }

    public async Task<bool> IsIfscCodeExistsAsync(string ifscCode)
    {
        if (string.IsNullOrWhiteSpace(ifscCode))
            return false;

        var matches = await _unitOfWork.Branch.FindAsync(
            x => x.IfscCode == NormalizeCode(ifscCode));

        return matches.Any();
    }

    public async Task<bool> IsMicrCodeExistsAsync(string micrCode)
    {
        if (string.IsNullOrWhiteSpace(micrCode))
            return false;

        var matches = await _unitOfWork.Branch.FindAsync(
            x => x.MicrCode == micrCode.Trim());

        return matches.Any();
    }

    public async Task<bool> IsBranchInUseAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        return await _unitOfWork.Branch.IsBranchInUseAsync(branchId);
    }

    public async Task<bool> CanBranchBeDeletedAsync(int branchId)
    {
        ValidateId(branchId, nameof(branchId));

        var branch = await _unitOfWork.Branch.GetByIdAsync(branchId);

        if (branch == null)
            return false;

        return !await IsBranchInUseAsync(branchId);
    }

    // PRIVATE VALIDATION AND BUSINESS-RULE HELPERS

    private async Task EnsureUniqueCodesAsync(
        string branchCode,
        string ifscCode,
        string? micrCode,
        int? excludeBranchId = null)
    {
        var branchCodeMatches = await _unitOfWork.Branch.FindAsync(
            x => x.BranchCode == branchCode);

        if (branchCodeMatches.Any(x => x.BranchId != excludeBranchId))
            throw new InvalidOperationException(
                $"Branch code '{branchCode}' is already assigned.");

        var ifscMatches = await _unitOfWork.Branch.FindAsync(
            x => x.IfscCode == ifscCode);

        if (ifscMatches.Any(x => x.BranchId != excludeBranchId))
            throw new InvalidOperationException(
                $"IFSC code '{ifscCode}' is already assigned.");

        if (micrCode == null)
            return;

        var micrMatches = await _unitOfWork.Branch.FindAsync(
            x => x.MicrCode == micrCode);

        if (micrMatches.Any(x => x.BranchId != excludeBranchId))
            throw new InvalidOperationException(
                $"MICR code '{micrCode}' is already assigned.");
    }

    private async Task ValidateCountryAsync(int countryId)
    {
        ValidateId(countryId, nameof(countryId));

        var country = await _unitOfWork.Country.GetByIdAsync(countryId);

        if (country == null)
            throw new InvalidOperationException(
                "The selected country does not exist.");

        if (!country.IsActive)
            throw new InvalidOperationException(
                "A branch cannot be assigned to an inactive country.");
    }

    private async Task ValidateManagerAsync(int? managerEmployeeId)
    {
        if (!managerEmployeeId.HasValue)
            return;

        ValidateId(managerEmployeeId.Value, nameof(managerEmployeeId));

        var employee = await _employeeRepository.GetByIdAsync(
            managerEmployeeId.Value);

        if (employee == null)
            throw new InvalidOperationException(
                "The selected branch manager does not exist.");

        if (!string.Equals(
                employee.EmploymentStatus,
                "Active",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Only an active employee can be assigned as a branch manager.");
        }
    }

    private async Task ValidateParentBranchAsync(
        int? parentBranchId,
        int? currentBranchId,
        int countryId)
    {
        if (!parentBranchId.HasValue)
            return;

        ValidateId(parentBranchId.Value, nameof(parentBranchId));

        if (currentBranchId.HasValue &&
            parentBranchId.Value == currentBranchId.Value)
        {
            throw new InvalidOperationException(
                "A branch cannot be its own parent.");
        }

        var parent = await _unitOfWork.Branch.GetByIdAsync(
            parentBranchId.Value);

        if (parent == null)
            throw new InvalidOperationException(
                "The selected parent branch does not exist.");

        if (!parent.IsActive || parent.ClosedOn.HasValue)
            throw new InvalidOperationException(
                "The parent branch must be active and not closed.");

        if (parent.CountryId != countryId)
            throw new InvalidOperationException(
                "A parent branch must belong to the same country.");

        var visited = new HashSet<int>();
        Branch? current = parent;

        while (current != null)
        {
            if (currentBranchId.HasValue &&
                current.BranchId == currentBranchId.Value)
            {
                throw new InvalidOperationException(
                    "The requested parent assignment would create a circular branch hierarchy.");
            }

            if (!visited.Add(current.BranchId))
                throw new InvalidOperationException(
                    "An existing circular branch hierarchy was detected.");

            if (!current.ParentBranchId.HasValue)
                break;

            current = await _unitOfWork.Branch.GetByIdAsync(
                current.ParentBranchId.Value);
        }
    }

    private static void ValidateIfsc(string ifscCode)
    {
        if (!IfscRegex.IsMatch(ifscCode))
            throw new ArgumentException(
                "IFSC code must contain 11 characters: four letters, zero, and six alphanumeric characters.",
                nameof(ifscCode));
    }

    private static void ValidateMicr(string? micrCode)
    {
        if (micrCode != null && !MicrRegex.IsMatch(micrCode))
            throw new ArgumentException(
                "MICR code must contain exactly 9 digits.",
                nameof(micrCode));
    }

    private static void ValidateBranchType(string branchType)
    {
        if (!new[] { "Regular", "Main", "Digital", "Service" }
            .Contains(branchType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Branch type must be Regular, Main, Digital or Service.",
                nameof(branchType));
        }
    }

    private static void ValidateDates(
        DateTime openedOn,
        DateTime? closedOn,
        bool isActive)
    {
        if (openedOn == default)
            throw new ArgumentException(
                "Branch opening date is required.",
                nameof(openedOn));

        if (closedOn.HasValue &&
            closedOn.Value.Date < openedOn.Date)
        {
            throw new ArgumentException(
                "Branch closing date cannot be earlier than its opening date.",
                nameof(closedOn));
        }

        if (isActive && closedOn.HasValue)
            throw new InvalidOperationException(
                "A closed branch cannot be marked as active.");
    }

    private static string NormalizeCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "A code value is required.",
                nameof(value));

        return value.Trim().ToUpperInvariant();
    }

    private static string? NormalizeOptionalCode(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim().ToUpperInvariant();
    }

    private static string RequiredText(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "A value is required.",
                parameterName);

        return value.Trim();
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(
                parameterName,
                "ID must be greater than zero.");
    }
}