# Contributing to ElectroShop

Thank you for helping improve ElectroShop. This repository is a .NET 10 e-commerce backend organized around Clean Architecture, domain-driven design, CQRS with MediatR, and Vertical Slice Architecture.

This guide describes the contribution workflow and the project-specific standards expected for changes to the catalog, identity, cart, order, payment, inventory, procurement, review, and newsletter capabilities.

## Before You Start

1. Read the [README](../README.md) to understand the architecture, domain boundaries, local setup, and Azure roadmap.
2. Check existing [issues](../../issues) and pull requests before starting work.
3. For significant changes, open or discuss an issue first so the proposed domain and API behavior is understood before implementation.
4. Confirm that your contribution complies with the [PolyForm Noncommercial License 1.0.0](../LICENSE.md).

## Local Development

### Requirements

- .NET 10 SDK
- SQL Server or a compatible local SQL Server instance
- Redis for the configured cache integration
- Git

### Build, Test, and Run

From the repository root:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run --project Ecommerce.Api
```

Before running the API against a new or changed database schema, apply the EF Core migrations:

```powershell
dotnet ef database update --project Infrastructure --startup-project Ecommerce.Api
```

The API documentation is available at:

```text
https://localhost:<port>/scalar
```

Use Scalar's **Authorize** control with a JWT access token to test protected endpoints.

## Architecture Rules

### Clean Architecture Boundaries

- `Domain` contains entities, value objects, enums, and business invariants.
- `Application` contains use cases, CQRS commands and queries, validators, DTOs, pipeline behaviors, and abstractions.
- `Infrastructure` contains EF Core persistence, repositories, Identity implementation, external services, background jobs, and storage implementations.
- `Ecommerce.Api` contains controllers, HTTP contracts, middleware, authentication setup, and presentation concerns.
- Dependencies must point inward. Do not reference `Ecommerce.Api` or `Infrastructure` from `Application` or `Domain`.
- The Domain project has a deliberate Identity dependency for `AppUser : IdentityUser<Guid>` and `AppRole : IdentityRole<Guid>`. Do not add other external or infrastructure dependencies to Domain.

### Vertical Slice Organization

Place new use cases inside the relevant feature folder:

```text
Application/Features/<Feature>/
├── Commands/<UseCase>/
│   ├── <UseCase>Command.cs
│   ├── <UseCase>Handler.cs
│   └── <UseCase>Validator.cs
├── Queries/<UseCase>/
│   ├── <UseCase>Query.cs
│   ├── <UseCase>Handler.cs
│   └── <UseCase>Validator.cs
├── Dtos/
└── <Feature>Mapping.cs
```

Follow these rules:

- One command or query represents one focused use case.
- Keep the handler, validator, request, and use-case-specific models together.
- Put shared abstractions in `Application/Abstractions` only when they are genuinely shared.
- Keep controllers thin; they should translate HTTP concerns and delegate to MediatR.
- Keep business invariants in Domain entities rather than in controllers or infrastructure services.
- Use queries for reads and projections; use commands for state changes and business operations.
- Do not expose infrastructure types through API contracts or Application interfaces.

## Domain and API Expectations

- Preserve entity invariants and guarded state transitions.
- Keep order lines immutable and preserve purchase-time product and price snapshots.
- Treat inventory reservation and stock deduction as concurrency-sensitive operations.
- Make payment callbacks, retries, and externally triggered operations idempotent.
- Apply authorization policies appropriate to the role and business capability.
- Keep web refresh tokens in secure `HttpOnly` cookies and do not expose secrets in responses or logs.
- Update OpenAPI and Scalar-visible API behavior when adding or changing endpoints.
- Consider backward compatibility before changing existing routes, request contracts, response contracts, or order states.

## Testing Requirements

Put tests in the project that matches the code under test:

- `Domain.Test` for domain entities, invariants, value objects, and state transitions.
- `Application.Test` for handlers, validators, pipeline behaviors, and application use cases.
- `Infrastructure.Test` for persistence and infrastructure integrations.

Every behavior change should include or update tests where practical. At minimum, test:

- Valid and invalid input paths.
- Authorization and ownership rules.
- Boundary values and empty collections.
- Inventory concurrency, reservation release, and cancellation behavior.
- Payment success, failure, retry, and duplicate-callback behavior.
- Order state transition rules.

Run the complete test suite before opening a pull request:

```powershell
dotnet test
```

The CI workflow also runs restore, vulnerable-package checks, a Release build with warnings treated as errors, and tests on .NET 10.

## Database and Migration Changes

When changing entities, relationships, configurations, or persistence behavior:

1. Explain the schema impact in the pull request.
2. Add or update the appropriate EF Core migration when required.
3. Verify the migration against a local database.
4. Check that existing data remains safe and that rollback or recovery considerations are documented.
5. Do not commit connection strings, credentials, tokens, or production configuration.

## Commit and Branch Guidance

Use a short, descriptive branch name, for example:

- `feature/product-search`
- `fix/order-stock-reservation`
- `refactor/account-token-service`
- `docs/contributing-guide`

Use focused commits that describe the project behavior being changed. Keep unrelated formatting or refactoring out of feature commits.

## Pull Requests

Use the repository pull request template and include:

- A concise summary of the problem and solution.
- Related issue links, such as `Closes #123`.
- The affected layer, feature slice, and domain capability.
- API contract, authorization, database, migration, or configuration impact.
- Tests added or updated and the exact validation performed.
- Any known limitations or follow-up work.

Before requesting review, confirm:

- [ ] `dotnet build` succeeds without warnings.
- [ ] `dotnet test` passes.
- [ ] New use cases follow the existing vertical slice structure.
- [ ] Clean Architecture dependency direction is preserved.
- [ ] Domain invariants and authorization rules are covered.
- [ ] Database migrations are included or explicitly marked as unnecessary.
- [ ] No secrets or local configuration values are committed.
- [ ] README or PRD documentation is updated when behavior or architecture changes.

## Reporting Bugs and Requesting Features

Use the repository issue templates:

- **Bug reports:** Include the endpoint, HTTP method, request context, response code, runtime, database/tools, and reproducible steps.
- **Feature requests:** Describe the business problem, proposed solution, affected component or layer, API/use-case design, alternatives, and whether the change affects contracts, migrations, or tests.

Security vulnerabilities should not be disclosed in a public issue. Contact the project maintainer privately using the contact information provided in the README.

## License and Contributions

By contributing, you acknowledge that this project is licensed under the [PolyForm Noncommercial License 1.0.0](../LICENSE.md). Contributions must remain consistent with the license terms and the project's stated noncommercial scope.
