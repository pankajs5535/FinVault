using AutoMapper;
using FinVault.Application.DTOs.FraudAlertDtos;
using FinVault.Application.DTOs.RiskAssessmentDtos;
using FinVault.Application.Interfaces.Services;
using FinVault.Application.Interface.IUnitOfWork;
using FinVault.Domain.Entities;

namespace FinVault.Application.Services;

public sealed class FraudRiskService : IFraudRiskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FraudRiskService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // 1. Retrieve alerts, optionally limited to one customer.
    public async Task<IReadOnlyList<FraudAlertDto>> GetAlertsAsync(int? customerId)
    {
        var alerts = customerId.HasValue
            ? await _unitOfWork.FraudAlert.GetByCustomerIdAsync(customerId.Value)
            : await _unitOfWork.FraudAlert.GetActiveFraudAlertsAsync();
        return _mapper.Map<IReadOnlyList<FraudAlertDto>>(alerts);
    }

    // 2. Create an alert after checking its unique alert number.
    public async Task<FraudAlertDto> CreateAlertAsync(CreateFraudAlertDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (await _unitOfWork.FraudAlert.ExistsByAlertNumberAsync(dto.AlertNumber.Trim()))
            throw new InvalidOperationException("An alert with this number already exists.");
        var alert = _mapper.Map<FraudAlert>(dto);
        alert.AlertNumber = dto.AlertNumber.Trim();
        alert.Status = "Open";
        alert.RaisedAt = dto.RaisedAt == default ? DateTime.UtcNow : dto.RaisedAt;
        alert.CreatedAt = DateTime.UtcNow;
        await _unitOfWork.FraudAlert.AddAsync(alert);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<FraudAlertDto>(alert);
    }

    // 3. Create a basic rule-based assessment. This is not a trained fraud model or a production decision engine.
    public async Task<RiskAssessmentDto> AssessRiskAsync(int customerId, int? accountId, decimal amount)
    {
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        var score = amount >= 1000000m ? 90m : amount >= 100000m ? 65m : amount >= 25000m ? 35m : 10m;
        var level = score >= 80 ? "Critical" : score >= 60 ? "High" : score >= 30 ? "Medium" : "Low";
        var decision = score >= 80 ? "Review" : "Approve";
        var assessment = new RiskAssessment
        {
            AssessmentType = "Customer",
            RiskScore = score,
            RiskLevel = level,
            Decision = decision,
            ModelVersion = "FinVault-Rules-1.0",
            FactorsJson = System.Text.Json.JsonSerializer.Serialize(new { amount, thresholdBased = true }),
            AssessedAt = DateTime.UtcNow,
            CustomerId = customerId,
            AccountId = accountId,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.RiskAssessment.AddAsync(assessment);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<RiskAssessmentDto>(assessment);
    }

    // 4. Close an alert with a resolution note.
    public async Task<bool> ResolveAlertAsync(int fraudAlertId, string resolution)
    {
        if (string.IsNullOrWhiteSpace(resolution)) throw new ArgumentException("A resolution note is required.", nameof(resolution));
        var alert = await _unitOfWork.FraudAlert.GetFraudAlertDetailsAsync(fraudAlertId);
        if (alert == null) return false;
        if (string.Equals(alert.Status, "Resolved", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("This alert is already resolved.");
        alert.Status = "Resolved";
        alert.ReviewNotes = resolution.Trim();
        alert.ReviewedAt = DateTime.UtcNow;
        alert.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.FraudAlert.Update(alert);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
