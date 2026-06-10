# Wholesale Purchasing Platform

REST API для платформы оптовых закупок. Проект сделан на .NET 10, PostgreSQL, EF Core code-first и разделен на слои в стиле Clean Architecture.

## Требования

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) (для запуска PostgreSQL и API в контейнерах)
- PostgreSQL 17 (при локальном запуске без Docker)

## Быстрый старт

### Через Docker (рекомендуется)

```bash
docker-compose up
```

API будет доступен на `http://localhost:5000`.

PostgreSQL будет доступен на `5432` порту.

### Локальный запуск

1. Убедитесь, что PostgreSQL запущен и доступен.
2. Создайте `.env` на основе `.env.example` (или используйте значения по умолчанию).
3. Запустите API:

```bash
cd src/WholesalePlatform.WebApi
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

API будет доступен на `http://localhost:5000`.

## Учётные данные для Development

При первом запуске в режиме `Development` автоматически создаётся встроенный администратор:

| Поле       | Значение            |
|------------|---------------------|
| **Email**  | `admin@example.com` |
| **Пароль** | `Admin123456!`      |

**Вход:**

```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "Admin123456!"
}
```

> В production необходимо отключить `BootstrapAdmin:UseBuiltInDevelopmentAdmin` и задать собственные email/пароль через `BootstrapAdmin:Email` и `BootstrapAdmin:Password`.

## Архитектура

```
WholesalePurchasingPlatform/
├── src/
│   ├── WholesalePlatform.Domain/        # Доменные сущности, Value Objects, Enums
│   │   ├── Customers/                   # Customer (агрегат)
│   │   ├── Orders/                      # Order, OrderItem
│   │   ├── Products/                    # Product
│   │   ├── Users/                       # User (админ / кастомер)
│   │   ├── Permissions/                 # Права доступа (PermissionCode)
│   │   └── Enums/                       # UserRole, OrderStatus
│   ├── WholesalePlatform.Application/   # Use Cases (MediatR), валидация, DTO
│   │   ├── Auth/                        # Login, SetPassword
│   │   ├── Customers/                   # Регистрация, управление
│   │   ├── Orders/                      # Создание, отмена, просмотр
│   │   ├── Products/                    # CRUD товаров
│   │   └── Common/                      # ApiResponse, PagedResult
│   ├── WholesalePlatform.Infrastructure/ # EF Core, Unit of Work, Auth, Outbox
│   │   ├── Persistence/                 # AppDbContext, миграции, репозитории
│   │   ├── Auth/                        # PasswordHasher, JWT провайдер
│   │   ├── Outbox/                      # Outbox (иденпотентность)
│   │   └── Email/                       # Email-провайдер (ссылки сброса пароля)
│   └── WholesalePlatform.WebApi/        # ASP.NET хост, контроллеры, middleware
│       ├── Controllers/                 # Auth, Customers, Orders, Products
│       ├── Contracts/                   # Request/Response DTO
│       ├── Authorization/               # Permission-based авторизация
│       ├── Extensions/                  # BootstrapAdmin, миграции, DI
│       ├── Middleware/                  # Exception handling, логирование
│       └── Routing/                     # ApiRoutes (централизованные пути)
├── tests/WholesalePlatform.Tests/       # Модульные тесты (xUnit)
├── docker/postgres/init/                # SQL-скрипты инициализации
├── docker-compose.yml                   # API + PostgreSQL
├── Dockerfile
└── requests.http                        # Примеры запросов (REST Client)
```


Полный список endpoint'ов — в `src/WholesalePlatform.WebApi/Controllers/` и `requests.http`. или в сваггере

## Тестирование

```bash
dotnet test
```

Тесты находятся в `tests/WholesalePlatform.Tests/` и покрывают business/security/domain правила.
