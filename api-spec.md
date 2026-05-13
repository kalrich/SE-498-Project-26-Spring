# Marvel•ous Reads — REST API Specification

**Project:** SE-498 Capstone · Spring  
**System:** `Project498.WebApi`  
**Stack:** ASP.NET Core · Entity Framework Core · PostgreSQL · Docker · Swagger/OpenAPI  
**Last Updated:** 2026-05-13

---

## 1. Overview

`Project498.WebApi` is the REST API and primary data service for **Marvel•ous Reads**. It stores and serves comic data, user account data, shelf/progress information, checkout records, Marvel character information, and supporting character images.

The API is consumed by the ASP.NET Core MVC WebServer frontend through typed HTTP client services. The WebServer handles browser-facing pages and user sessions, while the WebApi handles JSON data access and persistence through PostgreSQL.

The full endpoint-level request/response details are documented separately in:

> `docs/api-contracts.md`

---

## 2. Current API Responsibilities

The API currently supports the following application domains:

- **Authentication** — user signup and login
- **Comics** — comic browsing, filtering, series lookup, featured/trending/recommended collections
- **Shelves** — adding comics to user shelves and retrieving shelf contents
- **Reading Progress** — storing progress percentage and current page by user/comic
- **Checkouts** — creating checkout records, viewing user checkouts, and returning comics
- **Marvel Characters** — serving Marvel character profile data
- **Character Images** — serving character image records
- **Users** — user lookup by email and profile updates

The API no longer exposes the older planned `Books`, `Tags`, generic `Items`, or generic `ShelfItems` endpoints. Those were part of an earlier design and are out of scope for the current implementation.

---

## 3. System Architecture

```text
Browser
   |
   v
Project498.WebServer
ASP.NET Core MVC + Razor Views
Cookie/session-based frontend experience
Typed HttpClient services
   |
   v
Project498.WebApi
ASP.NET Core Web API
JWT bearer authentication support
Swagger/OpenAPI documentation
Entity Framework Core
   |
   v
PostgreSQL Database
Users, Comics, UserComics, Checkouts,
MarvelCharacters, CharacterImages
```

### WebServer-to-API Integration

The WebServer communicates with the WebApi through registered typed HTTP clients:

- `IAuthService` / `AuthApiService`
- `IComicService` / `ComicApiService`
- `ICheckoutService` / `CheckoutService`
- `IMarvelCharacterService` / `MarvelCharacterService`
- `ICharacterImageService` / `CharacterImageService`

The WebServer also registers a `BearerTokenHandler`, allowing API requests to include bearer tokens when required.

---

## 4. Scope

### Included

- ASP.NET Core Web API controllers
- Entity Framework Core data access
- PostgreSQL persistence
- Swagger/OpenAPI documentation
- JWT bearer authentication configuration
- CORS policy for local frontend/backend communication
- Comic browsing and recommendation-style collections
- User shelf and reading progress data
- Checkout and return workflow
- Marvel character and image data
- Dockerized API and database startup

### Excluded

- Generic book-management endpoints
- Generic tag-management endpoints
- Generic `items`, `books`, `tags`, `item_tags`, `shelves`, and `shelf_items` schema
- Frontend rendering or Razor Views
- Browser session/cookie management
- Password reset flow
- Admin content-management panel
- Payment or purchasing workflow
- File upload or PDF processing APIs

---

## 5. Functional Requirements

1. The API shall allow users to sign up and log in through authentication endpoints.
2. The API shall expose comic data through browse, detail, genre, featured, trending, recommended, because-you-read, hidden-gem, and series endpoints.
3. The API shall allow a user's comics to be organized into shelves using username, comic ID, and shelf name.
4. The API shall store user reading progress using progress percentage and current page.
5. The API shall support comic checkout records with checkout date, due date, optional return date, and status.
6. The API shall allow checkout records to be retrieved by checkout ID or by user ID.
7. The API shall allow checkout records to be marked as returned.
8. The API shall expose Marvel character data through list and detail endpoints.
9. The API shall expose character image records through a read-only endpoint.
10. The API shall allow user profile lookup by email and updates by user ID.
11. The API shall document its available endpoints through Swagger UI.

---

## 6. Non-Functional Requirements

1. **Reliability:** The API and PostgreSQL database shall start through Docker Compose without requiring manual database creation.
2. **Maintainability:** API controllers, models, and EF Core database configuration shall remain separated by responsibility.
3. **Security:** JWT bearer authentication shall be configured in the WebApi, and protected endpoints may require an `Authorization: Bearer <token>` header.
4. **Data Integrity:** Foreign key relationships shall connect users and comics through `UserComics` and `Checkouts`.
5. **Usability for Developers:** Swagger UI shall be available in development for testing and documentation.
6. **Frontend Compatibility:** CORS shall allow the frontend origin used during local development.
7. **Portability:** The API shall use environment-based connection strings so it can run locally or inside Docker.

---

## 7. Technology Stack

| Concern | Current Choice |
|---|---|
| API Framework | ASP.NET Core Web API |
| Data Access | Entity Framework Core |
| Database Provider | Npgsql |
| Database | PostgreSQL 16 |
| Authentication | JWT Bearer configuration |
| API Documentation | Swagger/OpenAPI |
| Containerization | Docker + Docker Compose |
| Testing | xUnit project in repository |

---

## 8. Authentication and Authorization

The WebApi is configured with JWT bearer authentication using:

- Issuer validation
- Audience validation
- Lifetime validation
- Issuer signing key validation

The API reads JWT settings from configuration:

- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`

Swagger includes a bearer authorization dialog so developers can test protected endpoints by supplying a token.

The WebServer uses cookie authentication for the browser-facing experience and forwards API requests through service classes. When required, bearer tokens can be attached through the `BearerTokenHandler`.

---

## 9. CORS Configuration

The API defines a local development CORS policy named `AllowFrontend`.

The policy currently allows the frontend origin:

```text
http://localhost:8082
```

The policy allows any header and any method for local development.

---

## 10. Database Schema Summary

The WebApi uses a PostgreSQL database managed through Entity Framework Core.

Current tables:

- `Users`
- `Comics`
- `UserComics`
- `Checkouts`
- `MarvelCharacters`
- `CharacterImages`

### Users

Stores registered user account data.

Key fields:

- `Id`
- `Username`
- `Email`
- `Password`

### Comics

Stores comic metadata and file paths.

Key fields:

- `Id`
- `Title`
- `Author`
- `Genre`
- `SecondaryGenre`
- `Description`
- `CoverImagePath`
- `PdfPath`
- `IsIReadPick`
- `SeriesName`
- `VolumeNumber`
- `IssueNumber`

### UserComics

Join table connecting users to comics while storing shelf and reading-progress information.

Key fields:

- `Id`
- `UserId`
- `ComicId`
- `Shelf`
- `ProgressPercent`
- `CurrentPage`

Relationships:

- Many `UserComics` records belong to one `User`
- Many `UserComics` records belong to one `Comic`

### Checkouts

Stores checkout and return workflow data.

Key fields:

- `Id`
- `UserId`
- `ComicId`
- `CheckoutDate`
- `DueDate`
- `ReturnDate`
- `Status`

Relationships:

- Many `Checkouts` records belong to one `User`
- Many `Checkouts` records belong to one `Comic`

### MarvelCharacters

Stores Marvel character profile data.

Key fields:

- `Id`
- `Name`
- `Alias`
- `Description`
- `ImagePath`

### CharacterImages

Stores additional character image records by alias.

Key fields:

- `Id`
- `Alias`
- `ImagePath`

For the complete schema and ERD, see:

> `docs/database-schema.md`

---

## 11. API Surface Summary

Detailed request and response contracts live in `docs/api-contracts.md`.

Current endpoint groups include:

### Authentication

- `POST /api/Auth/login`
- `POST /api/Auth/signup`

### Comics

- `GET /api/Comics`
- `GET /api/Comics/{id}`
- `GET /api/Comics/genres`
- `GET /api/Comics/featured`
- `GET /api/Comics/trending`
- `GET /api/Comics/recommended`
- `GET /api/Comics/because-you-read`
- `GET /api/Comics/hidden-gems`
- `GET /api/Comics/series/{seriesName}`

### Shelves

- `GET /api/Shelves/{username}/{shelf}`
- `POST /api/Shelves/add`
- `GET /api/Shelves/progress/{username}/{comicId}`
- `PATCH /api/Shelves/update-progress`

### Checkouts

- `POST /api/Checkouts`
- `GET /api/Checkouts/{id}`
- `GET /api/Checkouts/user/{userId}`
- `PUT /api/Checkouts/{id}/return`

### Marvel Characters

- `GET /api/marvel-characters`
- `GET /api/marvel-characters/{id}`

### Character Images

- `GET /api/character-images`

### Users

- `GET /api/Users/by-email`
- `PUT /api/Users/{id}`

---

## 12. Swagger / OpenAPI

Swagger UI is enabled in development mode.

Default local Swagger URL when running through Docker:

```text
http://localhost:8082/swagger
```

Swagger provides:

- Endpoint list
- Request body schemas
- Response schemas
- Path/query parameters
- Bearer token authorization dialog

The Swagger output is the source of truth for endpoint-level request/response details.

---

## 13. Containerization

The API and database run through Docker Compose.

### Services

| Service | Purpose |
|---|---|
| `project498.webapi` | Builds and runs the ASP.NET Core WebApi |
| `db` | Runs PostgreSQL 16 |

### WebApi Service

The WebApi container uses:

```text
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://0.0.0.0:8080
ConnectionStrings__DefaultConnection=Host=db;Database=project498;Username=postgres;Password=postgres
```

The container maps:

```text
localhost:8082 -> container:8080
```

### Database Service

The PostgreSQL container uses:

```text
POSTGRES_DB=project498
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
```

The database includes a healthcheck using `pg_isready`.

---

## 14. Configuration

Important configuration values:

| Setting | Purpose |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `Jwt:Key` | JWT signing key |
| `Jwt:Issuer` | Expected token issuer |
| `Jwt:Audience` | Expected token audience |
| `ASPNETCORE_ENVIRONMENT` | Controls environment-specific behavior |
| `ASPNETCORE_URLS` | Configures container listening URL |

---

## 15. Testing

The repository includes an API test project. Testing should focus on:

- Controller behavior
- Authentication request behavior
- Shelf/progress business logic
- Checkout creation and return behavior
- Database schema and EF Core mappings
- Serialization/deserialization of API DTOs

Tests should avoid depending on manually created local databases whenever possible.

---

## 16. Out-of-Scope / Deprecated Design Notes

The current API specification replaces earlier planned architecture that included:

- `Books`
- `Tags`
- `Items`
- `ItemTags`
- `ShelfItems`
- A generic `/api/recommendations` endpoint
- A full tag-overlap recommendation engine inside the API
- A generic book/comic polymorphic item model

Those concepts were removed or deferred. The current implementation is comic-focused and uses `Comics`, `UserComics`, `Checkouts`, `MarvelCharacters`, and `CharacterImages`.

---

## 17. Future Improvements

Potential future improvements include:

- Add explicit health check endpoint for API/database readiness
- Add admin endpoints for comic and character management
- Add stronger DTO separation so password fields are never returned in response examples
- Add response documentation for non-200 errors
- Add pagination for large comic and character lists
- Add richer recommendation logic based on shelf and reading-history data
- Add automated database migration/seed setup during Docker startup
