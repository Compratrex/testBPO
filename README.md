# Wholesale Purchasing Platform

REST API для платформы оптовых закупок. Проект сделан на .NET 10, PostgreSQL, EF Core code-first и разделен на слои в стиле Clean Architecture.

## Архитектура

- `WholesalePlatform.Domain` - доменные сущности, статусы, права, инварианты.
- `WholesalePlatform.Application` - use cases через MediatR, validators, DTO, абстракции портов.
- `WholesalePlatform.Infrastructure` - EF Core, Unit of Work, repositories, auth services, outbox.
- `WholesalePlatform.WebApi` - controllers, routing, JWT/permission authorization, middleware, OpenAPI.
- `WholesalePlatform.Tests` - focused tests для security/business/domain правил.
