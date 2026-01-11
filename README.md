# TaskManagementSystem

REST API for task management built with ASP.NET Core using Clean Architecture principles.

## Architecture
The solution is structured into independent layers:

- **Api** — HTTP layer (controllers, middleware)
- **Application** — business logic, DTOs, service interfaces
- **Domain** — core entities and enums
- **Infrastructure** — persistence, repositories, authentication
- **Tests** — unit tests for application services

## Authentication
- JWT-based authentication
- Passwords are stored using BCrypt hashing
- Tokens are generated via a dedicated JWT service
- All task operations are scoped to the authenticated user
- Users can only access and modify their own tasks

## Error Handling
The API uses centralized exception handling middleware to convert application and domain exceptions into consistent HTTP error responses.

## Status
The project is under active development.