# Atrio Core Backend

Clean Architecture ASP.NET Core API for Atrio (Zynthra Technologies).

## Projects

| Project | Layer |
| --- | --- |
| `Atrio.Domain` | Entities, enums, domain calculation |
| `Atrio.Application` | DTOs, service contracts, services, validation |
| `Atrio.Infrastructure` | EF Core `ApplicationDbContext`, Fluent API, PostgreSQL |
| `Atrio.API` | Controllers, CORS, Swagger, composition root |

## Setup

1. Update `Atrio.API/appsettings.Development.json` with your PostgreSQL password.
2. Restore tools and apply migrations:

```bash
dotnet tool restore
dotnet ef migrations add InitialCreate --project Atrio.Infrastructure --startup-project Atrio.API
dotnet ef database update --project Atrio.Infrastructure --startup-project Atrio.API
dotnet run --project Atrio.API
```

Swagger UI: `http://localhost:5289/swagger`
