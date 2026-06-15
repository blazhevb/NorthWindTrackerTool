namespace NorthWindTracker.Application.DTOs;

public record CustomerSummaryDto(string CustomerId, string CompanyName, int OrderCount);
