# NorthWind Tracker Tool — Architecture & Development Guide

## Project Overview

**Assessment:** Fourth .NET Software Engineer Technical Assessment  
**Data source:** Microsoft Northwind sample database  
**Goal:** Internal staff tool to look up customers and review their order history.

---

## Repository Structure

```
NorthWindTrackerTool/          ← repo root (one .git, one .gitignore, one .claude/)
├── database/
│   └── instnwnd.sql           # Microsoft Northwind setup script
├── api/                       ← .NET solution root (NorthWindTracker.sln lives here)
│   ├── logs/                  # Runtime only — logs.txt written here, gitignored (.gitkeep tracked)
│   ├── NorthWindTracker.Api/
│   ├── NorthWindTracker.Application/
│   ├── NorthWindTracker.Domain/
│   ├── NorthWindTracker.Infrastructure/
│   └── NorthWindTracker.Tests/
└── ui/                        ← Angular project root
    └── northwind-tracker-ui/
```

---

## Clean Architecture Layers

### Domain (`NorthWindTracker.Domain`)
- Pure C# class library — no framework dependencies.
- Contains entity classes: `Customer`, `Order`, `OrderDetail`, `Product`.
- No data annotations, no EF Core attributes.

### Application (`NorthWindTracker.Application`)
- Pure C# class library — depends only on Domain.
- Defines interfaces consumed by the API and implemented by Infrastructure:
  - `ICustomerRepository` — `GetAllAsync(string? nameFilter)`, `GetByIdAsync(string customerId)`
- Contains DTOs:
  - `CustomerSummaryDto` — `CustomerId`, `CompanyName`, `OrderCount`
  - `CustomerDetailDto` — full customer fields + `IEnumerable<OrderSummaryDto>`
  - `OrderSummaryDto` — `OrderId`, `OrderDate`, `TotalValue`, `ProductCount`
- Contains `CustomerService` — orchestrates repository calls, returns DTOs.
- Registers services via `ServiceExtensions.AddApplication()`.

### Infrastructure (`NorthWindTracker.Infrastructure`)
- Depends on Application + Domain.
- `NorthwindDbContext` — EF Core DbContext with full Fluent API mapping (no data annotations).
- `CustomerRepository` — implements `ICustomerRepository` with EF Core projections.
- `DatabaseInitializer` — on startup, checks if Northwind DB exists; if not, creates it and runs `instnwnd.sql`. Drops partial DB and rethrows on failure.
- Registers services via `ServiceExtensions.AddInfrastructure(config)`.

### API (`NorthWindTracker.Api`)
- ASP.NET Core Web API targeting .NET 10.
- Depends on Application + Infrastructure.
- `CustomersController` — thin controller, two endpoints, no business logic.
- `GlobalExceptionHandler` — implements `IExceptionHandler`, returns RFC 7807 `ProblemDetails` on unhandled exceptions, logs via Serilog.
- Logging: Serilog with console + file sinks, file overwritten on each start.
- OpenAPI: built-in .NET 10 `Microsoft.AspNetCore.OpenApi` at `/openapi/v1.json`.
- CORS: allows `http://localhost:4200`.
- Registers services via `ServiceExtensions` in each layer, wired in `Program.cs`.

---

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/customers` | List all customers with order count. Optional `?name=` filter. |
| GET | `/api/customers/{id}` | Customer detail + full order history summary. |

Status codes: `200`, `404` (customer not found), `500` (unhandled exception via `GlobalExceptionHandler`).

---

## Key Technology Decisions

| Concern | Choice | Reason |
|---------|--------|--------|
| Runtime | .NET 10 | Installed SDK version |
| ORM | EF Core 10 | Northwind schema maps cleanly; projections avoid over-fetching |
| Logging | Serilog | Structured logging, easy sink configuration |
| API docs | .NET 10 built-in OpenAPI | Native, no Swashbuckle dependency |
| Testing | NUnit + NSubstitute | NUnit as required; NSubstitute for clean mocking |
| Frontend | Angular (latest stable) | As specified |
| HTTP client | Angular `HttpClient` | Standard Angular HTTP layer |

---

## Testing (`NorthWindTracker.Tests`)

- Framework: **NUnit**
- Mocking: **NSubstitute**
- Tests target `CustomerService` with mocked `ICustomerRepository` — no database required.
- Covers:
  - `GetAllAsync` returns mapped `CustomerSummaryDto` list
  - `GetAllAsync` with name filter passes filter to repository
  - `GetByIdAsync` returns `CustomerDetailDto` when customer exists
  - `GetByIdAsync` returns null when customer not found

---

## Angular UI (`ui/northwind-tracker-ui`)

- Generated with Angular CLI.
- No direct database access — all data fetched via HTTP from the Web API.
- Structure:
  - `CustomerListComponent` — table/list with name search input, calls `GET /api/customers?name=`
  - `CustomerDetailComponent` — routed view, calls `GET /api/customers/{id}`
  - `CustomerService` (Angular) — wraps `HttpClient` calls
  - `environment.ts` — base API URL configuration
- Routing: Angular Router with `/customers` and `/customers/:id` routes.
- Styling: minimal and functional — no over-engineering.
- `provideHttpClient()` configured in `app.config.ts`.

---

## Northwind Database

- Script: `database/instnwnd.sql` — does not create the database itself, only the schema and data.
- `DatabaseInitializer` handles DB creation then runs the script against it on first startup.
- Relevant tables:
  - `Customers` — `CustomerID`, `CompanyName`, `ContactName`, `City`, `Country`
  - `Orders` — `OrderID`, `CustomerID`, `OrderDate`
  - `Order Details` — `OrderID`, `ProductID`, `UnitPrice`, `Quantity`, `Discount`
  - `Products` — `ProductID`, `ProductName`
- **Order total value** = `SUM(UnitPrice * Quantity * (1 - Discount))`
- **Product count** = distinct count of `ProductID` in `Order Details` per order

---

## Logging Setup

- Console + file sinks via Serilog.
- Log file: `api/logs/logs.txt` — overwritten on each app start (deleted in `Program.cs` before Serilog initialises).
- `api/logs/` folder tracked in git via `.gitkeep`; `logs.txt` is gitignored.

---

## Configuration

`appsettings.json` (API project):
```json
{
  "ConnectionStrings": {
    "NorthwindDb": "Server=.;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "../logs/logs.txt", "rollOnFileSizeLimit": false, "retainedFileCountLimit": 1 } }
    ]
  }
}
```

---

## Dependency Injection Wiring

```csharp
// Program.cs
builder.Host.UseSerilog(...);

builder.Services.AddInfrastructure(builder.Configuration); // DbContext, CustomerRepository, DatabaseInitializer
builder.Services.AddApplication();                          // CustomerService
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
```

Each layer exposes a `ServiceExtensions` static class with an `IServiceCollection` extension method.

---

## OOP Principles Demonstrated

- **Single Responsibility:** each class has one job (repository fetches, service orchestrates, controller routes).
- **Interface Segregation:** `ICustomerRepository` declares only what the application needs.
- **Dependency Inversion:** Application defines the interface; Infrastructure implements it.
- **Encapsulation:** domain entities use `init`-only properties — no public setters.

---

## What to Avoid

- No business logic in controllers.
- No direct EF Core usage in the API project.
- No hardcoded connection strings.
- No raw SQL — EF Core projections used throughout.
- No over-engineering — focused assessment, not a microservices platform.
