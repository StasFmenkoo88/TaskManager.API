# TaskManager API

TaskManager API is a RESTful backend application built with ASP.NET Core.

The application allows users to register, log in, and manage their personal tasks. Each user can access and modify only their own tasks.

## Features

- User registration
- User login
- JWT authentication
- Password hashing
- User-specific task management
- Create tasks
- Get all tasks
- Get task by ID
- Update tasks
- Delete tasks
- Task filtering
- Input validation
- Global exception handling
- Logging
- Unit tests

## Technologies

- C#
- ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQL Server
- JWT Authentication
- AutoMapper
- FluentValidation
- xUnit
- Moq
- Swagger / OpenAPI

## Architecture

The project is separated into several layers:

- Controllers — handle HTTP requests
- Services — contain business logic
- Repositories — handle database access
- DTOs — define API request and response models
- Models — database entities
- Validators — validate incoming data
- Middleware — global exception handling

## Authentication

The API uses JWT Bearer authentication.

After login, the user receives a JWT token. Protected endpoints use the user ID stored in the token to ensure users can access only their own tasks.

## HTTP Status Codes

The API uses appropriate HTTP status codes, including:

- `201 Created` — resource successfully created
- `400 Bad Request` — invalid request
- `401 Unauthorized` — invalid credentials or missing authentication
- `404 Not Found` — resource not found
- `409 Conflict` — email already exists
- `500 Internal Server Error` — unexpected server error

## Testing

The project includes unit tests using xUnit and Moq.

Tests cover important TaskService and AuthService scenarios, including:

- Getting existing and non-existing tasks
- Creating tasks
- Updating tasks
- Deleting tasks
- Getting task lists
- Invalid login
- Successful login and JWT generation

## Security

Passwords are never stored as plain text and are hashed using ASP.NET Core PasswordHasher.

JWT secret keys are not stored in the Git repository and should be configured using User Secrets or environment variables.

## Author

Built as a backend development project using ASP.NET Core and C#.