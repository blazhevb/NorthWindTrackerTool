# AI Usage and Development Process

## Approach

I used Claude Code (Anthropic) as a development assistant throughout this project.

My process mirrored what I would normally do without AI — starting from an empty solution, adding projects one by one, then implementing logic layer by layer. The difference is that I used Claude to implement each step while I drove the design decisions, challenged the reasoning behind each choice, and made the final calls.

This meant asking not just "implement X" but "why this way and not that way?" — for example, questioning why `ICustomerRepository` belongs in Application and not Domain, whether `CustomerService` needed an interface, why each layer is scoped and not singleton, where mapping should happen, and how EF Core should be configured. In several cases I pushed back on the initial approach and we revised it together (the mapping location being a good example — the first implementation projected to DTOs inside the repository, which I questioned and we refactored to the cleaner service-layer mapping).

The result is a codebase I understand fully and can reason about, not one that was generated and accepted without scrutiny.

---

## Key Prompts

**1. Initial architecture planning**
> "I am building a .NET full-stack assessment project using the Microsoft Northwind sample database. The goal is an internal staff tool to look up customers and review their order history. Design a Clean Architecture solution using ASP.NET Core Web API (.NET 10), EF Core, Serilog, NUnit, NSubstitute, and Angular. The solution should have separate Domain, Application, Infrastructure, and API projects with clear separation of concerns. Include two API endpoints: a customer list with optional name filter and order count, and a customer detail endpoint returning full order history with total value and product count per order."

**2. Database setup**
> "The project uses the official Microsoft Northwind SQL Server script (`instnwnd.sql`). Implement a `DatabaseInitializer` class in the Infrastructure layer that checks on application startup whether the Northwind database exists, creates it if not, then executes the SQL script against it. Split the script on `GO` batch separators. On failure, drop the partial database and rethrow so the application fails fast with a clear log message."

**3. EF Core mapping**
> "Map the Northwind schema to domain entities using EF Core Fluent API only — no data annotations. Map `Customers`, `Orders`, `Order Details` (note the space in the table name), and `Products`. Configure the composite primary key for `Order Details`. All column name mappings should be explicit."

**4. Architecture decision — mapping location**
> "The repository currently projects directly to DTOs inside EF Core queries. Refactor so the repository returns domain entities and the Application service layer performs all mapping to DTOs. The trade-off is over-fetching vs clean separation — document this in the README."

**5. Error handling**
> "Implement a global exception handler using `IExceptionHandler` and `ProblemDetails` (RFC 7807). It should log the full exception via Serilog and return a generic 500 response to the client with no internal details leaked."

**6. Unit tests**
> "Write NUnit unit tests for `CustomerService` using NSubstitute to mock `ICustomerRepository`. Cover: `GetAllAsync` returns mapped summary list, `GetAllAsync` passes name filter to repository, `GetByIdAsync` returns correct order totals including the discount calculation, `GetByIdAsync` returns null when customer does not exist."

**7. Angular frontend**
> "Scaffold an Angular 22 application with two components: `CustomerListComponent` showing a searchable table of customers calling `GET /api/customers?name=`, and `CustomerDetailComponent` showing customer details and order history calling `GET /api/customers/{id}`. Use Angular signals for state, `provideHttpClient()` in `app.config.ts`, and plain CSS with no UI framework. Wire up routing with `/customers` and `/customers/:id` routes."
