# Merit Journal API

The backend API for the Merit Journal application built with ASP.NET Core 8 using Clean Architecture principles.

## Technologies

- ASP.NET Core 8 Minimal API
- Entity Framework Core 8
- PostgreSQL
- CQRS pattern with MediatR
- Clean Architecture
- JWT/OIDC Authentication

## Project Structure

The solution follows Clean Architecture principles:

- **MeritJournal.Domain**: Contains all entities, enums, exceptions, interfaces, types and logic specific to the domain layer.
- **MeritJournal.Application**: Contains business logic, commands, queries, and interfaces for the application layer.
- **MeritJournal.Infrastructure**: Contains implementations of persistence, identity, and other infrastructure concerns.
- **MeritJournal.API**: Contains controllers, middleware, and API configurations.
- **MeritJournal.UnitTests**: Contains unit tests for the application.
- **MeritJournal.IntegrationTests**: Contains integration tests for the application.

## Prerequisites

- .NET 8 SDK
- PostgreSQL
- Docker (optional, for containerization)

## Getting Started

### Database Setup

1. Ensure PostgreSQL is running and accessible
2. Update the connection string in `appsettings.json` or use User Secrets:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=meritjournal;Username=postgres;Password=yourpassword;"
  }
}
```

### Running Migrations

To apply migrations and update the database:

```bash
cd MeritJournal.API
dotnet ef database update
```

### Running the Application

```bash
cd MeritJournal.API
dotnet run
```

The API will be available at:
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000

### Authentication

The API uses JWT/OIDC authentication. For local development, authentication can be disabled in the endpoints by commenting out the `.RequireAuthorization()` calls.

## API Endpoints

### Journal Entries

- `GET /api/journal-entries`: Get all journal entries for the current user
- `GET /api/journal-entries/{id}`: Get a journal entry by ID
- `POST /api/journal-entries`: Create a new journal entry
- `PUT /api/journal-entries/{id}`: Update an existing journal entry
- `DELETE /api/journal-entries/{id}`: Delete a journal entry

## Docker Support

The application includes Docker support. To build and run with Docker:

```bash
docker build -t meritjournal-api .
docker run -p 8080:80 -p 8081:443 meritjournal-api
```
