# API Documentation

ExpenseAI exposes a RESTful API built with ASP.NET Core Web API, following best practices for security, validation, and error handling.

## Conventions

- **Base URL**: `/api/v1/`
- **Versioning**: URL-based versioning
- **Content-Type**: `application/json`
- **Authentication**: JWT Bearer tokens (see `/api/v1/auth`)
- **Authorization**: Role-based (RBAC)

## Common Endpoints

- `POST /api/v1/auth/login` — Authenticate user, returns JWT and refresh token
- `POST /api/v1/auth/refresh` — Refresh JWT token
- `GET /api/v1/expenses` — List expenses (supports pagination, filtering)
- `POST /api/v1/expenses` — Create new expense
- `GET /api/v1/expenses/{id}` — Get expense by ID
- `PUT /api/v1/expenses/{id}` — Update expense
- `DELETE /api/v1/expenses/{id}` — Delete expense
- `GET /api/v1/analytics/summary` — Get analytics summary
- `POST /api/v1/ai/query` — Natural language query endpoint

## Authentication

- Use `Authorization: Bearer {token}` header for all secured endpoints.
- Obtain token via `/api/v1/auth/login`.

## Error Handling

- All errors return a structured JSON object:
  ```json
  {
    "success": false,
    "errors": [
      {
        "code": "ValidationError",
        "message": "Amount must be greater than zero."
      }
    ]
  }
  ```
- Validation errors use FluentValidation.
- Global exception middleware logs and returns user-friendly messages.

## Documentation

- **Swagger/OpenAPI**: Available at `/swagger`
- **XML Comments**: All public APIs are documented
- **DTOs**: Well-documented with examples

## Security

- All input is validated and sanitized.
- HTTPS enforced, CORS configured.
- Rate limiting and audit logging enabled.

---

For detailed API contracts, see `shared/ExpenseAI.Contracts/`.
