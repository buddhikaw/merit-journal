using MeritJournal.Application.Interfaces;
using MeritJournal.Domain.Entities;
using MeritJournal.Infrastructure.Persistence;
using MeritJournal.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MeritJournal.Infrastructure;

/// <summary>
/// Provides extension methods for registering infrastructure services with the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds infrastructure services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The configuration from which to retrieve connection strings.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the database context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
          // Register repositories and unit of work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
