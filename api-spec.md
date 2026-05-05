# Marvel-ous Reads — REST API Specification

**Project:** SE-498 Capstone · Spring
**Stack:** ASP.NET Core (.NET 10) · PostgreSQL · Docker · Swagger/OpenAPI
**Last Updated:** 2026-03-16

---

## 1. Overview

The `Project498.WebApi` project is the central data layer for Marvel-ous Reads. It exposes a RESTful HTTP API that is consumed by the Website Backend. All access is protected by a JWT bearer token, and the full API surface is documented via a Swagger UI.

The API manages three content domains:

- **Books** — novels, audiobooks, and other book-format entries
- **Comics** — Marvel comics, issues, and story arcs
- **Shelves** — user-owned reading lists with per-item progress tracking
- **Tags** — shared vocabulary that drives cross-domain recommendations

---

## 2. Scope

**Included:**
- RESTful CRUD endpoints for Books, Comics, Tags, Shelves, and Shelf Items
- User registration and login with JWT bearer token issuance
- Tag-based recommendation endpoint
- Bearer token authentication on all protected routes
- OpenAPI/Swagger documentation
- PostgreSQL database running in a Docker container
- Unit tests for request validation and schema constraints
- Health check endpoint (`GET /health`)

**Excluded:**
- Frontend rendering or HTML generation (handled by the Website Backend)
- Session or cookie management (handled by the Website Backend)
- Recommendation scoring logic (computed in the Website Backend's `RecommendationService`)
- Integration with another team's API (future phase)
- Password reset or multi-factor authentication

---

## 3. Functional Requirements

1. The API shall provide a registration endpoint that accepts a username, email, and password, validates uniqueness, stores a bcrypt-hashed password, and returns the created user object.
2. The API shall provide a login endpoint that validates credentials and returns a signed JWT on success, or a 401 error on failure.
3. All endpoints except `/api/auth/register` and `/api/auth/login` shall require a valid JWT bearer token; requests without a valid token shall receive a 401 response.
4. The API shall expose CRUD endpoints for Books (`GET /api/books`, `GET /api/books/{id}`, `POST /api/books`) and Comics with equivalent routes.
5. The API shall expose Tag endpoints (`GET /api/tags`, `POST /api/tags`) for managing the shared recommendation vocabulary.
6. The API shall expose Shelf endpoints allowing authenticated users to list their shelves, add items, update progress, move items, and remove items.
7. The API shall expose a `GET /api/recommendations` endpoint that returns a ranked list of items with matched tags for the authenticated user.
8. All error responses shall follow a consistent JSON shape: `{ "error": string, "details": string[], "statusCode": int }`.
9. The Swagger UI shall be accessible at `/swagger` and shall include the bearer token authorization dialog.

---

## 4. Non-Functional Requirements

1. **Performance:** All CRUD endpoints shall respond in under 500ms under normal load. The recommendations endpoint shall respond in under 1 second.
2. **Security:** Passwords shall be hashed using bcrypt with a minimum cost factor of 10. JWTs shall be signed with HS256 and expire after 24 hours. The JWT secret shall never be committed to source control — it shall be provided via environment variable.
3. **Reliability:** The API shall start cleanly via `docker compose up` without manual database setup. The health check endpoint shall return `200 OK` when the DB connection is healthy.
4. **Scalability:** The database schema shall use indexed foreign keys on `user_id`, `shelf_id`, and `item_id` columns to support growth.
5. **Containerization:** The API shall build and run entirely inside Docker. No local .NET installation shall be required to run the project.
6. **Testability:** All business logic shall be unit-testable without a running database. Tests shall run in under 30 seconds in CI.

---

## 5. Technology Stack

| Concern | Choice |
|---|---|
| Framework | ASP.NET Core 10 (Minimal + Controllers) |
| ORM | Entity Framework Core with Npgsql |
| Database | PostgreSQL 16 (Docker container) |
| Auth | JWT Bearer tokens (System.IdentityModel.Tokens.Jwt) |
| API docs | Built-in OpenAPI (`app.MapOpenApi()`) + Swagger UI |
| Testing | xUnit + in-memory or test-container DB |
| Containerization | Docker + Docker Compose |

---

## 6. Database Schema

> Full schema with all relationships and seed data: [`docs/database-schema.md`](database-schema.md)

### `users`

| Column | Type | Notes |
|---|---|---|
| `id` | `SERIAL PRIMARY KEY` | |
| `username` | `VARCHAR(100) NOT NULL UNIQUE` | |
| `email` | `VARCHAR(255) NOT NULL UNIQUE` | |
| `password_hash` | `TEXT NOT NULL` | bcrypt hash (cost ≥ 10) |
| `created_at` | `TIMESTAMPTZ DEFAULT NOW()` | |
| `updated_at` | `TIMESTAMPTZ DEFAULT NOW()` | |

### `items` (base table)

| Column | Type | Notes |
|---|---|---|
| `id` | `SERIAL PRIMARY KEY` | |
| `title` | `VARCHAR(255) NOT NULL` | |
| `description` | `TEXT` | |
| `item_type` | `VARCHAR(20) NOT NULL` | `'book'` or `'comic'` |
| `author_creator` | `VARCHAR(255)` | |
| `published_date` | `DATE` | |
| `cover_image_url` | `TEXT` | |
| `average_rating` | `DECIMAL(3,2)` | |
| `featured` | `BOOLEAN DEFAULT FALSE` | Home page "Featured Today" flag |
| `view_count` | `INT DEFAULT 0` | Used for "Trending" sort |
| `created_at` | `TIMESTAMPTZ DEFAULT NOW()` | |
| `updated_at` | `TIMESTAMPTZ DEFAULT NOW()` | |

### `books`

| Column | Type | Notes |
|---|---|---|
| `item_id` | `INT PRIMARY KEY` → `items.id` | |
| `isbn` | `VARCHAR(20)` | |
| `page_count` | `INT` | |
| `publisher` | `VARCHAR(255)` | |
| `format` | `VARCHAR(50)` | `'paperback'`, `'hardcover'`, `'ebook'`, `'audiobook'` |

### `comics`

| Column | Type | Notes |
|---|---|---|
| `item_id` | `INT PRIMARY KEY` → `items.id` | |
| `marvel_character` | `VARCHAR(255)` | Primary character |
| `story_arc` | `VARCHAR(255)` | e.g. `'Civil War'` |
| `issue_number` | `INT` | |
| `series_name` | `VARCHAR(255)` | |
| `universe` | `VARCHAR(50)` | e.g. `'Earth-616'` |
| `publisher` | `VARCHAR(100) DEFAULT 'Marvel'` | |

### `tags`

| Column | Type | Notes |
|---|---|---|
| `id` | `SERIAL PRIMARY KEY` | |
| `name` | `VARCHAR(100) NOT NULL UNIQUE` | e.g. `'Action'`, `'found family'` |
| `tag_category` | `VARCHAR(50) NOT NULL DEFAULT 'theme'` | `'genre'` (sidebar filter) or `'theme'` (recommendation scoring) |
| `description` | `TEXT` | |

### `item_tags`

| Column | Type | Notes |
|---|---|---|
| `item_id` | `INT` → `items.id` | Composite PK |
| `tag_id` | `INT` → `tags.id` | Composite PK |

### `shelves`

| Column | Type | Notes |
|---|---|---|
| `id` | `SERIAL PRIMARY KEY` | |
| `user_id` | `INT` → `users.id` | |
| `name` | `VARCHAR(100) NOT NULL` | Display label |
| `shelf_type` | `VARCHAR(50)` | `'reading'`, `'want_to_read'`, `'finished'`, `'favourites'`, `'dropped'`, `'custom'` |
| `created_at` | `TIMESTAMPTZ DEFAULT NOW()` | |

Default shelves created on registration: `reading`, `want_to_read`, `finished`, `favourites`

### `shelf_items`

| Column | Type | Notes |
|---|---|---|
| `id` | `SERIAL PRIMARY KEY` | |
| `shelf_id` | `INT` → `shelves.id` | |
| `item_id` | `INT` → `items.id` | |
| `progress_percent` | `INT DEFAULT 0` | 0–100 |
| `date_added` | `TIMESTAMPTZ DEFAULT NOW()` | |
| `date_started` | `TIMESTAMPTZ` | Set on first move to `'reading'` |
| `date_finished` | `TIMESTAMPTZ` | Set on move to `'finished'` or progress = 100 |
| `notes` | `TEXT` | Personal reading notes |

Unique constraint: `UNIQUE(shelf_id, item_id)`

---

## 7. Authentication

All endpoints except `/api/auth/register` and `/api/auth/login` require a valid JWT in the `Authorization: Bearer <token>` header.

- Token format: JWT (HS256)
- Expiry: 24 hours
- Payload claims: `sub` (userId), `username`, `email`, `iat`, `exp`

---

## 8. REST Endpoints

### 5.1 Auth

#### `POST /api/auth/register`

Register a new user.

**Request body:**
```json
{
  "username": "kalin",
  "email": "kalin@example.com",
  "password": "securePassword123"
}
```

**Response `201 Created`:**
```json
{
  "id": 1,
  "username": "kalin",
  "email": "kalin@example.com",
  "createdAt": "2026-03-16T00:00:00Z"
}
```

**Errors:** `400` if email or username already taken; `400` if validation fails.

---

#### `POST /api/auth/login`

Authenticate and receive a bearer token.

**Request body:**
```json
{
  "email": "kalin@example.com",
  "password": "securePassword123"
}
```

**Response `200 OK`:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-03-17T00:00:00Z",
  "user": {
    "id": 1,
    "username": "kalin",
    "email": "kalin@example.com"
  }
}
```

**Errors:** `401` if credentials invalid.

---

### 5.2 Books

All endpoints require `Authorization: Bearer <token>`.

#### `GET /api/books`

List all books. Supports optional query params: `?tag=mythology&search=dune`.

**Response `200 OK`:**
```json
[
  {
    "id": 1,
    "title": "Dune",
    "description": "...",
    "authorCreator": "Frank Herbert",
    "publishedDate": "1965-08-01",
    "coverImageUrl": "https://...",
    "averageRating": 4.7,
    "isbn": "978-0-441-17271-9",
    "pageCount": 412,
    "publisher": "Chilton Books",
    "format": "paperback",
    "tags": ["space opera", "found family"]
  }
]
```

#### `GET /api/books/{id}`

Get a single book by ID. Returns `404` if not found.

#### `POST /api/books`

Create a new book entry.

**Request body:**
```json
{
  "title": "Dune",
  "description": "...",
  "authorCreator": "Frank Herbert",
  "publishedDate": "1965-08-01",
  "coverImageUrl": "https://...",
  "isbn": "978-0-441-17271-9",
  "pageCount": 412,
  "publisher": "Chilton Books",
  "format": "paperback",
  "tagIds": [1, 3]
}
```

**Response `201 Created`** with the created book object.
**Errors:** `400` if required fields missing or validation fails.

#### `PUT /api/books/{id}`

Update an existing book. Body same shape as POST. Returns `204 No Content`.

#### `DELETE /api/books/{id}`

Delete a book. Returns `204 No Content` or `404`.

---

### 5.3 Comics

Same CRUD shape as Books, at `/api/comics`.

#### `GET /api/comics` · `GET /api/comics/{id}` · `POST /api/comics` · `PUT /api/comics/{id}` · `DELETE /api/comics/{id}`

Comic-specific fields in POST/PUT body:

```json
{
  "title": "Amazing Spider-Man #1",
  "authorCreator": "Stan Lee",
  "publishedDate": "1963-03-01",
  "marvelCharacter": "Spider-Man",
  "storyArc": "The Coming of the Chameleon",
  "issueNumber": 1,
  "seriesName": "Amazing Spider-Man",
  "universe": "Earth-616",
  "publisher": "Marvel",
  "tagIds": [2]
}
```

---

### 5.4 Tags

#### `GET /api/tags`

List all tags.

**Response `200 OK`:**
```json
[
  { "id": 1, "name": "found family", "description": "..." },
  { "id": 2, "name": "mythology", "description": "..." }
]
```

#### `GET /api/tags/{id}`

Get a single tag.

#### `POST /api/tags`

Create a tag.

```json
{ "name": "space opera", "description": "Epic science fiction spanning star systems" }
```

**Response `201 Created`.**

---

### 5.5 Shelves

#### `GET /api/shelves`

Get all shelves for the authenticated user.

**Response `200 OK`:**
```json
[
  { "id": 1, "name": "To Read", "shelfType": "to_read", "itemCount": 5 },
  { "id": 2, "name": "Finished", "shelfType": "finished", "itemCount": 12 }
]
```

#### `POST /api/shelves`

Create a new shelf.

```json
{ "name": "Favorites", "shelfType": "custom" }
```

**Response `201 Created`.**

#### `GET /api/shelves/{shelfId}/items`

Get all items on a shelf.

**Response `200 OK`:**
```json
[
  {
    "id": 1,
    "shelfId": 1,
    "item": { "id": 5, "title": "Dune", "itemType": "book", ... },
    "dateAdded": "2026-03-01T00:00:00Z",
    "progressPercent": 45,
    "notes": "Great read so far"
  }
]
```

#### `POST /api/shelves/{shelfId}/items`

Add an item to a shelf.

```json
{ "itemId": 5, "progressPercent": 0, "notes": "" }
```

**Response `201 Created`.**
**Errors:** `409 Conflict` if the item is already on that shelf.

#### `PATCH /api/shelves/{shelfId}/items/{itemId}`

Update progress or notes for an item on a shelf.

```json
{ "progressPercent": 75, "notes": "Just got to the good part" }
```

**Response `200 OK`** with the updated shelf item.

#### `DELETE /api/shelves/{shelfId}/items/{itemId}`

Remove an item from a shelf. Returns `204 No Content`.

---

### 5.6 Recommendations

#### `GET /api/recommendations`

Return recommended items for the authenticated user based on tag overlap with items on their shelves.

**Query params:** `?limit=10` (default 10)

**Response `200 OK`:**
```json
[
  {
    "item": { "id": 8, "title": "Ender's Game", "itemType": "book", ... },
    "matchedTags": ["space opera", "found family"],
    "score": 0.85
  }
]
```

---

### 5.7 Health

#### `GET /health`

Returns API health status (no auth required). Used for monitoring and CI readiness checks.

**Response `200 OK`:**
```json
{ "status": "healthy", "timestamp": "2026-03-16T00:00:00Z" }
```

---

## 9. Error Response Format

All errors follow a consistent shape:

```json
{
  "error": "Validation failed",
  "details": ["title is required", "isbn must be 10 or 13 characters"],
  "statusCode": 400
}
```

---

## 10. Containerization

The API builds and runs inside Docker via the existing `compose.yaml`. Key environment variables:

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | Postgres connection string |
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production` |
| `Jwt__SecretKey` | Secret for JWT signing |
| `Jwt__Issuer` | Token issuer string |
| `Jwt__ExpiryHours` | Token lifetime in hours (default 24) |

---

## 11. Unit Tests

Unit tests live in `Project498.WebApi.Tests` and run on every PR and merge to `main` via GitHub Actions.

Test coverage targets:

- Request validation (missing required fields, invalid types)
- Schema constraints (e.g. `progressPercent` must be 0–100)
- Auth token generation and validation
- Recommendation scoring logic

---

## 12. Swagger

Swagger UI is available at `/swagger` in all environments. The OpenAPI JSON spec is at `/openapi/v1.json`.

---

## 13. Future Phases

- Expose a `GET /api/health/detailed` endpoint with DB ping and dependency checks
- Add `GET /api/reviews` for integration with another team's API
- Support `?locale=es` query param for localized genre/tag names
