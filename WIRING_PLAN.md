# Accounting System Backend Wiring Plan

## Solution Scaffolding Commands

```bash
# Create solution
dotnet new sln -n Accounting

# Create projects
dotnet new webapi -n Accounting.API -o src/Accounting.API
dotnet new classlib -n Accounting.Application -o src/Accounting.Application
dotnet new classlib -n Accounting.Domain -o src/Accounting.Domain
dotnet new classlib -n Accounting.Infrastructure -o src/Accounting.Infrastructure

# Add projects to solution
dotnet sln add src/Accounting.API/Accounting.API.csproj
dotnet sln add src/Accounting.Application/Accounting.Application.csproj
dotnet sln add src/Accounting.Domain/Accounting.Domain.csproj
dotnet sln add src/Accounting.Infrastructure/Accounting.Infrastructure.csproj

# Add project references
dotnet add src/Accounting.API/Accounting.API.csproj reference src/Accounting.Application/Accounting.Application.csproj
dotnet add src/Accounting.Application/Accounting.Application.csproj reference src/Accounting.Domain/Accounting.Domain.csproj
dotnet add src/Accounting.Infrastructure/Accounting.Infrastructure.csproj reference src/Accounting.Domain/Accounting.Domain.csproj
dotnet add src/Accounting.API/Accounting.API.csproj reference src/Accounting.Infrastructure/Accounting.Infrastructure.csproj
```

## Project Structure

```
Accounting/
├── src/
│   ├── Accounting.API/          # Web API layer (Minimal APIs)
│   ├── Accounting.Application/  # Application layer (CQRS + FluentValidation)
│   ├── Accounting.Domain/       # Domain layer (Entities, Value Objects)
│   └── Accounting.Infrastructure/ # Infrastructure layer (EF Core, External services)
├── travel-accounting-ui/        # Existing Vue.js UI
├── docker-compose.yml
└── README.md
```

## Technology Stack

- **.NET 8** - Framework
- **C#** - Programming language
- **SQL Server 2022** - Database
- **EF Core 8** - ORM
- **CQRS + FluentValidation** - Application patterns
- **Minimal APIs** - API endpoints
- **Docker** - Containerization

## Next Steps

1. ✅ Solution scaffolding
2. 🔄 Add project references and NuGet packages
3. 🔄 Implement authentication (cookie/JWT with company scope)
4. 🔄 Create domain entities and EF configurations
5. 🔄 Implement document numbering system
6. 🔄 Build Ticket workflow APIs
7. 🔄 Develop Finance APIs
8. 🔄 Implement FX smart inventory (FIFO)
9. 🔄 Create report endpoints
10. 🔄 Add audit trail middleware
11. 🔄 Setup seed data and migrations
12. 🔄 Configure docker-compose
13. 🔄 Create sample HTTP calls