namespace NorthWindTracker.Application.DTOs;

public record CustomerDetailDto(
    string CustomerId,
    string CompanyName,
    string? ContactName,
    string? City,
    string? Country,
    IEnumerable<OrderSummaryDto> Orders);
