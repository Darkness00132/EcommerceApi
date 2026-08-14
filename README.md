# Ecommerce API

A comprehensive, clean architecture-based .NET backend for an e-commerce platform.

## Architecture Highlights
This solution follows Domain-Driven Design (DDD) and Clean Architecture principles:

- **Domain**: Contains the core business models and entities (e.g., `Category`, `Product`, `Order`, `AppUser`).
- **Application**: Contains the business logic, CQRS commands/queries using MediatR, and abstraction interfaces for services and repositories. It is strictly agnostic of presentation-layer concerns (like Cookies).
- **Infrastructure**: Implementations of abstractions (Database contexts, Repositories, Identity, Email Services, etc.).
- **Ecommerce.Api**: The presentation layer. It houses the Controllers, handles Web/Mobile DTO mappings, and deals with HTTP specific constructs (like setting the HttpOnly RefreshToken cookie).

## Project Structure
- `Ecommerce.Api/` - The entry point and Web API project.
- `Application/` - MediatR handlers, interfaces, and DTOs.
- `Domain/` - Core entities, enums, and domain logic.
- `Infrastructure/` - Persistence (EF Core) and external services.

## Setup & Running Locally

1. **Clone the repository** and navigate to the project directory:
   ```bash
   cd C:\codes\Ecommerce.Api
   ```

2. **Configure Database**:
   Ensure your database connection string is properly set in `appsettings.Development.json`.

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the API**:
   ```bash
   dotnet run --project Ecommerce.Api
   ```

5. **API Documentation**:
   Navigate to `https://localhost:<port>/swagger` in your browser to view the Swagger UI. Note: XML documentation comments are explicitly configured for the API project to enhance the Swagger documentation.

## Authentication & Authorization
The API supports both Mobile and Web clients:
- **Mobile/Native**: Uses pure JWT Bearer tokens via `/api/account/login`.
- **Web**: Uses `/api/account/login-web` which returns a JWT for the `Authorization` header and sets an `HttpOnly` secure cookie for the refresh token to mitigate XSS attacks.
