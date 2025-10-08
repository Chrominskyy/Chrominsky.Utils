# Quick Start Guide - Docker with Chrominsky.Utils

Get up and running with Docker in under 5 minutes!

## 1. Start the Services

```bash
# Clone the repository (if you haven't already)
git clone https://github.com/Chrominskyy/Chrominsky.Utils.git
cd Chrominsky.Utils

# Start all services
docker-compose up -d
```

This starts:
- ✅ Redis on port 6379
- ✅ PostgreSQL on port 5432
- ✅ SQL Server on port 1433

## 2. Verify Services Are Running

```bash
# Check service status
docker-compose ps

# Test Redis
docker exec -it chrominsky-redis redis-cli ping
# Should return: PONG

# Test PostgreSQL
docker exec -it chrominsky-postgres psql -U postgres -d chrominsky_db -c "SELECT 1;"

# Test SQL Server
docker exec -it chrominsky-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT @@VERSION"
```

## 3. Connect from Your .NET Application

### Option A: Using appsettings.json

Add to your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=chrominsky_db;Username=postgres;Password=postgres123",
    "SqlServer": "Server=localhost,1433;Database=chrominsky_db;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  }
}
```

### Option B: Using Environment Variables

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=chrominsky_db;Username=postgres;Password=postgres123"
export Redis__ConnectionString="localhost:6379"
```

## 4. Install Chrominsky.Utils

```bash
dotnet add package Chrominsky.Utils
```

## 5. Configure Services in Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using Chrominsky.Utils.Repositories;
using Chrominsky.Utils.Repositories.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;
using Chrominsky.Utils.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration["Redis:ConnectionString"]!)
);
builder.Services.AddScoped<ICacheRepository, RedisCacheRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Repositories
builder.Services.AddScoped<IObjectVersioningRepository, ObjectVersioningRepository>();

var app = builder.Build();
app.Run();
```

## 6. Stop Services

```bash
# Stop services (keeps data)
docker-compose down

# Stop services and remove all data
docker-compose down -v
```

## Common Issues

### Port Already in Use

If you get "port already allocated" errors:

**Option 1:** Stop the conflicting service
```bash
# Find what's using the port
lsof -i :5432

# Kill the process
kill -9 <PID>
```

**Option 2:** Change the port in `docker-compose.yml`
```yaml
postgres:
  ports:
    - "5433:5432"  # Use port 5433 on host
```

### Connection Refused

Make sure services are healthy:
```bash
docker-compose ps

# Restart a specific service
docker-compose restart postgres
```

### Data Persistence

Data is automatically saved in Docker volumes. To view volumes:
```bash
docker volume ls
```

To remove all data:
```bash
docker-compose down -v
```

## Next Steps

- 📖 Read the [full Docker documentation](README.md)
- 🔧 Check [sample configurations](sample-api/)
- 📚 Review [Chrominsky.Utils documentation](../README.md)

## Need Help?

- [Open an issue](https://github.com/Chrominskyy/Chrominsky.Utils/issues)
- [View examples](../README.md)
- [Docker documentation](README.md)
