using AutoMapper;
using FinVault.Application.DTOs.BeneficiaryDtos;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Application.Interfaces.Services;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

/// <summary>Manages saved payees and enforces beneficiary ownership and activation rules.</summary>
public sealed class BeneficiaryService : IBeneficiaryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BeneficiaryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieves one saved payee.
    public async Task<BeneficiaryDto?> GetByIdAsync(int beneficiaryId)
    {
        var beneficiary = await _unitOfWork.Beneficiary.GetBeneficiaryDetailsAsync(beneficiaryId);
        return beneficiary is null ? null : _mapper.Map<BeneficiaryDto>(beneficiary);
    }

    // 2. Lists all payees saved by a customer.
    public async Task<IReadOnlyList<BeneficiaryDto>> GetByCustomerIdAsync(int customerId)
    {
        var beneficiaries = await _unitOfWork.Beneficiary.GetBeneficiariesByCustomerIdAsync(customerId);
        return _mapper.Map<IReadOnlyList<BeneficiaryDto>>(beneficiaries);
    }

    // 3. Adds a payee after checking customer ownership and duplicate bank/UPI identifiers.
    public async Task<BeneficiaryDto> AddAsync(CreateBeneficiaryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.Customer.GetByIdAsync(dto.CustomerId) is null)
            throw new InvalidOperationException("The selected customer does not exist.");
        if (dto.InternalAccountId.HasValue && await _unitOfWork.Account.GetByIdAsync(dto.InternalAccountId.Value) is null)
            throw new InvalidOperationException("The selected internal account does not exist.");
        if (!string.IsNullOrWhiteSpace(dto.AccountNumber) && !string.IsNullOrWhiteSpace(dto.IfscCode) &&
            await _unitOfWork.Beneficiary.HasActiveBankBeneficiaryAsync(dto.CustomerId, dto.AccountNumber.Trim(), dto.IfscCode.Trim()))
            throw new InvalidOperationException("This bank account is already saved as an active beneficiary.");
        if (!string.IsNullOrWhiteSpace(dto.UpiVpa) &&
            await _unitOfWork.Beneficiary.HasActiveUpiBeneficiaryAsync(dto.CustomerId, dto.UpiVpa.Trim()))
            throw new InvalidOperationException("This UPI VPA is already saved as an active beneficiary.");

        var beneficiary = _mapper.Map<Beneficiary>(dto);
        beneficiary.BeneficiaryName = dto.BeneficiaryName.Trim();
        beneficiary.AccountNumber = dto.AccountNumber?.Trim();
        beneficiary.IfscCode = dto.IfscCode?.Trim().ToUpperInvariant();
        beneficiary.UpiVpa = dto.UpiVpa?.Trim();
        beneficiary.Status = "Pending";
        beneficiary.ActivatedAt = null;
        beneficiary.CoolingPeriodEndsAt ??= DateTime.UtcNow.AddHours(24);
        beneficiary.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.Beneficiary.AddAsync(beneficiary);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BeneficiaryDto>(beneficiary);
    }

    // 4. Updates a payee only after verifying it belongs to the supplied customer.
    public async Task<BeneficiaryDto?> UpdateAsync(int beneficiaryId, UpdateBeneficiaryDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (dto.BeneficiaryId != beneficiaryId)
            throw new ArgumentException("The route beneficiary ID and DTO ID do not match.", nameof(dto));
        var beneficiary = await _unitOfWork.Beneficiary.GetByIdAndCustomerIdAsync(beneficiaryId, dto.CustomerId);
        if (beneficiary is null) return null;
        _mapper.Map(dto, beneficiary);
        beneficiary.BeneficiaryName = dto.BeneficiaryName.Trim();
        beneficiary.AccountNumber = dto.AccountNumber?.Trim();
        beneficiary.IfscCode = dto.IfscCode?.Trim().ToUpperInvariant();
        beneficiary.UpiVpa = dto.UpiVpa?.Trim();
        beneficiary.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<BeneficiaryDto>(beneficiary);
    }

    // 5. Marks the payee as removed to preserve payment-history relationships.
    public async Task<bool> RemoveAsync(int beneficiaryId)
    {
        var beneficiary = await _unitOfWork.Beneficiary.GetByIdAsync(beneficiaryId);
        if (beneficiary is null) return false;
        beneficiary.Status = "Removed";
        beneficiary.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    // 6. Activates a pending payee only after its cooling period has ended.
    public async Task<bool> ActivateAfterCoolingPeriodAsync(int beneficiaryId)
    {
        var beneficiary = await _unitOfWork.Beneficiary.GetByIdAsync(beneficiaryId);
        if (beneficiary is null || beneficiary.Status != "Pending") return false;
        if (!beneficiary.CoolingPeriodEndsAt.HasValue || beneficiary.CoolingPeriodEndsAt.Value > DateTime.UtcNow)
            return false;
        beneficiary.Status = "Active";
        beneficiary.ActivatedAt = DateTime.UtcNow;
        beneficiary.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Beneficiary.Update(beneficiary);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
