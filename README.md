# Chrominsky.Utils

`Chrominsky.Utils` to biblioteka narzędziowa dla .NET 8.0, która dostarcza zaawansowane komponenty do uproszczenia typowych zadań programistycznych w C#. Biblioteka oferuje gotowe rozwiązania dla operacji bazodanowych, cache'owania, wysyłania emaili i zarządzania bezpieczeństwem.

[![NuGet](https://img.shields.io/nuget/v/Chrominsky.Utils.svg)](https://www.nuget.org/packages/Chrominsky.Utils/)
[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)

## 📋 Spis treści

- [Funkcjonalności](#-funkcjonalności)
- [Instalacja](#-instalacja)
- [Użycie](#-użycie)
  - [Repository bazowe z wersjonowaniem](#repository-bazowe-z-wersjonowaniem)
  - [Cache Redis](#cache-redis)
  - [Hashowanie haseł BCrypt](#hashowanie-haseł-bcrypt)
  - [Wysyłanie emaili](#wysyłanie-emaili)
  - [Wyszukiwanie i paginacja](#wyszukiwanie-i-paginacja)
- [Wymagania](#-wymagania)
- [Changelog](#-changelog)
- [Współpraca](#-współpraca)
- [Licencja](#-licencja)
- [Kontakt](#-kontakt)

## ✨ Funkcjonalności

### 🗄️ Repository bazowe (BaseDatabaseRepository)
- **CRUD z automatycznym wersjonowaniem** - pełna obsługa operacji Create, Read, Update, Delete
- **Wyszukiwanie zaawansowane** - filtry z operatorami (`Contains`, `Equals`, `LessThan`, `GreaterThan`, etc.)
- **Paginacja** - wbudowana obsługa stronicowania wyników
- **Śledzenie zmian** - automatyczne wersjonowanie obiektów (ObjectVersioning)
- **Soft delete** - usuwanie logiczne z wykorzystaniem statusów encji
- **Metadane kolumn** - pobieranie informacji o strukturze tabel

### 💾 Cache Redis
- **RedisCacheRepository** - implementacja cache'u z Redis
- **RedisCacheService** - serwis z funkcją failover (`GetOrAddAsync`)
- Operacje: Get, Set, Remove, Exists
- Obsługa czasu wygaśnięcia (expiry)

### 🔐 Bezpieczeństwo
- **BCryptHelper** - hashowanie i weryfikacja haseł z użyciem BCrypt
- Bezpieczne przechowywanie haseł zgodne z najlepszymi praktykami

### 📧 Wysyłanie emaili
- **SimpleEmailSender** - prosty interfejs do wysyłania emaili przez SMTP
- Konfiguracja przez `IOptions<SimpleEmailSettings>`
- Obsługa HTML w treści wiadomości

### 📦 Modele bazowe
- **BaseDatabaseEntity** - bazowa klasa encji z pełnymi metadanymi (Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Status)
- **BaseDatabaseEntityWithTenantId** - encja z obsługą multi-tenancy
- **BaseSimpleEntity** - uproszczona encja
- **ObjectVersion** - model do śledzenia historii zmian obiektów

### 🔍 Typy i enumeracje
- **DatabaseEntityStatus** - statusy encji (Active, Deleted, etc.)
- **SearchOperator** - operatory wyszukiwania
- **SearchOrder** - porządkowanie wyników
- **DatabaseColumnTypes** - klasyfikacja typów kolumn bazodanowych

## 📦 Instalacja

Zainstaluj pakiet przez NuGet:

```bash
dotnet add package Chrominsky.Utils
```

Lub dodaj bezpośrednio do pliku `.csproj`:

```xml
<PackageReference Include="Chrominsky.Utils" Version="1.2.2" />
```

## 🚀 Użycie

### Repository bazowe z wersjonowaniem

```csharp
using Chrominsky.Utils.Models.Base;
using Chrominsky.Utils.Repositories.Base;
using Chrominsky.Utils.Repositories.ObjectVersioning;

// Definicja encji
public class User : BaseDatabaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// Implementacja repository
public class UserRepository : BaseDatabaseRepository<User>
{
    public UserRepository(DbContext dbContext, IObjectVersioningRepository versioningRepo) 
        : base(dbContext, versioningRepo)
    {
    }
}

// Użycie
var user = new User 
{ 
    Name = "Jan Kowalski", 
    Email = "jan@example.com",
    CreatedBy = "system"
};

// Dodanie - automatyczne wersjonowanie
Guid userId = await userRepository.AddAsync(user);

// Pobranie
var existingUser = await userRepository.GetByIdAsync<User>(userId);

// Aktualizacja - automatyczne śledzenie zmian
existingUser.Email = "nowy@example.com";
existingUser.UpdatedBy = "admin";
await userRepository.UpdateAsync(existingUser);

// Usunięcie (soft delete)
await userRepository.DeleteAsync<User>(userId, "admin");

// Pobranie wszystkich aktywnych
var activeUsers = await userRepository.GetAllActiveAsync<User>();
```

### Cache Redis

```csharp
using Chrominsky.Utils.Services;
using Chrominsky.Utils.Repositories;
using StackExchange.Redis;

// Konfiguracja w Program.cs / Startup.cs
services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379")
);
services.AddScoped<ICacheRepository, RedisCacheRepository>();
services.AddScoped<ICacheService, RedisCacheService>();

// Użycie
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
        
        // GetOrAddAsync - pobiera z cache lub wykonuje failover
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

### Hashowanie haseł BCrypt

```csharp
using Chrominsky.Utils.Helpers;

var bcryptHelper = new BCryptHelper();

// Hashowanie hasła
string password = "MojeSilneHaslo123!";
string hashedPassword = bcryptHelper.HashPassword(password);
// $2a$11$... (hash BCrypt)

// Weryfikacja hasła
string inputPassword = "MojeSilneHaslo123!";
bool isValid = bcryptHelper.VerifyPassword(inputPassword, hashedPassword);
// true
```

### Wysyłanie emaili

```csharp
using Chrominsky.Utils.Features.SimpleEmailSender;

// Konfiguracja w appsettings.json
{
  "SimpleEmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "twoj-email@gmail.com",
    "SenderPassword": "twoje-haslo-aplikacji"
  }
}

// Rejestracja w Program.cs
services.Configure<SimpleEmailSettings>(
    configuration.GetSection("SimpleEmailSettings")
);
services.AddScoped<SimpleEmailSender>();

// Użycie
public class NotificationService
{
    private readonly SimpleEmailSender _emailSender;

    public NotificationService(SimpleEmailSender emailSender)
    {
        _emailSender = emailSender;
    }

    public void SendWelcomeEmail(string userEmail, string userName)
    {
        string subject = "Witamy w naszej aplikacji!";
        string body = $"<h1>Witaj {userName}!</h1><p>Dziękujemy za rejestrację.</p>";
        
        _emailSender.SendEmail(userEmail, subject, body);
    }
}
```

### Wyszukiwanie i paginacja

```csharp
using Chrominsky.Utils.Models;
using Chrominsky.Utils.Enums;

// Wyszukiwanie z filtrami
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
            Value = "Kowalski", 
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
// Zwraca: PaginatedResponse<IEnumerable<User>>
Console.WriteLine($"Znaleziono {results.TotalCount} użytkowników");
Console.WriteLine($"Strona {results.Page}/{Math.Ceiling((double)results.TotalCount / results.PageSize)}");

// Prosta paginacja
var paginatedUsers = await userRepository.GetPaginatedAsync<User>(page: 1, pageSize: 50);
```

### Pobieranie struktury tabel

```csharp
// Pobieranie informacji o kolumnach tabeli
var tableColumns = userRepository.GetTableColumnsAsync<User>();

if (tableColumns != null)
{
    Console.WriteLine($"Tabela: {tableColumns.TableName}");
    foreach (var column in tableColumns.Columns)
    {
        Console.WriteLine($"- {column.ColumnName}: {column.DataType} (Group: {column.Group})");
    }
}
```

## 📋 Wymagania

- **.NET 8.0** lub nowszy
- **Entity Framework Core 8.0+** (dla funkcjonalności bazodanowych)
- **StackExchange.Redis 2.7+** (dla cache Redis)
- **BCrypt.Net-Next 4.0+** (dla hashowania haseł)

## 📝 Changelog

Pełna historia zmian dostępna w pliku [CHANGELOG.md](Chrominsky.Utils/CHANGELOG.md).

### Najnowsze zmiany (1.2.2 - 2025-03-13)
- Dodano `DatabaseColumnTypes` - klasę enum do obsługi różnych typów kolumn bazodanowych

### Wersja 1.2.0
- Dodano `GetTableColumnsAsync` - metoda do pobierania struktury kolumn tabeli
- Nowe modele: `TableColumns`, `TableColumnsDto`, `TableColumnsMapper`

### Wersja 1.1.0
- Dodano `SimpleEmailSender` - funkcjonalność wysyłania emaili

### Wersja 1.0.8
- Dodano system wersjonowania obiektów (`ObjectVersion`, `ObjectVersioningRepository`)

### Wersja 1.0.6
- Dodano zaawansowane wyszukiwanie (`SearchAsync` w `BaseDatabaseRepository`)
- Nowe modele: `SearchParameterRequest`, `SearchParameter`, `SearchOperator`
- Utworzono projekt testów jednostkowych

## 🤝 Współpraca

Wkład w projekt jest mile widziany! Jeśli chcesz pomóc:

1. Zforkuj repozytorium
2. Stwórz branch dla swojej funkcjonalności (`git checkout -b feature/AmazingFeature`)
3. Commituj swoje zmiany (`git commit -m 'Add some AmazingFeature'`)
4. Push do brancha (`git push origin feature/AmazingFeature`)
5. Otwórz Pull Request

Przed wysłaniem PR, upewnij się że:
- ✅ Kod się kompiluje bez błędów
- ✅ Testy jednostkowe przechodzą
- ✅ Dodano dokumentację XML dla nowych klas/metod
- ✅ Zaktualizowano CHANGELOG.md

## 📄 Licencja

Ten projekt jest objęty licencją MIT. Szczegóły w pliku [LICENSE](LICENSE).

## 📧 Kontakt

**Bartosz Chrominski**

- GitHub: [@Chrominskyy](https://github.com/Chrominskyy)
- Repozytorium: [Chrominsky.Utils](https://github.com/Chrominskyy/Chrominsky.Utils)

W razie pytań lub sugestii, śmiało otwórz issue w repozytorium GitHub.

---

⭐ Jeśli ten projekt okazał się pomocny, zostaw gwiazdkę na GitHub!
