using System;
using System.ComponentModel.DataAnnotations;

namespace FinVault.Application.DTOs.UserDtos;

/// <summary>Input for deleting a User.</summary>
public class DeleteUserDto
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}
