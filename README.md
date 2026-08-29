# ElectroShop — E-Commerce Engine & Enterprise Backend Architecture

[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20%2F%20DDD--Inspired-blue)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![CQRS & MediatR](https://img.shields.io/badge/Pattern-CQRS%20with%20MediatR-brightgreen)](https://github.com/jbogard/MediatR)
[![Status](https://img.shields.io/badge/Status-In%20Active%20Development-orange)](#project-status--azure-roadmap)
[![License: PolyForm NonCommercial 1.0.0](https://img.shields.io/badge/License-PolyForm%20NC%201.0.0-red.svg)](./LICENSE.md)

ElectroShop is a modular e-commerce backend engine built with C# and .NET 10. It is designed as a production-oriented platform and an architectural blueprint demonstrating separation of concerns, domain modeling, transactional resilience, and infrastructure decoupling.

---

## Architectural Blueprint

The platform follows **Clean Architecture** and **Domain-Driven Design (DDD)** principles. Business rules remain isolated from databases, transport protocols, and infrastructure integrations. The Domain project has one deliberate framework dependency for the shared ASP.NET Core Identity base types used by `AppUser` and `AppRole`.

```text
              ┌───────────────────────────────────────────┐
              │                 API Layer                 │
              │   Controllers, middleware, and OpenAPI    │
              └─────────────────────┬─────────────────────┘
                                    │
                                    ▼
              ┌───────────────────────────────────────────┐
              │             Application Layer             │
              │   CQRS commands, queries, DTOs, pipeline  │
              └─────────────────────┬─────────────────────┘
                                    │
                                    ▼
              ┌───────────────────────────────────────────┐
              │               Domain Layer                │
              │     Entities, enums, value objects, rules │
              └─────────────────────▲─────────────────────┘
                                    │
              ┌─────────────────────┴─────────────────────┐
              │            Infrastructure Layer           │
              │    EF Core, persistence, and integrations │
              └───────────────────────────────────────────┘
```

### Architectural Layers & Design Rules

#### 1. Core Domain (`Domain`)

- **Role:** Contains business models, invariants, value objects, and domain enums.
- **Controlled dependency:** The Domain project references `Microsoft.AspNetCore.Identity.EntityFrameworkCore` only to derive `AppUser` from `IdentityUser<Guid>` and `AppRole` from `IdentityRole<Guid>`. It has no references to application, infrastructure, database-provider, or presentation projects.
- **Rich invariants:** Business rules are encapsulated within domain entities.
- **Encapsulation:** State mutations occur through explicit domain methods.

#### 2. Core Application (`Application`)

- **Role:** Orchestrates execution flows using CQRS and MediatR.
- **CQRS separation:** Read-side queries are isolated from write-side commands.
- **Pipeline behaviors:** Validation, logging, and performance tracking are handled through MediatR pipeline behaviors.
- **Infrastructure contracts:** External dependencies are represented by interfaces without concrete implementation details.

#### 3. Infrastructure (`Infrastructure`)

- **Role:** Manages persistence, data mappings, external systems, and cloud abstractions.
- **Persistence management:** Implements EF Core mappings, migrations, repositories, and transaction boundaries.
- **Pluggable architecture:** File storage, background processing, and notifications are implemented behind application interfaces.

#### 4. Presentation API (`Ecommerce.Api`)

- **Role:** Provides the HTTP entry point, routing, and request orchestration.
- **Thin endpoints:** Controllers delegate execution to the MediatR pipeline.
- **Cross-cutting HTTP concerns:** Handles exception middleware, identity, authorization, and API documentation.

---

## Vertical Slice Architecture

In addition to the horizontal Clean Architecture layers, the Application and API code are organized by **business capability** using Vertical Slice Architecture. Each slice owns the request flow for a specific use case instead of grouping every command, handler, validator, DTO, and mapping in separate global folders.

```text
Application/
└── Features/
    ├── Account/
    │   ├── Commands/
    │   │   ├── Login/
    │   │   ├── Register/
    │   │   └── RefreshToken/
    │   └── Queries/
    │       └── GetCurrent/
    ├── Products/
    │   ├── Commands/
    │   │   └── UpdateProduct/
    │   ├── Queries/
    │   │   ├── GetProducts/
    │   │   └── GetProductById/
    │   ├── Dtos/
    │   └── ProductMapping.cs
    ├── Categories/
    ├── Brands/
    └── Discounts/
```

### A Vertical Slice Request Flow

Each use case follows a focused path through the system:

```text
HTTP Request
    │
    ▼
Controller / Endpoint
    │
    ▼
Command or Query
    │
    ▼
Validator → MediatR Pipeline Behaviors
    │
    ▼
Handler
    │
    ├── Domain entities and business rules
    ├── Application abstractions
    └── Infrastructure implementations
    │
    ▼
HTTP Response / DTO
```

### Vertical Slice Design Rules

- **Feature ownership:** A feature owns the files required for its use cases, including commands, queries, handlers, validators, DTOs, and mappings.
- **Use-case focus:** Each command or query represents one business operation and has a single handler.
- **Thin presentation layer:** Controllers translate HTTP concerns and delegate execution to MediatR; business logic remains in the slice and domain.
- **Explicit dependencies:** Slices depend on Application abstractions rather than concrete infrastructure services.
- **Independent evolution:** Features can evolve independently without creating broad coupling between unrelated business capabilities.
- **Shared code discipline:** Shared abstractions are introduced only when behavior is genuinely common across multiple slices.
- **Read/write separation:** Queries are optimized for reading and projection, while commands enforce business rules and state changes.

### Clean Architecture and Vertical Slices Together

These approaches solve different architectural concerns:

| Concern | Approach |
|---|---|
| Dependency direction | Clean Architecture layers |
| Business boundaries | Domain-driven design and bounded capabilities |
| Request organization | Vertical slices by feature and use case |
| Read/write separation | CQRS with MediatR |
| External integrations | Application contracts with Infrastructure implementations |

The result is a codebase that is layered for dependency control but sliced vertically for discoverability, cohesion, and feature delivery.

---

## Project Structure

```text
Ecommerce.Api/
├── Application/
│   ├── Abstractions/
│   └── Features/
│       ├── Account/
│       ├── Brands/
│       ├── Categories/
│       ├── Discounts/
│       └── Products/
├── Domain/
├── Infrastructure/
├── Ecommerce.Api/
│   ├── Controllers/
│   └── Contracts/
├── Application.Test/
├── Domain.Test/
└── Infrastructure.Test/
```

## Domain Capabilities & Business Context

- **Catalog management:** Product hierarchies, brand mapping, variants, dynamic attributes, and media assets.
- **Orders and carts:** Persistent cart state, stock validation, discount calculations, purchase-time price snapshots, and order state transitions.
- **Inventory control:** Race-condition-resistant stock tracking to help prevent overselling during concurrent checkouts.
- **Customer and identity:** Customer profiles, shipping address hierarchies, and secure user identity boundaries.

## Project Status & Azure Roadmap

The platform is actively being developed and prepared for cloud-native deployment on Microsoft Azure.

| Local / development | Production target (Azure) |
|---|---|
| Local SQL Server | Azure SQL Database |
| Local file-system storage | Azure Blob Storage |
| Local background jobs | Azure App Service or containers |
| Local debugging | GitHub Actions CI/CD pipeline |

### Current Implementation State

- **Domain layer:** Core domain models, invariants, business constraints, and entity relationships are defined and stabilized.
- **Infrastructure layer:** Relational schema design, entity configurations, repository abstractions, and primary migrations are established. File storage is abstracted behind interfaces.
- **Application and API layers:** Handlers, command validators, and API endpoints are being built and refined.

### Planned Azure Integration

- **Azure Blob Storage:** Replace local media storage with Azure Blob Storage using secure SAS tokens and managed identities.
- **Azure SQL Database:** Provision managed SQL infrastructure with automated database migration steps.
- **CI/CD automation:** Use GitHub Actions for continuous build verification, testing, and deployment on branch merges.

---

## Software Engineering & Domain Constraints

1. **Strict dependency inversion:** The Domain project must not reference application, infrastructure, database-provider, or presentation projects. Its only framework dependency is the Identity package required by `AppUser` and `AppRole`; no other external dependencies are allowed.
2. **Explicit nullability and code quality:** Nullable reference types and repository `.editorconfig` rules are enforced across the solution.
3. **Immutable domain boundaries:** Entity IDs and core invariants are guarded against external modification.
4. **Decoupled infrastructure contracts:** Storage, payment, and notification services implement interfaces defined by the Application layer.

## Product Requirements

### Catalog & Inventory

- Products support dynamic key-value attributes such as RAM, storage, and color, together with media galleries.
- Stock reservation supports concurrent checkout scenarios.
- Product pricing remains traceable through order price snapshots and price history.

### Orders & Checkout

- Carts validate line-item stock levels before checkout state transitions.
- Orders create immutable order lines with product names and prices captured at purchase time.
- Orders follow a strict state machine: `Pending` → `Processing` → `Shipped` → `Delivered` or `Cancelled`.

### Media Assets

- Upload routines validate MIME types, optimize assets, and return secure URI references.
- Storage operations are decoupled through interfaces, allowing local development storage and Azure Blob Storage in production.

### Non-Functional Requirements

- **Maintainability:** Low coupling and high cohesion through Clean Architecture boundaries.
- **Testability:** Domain logic is unit-testable without database connections, Identity infrastructure, or external service connections. The Identity package remains a compile-time dependency for the identity entity base classes.
- **Traceability:** Strategic logging and domain state updates track critical operations throughout the application lifecycle.

## Setup & Running Locally

1. Clone the repository and navigate to the project directory:

   ```powershell
   cd C:\codes\Ecommerce.Api
   ```

2. Configure the database connection string in `appsettings.Development.json`.
3. Apply the EF Core database migrations:

   ```powershell
   dotnet ef database update --project Infrastructure --startup-project Ecommerce.Api
   ```

4. Build the solution:

   ```powershell
   dotnet build
   ```

5. Run the automated tests:

   ```powershell
   dotnet test
   ```

6. Run the API:

   ```powershell
   dotnet run --project Ecommerce.Api
   ```

7. Open `https://localhost:<port>/scalar` to view the API documentation. Use Scalar's **Authorize** control to enter a JWT access token as `Bearer <token>` when testing protected endpoints.

---

## Authentication & Authorization

The API supports mobile and web clients:

- **Mobile/native:** Uses JWT bearer tokens through `/api/account/login`.
- **Web:** `/api/account/login-web` returns a JWT for the `Authorization` header and sets an `HttpOnly` secure cookie for the refresh token.

## License

This repository is public for portfolio evaluation, code review, and educational purposes only. The source code is licensed under the [PolyForm NonCommercial License 1.0.0](./LICENSE.md), which prohibits commercial, business, or production use by third parties.

For commercial licensing, white-labeling, or other usage inquiries, contact **mustafamohamedanwar1@gmail.com**.
