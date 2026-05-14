# Marvel•ous Reads — REST API Specification

**Project:** SE-498 Capstone · Spring  
**System:** `Project498.WebApi`  
**Stack:** ASP.NET Core Web API · Entity Framework Core · PostgreSQL · Docker · Swagger/OpenAPI  
**Last Updated:** 2026-05-13

---

## 1. Overview

`Project498.WebApi` is the REST API and persistent data layer for Marvel•ous Reads. It exposes endpoints for authentication, comic browsing, shelves/progress, checkout workflows, Marvel characters, character images, and user profile management.

The current API is documented through Swagger UI and backed by PostgreSQL through Entity Framework Core.

---

## 2. Current API Domains

- **Auth** — login and signup
- **Comics** — comic catalog, search, genres, featured/trending/recommended lists, series lookup
- **Shelves** — add comics to shelves and track reading progress
- **Checkouts** — create, retrieve, filter, and return comic checkouts
- **Marvel Characters** — API-backed Marvel character data
- **Character Images** — character image records
- **Users** — lookup/update user profile data

---

## 3. Scope

### Included

- ASP.NET Core controller-based REST API
- Swagger/OpenAPI documentation
- JWT bearer authentication configuration
- PostgreSQL database using EF Core/Npgsql
- Docker Compose support for API + database
- CORS configuration for the frontend origin
- API endpoints matching current Swagger output

### Excluded / Not Current

- Books endpoints
- Tags endpoints
- Generic item base table
- Generic shelf item endpoints using shelf IDs
- Health endpoint in current Swagger
- Public admin/content management API
- Full CRUD for comics through exposed API endpoints

---

## 4. Functional Requirements

1. The API shall allow users to sign up with username, email, and password.
2. The API shall allow users to log in with email and password.
3. The API shall expose Swagger documentation.
4. The API shall expose comic catalog endpoints with optional search and genre filtering.
5. The API shall expose comic detail lookup by ID.
6. The API shall expose genre, featured, trending, recommended, because-you-read, hidden-gems, and series comic endpoints.
7. The API shall expose shelf endpoints for retrieving a user's shelf, adding a comic to a shelf, retrieving progress, and updating progress.
8. The API shall expose checkout endpoints for creating, retrieving, filtering, and returning checkouts.
9. The API shall expose Marvel character and character image endpoints.
10. The API shall expose user lookup by email and profile update by user ID.
11. The API shall persist data in PostgreSQL through Entity Framework Core.

---

## 5. Non-Functional Requirements

1. **Security:** JWT bearer authentication is configured in the API and Swagger includes a Bearer authorization dialog.
2. **Reliability:** The API and database should start through Docker Compose with Postgres health checks.
3. **Maintainability:** Data access should be centralized through `AppDbContext`.
4. **Interoperability:** The API should return JSON request/response bodies using camelCase-compatible model shapes.
5. **Developer Experience:** Swagger should be available in development for endpoint inspection and testing.
6. **Configurability:** Database and JWT settings should come from application configuration/environment variables.

---

## 6. Technology Stack

| Concern | Current Choice |
|---|---|
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core |
| Database provider | Npgsql |
| Database | PostgreSQL 16 |
| API docs | Swagger/OpenAPI |
| Auth | JWT Bearer configuration |
| Containerization | Docker Compose |
| Tests | xUnit where applicable |

---

## 7. API Program Configuration

The API currently configures:

- Controllers
- Endpoint API explorer
- Swagger generation
- Swagger Bearer security definition
- JWT bearer authentication
- CORS policy named `AllowFrontend`
- EF Core `AppDbContext` with Npgsql
- Controller route mapping

### CORS

The API allows the frontend origin:

```text
http://localhost:8082
```

---

## 8. Database Schema

The API database includes the following tables:

- `Users`
- `Comics`
- `UserComics`
- `Checkouts`
- `MarvelCharacters`
- `CharacterImages`

### Key Relationships

```text
Users 1 --- many UserComics
Comics 1 --- many UserComics

Users 1 --- many Checkouts
Comics 1 --- many Checkouts
```

`MarvelCharacters` and `CharacterImages` are currently standalone content tables.

---

## 9. Current REST Endpoints

### Auth

| Method | Path | Description |
|---|---|---|
| `POST` | `/api/Auth/login` | Authenticates a user |
| `POST` | `/api/Auth/signup` | Creates a new user account |

### Character Images

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/character-images` | Returns character image records |

### Checkouts

| Method | Path | Description |
|---|---|---|
| `POST` | `/api/Checkouts` | Creates a checkout for a user and comic |
| `GET` | `/api/Checkouts/{id}` | Returns one checkout by ID |
| `GET` | `/api/Checkouts/user/{userId}` | Returns checkout records for a user |
| `PUT` | `/api/Checkouts/{id}/return` | Marks a checkout as returned |

### Comics

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/Comics` | Returns comics, optionally filtered by query and genre |
| `GET` | `/api/Comics/{id}` | Returns one comic by ID |
| `GET` | `/api/Comics/genres` | Returns available comic genres |
| `GET` | `/api/Comics/featured` | Returns featured comics |
| `GET` | `/api/Comics/trending` | Returns trending comics |
| `GET` | `/api/Comics/recommended` | Returns recommended comics |
| `GET` | `/api/Comics/because-you-read` | Returns comics based on previous reading activity |
| `GET` | `/api/Comics/hidden-gems` | Returns hidden gem comics |
| `GET` | `/api/Comics/series/{seriesName}` | Returns comics from a series |

### Marvel Characters

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/marvel-characters` | Returns all Marvel characters |
| `GET` | `/api/marvel-characters/{id}` | Returns one Marvel character by ID |

### Shelves

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/Shelves/{username}/{shelf}` | Returns comics in a user's shelf |
| `POST` | `/api/Shelves/add` | Adds a comic to a user's shelf |
| `GET` | `/api/Shelves/progress/{username}/{comicId}` | Returns reading progress for a comic |
| `PATCH` | `/api/Shelves/update-progress` | Updates reading progress |

### Users

| Method | Path | Description |
|---|---|---|
| `GET` | `/api/Users/by-email` | Returns a user by email |
| `PUT` | `/api/Users/{id}` | Updates a user's profile |

---

## 10. Request/Response Models

Current Swagger schemas include:

- `AddToShelfRequest`
- `AuthResponse`
- `CharacterImage`
- `CheckoutResponse`
- `Comic`
- `CreateCheckoutRequest`
- `LoginRequest`
- `MarvelCharacter`
- `ReadingProgressResponse`
- `SignupRequest`
- `UpdateProfileRequest`
- `UpdateProgressRequest`
- `User`

---

## 11. Authentication

The API configures JWT bearer authentication with:

- Issuer validation
- Audience validation
- Lifetime validation
- Signing key validation
- Swagger Bearer token support

Swagger includes the `Authorize` button. Individual endpoint authorization behavior should be verified against controller attributes.

---

## 12. Containerization

The current Docker Compose setup runs:

| Service | Purpose |
|---|---|
| `project498.webapi` | ASP.NET Core Web API |
| `db` | PostgreSQL 16 database |

### Web API Environment

```text
ConnectionStrings__DefaultConnection=Host=db;Database=project498;Username=postgres;Password=postgres
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://0.0.0.0:8080
```

### Ports

| Service | Host Port | Container Port |
|---|---:|---:|
| Web API | `8082` | `8080` |
| PostgreSQL | `5432` | `5432` |

Postgres uses a named Docker volume:

```text
postgres-data
```

---

## 13. Swagger

Swagger UI is available in development at:

```text
/swagger
```

The Swagger UI is used as the source of truth for current endpoint names, HTTP methods, request bodies, and response models.

---

## 14. Future Improvements

- Add a formal health endpoint if needed for deployment checks
- Add explicit `[Authorize]` attributes where required
- Add clearer response status codes beyond `200 OK`
- Remove password fields from response DTOs where possible
- Add integration tests against Postgres/Testcontainers
- Add admin-only comic management endpoints if content editing becomes required
