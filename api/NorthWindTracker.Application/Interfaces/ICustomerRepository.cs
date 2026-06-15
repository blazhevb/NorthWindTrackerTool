using NorthWindTracker.Domain;

namespace NorthWindTracker.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAllAsync(string? nameFilter);
    Task<Customer?> GetByIdAsync(string customerId);
}
