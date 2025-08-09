---
applyTo: "**"
---

## Tech Stack
- Backend: C# .NET 9, Entity Framework Core, SQL Server
- Frontend: TypeScript Angular (latest LTS)
- Tools: Docker, Azure services

## Architecture & Patterns
- Clean Architecture: separate API/Application/Domain/Infrastructure layers
- Domain-Driven Design: Aggregates, Value Objects, Domain Services
- CQRS with MediatR for command/query separation
- Repository and Specification patterns for data access
- Dependency injection with proper service lifetimes

## Backend (.NET/C#)
- Use async/await with ConfigureAwait(false) in libraries
- Identity Framework with JWT for auth
- FluentValidation for input validation
- AutoMapper/Mapster for object mapping
- Result pattern for error handling over exceptions
- Global exception middleware with structured logging
- IMemoryCache local, Redis distributed caching
- Background services for long-running tasks

## Frontend (Angular/TypeScript)
- Feature modules with lazy loading
- Reactive patterns with RxJS (switchMap, map, filter)
- OnPush change detection for performance
- Angular Guards for route protection
- Reactive forms with custom validators
- Global error interceptors
- Track by functions in *ngFor

## Data Access & Performance
- EF Core with Include, Select projections, AsNoTracking()
- Database indexing for frequently queried columns
- Pagination with Skip/Take for large datasets
- Connection pooling and proper DbContext configuration
- Compiled queries for repeated patterns

## Security
- Validate/sanitize all input (FluentValidation, Angular validators)
- Parameterized queries, never string concatenation
- HTTPS everywhere, proper CORS policies
- JWT with refresh tokens, rate limiting
- Environment variables for secrets, Azure Key Vault
- Security headers (CSP, HSTS)

## Testing
- Unit tests: xUnit for C#, Jasmine/Karma for Angular
- Moq/NSubstitute for mocking
- Integration tests with WebApplicationFactory
- TestContainers for database testing
- High code coverage standards

## Code Quality
- SOLID principles, meaningful naming conventions
- PascalCase C#, camelCase TypeScript
- Nullable reference types in C#
- Structured logging with Serilog
- IOptions<T> for configuration
- Extension methods for common operations
- Proper disposal patterns (using statements)

## Performance & DevOps
- Response compression (Gzip, Brotli)
- Docker multi-stage builds
- Environment-specific appsettings
- Health checks for monitoring
- Bundle optimization and tree shaking
- CDN for static assets

## Documentation
- XML docs for public APIs
- Swagger/OpenAPI with examples
- DTOs with proper documentation
- Clear error messages and user feedback
