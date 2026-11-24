using AutoMapper;
using FinVault.Application.DTOs.BranchDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Manages bank branch records and protects branches that are still operationally referenced.</summary>
public sealed class BranchService : IBranchService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BranchService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves a branch with its related detail data.
    public async Task<BranchDto?> GetByIdAsync(int branchId)
    {
        var entity = await _unitOfWork.Branch.GetBranchDetailsAsync(branchId);
        return entity is null ? null : _mapper.Map<BranchDto>(entity);
    }

    // 2. Returns all branches for administration screens.
    public async Task<IReadOnlyList<BranchDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Branch.GetAllAsync();
        return _mapper.Map<IReadOnlyList<BranchDto>>(entities);
    }

    // 3. Validates country and unique branch identifiers before creating a branch.
    public async Task<BranchDto> CreateAsync(CreateBranchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.Country.GetByIdAsync(dto.CountryId) is null)
            throw new InvalidOperationException("The selected country does not exist.");
        var code = dto.BranchCode.Trim().ToUpperInvariant();
        var ifsc = dto.IfscCode.Trim().ToUpperInvariant();
        if (await _unitOfWork.Branch.GetByBranchCodeAsync(code) is not null ||
            await _unitOfWork.Branch.GetByIfscCodeAsync(ifsc) is not null)
            throw new InvalidOperationException("The branch code or IFSC code is already registered.");
        if (!string.IsNullOrWhiteSpace(dto.MicrCode) && await _unitOfWork.Branch.GetByMicrCodeAsync(dto.MicrCode.Trim()) is not null)
            throw new InvalidOperationException("The MICR code is already registered.");
        if (dto.ParentBranchId.HasValue && await _unitOfWork.Branch.GetByIdAsync(dto.ParentBranchId.Value) is null)
            throw new InvalidOperationException("The selected parent branch does not exist.");

        var entity = _mapper.Map<Branch>(dto);
        entity.BranchCode = code;
        entity.IfscCode = ifsc;
        entity.BranchName = dto.BranchName.Trim();
        entity.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Branch.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BranchDto>(entity);
    }

    // 4. Updates an existing branch after validating IDs and unique identifiers.
    public async Task<BranchDto?> UpdateAsync(int branchId, UpdateBranchDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.BranchId != branchId) throw new ArgumentException("Route ID and DTO ID do not match.", nameof(dto));
        var entity = await _unitOfWork.Branch.GetByIdAsync(branchId);
        if (entity is null) return null;
        if (await _unitOfWork.Country.GetByIdAsync(dto.CountryId) is null)
            throw new InvalidOperationException("The selected country does not exist.");
        var code = dto.BranchCode.Trim().ToUpperInvariant();
        var ifsc = dto.IfscCode.Trim().ToUpperInvariant();
        var codeMatch = await _unitOfWork.Branch.GetByBranchCodeAsync(code);
        var ifscMatch = await _unitOfWork.Branch.GetByIfscCodeAsync(ifsc);
        var micrMatch = string.IsNullOrWhiteSpace(dto.MicrCode) ? null : await _unitOfWork.Branch.GetByMicrCodeAsync(dto.MicrCode.Trim());
        if ((codeMatch is not null && codeMatch.BranchId != branchId) || (ifscMatch is not null && ifscMatch.BranchId != branchId) || (micrMatch is not null && micrMatch.BranchId != branchId))
            throw new InvalidOperationException("Another branch already uses one of the supplied codes.");
        if (dto.ParentBranchId == branchId) throw new InvalidOperationException("A branch cannot be its own parent.");
        if (dto.ParentBranchId.HasValue && await _unitOfWork.Branch.GetByIdAsync(dto.ParentBranchId.Value) is null)
            throw new InvalidOperationException("The selected parent branch does not exist.");

        _mapper.Map(dto, entity);
        entity.BranchCode = code;
        entity.IfscCode = ifsc;
        entity.BranchName = dto.BranchName.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Branch.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BranchDto>(entity);
    }

    // 5. Prevents deleting a branch used by customers, employees, accounts, loans or child branches.
    public async Task<bool> DeleteAsync(int branchId)
    {
        var entity = await _unitOfWork.Branch.GetByIdAsync(branchId);
        if (entity is null || await _unitOfWork.Branch.IsBranchInUseAsync(branchId)) return false;
        _unitOfWork.Branch.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
