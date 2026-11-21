using FinVault.Application.Interface.IRepositories;
using FinVault.Persistence.Data;
using FinVault.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinVault.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<FinVaultDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Future registrations: Repositories, Services, Authentication, AutoMapper, Validation

        // Repositories
        services.AddScoped<ICountryRepository,CountryRepository>();


        return services;
    }
}