using AutoMapper;
using FinVault.Application.DTOs.AccountHolderDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Manages the relationship between bank accounts and their customer holders.</summary>
public sealed class AccountHolderService : IAccountHolderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AccountHolderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Lists all holders attached to an account.
    public async Task<IReadOnlyList<AccountHolderDto>> GetByAccountIdAsync(int accountId)
    {
        var holders = await _unitOfWork.AccountHolder.GetAccountHoldersByAccountIdAsync(accountId);
        return _mapper.Map<IReadOnlyList<AccountHolderDto>>(holders);
    }

    // 2. Adds a holder after checking the account, customer and duplicate relationship.
    public async Task<AccountHolderDto> AddHolderAsync(CreateAccountHolderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.Account.GetByIdAsync(dto.AccountId) is null)
            throw new InvalidOperationException("The selected account does not exist.");
        if (await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId) is null)
            throw new InvalidOperationException("The selected customer does not exist.");
        if (await _unitOfWork.AccountHolder.GetByAccountAndCustomerAsync(dto.AccountId, dto.CustomerId) is not null)
            throw new InvalidOperationException("This customer is already linked to the account.");

        var holder = _mapper.Map<AccountHolder>(dto);
        holder.EffectiveFrom = dto.EffectiveFrom == default ? DateTime.UtcNow : dto.EffectiveFrom;
        holder.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.AccountHolder.AddAsync(holder);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AccountHolderDto>(holder);
    }

    // 3. Updates the holder type, permissions and effective period.
    public async Task<AccountHolderDto?> UpdateHolderAsync(int accountHolderId, UpdateAccountHolderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.AccountHolderId != accountHolderId)
            throw new ArgumentException("The route account-holder ID and DTO ID do not match.", nameof(dto));
        var holder = await _unitOfWork.AccountHolder.GetByIdAsync(accountHolderId);
        if (holder is null) return null;
        if (await _unitOfWork.Account.GetByIdAsync(dto.AccountId) is null ||
            await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId) is null)
            throw new InvalidOperationException("The selected account or customer does not exist.");
        var duplicate = await _unitOfWork.AccountHolder.GetByAccountAndCustomerAsync(dto.AccountId, dto.CustomerId);
        if (duplicate is not null && duplicate.AccountHolderId != accountHolderId)
            throw new InvalidOperationException("This customer is already linked to the account.");

        _mapper.Map(dto, holder);
        holder.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AccountHolderDto>(holder);
    }

    // 4. Ends the holder relationship instead of erasing its historical record.
    public async Task<bool> RemoveHolderAsync(int accountHolderId)
    {
        var holder = await _unitOfWork.AccountHolder.GetByIdAsync(accountHolderId);
        if (holder is null) return false;
        holder.IsActive = false;
        holder.CanOperate = false;
        holder.EffectiveTo = DateTime.UtcNow;
        holder.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.AccountHolder.Update(holder);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
