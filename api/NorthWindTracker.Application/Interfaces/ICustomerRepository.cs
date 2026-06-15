using NorthWindTracker.Application.DTOs;

namespace NorthWindTracker.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<CustomerSummaryDto>> GetAllAsync(string? nameFilter);
    Task<CustomerDetailDto?> GetByIdAsync(string customerId);
}
