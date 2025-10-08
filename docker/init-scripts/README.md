# Database Initialization Scripts

This directory contains SQL scripts to initialize PostgreSQL and SQL Server databases with sample schema compatible with `Chrominsky.Utils`.

## Scripts

### init-postgres.sql

PostgreSQL initialization script that creates:

- **users** table - Sample table with `BaseDatabaseEntity` structure
- **object_versions** table - For tracking entity changes
- **products** table - Sample products table
- Indexes for performance
- Sample data

### init-sqlserver.sql

SQL Server initialization script that creates the same structure as PostgreSQL but with SQL Server syntax and data types.

## Using the Scripts

### Automatic Initialization (Recommended)

When using docker-compose, you can automatically run these scripts on container startup:

**For PostgreSQL:**

Update `docker-compose.yml`:

```yaml
postgres:
  image: postgres:16-alpine
  volumes:
    - postgres-data:/var/lib/postgresql/data
    - ./docker/init-scripts/init-postgres.sql:/docker-entrypoint-initdb.d/init.sql
```

**For SQL Server:**

SQL Server requires a different approach. You can run the script after the container starts:

```bash
# Wait for SQL Server to be ready
sleep 30

# Run the initialization script
docker exec -i chrominsky-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' < docker/init-scripts/init-sqlserver.sql
```

### Manual Execution

**PostgreSQL:**

```bash
# Copy script to container
docker cp docker/init-scripts/init-postgres.sql chrominsky-postgres:/tmp/init.sql

# Execute script
docker exec -it chrominsky-postgres psql -U postgres -d chrominsky_db -f /tmp/init.sql
```

**SQL Server:**

```bash
# Execute script from host
docker exec -i chrominsky-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' < docker/init-scripts/init-sqlserver.sql
```

## Schema Details

### Users Table

```sql
-- PostgreSQL
CREATE TABLE users (
    id UUID PRIMARY KEY,
    username VARCHAR(255) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP,
    created_by VARCHAR(255),
    updated_by VARCHAR(255),
    status INTEGER NOT NULL
);
```

This matches the `BaseDatabaseEntity` structure from Chrominsky.Utils:

```csharp
public class User : BaseDatabaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}
```

### Object Versions Table

Tracks all changes to entities when using `IObjectVersioningRepository`:

```sql
CREATE TABLE object_versions (
    id UUID PRIMARY KEY,
    object_id UUID NOT NULL,
    object_type VARCHAR(255) NOT NULL,
    property_name VARCHAR(255) NOT NULL,
    old_value TEXT,
    new_value TEXT,
    changed_at TIMESTAMP NOT NULL,
    changed_by VARCHAR(255)
);
```

### Products Table

Sample table demonstrating the base entity pattern:

```sql
CREATE TABLE products (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price DECIMAL(18, 2) NOT NULL,
    stock_quantity INTEGER NOT NULL,
    created_at TIMESTAMP NOT NULL,
    updated_at TIMESTAMP,
    created_by VARCHAR(255),
    updated_by VARCHAR(255),
    status INTEGER NOT NULL
);
```

## Customizing the Scripts

### Adding Your Own Tables

Add your table definitions after the existing ones. Follow the `BaseDatabaseEntity` pattern:

```sql
CREATE TABLE your_table (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    -- Your custom fields
    your_field VARCHAR(255),
    -- BaseDatabaseEntity fields
    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMP,
    created_by VARCHAR(255),
    updated_by VARCHAR(255),
    status INTEGER NOT NULL DEFAULT 0
);
```

### Modifying Sample Data

Replace the INSERT statements with your own data:

```sql
INSERT INTO users (username, email, password_hash, created_by, status)
VALUES 
    ('your_user', 'your@email.com', 'hashed_password', 'system', 0);
```

## Using with Entity Framework Migrations

If you prefer to use EF Core migrations instead of these scripts:

1. Remove the volume mount from docker-compose.yml
2. Run your migrations after the container starts:

```bash
# Start the database
docker-compose up -d postgres

# Run migrations
dotnet ef database update
```

## Security Notes

⚠️ **Important**: The sample data includes placeholder passwords. In production:

- Use `BCryptHelper` from Chrominsky.Utils to hash passwords
- Never use default or weak passwords
- Store sensitive data securely
- Use environment variables for connection strings

Example of proper password hashing:

```csharp
using Chrominsky.Utils.Helpers;

var bcrypt = new BCryptHelper();
var hashedPassword = bcrypt.HashPassword("your_password");
```

## Troubleshooting

### Script Not Running

**PostgreSQL:** Scripts in `/docker-entrypoint-initdb.d/` only run on first startup. If the database already exists:

```bash
# Remove the volume and recreate
docker-compose down -v
docker-compose up -d postgres
```

**SQL Server:** Check if the database exists:

```bash
docker exec -it chrominsky-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourStrong!Passw0rd' -Q "SELECT name FROM sys.databases"
```

### Permission Errors

PostgreSQL scripts must be readable:

```bash
chmod +r docker/init-scripts/init-postgres.sql
```

### Syntax Errors

- PostgreSQL uses different syntax than SQL Server
- Check data types (e.g., `UUID` vs `UNIQUEIDENTIFIER`)
- Check function names (e.g., `gen_random_uuid()` vs `NEWID()`)

## Related Documentation

- [Docker README](../README.md) - Full Docker documentation
- [Quick Start Guide](../QUICKSTART.md) - Get started quickly
- [Chrominsky.Utils Documentation](../../README.md) - Library documentation
