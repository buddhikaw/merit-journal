# Merit Journal Backend Development Guide

## Project Overview

Merit Journal is a full-stack application that allows users to record and manage meritorious acts or significant things done on a particular day. The backend is built with C# and ASP.NET Core, following Clean Architecture principles.

## Architecture

The backend follows **Clean Architecture** with distinct layers:

1. **Domain Layer** (`MeritJournal.Domain`): Contains entities, enums, value objects, and domain exceptions.
2. **Application Layer** (`MeritJournal.Application`): Contains business logic, interfaces, DTOs, and CQRS implementation with MediatR.
3. **Infrastructure Layer** (`MeritJournal.Infrastructure`): Contains implementations of interfaces defined in the application layer, including database access via EF Core.
4. **Presentation Layer** (`MeritJournal.API`): Contains API endpoints, controllers, and configuration for the web services.

## Key Design Patterns and Principles

### Repository Pattern

The backend uses the Repository pattern for data access abstraction. This decouples the application from specific data access technologies.

- **IRepository<T>**: Generic interface for CRUD operations on entities.
- **Repository<T>**: Concrete implementation of `IRepository<T>` using EF Core.

### Unit of Work Pattern

The Unit of Work pattern manages transactions and ensures atomic operations:

- **IUnitOfWork**: Interface that defines the contract for unit of work.
- **UnitOfWork**: Implementation that coordinates the work of multiple repositories.

### CQRS Pattern

The Command Query Responsibility Segregation pattern is implemented using MediatR:

- **Commands**: For operations that change state (Create, Update, Delete).
- **Queries**: For operations that retrieve data without changing state.

## Database

- **PostgreSQL**: The database engine used for persistent storage.
- **Entity Framework Core**: ORM used for database interactions.
- **Migrations**: Used for database schema versioning and updates.

## Project Structure

```
src/
  Backend/
    MeritJournal.Domain/
      Entities/               # Domain entities
    MeritJournal.Application/
      DTOs/                   # Data transfer objects
      Features/               # CQRS commands and queries
      Interfaces/             # Interfaces for infrastructure implementations
    MeritJournal.Infrastructure/
      Persistence/            # Database context and configurations
        Repositories/         # Repository implementations
      DependencyInjection.cs  # Infrastructure service registrations
    MeritJournal.API/
      Endpoints/              # API endpoint configurations
      Configuration/          # API-specific configurations
    tests/
      MeritJournal.UnitTests/       # Unit tests for application logic
      MeritJournal.IntegrationTests/ # Integration tests for API and database
```

## Key Files and Their Roles

### Domain Layer
- `JournalEntry.cs`: Core entity representing a journal entry.
- `JournalImage.cs`: Entity representing an image associated with a journal entry.
- `Tag.cs`: Entity for categorizing journal entries.
- `JournalEntryTag.cs`: Join entity connecting journal entries and tags.

### Application Layer
- `IRepository.cs`: Interface for generic repository operations.
- `IUnitOfWork.cs`: Interface for unit of work operations.
- Various DTOs in `DTOs/` folder.
- CQRS handlers in `Features/` folder.

### Infrastructure Layer
- `ApplicationDbContext.cs`: EF Core database context.
- `Repository.cs`: Generic repository implementation.
- `UnitOfWork.cs`: Implementation of the unit of work interface.
- `DependencyInjection.cs`: Service registrations.

### API Layer
- `Program.cs`: Application startup and configuration.
- `JournalEntryEndpoints.cs`: API endpoints for journal entries.

## Authentication and Authorization

The backend uses JWT Bearer token authentication with OIDC providers:
- Users are identified by their OIDC provider's sub claim.
- JWTs are validated using Microsoft.AspNetCore.Authentication.JwtBearer.
- User IDs from the JWT are used to filter journal entries for the current user.

## Recent Changes and Important Notes

1. **Repository Refactoring**: The repository pattern has been fully implemented with `Repository<T>` class:
   - Removed `EfRepository<T>` which was no longer needed
   - `Repository<T>` now fully implements the `IRepository<T>` interface
   - Both sync and async operations are supported

2. **Unit of Work Pattern**: All data operations are now done through the Unit of Work:
   - `UnitOfWork` class provides access to individual entity repositories
   - Ensures consistent transaction management

3. **Dependency Injection**:
   - Only `IUnitOfWork` is registered in the DI container
   - Removed `IApplicationDbContext` as it's no longer needed

4. **Nullable Reference Types**:
   - The project uses C# nullable reference types
   - Be careful with collections like `Images` and `Tags` in DTOs, which can be null

5. **Testing**:
   - Unit tests use mock repositories and unit of work
   - Ensure proper null-checking in test assertions

## Common Tasks

### Adding a New Entity

1. Create the entity class in `MeritJournal.Domain/Entities/`
2. Update `ApplicationDbContext.cs` to include a `DbSet<T>` for the new entity
3. Add a repository property in `IUnitOfWork` and implement it in `UnitOfWork`
4. Create migrations: `dotnet ef migrations add AddNewEntity`
5. Apply migrations: `dotnet ef database update`

### Creating a New API Endpoint

1. Create DTOs in `MeritJournal.Application/DTOs/` if needed
2. Create Command/Query classes in `MeritJournal.Application/Features/`
3. Implement Command/Query handlers
4. Add endpoints in the appropriate endpoint configuration file or create a new one

### Running Database Migrations

```
dotnet ef migrations add <MigrationName> -p ..\MeritJournal.Infrastructure -s ..\MeritJournal.API
dotnet ef database update -p ..\MeritJournal.Infrastructure -s ..\MeritJournal.API
```

### Running Tests

```
dotnet test src/Backend/tests/MeritJournal.UnitTests
dotnet test src/Backend/tests/MeritJournal.IntegrationTests
```

## PowerShell Command Guidelines

When running PowerShell commands for this project, use semicolons (`;`) for command chaining instead of ampersands (`&&`). This ensures compatibility with PowerShell's syntax.

### Examples:

```powershell
# Correct (using semicolons)
cd src/Backend; dotnet build; dotnet run

# Avoid (using &&)
cd src/Backend && dotnet build && dotnet run
```

### Common PowerShell Commands for Development

```powershell
# Building the solution
cd src/Backend; dotnet build

# Running the API
cd src/Backend/MeritJournal.API; dotnet run

# Creating and applying migrations
cd src/Backend; dotnet ef migrations add <MigrationName> -p MeritJournal.Infrastructure -s MeritJournal.API; dotnet ef database update -p MeritJournal.Infrastructure -s MeritJournal.API

# Running all tests
cd src/Backend; dotnet test
```

## Troubleshooting

### Common Build Errors

1. **Missing interface implementations**: Ensure concrete classes fully implement their interfaces.
2. **Null reference exceptions**: Check for proper null handling, especially with nullable reference types.
3. **Entity Framework errors**: Ensure migrations are up-to-date and the database connection is valid.

### General Tips

- Use `IUnitOfWork` for all data access operations
- Prefer async methods when available
- Always validate user input at the API layer
- Use the CQRS pattern for new features
- Follow Clean Architecture principles: domain entities should not depend on infrastructure
