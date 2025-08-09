# Architecture Overview

ExpenseAI is built using Clean Architecture principles, Domain-Driven Design (DDD), and CQRS for scalable, maintainable, and testable code.

## Clean Architecture

- **Separation of Concerns**: Divides the system into API, Application, Domain, and Infrastructure layers.
- **Dependency Rule**: Inner layers (Domain, Application) have no dependencies on outer layers (API, Infrastructure).

### Layered Structure

```
src/
├── backend/
│   ├── ExpenseAI.Api/              # Web API & Controllers
│   ├── ExpenseAI.Application/      # Business Logic & CQRS
│   ├── ExpenseAI.Domain/           # Domain Models & Business Rules
│   └── ExpenseAI.Infrastructure/   # External Concerns (EF Core, AI, Storage)
├── frontend/
│   └── src/app/                    # Angular app (feature modules, core, shared)
└── shared/
    └── ExpenseAI.Contracts/        # Shared DTOs & contracts
```

## Domain-Driven Design (DDD)

- **Aggregates**: Encapsulate business logic and invariants (e.g., Expense, Invoice).
- **Value Objects**: Represent concepts with equality by value (e.g., Money, Currency).
- **Domain Services**: Encapsulate domain logic not naturally belonging to entities.

## CQRS & MediatR

- **Commands**: Write operations (create, update, delete).
- **Queries**: Read operations (fetch, list, search).
- **MediatR**: Decouples request handling from controllers.

## Repository & Specification Patterns

- **Repository**: Abstracts data access for aggregates.
- **Specification**: Encapsulates query logic for reuse and testability.

## Frontend Architecture

- **Angular Feature Modules**: Lazy-loaded, domain-focused modules (expenses, invoices, analytics, ai-chat).
- **Core/Shared**: Singleton services and reusable components.
- **Reactive Patterns**: RxJS for state and data flow.

## Security & Infrastructure

- **Authentication**: ASP.NET Core Identity + JWT.
- **Validation**: FluentValidation (backend), Angular Validators (frontend).
- **Caching**: IMemoryCache (local), Redis (distributed).
- **Logging**: Serilog for structured logs.
- **Cloud**: Azure SQL, Blob Storage, Cognitive Services, CDN.

## DevOps

- **Docker**: Multi-stage builds for API and frontend.
- **CI/CD**: GitHub Actions, Azure DevOps.
- **Monitoring**: Azure Application Insights, health checks.

---
