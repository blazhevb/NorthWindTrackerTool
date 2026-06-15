using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthWindTracker.Application.Interfaces;
using NorthWindTracker.Infrastructure.Data;
using NorthWindTracker.Infrastructure.Initialisation;

namespace NorthWindTracker.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NorthwindDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("NorthwindDb")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}
