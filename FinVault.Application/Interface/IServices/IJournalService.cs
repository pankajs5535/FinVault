using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinVault.Application.DTOs.JournalEntryDtos;

namespace FinVault.Application.Interfaces.Services;

/// <summary>
/// Defines application-level operations for the Journal banking module.
/// Controllers call this contract; implementations enforce business rules and use repositories.
/// </summary>
public interface IJournalService
{
        Task<JournalEntryDto?> GetEntryByIdAsync(int journalEntryId);
        Task<IReadOnlyList<JournalEntryDto>> GetEntriesAsync(DateTime from, DateTime to);
        Task<JournalEntryDto> CreateEntryAsync(CreateJournalEntryDto dto);
        Task<bool> PostEntryAsync(int journalEntryId);
}
