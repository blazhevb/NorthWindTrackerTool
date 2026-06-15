using Microsoft.Extensions.DependencyInjection;
using NorthWindTracker.Application.Services;

namespace NorthWindTracker.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CustomerService>();
        return services;
    }
}
