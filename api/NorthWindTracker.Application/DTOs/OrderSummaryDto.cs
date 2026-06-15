namespace NorthWindTracker.Application.DTOs;

public record OrderSummaryDto(int OrderId, DateTime? OrderDate, decimal TotalValue, int ProductCount);
