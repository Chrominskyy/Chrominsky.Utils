# Copilot Instructions for Chrominsky.Utils

This repository contains a utility library for .NET 8.0 that provides advanced components to simplify common programming tasks in C#, including database operations, caching, email sending, and security management.

## Project Overview

- **Technology Stack**: .NET 8.0, C#, Entity Framework Core 8.0+
- **Key Dependencies**: StackExchange.Redis, BCrypt.Net-Next, Newtonsoft.Json
- **Package Type**: NuGet library
- **Testing Framework**: xUnit with Moq for mocking

## Coding Standards

### General Guidelines

- Use **nullable reference types** - the project has `<Nullable>enable</Nullable>` enabled
- Use **implicit usings** - avoid redundant using statements
- Follow **C# naming conventions**: PascalCase for classes/methods/properties, camelCase for local variables
- Keep code clean and maintainable with minimal complexity

### Documentation

- **Always add XML documentation** for public classes, methods, and properties
- Use `<summary>`, `<param>`, `<returns>`, and `<exception>` tags appropriately
- Document intent and behavior, not just what the code does
- Include examples in XML docs when helpful

Example:
```csharp
/// <summary>
/// Hashes a password using BCrypt.
/// </summary>
/// <param name="password">The password to hash.</param>
/// <returns>The hashed password.</returns>
public string HashPassword(string password)
{
    return BCrypt.Net.BCrypt.HashPassword(password);
}
```

### Code Style

- Use **4 spaces for indentation** (not tabs)
- Place opening braces on the same line for methods and classes
- Keep methods focused and single-purpose
- Use meaningful variable and method names
- Prefer explicit types over `var` when it improves readability

## Project Structure

```
Chrominsky.Utils/
├── Dto/              # Data Transfer Objects
├── Enums/            # Enumerations (DatabaseEntityStatus, SearchOperator, etc.)
├── Features/         # Feature-specific implementations (e.g., SimpleEmailSender)
├── Helpers/          # Helper classes (e.g., BCryptHelper)
├── Mappers/          # Data mapping classes
├── Models/           # Domain models
│   └── Base/         # Base entity classes (BaseEntity, BaseDatabaseEntity, etc.)
├── Repositories/     # Repository implementations
│   ├── Base/         # Base repository classes
│   └── ObjectVersioning/  # Version tracking repositories
└── Services/         # Service layer implementations
```

## Key Architectural Patterns

### Base Entities

- Use `BaseDatabaseEntity` for entities with full metadata tracking (Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Status)
- Use `BaseDatabaseEntityWithTenantId` for multi-tenant scenarios
- All entities should inherit from appropriate base classes

### Repository Pattern

- Repositories should extend `BaseDatabaseRepository<T>` for database operations
- Include version tracking via `IObjectVersioningRepository` when needed
- Support soft delete using `DatabaseEntityStatus` enum
- Implement pagination and search functionality

### Dependency Injection

- Use constructor injection for all dependencies
- Register services with appropriate lifetime (Scoped, Singleton, Transient)
- Use `IOptions<T>` pattern for configuration settings

## Testing Requirements

### Test Structure

- Place tests in `Chrominsky.Utils.Tests` project
- Mirror the source project structure in tests
- Use xUnit as the testing framework
- Use Moq for mocking dependencies

### Test Naming Convention

```csharp
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    // Act
    // Assert
}
```

### Testing Guidelines

- Write unit tests for all new features
- Use Arrange-Act-Assert pattern
- Mock external dependencies (databases, external services)
- Ensure tests are isolated and repeatable
- Test both success and failure scenarios
- Test edge cases and boundary conditions

## Contribution Requirements

Before submitting a pull request, ensure:

1. ✅ **Code compiles without errors** - run `dotnet build`
2. ✅ **All unit tests pass** - run `dotnet test`
3. ✅ **XML documentation added** for new public classes/methods
4. ✅ **CHANGELOG.md updated** with changes in format:
   ```markdown
   ## [Version] - YYYY-MM-DD
   ### Added/Changed/Fixed
   - Description of changes
   ```
5. ✅ **Tests written** for new functionality
6. ✅ **Code follows project conventions** and style

## Common Patterns in This Repository

### Versioning

The project uses automatic object versioning:
- Changes are tracked via `ObjectVersion` model
- `ObjectVersioningRepository` handles version storage
- Repositories automatically create version records on updates

### Search and Filtering

- Use `SearchParameterRequest` for complex searches
- Support operators: Contains, Equals, LessThan, GreaterThan, etc.
- Implement pagination via Skip/Take pattern

### Configuration

- Use `IOptions<T>` for settings (e.g., `SimpleEmailSettings`)
- Configure in `appsettings.json`
- Validate configuration in constructors

### Async/Await

- Use async methods for all I/O operations
- Suffix async methods with `Async`
- Always await async calls properly

## Specific Domain Knowledge

### Database Operations

- Use Entity Framework Core for database access
- Support SQL Server operations
- Implement soft delete pattern (Status field, not physical deletion)
- Track who created/updated entities with CreatedBy/UpdatedBy fields

### Caching

- Redis cache implementation via `RedisCacheRepository`
- Support for cache expiry
- Failover handling in `RedisCacheService`

### Security

- Use BCrypt for password hashing
- Never store plain text passwords
- Use work factor appropriate for security needs

### Email

- SMTP-based email sending via `SimpleEmailSender`
- Support HTML content in email body
- Configuration-driven (SMTP host, port, credentials)

## Notes

- This is a published NuGet package - maintain backward compatibility
- Follow semantic versioning (MAJOR.MINOR.PATCH)
- Keep dependencies up to date but stable
- Maintain comprehensive documentation in README.md
- Update CHANGELOG.md for every release
