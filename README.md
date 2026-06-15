# Northwind Tracker

An internal staff tool to look up customers and review their order history, built with ASP.NET Core and Angular.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server or SQL Server Express (LocalDB also works — see note below)
- [Node.js 22 LTS](https://nodejs.org) and npm
- Angular CLI: `npm install -g @angular/cli`

> **SQL Server Express:** the default connection string targets the default instance (`Server=.`). If you have a named instance, update to `Server=.\SQLEXPRESS` in `api/NorthWindTracker.Api/appsettings.json`.
>
> **LocalDB:** If using LocalDB instead of a full SQL Server instance, update the connection string to:
> ```
> "Server=(localdb)\\MSSQLLocalDB;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True;"
> ```

---

## Running the Project

### 1. Start the API

Open `api/NorthWindTracker.sln` in Visual Studio and press **F5**, or from the terminal:

```bash
cd api/NorthWindTracker.Api
dotnet run
```

The API starts at `http://localhost:5229`.

> **First run:** the API will automatically create and populate the Northwind database on startup. No manual database setup is required.

### 2. Start the Angular app

In a second terminal:

```bash
cd ui
npm install
ng serve
```

Open `http://localhost:4200` in your browser.

---

## API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/customers` | List all customers with order count. Optional `?name=` filter. |
| GET | `/api/customers/{id}` | Customer detail and full order history. |

OpenAPI spec available at `http://localhost:5229/openapi/v1.json`.

---

## Running the Tests

```bash
cd api
dotnet test
```

---

## Architecture

The solution follows Clean Architecture with four layers:

- **Domain** — entity classes (`Customer`, `Order`, `OrderDetail`, `Product`). No framework dependencies.
- **Application** — `ICustomerRepository` interface, DTOs, and `CustomerService`. Depends only on Domain.
- **Infrastructure** — EF Core `NorthwindDbContext`, `CustomerRepository`, and `DatabaseInitializer`. Implements Application interfaces.
- **API** — ASP.NET Core controllers, global exception handling, Serilog logging, CORS, and OpenAPI.

The Angular frontend communicates with the backend exclusively over HTTP — no direct database access.

---

## Key Technology Choices

| Concern | Choice | Reason |
|---------|--------|--------|
| Runtime | .NET 10 | Installed SDK version |
| ORM | EF Core 10 | Clean schema mapping via Fluent API; projections avoid over-fetching |
| Logging | Serilog | Structured logging with console and file sinks |
| API docs | .NET 10 built-in OpenAPI | Native support, no Swashbuckle dependency |
| Error handling | `IExceptionHandler` + `ProblemDetails` | RFC 7807 compliant error responses, no internal details leaked |
| Testing | NUnit + NSubstitute | NUnit as specified; NSubstitute for clean interface mocking |
| Frontend | Angular 22 | As specified |

---

## Assumptions and Trade-offs

**Database initialisation on startup**
The `DatabaseInitializer` checks whether the Northwind database exists on first run and creates and populates it automatically using the bundled `database/instnwnd.sql` script. This removes all manual setup steps for the reviewer. In a production system, schema management would be handled by a dedicated migration tool such as EF Core Migrations or DbUp.

**Mapping in the service layer, not the repository**
`CustomerRepository` returns domain entities. `CustomerService` maps them to DTOs. This keeps the repository as a pure data-access concern and makes the mapping logic unit-testable without a database. The trade-off is that EF Core loads full entities rather than projecting directly to DTOs in SQL — for this dataset size this has no practical impact.

**No API versioning**
The assessment scope does not require versioning. If this were a public or long-lived API, URL path versioning (`/api/v1/`) or header-based versioning via `Asp.Versioning.Mvc` would be appropriate.

**Plain CSS, no UI framework**
Styling is intentionally minimal. Angular Material or a component library would be the natural next step for a production tool.

**`CustomerId` on the `Order` entity**
EF Core requires an explicit FK property to correctly map the `Customer` → `Orders` relationship — without it, it generates a duplicate shadow property (`OrderID1`) and fails at runtime. Adding `CustomerId` to `Order` is pragmatic but technically leaks an infrastructure concern into the domain. The stricter alternative would be to configure a shadow property in `OnModelCreating` with an explicit column mapping, keeping the domain entity clean.

---

## What I Would Improve with More Time

- Add Scalar UI (`Scalar.AspNetCore`) for interactive API exploration in the browser
- Add an integration test hitting a real database using EF Core's in-memory provider or a test container
- Add a loading skeleton / better UX states in the Angular app
- Add pagination to the customer list for large datasets
- Add input debounce to the search field to avoid firing a request on every keystroke

---

## AI Tools

This project was built with the assistance of Claude Code (Anthropic).
