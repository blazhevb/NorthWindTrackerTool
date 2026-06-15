namespace NorthWindTracker.Domain;

public class Customer
{
    public string CustomerId { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public string? ContactName { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }

    public ICollection<Order> Orders { get; init; } = [];
}
