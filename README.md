# Chrominsky.Utils

`Chrominsky.Utils` is a utility library for .NET 8.0 that provides advanced components to simplify common programming tasks in C#. The library offers ready-made solutions for database operations, caching, email sending, and security management.

[![NuGet](https://img.shields.io/nuget/v/Chrominsky.Utils.svg)](https://www.nuget.org/packages/Chrominsky.Utils/)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)

## 📋 Table of Contents

- [Features](#-features)
- [Installation](#-installation)
- [Usage](#-usage)
  - [Base Repository with Versioning](#base-repository-with-versioning)
  - [Redis Cache](#redis-cache)
  - [BCrypt Password Hashing](#bcrypt-password-hashing)
  - [Email Sending](#email-sending)
  - [Search and Pagination](#search-and-pagination)
- [Requirements](#-requirements)
- [Changelog](#-changelog)
- [Contributing](#-contributing)
- [License](#-license)
- [Contact](#-contact)

## ✨ Features

### 🗄️ Base Repository (BaseDatabaseRepository)
- **CRUD with automatic versioning** - full support for Create, Read, Update, Delete operations
- **Advanced search** - filters with operators (`Contains`, `Equals`, `LessThan`, `GreaterThan`, etc.)
- **Pagination** - built-in result pagination support
- **Change tracking** - automatic object versioning (ObjectVersioning)
- **Soft delete** - logical deletion using entity statuses
- **Column metadata** - retrieve information about table structure
- **Database support** - works with SQL Server and PostgreSQL

### 💾 Redis Cache
- **RedisCacheRepository** - Redis cache implementation
- **RedisCacheService** - service with failover functionality (`GetOrAddAsync`)
- Operations: Get, Set, Remove, Exists
- Expiry time support

### 🔐 Security
- **BCryptHelper** - password hashing and verification using BCrypt
- Secure password storage following best practices

### 📧 Email Sending
- **SimpleEmailSender** - simple interface for sending emails via SMTP
- Configuration through `IOptions<SimpleEmailSettings>`
- HTML support in message body

### 📦 Base Models
- **BaseDatabaseEntity** - base entity class with full metadata (Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Status)
- **BaseDatabaseEntityWithTenantId** - entity with multi-tenancy support
- **BaseSimpleEntity** - simplified entity
- **ObjectVersion** - model for tracking object change history

### 🔍 Types and Enumerations
- **DatabaseEntityStatus** - entity statuses (Active, Deleted, etc.)
- **SearchOperator** - search operators
- **SearchOrder** - result ordering
- **DatabaseColumnTypes** - database column type classification (supports SQL Server and PostgreSQL)

## 📦 Installation

Install the package via NuGet:

```bash
dotnet add package Chrominsky.Utils
```

Or add directly to your `.csproj` file:

```xml
<PackageReference Include="Chrominsky.Utils" Version="1.3.0" />
```

## 🚀 Usage

### Base Repository with Versioning

```csharp
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Repositories.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;

// Entity definition
public class User : BaseDatabaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Repository implementation
public class UserRepository : BaseDatabaseRepository<User>
{
    public UserRepository(DbContext dbContext, IObjectVersioningRepository versioningRepo) 
        : base(dbContext, versioningRepo)
    {
    }
}

// Usage
var user = new User 
{ 
    Name = "John Doe", 
    Email = "john@example.com",
    CreatedBy = "system"
};

// Add - automatic versioning
Guid userId = await userRepository.AddAsync(user);

// Retrieve
var existingUser = await userRepository.GetByIdAsync<User>(userId);

// Update - automatic change tracking
existingUser.Email = "new@example.com";
existingUser.UpdatedBy = "admin";
await userRepository.UpdateAsync(existingUser);

// Delete (soft delete)
await userRepository.DeleteAsync<User>(userId, "admin");

// Get all active
var activeUsers = await userRepository.GetAllActiveAsync<User>();
```

### Redis Cache

```csharp
using Chrominsky.Utils.Services;
using Chrominsky.Utils.Repositories;
using StackExchange.Redis;

// Configuration in Program.cs / Startup.cs
services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379")
);
services.AddScoped<ICacheRepository, RedisCacheRepository>();
services.AddScoped<ICacheService, RedisCacheService>();

// Usage
public class ProductService
{
    private readonly ICacheService _cacheService;
    private readonly IProductRepository _productRepository;

    public ProductService(ICacheService cacheService, IProductRepository productRepository)
    {
        _cacheService = cacheService;
        _productRepository = productRepository;
    }

    public async Task<Product> GetProductAsync(Guid productId)
    {
        string cacheKey = $"product:{productId}";
        
        // GetOrAddAsync - retrieves from cache or executes failover
        return await _cacheService.GetOrAddAsync(
            cacheKey,
            async () => await _productRepository.GetByIdAsync<Product>(productId),
            TimeSpan.FromMinutes(30)
        );
    }

    public async Task InvalidateProductCache(Guid productId)
    {
        await _cacheService.RemoveAsync($"product:{productId}");
    }
}
```

### BCrypt Password Hashing

```csharp
using Chrominsky.Utils.Helpers;

var bcryptHelper = new BCryptHelper();

// Hash password
string password = "MyStrongPassword123!";
string hashedPassword = bcryptHelper.HashPassword(password);
// $2a$11$... (BCrypt hash)

// Verify password
string inputPassword = "MyStrongPassword123!";
bool isValid = bcryptHelper.VerifyPassword(inputPassword, hashedPassword);
// true
```

### Email Sending

```csharp
using Chrominsky.Utils.Features.SimpleEmailSender;

// Configuration in appsettings.json
{
  "SimpleEmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}

// Registration in Program.cs
services.Configure<SimpleEmailSettings>(
    configuration.GetSection("SimpleEmailSettings")
);
services.AddScoped<SimpleEmailSender>();

// Usage
public class NotificationService
{
    private readonly SimpleEmailSender _emailSender;

    public NotificationService(SimpleEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public void SendWelcomeEmail(string userEmail, string userName)
    {
        string subject = "Welcome to our application!";
        string body = $"<h1>Welcome {userName}!</h1><p>Thank you for registering.</p>";
        
        _emailSender.SendEmail(userEmail, subject, body);
    }
}
```

### Search and Pagination

```csharp
using Chrominsky.Utils.Models;
using Chrominsky.Utils.Enums;

// Search with filters
var searchRequest = new SearchParameterRequest
{
    Page = 1,
    PageSize = 20,
    IncludeNotActive = false,
    SearchParameters = new List<SearchParameter>
    {
        new SearchParameter 
        { 
            FieldName = "Name", 
            Value = "Doe", 
            Operator = SearchOperator.Contains 
        },
        new SearchParameter 
        { 
            FieldName = "CreatedAt", 
            Value = DateTime.UtcNow.AddDays(-30).ToString("o"), 
            Operator = SearchOperator.GreaterThan 
        }
    }
};

var results = await userRepository.SearchAsync<User>(searchRequest);
// Returns: PaginatedResponse<IEnumerable<User>>
Console.WriteLine($"Found {results.TotalCount} users");
Console.WriteLine($"Page {results.Page}/{Math.Ceiling((double)results.TotalCount / results.PageSize)}");

// Simple pagination
var paginatedUsers = await userRepository.GetPaginatedAsync<User>(page: 1, pageSize: 50);
```

### Retrieving Table Structure

```csharp
// Retrieve table column information
var tableColumns = userRepository.GetTableColumnsAsync<User>();

if (tableColumns != null)
{
    Console.WriteLine($"Table: {tableColumns.TableName}");
    foreach (var column in tableColumns.Columns)
    {
        Console.WriteLine($"- {column.ColumnName}: {column.DataType} (Group: {column.Group})");
    }
}
```

### Using with PostgreSQL

```csharp
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Repositories.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;

// Configure DbContext for PostgreSQL
public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ObjectVersion> ObjectVersions { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=mydb;Username=postgres;Password=password");
    }
}

// Repository implementation (same as SQL Server)
public class UserRepository : BaseDatabaseRepository<User>
{
    public UserRepository(DbContext dbContext, IObjectVersioningRepository versioningRepo) 
        : base(dbContext, versioningRepo)
    {
    }
}

// Usage - DatabaseColumnTypes automatically handles PostgreSQL types
// PostgreSQL types: varchar, integer, bigint, boolean, uuid, timestamp, bytea, etc.
// SQL Server types: nvarchar, int, bigint, bit, uniqueidentifier, datetime2, varbinary, etc.

using Chrominsky.Utils.Enums;

// Example: Check if a data type is a text type
string postgresType = "character varying";
string group = DatabaseColumnTypes.GetGroup(postgresType); // Returns "Text"

// Works with both PostgreSQL and SQL Server types
string sqlServerType = "nvarchar";
string sqlGroup = DatabaseColumnTypes.GetGroup(sqlServerType); // Returns "Text"
```

## 📋 Requirements

- **.NET 8.0** or newer
- **Entity Framework Core 8.0+** (for database functionality)
- **Npgsql.EntityFrameworkCore.PostgreSQL 8.0+** (optional, for PostgreSQL support)
- **StackExchange.Redis 2.7+** (for Redis cache)
- **BCrypt.Net-Next 4.0+** (for password hashing)

## 📝 Changelog

Full change history available in [CHANGELOG.md](Chrominsky.Utils/CHANGELOG.md).

### Latest Changes (1.3.0 - 2025-03-14)
- Added PostgreSQL support to `DatabaseColumnTypes`
- Support for PostgreSQL data types: varchar, character varying, integer, bigint, boolean, uuid, timestamp, bytea, and more
- New unit tests for PostgreSQL type mapping

### Version 1.2.2
- Added `DatabaseColumnTypes` - enum class to handle different database column types

### Version 1.2.0
- Added `GetTableColumnsAsync` - method to retrieve table column structure
- New models: `TableColumns`, `TableColumnsDto`, `TableColumnsMapper`

### Version 1.1.0
- Added `SimpleEmailSender` - email sending functionality

### Version 1.0.8
- Added object versioning system (`ObjectVersion`, `ObjectVersioningRepository`)

### Version 1.0.6
- Added advanced search (`SearchAsync` in `BaseDatabaseRepository`)
- New models: `SearchParameterRequest`, `SearchParameter`, `SearchOperator`
- Created unit test project

## 🤝 Contributing

Contributions are welcome! If you'd like to help:

1. Fork the repository
2. Create a branch for your feature (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

Before submitting a PR, make sure that:
- ✅ Code compiles without errors
- ✅ Unit tests pass
- ✅ XML documentation added for new classes/methods
- ✅ CHANGELOG.md updated

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## 📧 Contact

**Bartosz Chrominski**

- GitHub: [@Chrominskyy](https://github.com/Chrominskyy)
- Repository: [Chrominsky.Utils](https://github.com/Chrominskyy/Chrominsky.Utils)

For questions or suggestions, feel free to open an issue in the GitHub repository.

---

⭐ If this project was helpful, leave a star on GitHub!
