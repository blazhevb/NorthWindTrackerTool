using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthWindTracker.Application.Interfaces;
using NorthWindTracker.Domain;

namespace NorthWindTracker.Infrastructure.Data;

public class CustomerRepository(NorthwindDbContext context, ILogger<CustomerRepository> logger) : ICustomerRepository
{
    public async Task<IEnumerable<Customer>> GetAllAsync(string? nameFilter)
    {
        logger.LogInformation("Fetching customers with filter: {Filter}", nameFilter ?? "none");

        var query = context.Customers
            .AsNoTracking()
            .Include(c => c.Orders);

        var result = string.IsNullOrWhiteSpace(nameFilter)
            ? await query.ToListAsync()
            : await query.Where(c => c.CompanyName.Contains(nameFilter)).ToListAsync();

        logger.LogInformation("Fetched {Count} customers", result.Count);
        return result;
    }

    public async Task<Customer?> GetByIdAsync(string customerId)
    {
        logger.LogInformation("Fetching customer {CustomerId}", customerId);

        var customer = await context.Customers
            .AsNoTracking()
            .Include(c => c.Orders)
                .ThenInclude(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);

        if (customer is null)
            logger.LogWarning("Customer {CustomerId} not found", customerId);

        return customer;
    }
}
