using NorthWindTracker.Application.DTOs;
using NorthWindTracker.Application.Interfaces;
using NorthWindTracker.Domain;

namespace NorthWindTracker.Application.Services;

public class CustomerService(ICustomerRepository repository)
{
    public async Task<IEnumerable<CustomerSummaryDto>> GetAllAsync(string? nameFilter)
    {
        var customers = await repository.GetAllAsync(nameFilter);
        return customers.Select(ToSummary);
    }

    public async Task<CustomerDetailDto?> GetByIdAsync(string customerId)
    {
        var customer = await repository.GetByIdAsync(customerId);
        return customer is null ? null : ToDetail(customer);
    }

    private static CustomerSummaryDto ToSummary(Customer customer) =>
        new(customer.CustomerId, customer.CompanyName, customer.Orders.Count());

    private static CustomerDetailDto ToDetail(Customer customer) =>
        new(
            customer.CustomerId,
            customer.CompanyName,
            customer.ContactName,
            customer.City,
            customer.Country,
            customer.Orders.Select(ToOrderSummary));

    private static OrderSummaryDto ToOrderSummary(Order order) =>
        new(
            order.OrderId,
            order.OrderDate,
            order.OrderDetails.Sum(od => (decimal)(od.UnitPrice * od.Quantity * (decimal)(1 - od.Discount))),
            order.OrderDetails.Select(od => od.ProductId).Distinct().Count());
}
