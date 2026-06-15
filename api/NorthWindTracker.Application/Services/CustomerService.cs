using NorthWindTracker.Application.DTOs;
using NorthWindTracker.Application.Interfaces;

namespace NorthWindTracker.Application.Services;

public class CustomerService(ICustomerRepository repository)
{
    public Task<IEnumerable<CustomerSummaryDto>> GetAllAsync(string? nameFilter)
        => repository.GetAllAsync(nameFilter);

    public Task<CustomerDetailDto?> GetByIdAsync(string customerId)
        => repository.GetByIdAsync(customerId);
}
