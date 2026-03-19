# MarvelBookVerse — API Contracts

**Project:** SE-498 Capstone · Spring  
**System:** Project498.WebApi  
**Version:** v1  
**Base Path:** `/api`  
**Content Type:** `application/json`

## 1. Purpose

This document defines the formal API contracts for the MarvelBookVerse REST API. It specifies the request and response shapes, authentication requirements, validation rules, and status codes that the Website Backend may rely on when communicating with the API service.

## 2. Global Rules

### Authentication
- Public endpoints:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
  - `GET /health`
- All other endpoints require:

```http
Authorization: Bearer <jwt>
```

### Standard Error Format
All non-2xx responses return:

```json
{
  "error": "Validation failed",
  "details": ["title is required"],
  "statusCode": 400
}
```

### Common Status Codes
- `200 OK` — successful read/update
- `201 Created` — resource created successfully
- `204 No Content` — delete or update succeeded with no body
- `400 Bad Request` — validation or malformed input
- `401 Unauthorized` — missing or invalid JWT / bad login credentials
- `403 Forbidden` — authenticated but not allowed to access resource
- `404 Not Found` — resource does not exist
- `409 Conflict` — duplicate resource or business rule conflict

### Naming Conventions
- JSON uses `camelCase`
- Dates use ISO 8601 strings
- IDs are integers

## 3. Resource Models

### User
```json
{
  "id": 1,
  "username": "alex",
  "email": "alex@example.com",
  "createdAt": "2026-03-16T00:00:00Z"
}
```

### Book
```json
{
  "id": 1,
  "title": "Dune",
  "description": "Epic sci-fi novel",
  "authorCreator": "Frank Herbert",
  "publishedDate": "1965-08-01",
  "coverImageUrl": "https://example.com/dune.jpg",
  "averageRating": 4.7,
  "isbn": "9780441172719",
  "pageCount": 412,
  "publisher": "Chilton Books",
  "format": "paperback",
  "tags": ["space opera", "politics"]
}
```

### Comic
```json
{
  "id": 10,
  "title": "Amazing Spider-Man #1",
  "description": "First issue in the run",
  "authorCreator": "Stan Lee",
  "publishedDate": "1963-03-01",
  "coverImageUrl": "https://example.com/spiderman1.jpg",
  "averageRating": 4.6,
  "marvelCharacter": "Spider-Man",
  "storyArc": "The Coming of the Chameleon",
  "issueNumber": 1,
  "seriesName": "Amazing Spider-Man",
  "universe": "Earth-616",
  "publisher": "Marvel",
  "tags": ["superhero", "origin"]
}
```

### Tag
```json
{
  "id": 2,
  "name": "mythology",
  "description": "Stories inspired by mythological themes"
}
```

### Shelf
```json
{
  "id": 1,
  "name": "To Read",
  "shelfType": "to_read",
  "itemCount": 5
}
```

### Shelf Item
```json
{
  "id": 12,
  "shelfId": 1,
  "item": {
    "id": 5,
    "title": "Dune",
    "itemType": "book"
  },
  "dateAdded": "2026-03-01T00:00:00Z",
  "progressPercent": 45,
  "notes": "Great read so far"
}
```

### Recommendation
```json
{
  "item": {
    "id": 8,
    "title": "Ender's Game",
    "itemType": "book"
  },
  "matchedTags": ["space opera", "found family"],
  "score": 0.85
}
```

## 4. Endpoint Contracts

---

## 4.1 Authentication

### `POST /api/auth/register`
Creates a new user account.

**Auth required:** No

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `username` | string | yes | unique, 3–100 chars |
| `email` | string | yes | valid email, unique, max 255 chars |
| `password` | string | yes | minimum 8 chars |

#### Example Request
```json
{
  "username": "alex",
  "email": "alex@example.com",
  "password": "securePassword123"
}
```

#### Success Response
**Status:** `201 Created`

```json
{
  "id": 1,
  "username": "alex",
  "email": "alex@example.com",
  "createdAt": "2026-03-16T00:00:00Z"
}
```

#### Errors
- `400 Bad Request` — invalid input, duplicate email, or duplicate username

---

### `POST /api/auth/login`
Authenticates a user and returns a JWT bearer token.

**Auth required:** No

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `email` | string | yes | valid email |
| `password` | string | yes | minimum 8 chars |

#### Example Request
```json
{
  "email": "alex@example.com",
  "password": "securePassword123"
}
```

#### Success Response
**Status:** `200 OK`

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2026-03-17T00:00:00Z",
  "user": {
    "id": 1,
    "username": "alex",
    "email": "alex@example.com"
  }
}
```

#### Errors
- `401 Unauthorized` — invalid credentials

---

## 4.2 Books

### `GET /api/books`
Returns all books, optionally filtered by tag or search query.

**Auth required:** Yes

#### Query Parameters
| Parameter | Type | Required | Notes |
|---|---|---:|---|
| `tag` | string | no | exact or normalized tag name |
| `search` | string | no | title/author search |

#### Success Response
**Status:** `200 OK`

```json
[
  {
    "id": 1,
    "title": "Dune",
    "description": "Epic sci-fi novel",
    "authorCreator": "Frank Herbert",
    "publishedDate": "1965-08-01",
    "coverImageUrl": "https://example.com/dune.jpg",
    "averageRating": 4.7,
    "isbn": "9780441172719",
    "pageCount": 412,
    "publisher": "Chilton Books",
    "format": "paperback",
    "tags": ["space opera", "found family"]
  }
]
```

#### Errors
- `401 Unauthorized`

---

### `GET /api/books/{id}`
Returns one book by ID.

**Auth required:** Yes

#### Path Parameters
| Parameter | Type | Required |
|---|---|---:|
| `id` | int | yes |

#### Success Response
**Status:** `200 OK`

Response body matches the **Book** model.

#### Errors
- `401 Unauthorized`
- `404 Not Found`

---

### `POST /api/books`
Creates a new book.

**Auth required:** Yes

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `title` | string | yes | max 255 chars |
| `description` | string | no | |
| `authorCreator` | string | yes | max 255 chars |
| `publishedDate` | string | no | ISO 8601 date |
| `coverImageUrl` | string | no | valid URL |
| `isbn` | string | no | max 20 chars |
| `pageCount` | int | no | min 1 |
| `publisher` | string | no | max 255 chars |
| `format` | string | no | e.g. `paperback`, `ebook`, `audiobook` |
| `tagIds` | int[] | no | all IDs must exist |

#### Example Request
```json
{
  "title": "Dune",
  "description": "Epic sci-fi novel",
  "authorCreator": "Frank Herbert",
  "publishedDate": "1965-08-01",
  "coverImageUrl": "https://example.com/dune.jpg",
  "isbn": "9780441172719",
  "pageCount": 412,
  "publisher": "Chilton Books",
  "format": "paperback",
  "tagIds": [1, 3]
}
```

#### Success Response
**Status:** `201 Created`

Response body matches the created **Book** model.

#### Errors
- `400 Bad Request`
- `401 Unauthorized`

---

### `PUT /api/books/{id}`
Updates an existing book.

**Auth required:** Yes

#### Request Body
Same shape and validation rules as `POST /api/books`.

#### Success Response
**Status:** `204 No Content`

#### Errors
- `400 Bad Request`
- `401 Unauthorized`
- `404 Not Found`

---

### `DELETE /api/books/{id}`
Deletes a book.

**Auth required:** Yes

#### Success Response
**Status:** `204 No Content`

#### Errors
- `401 Unauthorized`
- `404 Not Found`

---

## 4.3 Comics

### `GET /api/comics`
Returns all comics, optionally filtered by tag or search query.

**Auth required:** Yes

#### Query Parameters
| Parameter | Type | Required | Notes |
|---|---|---:|---|
| `tag` | string | no | exact or normalized tag name |
| `search` | string | no | title/creator search |

#### Success Response
**Status:** `200 OK`

Response body is an array of **Comic** objects.

#### Errors
- `401 Unauthorized`

---

### `GET /api/comics/{id}`
Returns one comic by ID.

**Auth required:** Yes

#### Success Response
**Status:** `200 OK`

Response body matches the **Comic** model.

#### Errors
- `401 Unauthorized`
- `404 Not Found`

---

### `POST /api/comics`
Creates a new comic.

**Auth required:** Yes

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `title` | string | yes | max 255 chars |
| `description` | string | no | |
| `authorCreator` | string | yes | max 255 chars |
| `publishedDate` | string | no | ISO 8601 date |
| `coverImageUrl` | string | no | valid URL |
| `marvelCharacter` | string | no | max 255 chars |
| `storyArc` | string | no | max 255 chars |
| `issueNumber` | int | no | min 1 |
| `seriesName` | string | no | max 255 chars |
| `universe` | string | no | max 50 chars |
| `publisher` | string | no | max 100 chars |
| `tagIds` | int[] | no | all IDs must exist |

#### Example Request
```json
{
  "title": "Amazing Spider-Man #1",
  "description": "First issue in the run",
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

#### Success Response
**Status:** `201 Created`

Response body matches the created **Comic** model.

#### Errors
- `400 Bad Request`
- `401 Unauthorized`

---

### `PUT /api/comics/{id}`
Updates an existing comic.

**Auth required:** Yes

#### Request Body
Same shape and validation rules as `POST /api/comics`.

#### Success Response
**Status:** `204 No Content`

#### Errors
- `400 Bad Request`
- `401 Unauthorized`
- `404 Not Found`

---

### `DELETE /api/comics/{id}`
Deletes a comic.

**Auth required:** Yes

#### Success Response
**Status:** `204 No Content`

#### Errors
- `401 Unauthorized`
- `404 Not Found`

---

## 4.4 Tags

### `GET /api/tags`
Returns all tags.

**Auth required:** Yes

#### Success Response
**Status:** `200 OK`

```json
[
  {
    "id": 1,
    "name": "found family",
    "description": "Stories centered on chosen family"
  },
  {
    "id": 2,
    "name": "mythology",
    "description": "Stories inspired by mythological themes"
  }
]
```

#### Errors
- `401 Unauthorized`

---

### `GET /api/tags/{id}`
Returns one tag by ID.

**Auth required:** Yes

#### Success Response
**Status:** `200 OK`

Response body matches the **Tag** model.

#### Errors
- `401 Unauthorized`
- `404 Not Found`

---

### `POST /api/tags`
Creates a new tag.

**Auth required:** Yes

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `name` | string | yes | unique, max 100 chars |
| `description` | string | no | |

#### Example Request
```json
{
  "name": "space opera",
  "description": "Epic science fiction spanning star systems"
}
```

#### Success Response
**Status:** `201 Created`

Response body matches the created **Tag** model.

#### Errors
- `400 Bad Request`
- `401 Unauthorized`
- `409 Conflict` — duplicate tag name

---

## 4.5 Shelves

### `GET /api/shelves`
Returns all shelves for the authenticated user.

**Auth required:** Yes

#### Success Response
**Status:** `200 OK`

```json
[
  {
    "id": 1,
    "name": "To Read",
    "shelfType": "to_read",
    "itemCount": 5
  },
  {
    "id": 2,
    "name": "Finished",
    "shelfType": "finished",
    "itemCount": 12
  }
]
```

#### Errors
- `401 Unauthorized`

---

### `POST /api/shelves`
Creates a new shelf for the authenticated user.

**Auth required:** Yes

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `name` | string | yes | max 100 chars |
| `shelfType` | string | yes | `to_read`, `reading`, `finished`, or `custom` |

#### Example Request
```json
{
  "name": "Favorites",
  "shelfType": "custom"
}
```

#### Success Response
**Status:** `201 Created`

Response body matches the created **Shelf** model.

#### Errors
- `400 Bad Request`
- `401 Unauthorized`

---

### `GET /api/shelves/{shelfId}/items`
Returns all items on a shelf.

**Auth required:** Yes

#### Path Parameters
| Parameter | Type | Required |
|---|---|---:|
| `shelfId` | int | yes |

#### Success Response
**Status:** `200 OK`

Response body is an array of **Shelf Item** objects.

#### Errors
- `401 Unauthorized`
- `403 Forbidden` — shelf belongs to another user
- `404 Not Found`

---

### `POST /api/shelves/{shelfId}/items`
Adds an item to a shelf.

**Auth required:** Yes

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `itemId` | int | yes | must refer to existing item |
| `progressPercent` | int | no | 0–100, default 0 |
| `notes` | string | no | |

#### Example Request
```json
{
  "itemId": 5,
  "progressPercent": 0,
  "notes": ""
}
```

#### Success Response
**Status:** `201 Created`

Response body matches the created **Shelf Item** model.

#### Errors
- `400 Bad Request`
- `401 Unauthorized`
- `403 Forbidden`
- `404 Not Found`
- `409 Conflict` — item already exists on that shelf

---

### `PATCH /api/shelves/{shelfId}/items/{itemId}`
Updates progress and/or notes for an item on a shelf.

**Auth required:** Yes

#### Request Body
| Field | Type | Required | Constraints |
|---|---|---:|---|
| `progressPercent` | int | no | 0–100 |
| `notes` | string | no | |

#### Example Request
```json
{
  "progressPercent": 75,
  "notes": "Just got to the good part"
}
```

#### Success Response
**Status:** `200 OK`

Response body matches the updated **Shelf Item** model.

#### Errors
- `400 Bad Request`
- `401 Unauthorized`
- `403 Forbidden`
- `404 Not Found`

---

### `DELETE /api/shelves/{shelfId}/items/{itemId}`
Removes an item from a shelf.

**Auth required:** Yes

#### Success Response
**Status:** `204 No Content`

#### Errors
- `401 Unauthorized`
- `403 Forbidden`
- `404 Not Found`

---

## 4.6 Recommendations

### `GET /api/recommendations`
Returns recommended items for the authenticated user based on tag overlap.

**Auth required:** Yes

#### Query Parameters
| Parameter | Type | Required | Notes |
|---|---|---:|---|
| `limit` | int | no | default 10 |

#### Success Response
**Status:** `200 OK`

```json
[
  {
    "item": {
      "id": 8,
      "title": "Ender's Game",
      "itemType": "book"
    },
    "matchedTags": ["space opera", "found family"],
    "score": 0.85
  }
]
```

#### Errors
- `401 Unauthorized`

---

## 4.7 Health

### `GET /health`
Returns API health status.

**Auth required:** No

#### Success Response
**Status:** `200 OK`

```json
{
  "status": "healthy",
  "timestamp": "2026-03-16T00:00:00Z"
}
```

## 5. Business Rules Captured by the Contract

- `progressPercent` must always be between `0` and `100`
- `username` and `email` must be unique at registration time
- `tag.name` must be unique
- `itemId` must refer to an existing book or comic
- a shelf item cannot be duplicated on the same shelf
- protected shelf endpoints only operate on shelves owned by the authenticated user

## 6. Minimum Endpoint Set for Sprint 1

The following contracts are the minimum required for Sprint 1 MVP:
- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/books`
- `GET /api/books/{id}`
- `POST /api/books`
- `GET /api/comics`
- `GET /api/comics/{id}`
- `POST /api/comics`
- `GET /api/shelves`
- `POST /api/shelves`
- `GET /api/shelves/{shelfId}/items`
- `POST /api/shelves/{shelfId}/items`
- `PATCH /api/shelves/{shelfId}/items/{itemId}`
- `DELETE /api/shelves/{shelfId}/items/{itemId}`
- `GET /api/tags`
- `POST /api/tags`
- `GET /api/recommendations`
- `GET /health`

## 7. Notes

This contract is intended to be consistent with the project's REST API specification and OpenAPI/Swagger documentation. If implementation details differ from this contract, the Swagger/OpenAPI document should be updated to match the implementation and this file should be revised accordingly.


