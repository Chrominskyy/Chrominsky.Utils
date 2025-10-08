# Docker Configuration for Chrominsky.Utils

This directory contains Docker configurations to help you get started with `Chrominsky.Utils` in containerized environments. It includes sample configurations for Redis, PostgreSQL, SQL Server, and .NET applications.

## 📋 Table of Contents

- [Quick Start](#quick-start)
- [Available Services](#available-services)
- [Configuration Files](#configuration-files)
- [Usage Examples](#usage-examples)
- [Environment Variables](#environment-variables)
- [Customization](#customization)
- [Troubleshooting](#troubleshooting)

## 🚀 Quick Start

### Prerequisites

- Docker Engine 20.10+
- Docker Compose 2.0+

### Start All Services

From the repository root:

```bash
# Start all services (Redis, PostgreSQL, SQL Server)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down

# Stop and remove volumes (WARNING: This deletes all data)
docker-compose down -v
```

### Start Individual Services

```bash
# Start only Redis
docker-compose up -d redis

# Start only PostgreSQL
docker-compose up -d postgres

# Start only SQL Server
docker-compose up -d sqlserver
```

## 📦 Available Services

### Redis (Port 6379)

Redis cache server for `RedisCacheRepository` and `RedisCacheService`.

- **Image**: `redis:7-alpine`
- **Port**: 6379
- **Connection String**: `localhost:6379`
- **Data Persistence**: Volume `redis-data`

### PostgreSQL (Port 5432)

PostgreSQL database server compatible with `Chrominsky.Utils`.

- **Image**: `postgres:16-alpine`
- **Port**: 5432
- **Default Database**: `chrominsky_db`
- **Default User**: `postgres`
- **Default Password**: `postgres123`
- **Connection String**: `Host=localhost;Port=5432;Database=chrominsky_db;Username=postgres;Password=postgres123`

### SQL Server (Port 1433)

Microsoft SQL Server for local development (Azure SQL compatible).

- **Image**: `mcr.microsoft.com/mssql/server:2022-latest`
- **Port**: 1433
- **Default User**: `sa`
- **Default Password**: `YourStrong!Passw0rd`
- **Connection String**: `Server=localhost,1433;Database=chrominsky_db;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True`

## 📄 Configuration Files

### docker-compose.yml

Located at repository root. Orchestrates all services including databases, cache, and optional .NET application.

### Dockerfile

Generic Dockerfile template for .NET 8.0 applications using Chrominsky.Utils.

Location: `docker/Dockerfile`

### .dockerignore

Optimizes Docker build by excluding unnecessary files.

## 💻 Usage Examples

### Example 1: Using Redis Cache

```csharp
using Chrominsky.Utils.Services;
using Chrominsky.Utils.Repositories;
using StackExchange.Redis;

// In Program.cs / Startup.cs
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379")
);
builder.Services.AddScoped<ICacheRepository, RedisCacheRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
```

### Example 2: Using PostgreSQL with Entity Framework

```csharp
using Microsoft.EntityFrameworkCore;
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Repositories.Base;

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=localhost;Port=5432;Database=chrominsky_db;Username=postgres;Password=postgres123")
);
```

### Example 3: Using SQL Server with Entity Framework

```csharp
using Microsoft.EntityFrameworkCore;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=localhost,1433;Database=chrominsky_db;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True")
);
```

### Example 4: Complete Application Setup

```csharp
using Microsoft.EntityFrameworkCore;
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Repositories.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;
using Chrominsky.Utils.Services;
using Chrominsky.Utils.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Database - PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Redis Cache
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379")
);
builder.Services.AddScoped<ICacheRepository, RedisCacheRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Repository with versioning
builder.Services.AddScoped<IObjectVersioningRepository, ObjectVersioningRepository>();
builder.Services.AddScoped(typeof(IBaseDatabaseRepository<>), typeof(BaseDatabaseRepository<>));

var app = builder.Build();
app.Run();
```

## 🔧 Environment Variables

### PostgreSQL Configuration

```bash
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres123
POSTGRES_DB=chrominsky_db
```

### SQL Server Configuration

```bash
ACCEPT_EULA=Y
SA_PASSWORD=YourStrong!Passw0rd
MSSQL_PID=Developer
```

### .NET Application Configuration

Add to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres;Port=5432;Database=chrominsky_db;Username=postgres;Password=postgres123",
    "SqlServer": "Server=sqlserver,1433;Database=chrominsky_db;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
  },
  "Redis": {
    "ConnectionString": "redis:6379"
  },
  "SimpleEmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}
```

## 🎨 Customization

### Change Database Passwords

Edit `docker-compose.yml`:

```yaml
postgres:
  environment:
    POSTGRES_PASSWORD: your_new_password

sqlserver:
  environment:
    SA_PASSWORD: "Your_New_Password123!"
```

### Use Different Ports

```yaml
postgres:
  ports:
    - "5433:5432"  # Map to different host port

redis:
  ports:
    - "6380:6379"  # Map to different host port
```

### Add Your .NET Application

1. Uncomment the `dotnet-api` service in `docker-compose.yml`
2. Create a `Dockerfile` in your application root
3. Update the build context path

```yaml
dotnet-api:
  build:
    context: .
    dockerfile: Dockerfile
  ports:
    - "5000:8080"
```

### Sample Dockerfile for Your Application

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["YourApp.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "YourApp.dll"]
```

## 🐛 Troubleshooting

### Redis Connection Issues

```bash
# Test Redis connection
docker exec -it chrominsky-redis redis-cli ping
# Should return: PONG

# View Redis logs
docker-compose logs redis
```

### PostgreSQL Connection Issues

```bash
# Test PostgreSQL connection
docker exec -it chrominsky-postgres psql -U postgres -d chrominsky_db -c "SELECT 1;"

# View PostgreSQL logs
docker-compose logs postgres
```

### SQL Server Connection Issues

```bash
# Test SQL Server connection
docker exec -it chrominsky-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT @@VERSION"

# View SQL Server logs
docker-compose logs sqlserver
```

### Port Already in Use

If you get "port already allocated" errors:

```bash
# Check what's using the port (e.g., 5432)
lsof -i :5432

# Option 1: Stop the conflicting service
# Option 2: Change the port in docker-compose.yml
```

### Container Health Check Failures

```bash
# Check container status
docker-compose ps

# View detailed container info
docker inspect chrominsky-postgres

# Restart specific service
docker-compose restart postgres
```

### Data Persistence

Data is stored in Docker volumes:

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect chrominsky_postgres-data

# Remove volumes (WARNING: deletes all data)
docker volume rm chrominsky_postgres-data
```

## 📚 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Redis Docker Hub](https://hub.docker.com/_/redis)
- [PostgreSQL Docker Hub](https://hub.docker.com/_/postgres)
- [SQL Server Docker Hub](https://hub.docker.com/_/microsoft-mssql-server)
- [Chrominsky.Utils GitHub](https://github.com/Chrominskyy/Chrominsky.Utils)

## 🔒 Security Notes

⚠️ **Important**: The default passwords in these examples are for **development only**. 

For production:
- Use strong, unique passwords
- Store credentials in environment variables or secrets management
- Enable SSL/TLS for database connections
- Use Docker secrets or Azure Key Vault
- Never commit passwords to version control
- Restrict network access appropriately

## 📝 License

These Docker configurations are part of the Chrominsky.Utils project and share the same license.
