# Production Cost API

A production and cost management system for baklava/pastry manufacturers. Manages raw materials, recipes, and production orders; automatically calculates unit cost and profit margin from recipes; enforces stock control on production; and secures endpoints with JWT authentication.

Built with ASP.NET Core Web API. Models the core workflow of an ERP module.

## Features

- **Raw material management** — price and stock tracking (CRUD)
- **Product + Recipe** — defines how much of each raw material a product contains (many-to-many with payload)
- **Cost calculation** — unit cost and profit margin based on the recipe and current material prices
- **Production orders** — stock availability check, stock deduction within a transaction, and cost snapshotting
- **Authentication** — JWT tokens, BCrypt password hashing, role-based authorization (Admin / Personnel)

## Tech Stack

- ASP.NET Core Web API (.NET 10)
- Entity Framework Core (Code-First, SQLite)
- JWT Authentication
- BCrypt.Net
- Swagger / OpenAPI

## Architecture

Layered structure:

- **Controller** — handles HTTP requests, validates input, returns responses
- **Service** — business logic (cost calculation, production rules)
- **DbContext** — data access (EF Core)
- **DTO** — data transfer with the outside world (entities are never exposed directly)

## Getting Started

```bash
# Restore dependencies
dotnet restore

# Create the database
dotnet tool run dotnet-ef database update

# Run
dotnet run
```

Once running, the Swagger UI is available at: `http://localhost:5213/swagger`

## Usage

1. `POST /api/Auth/kayit` — create a user
2. `POST /api/Auth/giris` — obtain a token
3. `POST /api/Hammadde` — add raw materials (pistachio, flour, butter...)
4. `POST /api/Urun` — add a product (e.g. Pistachio Baklava)
5. `POST /api/Urun/{id}/recete` — add recipe items
6. `GET /api/Urun/{id}/maliyet` — view cost and profit margin
7. `POST /api/Uretim` — create a production order (stock-checked, requires token)

## Technical Highlights

- **Atomic production via transactions** — stock deduction and order creation either both succeed or neither does (ACID)
- **Check-then-apply** — stock availability is verified before any deduction, preventing partial stock loss
- **Denormalization** — production cost is recorded at production time, so it stays fixed even if material prices change later
- **Dependency Injection** — DbContext and services are managed through DI
