using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.FraudRuleDtos;

/// <summary>Input for deleting a FraudRule.</summary>
public class DeleteFraudRuleDto
{
    [Range(1, int.MaxValue)]
    public int FraudRuleId { get; set; }
}
