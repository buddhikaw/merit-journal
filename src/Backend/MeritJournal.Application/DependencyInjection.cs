using System.Reflection;
using MeritJournal.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MeritJournal.Application;

/// <summary>
/// Provides extension methods for registering application services with the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds application services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The same service collection so that calls can be chained.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR services
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        // Register AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
