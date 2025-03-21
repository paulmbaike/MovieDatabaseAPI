# Movie Database API

A comprehensive RESTful API for managing a movie database, built with ASP.NET Core following clean architecture principles.

## Features

- Complete CRUD operations for movies, actors, directors, and genres
- JWT-based authentication and authorization
- API versioning
- Swagger/OpenAPI documentation
- Response caching
- Rate limiting
- Global exception handling
- Fluent validation
- Pagination
- Docker support
- Comprehensive test suite (unit and integration tests)

## Architecture

The solution follows clean architecture principles with a clear separation of concerns:

- **Core Layer** - Domain entities, interfaces, and DTOs
- **Infrastructure Layer** - Data access, external services
- **Services Layer** - Business logic implementation
- **API Layer** - REST endpoints and controllers

## Prerequisites

- .NET 8.0 SDK
- Docker (optional, for containerized deployment)

## Running the Application

### Using Docker (Recommended)

```bash
# Clone the repository
git clone https://github.com/paulmbaike/MovieDatabaseAPI
cd MovieDatabaseAPI

# Start the application using Docker Compose
docker-compose up -d
```

The API will be available at: http://localhost:5306/swagger

### Using .NET CLI

```bash
# Clone the repository
git clone https://github.com/paulmbaike/MovieDatabaseAPI
cd MovieDatabaseAPI

# Update the connection string in appsettings.json to your local database

# Build and run the application
dotnet build
cd src/MovieDatabaseAPI.API
dotnet run
```

The API will be available at: http://localhost:5306/swagger

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - Register a new user
- `POST /api/v1/auth/login` - Login and receive JWT token

### Movies
- `GET /api/v1/movies` - Get all movies
- `GET /api/v1/movies/{id}` - Get a specific movie
- `POST /api/v1/movies` - Create a new movie
- `PUT /api/v1/movies/{id}` - Update a movie
- `DELETE /api/v1/movies/{id}` - Delete a movie
- `GET /api/v1/movies/search` - Search movies by title
- `GET /api/v1/movies/director/{directorId}` - Get movies by director
- `GET /api/v1/movies/actor/{actorId}` - Get movies by actor
- `GET /api/v1/movies/genre/{genreId}` - Get movies by genre

Similar endpoints exist for Actors, Directors, and Genres.

## Testing

```bash
# Run unit tests
dotnet test tests/MovieDatabaseAPI.UnitTests

# Run integration tests
dotnet test tests/MovieDatabaseAPI.IntegrationTests
```

## Project Structure

```
MovieDatabaseAPI/
├── src/
│   ├── MovieDatabaseAPI.API/         # API controllers, configuration
│   ├── MovieDatabaseAPI.Core/        # Domain models, interfaces
│   ├── MovieDatabaseAPI.Infrastructure/ # Data access, repositories
│   └── MovieDatabaseAPI.Services/    # Business logic
├── tests/
│   ├── MovieDatabaseAPI.UnitTests/       # Unit tests
│   └── MovieDatabaseAPI.IntegrationTests/ # Integration tests
├── docker-compose.yml   # Docker configuration
└── README.md            # This file
```

## Database Schema

The database includes the following entities:
- Movies (with title, plot, release year, etc.)
- Actors (with name, bio, date of birth)
- Directors (with name, bio, date of birth)
- Genres (with name, description)
- Users (for authentication)

## License

MIT