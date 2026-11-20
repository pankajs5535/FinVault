namespace FinVault.Application.DTOs.CountryDtos;

public class CountryUsageSummaryDto
{
    // Identifies the country.
    public int CountryId { get; set; }

    // Displays the country name.
    public string CountryName { get; set; } = string.Empty;

    // Total customers associated with this country.
    public int TotalCustomers { get; set; }

    // Total branches associated with this country.
    public int TotalBranches { get; set; }

    // Total currencies associated with this country.
    public int TotalCurrencies { get; set; }
}
