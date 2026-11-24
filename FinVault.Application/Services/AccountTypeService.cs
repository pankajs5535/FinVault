using AutoMapper;
using FinVault.Application.DTOs.AccountTypeDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Maintains account product definitions such as Savings, Current and Fixed Deposit.</summary>
public sealed class AccountTypeService : IAccountTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves one account product by ID.
    public async Task<AccountTypeDto?> GetByIdAsync(int accountTypeId)
    {
        var entity = await _unitOfWork.AccountType.GetAccountTypeDetailsAsync(accountTypeId);
        return entity is null ? null : _mapper.Map<AccountTypeDto>(entity);
    }

    // 2. Returns all account product definitions.
    public async Task<IReadOnlyList<AccountTypeDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.AccountType.GetAllAsync();
        return _mapper.Map<IReadOnlyList<AccountTypeDto>>(entities);
    }

    // 3. Creates an account product after checking its unique code and rule consistency.
    public async Task<AccountTypeDto> CreateAsync(CreateAccountTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var code = dto.Code.Trim().ToUpperInvariant();
        if (await _unitOfWork.AccountType.GetByCodeAsync(code) is not null)
            throw new InvalidOperationException("An account type with this code already exists.");
        if (dto.MinTenureMonths.HasValue && dto.MaxTenureMonths.HasValue && dto.MinTenureMonths > dto.MaxTenureMonths)
            throw new ArgumentException("Minimum tenure cannot exceed maximum tenure.", nameof(dto));
        if (dto.IsInterestBearing && string.IsNullOrWhiteSpace(dto.InterestPayoutFrequency))
            throw new ArgumentException("Interest payout frequency is required for an interest-bearing account type.", nameof(dto));
        var entity = _mapper.Map<AccountType>(dto);
        entity.Code = code;
        entity.Name = dto.Name.Trim();
        entity.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.AccountType.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AccountTypeDto>(entity);
    }

    // 4. Updates the product rules while preventing a duplicate code.
    public async Task<AccountTypeDto?> UpdateAsync(int accountTypeId, UpdateAccountTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.AccountTypeId != accountTypeId) throw new ArgumentException("Route ID and DTO ID do not match.", nameof(dto));
        var entity = await _unitOfWork.AccountType.GetByIdAsync(accountTypeId);
        if (entity is null) return null;
        var code = dto.Code.Trim().ToUpperInvariant();
        var codeMatch = await _unitOfWork.AccountType.GetByCodeAsync(code);
        if (codeMatch is not null && codeMatch.AccountTypeId != accountTypeId)
            throw new InvalidOperationException("Another account type already uses this code.");
        if (dto.MinTenureMonths.HasValue && dto.MaxTenureMonths.HasValue && dto.MinTenureMonths > dto.MaxTenureMonths)
            throw new ArgumentException("Minimum tenure cannot exceed maximum tenure.", nameof(dto));
        if (dto.IsInterestBearing && string.IsNullOrWhiteSpace(dto.InterestPayoutFrequency))
            throw new ArgumentException("Interest payout frequency is required for an interest-bearing account type.", nameof(dto));
        _mapper.Map(dto, entity);
        entity.Code = code;
        entity.Name = dto.Name.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.AccountType.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AccountTypeDto>(entity);
    }

    // 5. Blocks deletion when accounts, interest rates or fees depend on this product.
    public async Task<bool> DeleteAsync(int accountTypeId)
    {
        var entity = await _unitOfWork.AccountType.GetByIdAsync(accountTypeId);
        if (entity is null || await _unitOfWork.AccountType.IsAccountTypeInUseAsync(accountTypeId)) return false;
        _unitOfWork.AccountType.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
