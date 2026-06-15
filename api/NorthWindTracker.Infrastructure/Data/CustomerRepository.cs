using Microsoft.EntityFrameworkCore;
using NorthWindTracker.Application.DTOs;
using NorthWindTracker.Application.Interfaces;

namespace NorthWindTracker.Infrastructure.Data;

public class CustomerRepository(NorthwindDbContext context) : ICustomerRepository
{
    public async Task<IEnumerable<CustomerSummaryDto>> GetAllAsync(string? nameFilter)
    {
        var query = context.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(nameFilter))
            query = query.Where(c => c.CompanyName.Contains(nameFilter));

        return await query
            .Select(c => new CustomerSummaryDto(
                c.CustomerId,
                c.CompanyName,
                c.Orders.Count()))
            .ToListAsync();
    }

    public async Task<CustomerDetailDto?> GetByIdAsync(string customerId)
    {
        return await context.Customers
            .AsNoTracking()
            .Where(c => c.CustomerId == customerId)
            .Select(c => new CustomerDetailDto(
                c.CustomerId,
                c.CompanyName,
                c.ContactName,
                c.City,
                c.Country,
                c.Orders.Select(o => new OrderSummaryDto(
                    o.OrderId,
                    o.OrderDate,
                    o.OrderDetails.Sum(od => (decimal)(od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))),
                    o.OrderDetails.Select(od => od.ProductId).Distinct().Count()
                ))))
            .FirstOrDefaultAsync();
    }
}
