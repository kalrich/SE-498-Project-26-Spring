# Marvel-ous Reads — Website Backend Specification

**Project:** SE-498 Capstone · Spring
**Stack:** ASP.NET Core (.NET 10) · PostgreSQL · Docker
**Last Updated:** 2026-03-16

---

## 1. Overview

The Website Backend is an intermediary service that sits between the user-facing frontend and the `Project498.WebApi` REST API. It handles browser sessions using HTTP Basic Authentication, executes server-side business logic (recommendation scoring, shelf constraint enforcement), and translates frontend requests into authenticated API calls.

### Responsibilities

- Authenticate users via HTTP Basic Auth and maintain session state
- Call the REST API on behalf of authenticated users using JWT bearer tokens
- Apply backend business logic before returning results to the frontend
- Maintain its own database for session management and cached user preferences
- Serve rendered HTML views or JSON to the frontend

### Architecture Position

```
Browser (Frontend)
     |
     | HTTP Basic Auth
     v
Website Backend  ←→  Web Server DB (Postgres)
     |
     | REST + JWT Bearer Token
     v
Project498.WebApi  ←→  API DB (Postgres)
```

---

## 2. Scope

**Included:**
- User registration and login using form-based credentials with bcrypt validation
- ASP.NET Core cookie authentication and session management
- Typed HTTP client (`ApiClient`) that forwards authenticated requests to the REST API
- JWT token caching and automatic refresh via `TokenCacheService`
- Server-side business logic: recommendation scoring, shelf constraint enforcement
- Razor Views rendered and served to the browser
- Web server's own PostgreSQL database for session and user data
- Unit tests for all business logic services

**Excluded:**
- Storing books, comics, or tags directly — all item data lives in the API database
- Exposing a public-facing REST API (this service is an internal backend, not a public service)
- Direct access to the REST API's database
- File uploads, image hosting, or media processing
- Password reset or OAuth/SSO (future phase)

---

## 3. Functional Requirements

1. The backend shall provide a registration form that collects username, email, and password, calls `POST /api/auth/register`, stores a bcrypt hash locally in `web_users`, and signs the user in on success.
2. The backend shall provide a login form that validates credentials against the local bcrypt hash, calls the API to obtain a JWT, caches it, and issues an ASP.NET Core auth cookie.
3. All routes requiring authentication shall be protected with `[Authorize]`; unauthenticated requests shall redirect to `/account/login?returnUrl=...`.
4. The backend shall proxy browse, search, and item detail requests to the REST API and pass the results to Razor Views as typed view models.
5. Before forwarding any shelf add or move request to the API, the backend shall enforce: an item cannot exist on both "To Read" and "Finished" simultaneously; moving to "Finished" auto-sets progress to 100%; progress must be between 0 and 100.
6. The `RecommendationService` shall fetch the user's shelved items from the API, compute a tag-overlap score for each unread catalog item, and return the top N results with matched tags included.
7. The backend shall automatically refresh the cached API JWT when it is expired or within 5 minutes of expiry, transparently to the user.
8. All user-facing errors (invalid login, shelf conflict, item not found) shall display a readable message using Bootstrap alerts — no raw exception messages shall be shown.

---

## 4. Non-Functional Requirements

1. **Performance:** Page loads shall complete in under 2 seconds on a broadband connection, including the round-trip to the REST API.
2. **Security:** Auth cookies shall be encrypted using ASP.NET Core Data Protection. Passwords shall be bcrypt-hashed with a minimum cost factor of 10. The API JWT secret shall never be exposed to the browser.
3. **Reliability:** API token refresh failures shall be handled gracefully — the user shall be redirected to login rather than seeing an unhandled exception.
4. **Usability:** Every error state (login failure, duplicate email, item already on shelf, empty recommendations) shall show a clear, human-readable message.
5. **Containerization:** The web server and its Postgres database shall start cleanly via `docker compose up` without manual setup steps.
6. **Testability:** `RecommendationService` and `ShelfConstraintService` shall be unit-testable without a running database or HTTP client.

---

## 5. Technology Stack

| Concern | Choice |
|---|---|
| Framework | ASP.NET Core 10 (MVC Controllers or Razor Pages) |
| ORM | Entity Framework Core with Npgsql |
| Database | PostgreSQL 16 (Docker container — separate from API DB) |
| Auth (inbound) | HTTP Basic Authentication |
| Auth (outbound) | JWT Bearer token (obtained from API login) |
| Testing | xUnit |
| Containerization | Docker + Docker Compose |

---

## 6. Database Schema (Web Server DB)

The web server maintains its own Postgres database for session state and user preferences. It does **not** duplicate the API's item/book/comic data.

### 3.1 `web_users`

Mirrors the API user record locally for session lookup without round-tripping to the API on every request.

| Column | Type | Notes |
|---|---|---|
| `id` | `SERIAL PRIMARY KEY` | |
| `api_user_id` | `INT NOT NULL UNIQUE` | Foreign key to API's `users.id` |
| `username` | `VARCHAR(100) NOT NULL` | |
| `email` | `VARCHAR(255) NOT NULL UNIQUE` | |
| `password_hash` | `TEXT NOT NULL` | bcrypt — used for Basic Auth validation |
| `api_token` | `TEXT` | Cached JWT for outbound API calls |
| `token_expires_at` | `TIMESTAMPTZ` | Used to detect expired tokens |
| `created_at` | `TIMESTAMPTZ DEFAULT NOW()` | |

### 3.2 `user_preferences`

Stores lightweight UI preferences that do not belong in the API layer.

| Column | Type | Notes |
|---|---|---|
| `id` | `SERIAL PRIMARY KEY` | |
| `web_user_id` | `INT` → `web_users.id` | |
| `preference_key` | `VARCHAR(100) NOT NULL` | e.g. `'default_shelf_view'` |
| `preference_value` | `TEXT` | |
| `updated_at` | `TIMESTAMPTZ DEFAULT NOW()` | |

---

## 7. Authentication Flow

### 4.1 Inbound — HTTP Basic Auth (Frontend → Backend)

All backend endpoints (except `/account/register` and `/account/login`) require an `Authorization: Basic <base64(username:password)>` header.

The backend:
1. Decodes the Base64 credentials
2. Looks up the user in `web_users` by username
3. Validates the password against the stored bcrypt hash
4. If valid, proceeds with the request under that user's identity

### 4.2 Outbound — JWT Bearer Token (Backend → API)

When the backend needs data from the API, it uses the cached `api_token` from `web_users`.

Token refresh logic:
- Before each API call, compare `token_expires_at` against `DateTime.UtcNow`
- If expired (or within 5 minutes of expiry), call `POST /api/auth/login` to get a fresh token
- Store the new token and updated expiry in `web_users`

### 4.3 Registration Flow

1. Frontend submits a registration form to `POST /account/register`
2. Backend calls `POST /api/auth/register` on the API → gets back the new user object
3. Backend hashes the password with bcrypt and stores the user in `web_users`
4. Backend calls `POST /api/auth/login` to obtain an initial JWT and caches it
5. Backend creates the three default shelves for the user (To Read, Reading, Finished) via the API
6. Returns a success response and sets a session cookie (or redirects to login)

---

## 8. Endpoints

The backend exposes HTTP endpoints consumed by the frontend (browser). These may return HTML (Razor Views) or JSON depending on whether the request sets `Accept: application/json`.

### 5.1 Account

| Method | Path | Description |
|---|---|---|
| `GET` | `/account/register` | Show registration form |
| `POST` | `/account/register` | Submit registration (calls API, stores locally) |
| `GET` | `/account/login` | Show login form |
| `POST` | `/account/login` | Validate Basic Auth, return session |
| `POST` | `/account/logout` | Clear session / cookie |

### 5.2 Home & Search

| Method | Path | Description |
|---|---|---|
| `GET` | `/` | Home page — featured items + quick search |
| `GET` | `/search?q=&type=` | Search results (proxied to `GET /api/books` + `GET /api/comics`) |

### 5.3 Items (Books + Comics)

| Method | Path | Description |
|---|---|---|
| `GET` | `/items/{id}` | Item detail page — fetches item from API + user's shelf status |
| `GET` | `/books` | Browse all books (proxied to `GET /api/books`) |
| `GET` | `/comics` | Browse all comics (proxied to `GET /api/comics`) |

### 5.4 Shelves

| Method | Path | Description |
|---|---|---|
| `GET` | `/shelves` | My Shelves overview — all shelves with item counts |
| `GET` | `/shelves/{shelfId}` | Shelf detail — all items on the shelf |
| `POST` | `/shelves/{shelfId}/items` | Add item to shelf (calls API, enforces constraints) |
| `PATCH` | `/shelves/{shelfId}/items/{itemId}` | Update progress/notes |
| `DELETE` | `/shelves/{shelfId}/items/{itemId}` | Remove item from shelf |

### 5.5 Recommendations

| Method | Path | Description |
|---|---|---|
| `GET` | `/recommendations` | Computed recommendations page |

---

## 9. Business Logic

### 6.1 Recommendation Scoring

The backend fetches items from the API and scores them against the authenticated user's shelved items using tag overlap. This logic lives in a dedicated `RecommendationService`.

**Algorithm:**
1. Fetch all items on the user's "Finished" and "Reading" shelves (via API)
2. Build a weighted tag frequency map — items on "Finished" count double
3. For each unread item in the catalog, compute a score:
   `score = sum(tagWeight[tag] for tag in item.tags) / item.tags.length`
4. Return top N items sorted by score descending

**Unit test coverage required:** scoring function, tie-breaking, empty shelf edge case.

### 6.2 Shelf Constraint Enforcement

Before forwarding an `add to shelf` request to the API, the backend validates:

- An item cannot appear on both "To Read" and "Finished" simultaneously
- Moving an item from "Reading" to "Finished" automatically sets `progressPercent` to 100
- `progressPercent` must be between 0 and 100

**Unit test coverage required:** each constraint rule.

### 6.3 Token Cache Management

The `TokenCacheService` manages API token lifecycle to avoid unnecessary login calls. Tokens are cached in the `web_users` table and refreshed proactively 5 minutes before expiry.

---

## 10. Spec — What Is Out of Scope

The backend does **not**:
- Store books, comics, tags, or shelves directly — all item data lives in the API DB
- Expose a public-facing REST API (it is an internal BFF, not a public service)
- Handle file uploads or image hosting

---

## 11. Unit Tests

Tests live in a `WebServer.Tests` project and run in CI on every PR and merge to `main`.

Test targets:

- `RecommendationService` — scoring algorithm with fixtures
- `ShelfConstraintService` — all three constraint rules
- `TokenCacheService` — token expiry detection and refresh trigger
- Auth middleware — valid credentials pass, invalid credentials return 401

---

## 12. Containerization

The backend runs in Docker alongside its own Postgres instance. It shares a Docker Compose network with the API service.

Key environment variables:

| Variable | Description |
|---|---|
| `ConnectionStrings__WebConnection` | Backend's Postgres connection string |
| `ApiBaseUrl` | Base URL of `Project498.WebApi` (e.g. `http://api:8080`) |
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production` |

---

## 13. Future Phases

- Replace Basic Auth with session cookies + OAuth (SSO)
- Add A/B test flag for two recommendation strategies (tag-overlap vs. collaborative filtering)
- Cache recommendation results in Redis to reduce API calls
- Add a `GET /health` endpoint for monitoring
