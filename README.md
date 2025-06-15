# Merit Journal Application
Merit Journal is a web application designed for Buddhist practitioners. It offers a private space to document Dhamma practice, including meditation sessions, sutta study, dāna (generosity), sīla (virtuous conduct), and bhāvanā (mental development). This personal digital record helps users cultivate wholesome states and observe their spiritual progress.


## Features

- User authentication via OpenID Connect (Google, Microsoft)
- Create journal entries with formatted text (HTML), images, and tags
- View entries chronologically and filter by tags
- Edit and delete journal entries
- Responsive design for mobile and desktop use

## Technology Stack

### Backend (C#)
- ASP.NET Core Minimal APIs
- Clean Architecture (Domain, Application, Infrastructure, API layers)
- PostgreSQL database with EF Core (Npgsql)
- MediatR for CQRS pattern
- AutoMapper for object mapping
- JWT Bearer Authentication
- Swagger/OpenAPI documentation
- Containerized with Docker

### Frontend (React)
- React 18 with functional components and hooks
- Material-UI for responsive design
- Redux Toolkit for state management
- redux-persist for persistent authentication state
- oidc-client-ts for authentication
- React Router for navigation
- Vite for fast development and optimized builds

## Project Structure

```
/src
  /Backend
    /MeritJournal.Domain         # Entity classes, domain events
    /MeritJournal.Application    # Business logic, CQRS handlers
    /MeritJournal.Infrastructure # Data access, external services
    /MeritJournal.API            # API endpoints, configuration
    /MeritJournal.UnitTests      # Unit tests
    /MeritJournal.IntegrationTests # Integration tests
  /Frontend
    /public                      # Static assets
    /src
      /app                       # Redux store setup
      /components                # Reusable UI components
      /features                  # Feature-specific components and logic
      /pages                     # Page components
      /services                  # API services
```

## Getting Started

### Prerequisites
- .NET 8 SDK
- Node.js 16+
- PostgreSQL 14+
- Docker (optional for containerized deployment)

### Backend Setup

1. Navigate to the Backend directory:
   ```
   cd src/Backend
   ```

2. Restore dependencies:
   ```
   dotnet restore
   ```

3. Update the database connection string in `MeritJournal.API/appsettings.json`

4. Apply migrations:
   ```
   dotnet ef database update --project MeritJournal.Infrastructure --startup-project MeritJournal.API
   ```

5. Run the API:
   ```
   dotnet run --project MeritJournal.API
   ```

### Frontend Setup

1. Navigate to the Frontend directory:
   ```
   cd src/Frontend
   ```

2. Install dependencies:
   ```
   npm install
   ```

3. Configure the API URL in the `.env` file:
   ```
   VITE_API_URL=https://localhost:5001
   ```

4. Start the development server:
   ```
   npm start
   ```

## Deployment

### Backend Deployment

Build and publish the API:
```
dotnet publish -c Release -o ./publish
```

Build Docker image:
```
docker build -t merit-journal-api .
```

### Frontend Deployment

Build for production:
```
npm run build
```

Deploy the `build` folder to AWS CloudFront:
1. Upload the contents of the build folder to an S3 bucket
2. Set up a CloudFront distribution pointing to the S3 bucket
3. Configure the distribution settings for optimal static site hosting
4. Update DNS records to point to the CloudFront distribution

## License

This project is licensed under the MIT License.
