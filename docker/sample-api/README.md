# Sample .NET API with Chrominsky.Utils

This directory contains a sample Dockerfile and configuration for building a .NET 8.0 API that uses Chrominsky.Utils.

## Using with Your Own Application

### Step 1: Copy the Dockerfile

Copy the `Dockerfile` to your application's root directory and modify the project references:

```dockerfile
# Update these lines with your actual project name
COPY ["YourApi/YourApi.csproj", "YourApi/"]
RUN dotnet restore "YourApi/YourApi.csproj"
```

### Step 2: Copy Configuration

Use the `appsettings.json` as a reference for configuring:
- Database connections (PostgreSQL and SQL Server)
- Redis cache connection
- Email settings

### Step 3: Update docker-compose.yml

In the main `docker-compose.yml`, update the `dotnet-api` service:

```yaml
dotnet-api:
  build:
    context: .
    dockerfile: Dockerfile
  ports:
    - "5000:8080"
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    - ConnectionStrings__DefaultConnection=Host=postgres;Database=chrominsky_db;Username=postgres;Password=postgres123
    - Redis__ConnectionString=redis:6379
  depends_on:
    - redis
    - postgres
```

### Step 4: Build and Run

```bash
# Build the image
docker build -t my-api .

# Or use docker-compose
docker-compose up -d
```

## Example Program.cs

Here's a minimal `Program.cs` that integrates Chrominsky.Utils:

```csharp
using Microsoft.EntityFrameworkCore;
using Chrominsky.Utils.Repositories;
using Chrominsky.Utils.Repositories.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;
using Chrominsky.Utils.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Tips

1. **Security**: Don't commit real passwords. Use environment variables or secrets management.
2. **Multi-stage builds**: The provided Dockerfile uses multi-stage builds to minimize image size.
3. **Health checks**: Add health check endpoints to monitor your application.
4. **Logging**: Configure proper logging for production environments.
