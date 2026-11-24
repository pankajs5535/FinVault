using AutoMapper;
using FinVault.Application.DTOs.TransactionTypeDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Maintains transaction definitions and prevents removal of types used by financial records.</summary>
public sealed class TransactionTypeService : ITransactionTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TransactionTypeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves one transaction type by ID.
    public async Task<TransactionTypeDto?> GetByIdAsync(int transactionTypeId)
    {
        var entity = await _unitOfWork.TransactionType.GetTransactionTypeDetailsAsync(transactionTypeId);
        return entity is null ? null : _mapper.Map<TransactionTypeDto>(entity);
    }

    // 2. Returns all transaction type definitions.
    public async Task<IReadOnlyList<TransactionTypeDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.TransactionType.GetAllAsync();
        return _mapper.Map<IReadOnlyList<TransactionTypeDto>>(entities);
    }

    // 3. Creates a transaction type after validating its unique code and approval rules.
    public async Task<TransactionTypeDto> CreateAsync(CreateTransactionTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var code = dto.Code.Trim().ToUpperInvariant();
        if (await _unitOfWork.TransactionType.GetByCodeAsync(code) is not null)
            throw new InvalidOperationException("A transaction type with this code already exists.");
        if (dto.RequiresApproval && (!dto.ApprovalThreshold.HasValue || dto.ApprovalThreshold <= 0))
            throw new ArgumentException("A positive approval threshold is required when approval is enabled.", nameof(dto));
        var entity = _mapper.Map<TransactionType>(dto);
        entity.Code = code;
        entity.Name = dto.Name.Trim();
        entity.Category = dto.Category.Trim();
        entity.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.TransactionType.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TransactionTypeDto>(entity);
    }

    // 4. Updates transaction rules and checks that the code is not owned by another record.
    public async Task<TransactionTypeDto?> UpdateAsync(int transactionTypeId, UpdateTransactionTypeDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.TransactionTypeId != transactionTypeId) throw new ArgumentException("Route ID and DTO ID do not match.", nameof(dto));
        var entity = await _unitOfWork.TransactionType.GetByIdAsync(transactionTypeId);
        if (entity is null) return null;
        var code = dto.Code.Trim().ToUpperInvariant();
        var codeMatch = await _unitOfWork.TransactionType.GetByCodeAsync(code);
        if (codeMatch is not null && codeMatch.TransactionTypeId != transactionTypeId)
            throw new InvalidOperationException("Another transaction type already uses this code.");
        if (dto.RequiresApproval && (!dto.ApprovalThreshold.HasValue || dto.ApprovalThreshold <= 0))
            throw new ArgumentException("A positive approval threshold is required when approval is enabled.", nameof(dto));
        _mapper.Map(dto, entity);
        entity.Code = code;
        entity.Name = dto.Name.Trim();
        entity.Category = dto.Category.Trim();
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.TransactionType.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TransactionTypeDto>(entity);
    }

    // 5. Prevents deleting a transaction type referenced by existing transactions or fees.
    public async Task<bool> DeleteAsync(int transactionTypeId)
    {
        var entity = await _unitOfWork.TransactionType.GetByIdAsync(transactionTypeId);
        if (entity is null || await _unitOfWork.TransactionType.IsTransactionTypeInUseAsync(transactionTypeId)) return false;
        _unitOfWork.TransactionType.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
