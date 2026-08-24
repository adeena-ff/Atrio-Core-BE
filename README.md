# Atrio Core Backend

ASP.NET Core Web API for the Atrio Student Attendance Management System (Zynthra Technologies).

## Stack

.NET 10 Web API, Entity Framework Core, PostgreSQL (Npgsql).

## Setup

1. Update `Atrio.API/appsettings.Development.json` with your PostgreSQL password.
2. Restore tools and apply migrations:

```bash
dotnet tool restore
cd Atrio.API
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Swagger UI: `http://localhost:5289/swagger`
