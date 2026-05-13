# Marvel•ous Reads — API Contracts

**Project:** SE-498 Capstone · Spring  
**System:** `Project498.WebApi`  
**Version:** v1  
**Base Path:** `/api`  
**Content Type:** `application/json`  
**Source of Truth:** Swagger/OpenAPI at `/swagger`

---

## 1. Purpose

This document defines the current REST API contracts for **Marvel•ous Reads**. It describes the endpoint routes, HTTP methods, request shapes, response shapes, and business rules used by the Website Backend when communicating with `Project498.WebApi`.

The current API supports:

- user authentication
- comic browsing and filtering
- curated comic sections such as featured, trending, recommended, because-you-read, and hidden-gems
- user shelves and reading progress
- comic checkout and return workflows
- Marvel character data
- character image data
- user lookup and profile update

This contract replaces earlier planned API documentation that referenced generic Books, Tags, or generalized Shelf Item endpoints that are not currently exposed in Swagger.

---

## 2. Global API Rules

### 2.1 Authentication

The API is configured with JWT Bearer authentication and Swagger includes a Bearer token authorization option. Authenticated requests should include the following header when required by the backend implementation:

```http
Authorization: Bearer <token>
```

The Website Backend stores/forwards the token through its server-side authentication flow. Tokens should not be exposed directly to frontend JavaScript.

### 2.2 Response Format

- Request and response bodies use JSON.
- JSON properties use `camelCase`.
- IDs are integers.
- Dates use ISO 8601 date/time strings.
- Most successful operations currently return `200 OK` according to Swagger.
- Swagger currently displays several responses with `text/plain` as the media type, but the example schemas represent JSON-shaped data.

### 2.3 Common Status Codes

| Code | Meaning |
|---|---|
| `200 OK` | Request succeeded |
| `400 Bad Request` | Request body or parameters were invalid |
| `401 Unauthorized` | Missing or invalid credentials/token |
| `404 Not Found` | Requested resource does not exist |
| `409 Conflict` | Duplicate record or violated business rule |
| `500 Internal Server Error` | Server-side error |

---

## 3. Data Models and DTOs

### 3.1 `LoginRequest`

Used by `POST /api/Auth/login`.

```json
{
  "email": "string",
  "password": "string"
}
```

### 3.2 `SignupRequest`

Used by `POST /api/Auth/signup`.

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

### 3.3 `AuthResponse`

Returned by signup/login-style authentication responses.

```json
{
  "user": {
    "id": 0,
    "username": "string",
    "email": "string"
  },
  "token": "string"
}
```

> Note: Swagger may expose `password` on the generated `User` schema because the backend model contains that property. For documentation purposes, API responses should avoid exposing passwords.

### 3.4 `User`

Represents a user account.

```json
{
  "id": 0,
  "username": "string",
  "email": "string"
}
```

### 3.5 `UpdateProfileRequest`

Used by `PUT /api/Users/{id}`.

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

### 3.6 `Comic`

Represents a comic returned by browsing, detail, shelf, and recommendation endpoints.

```json
{
  "id": 0,
  "title": "string",
  "author": "string",
  "genre": "string",
  "secondaryGenre": "string",
  "description": "string",
  "coverImagePath": "string",
  "pdfPath": "string",
  "isIReadPick": true,
  "seriesName": "string",
  "volumeNumber": 0,
  "issueNumber": 0,
  "shelf": "string",
  "progressPercent": 0,
  "currentPage": 0
}
```

### 3.7 `AddToShelfRequest`

Used by `POST /api/Shelves/add`.

```json
{
  "username": "string",
  "comicId": 0,
  "shelf": "string"
}
```

### 3.8 `ReadingProgressResponse`

Returned by `GET /api/Shelves/progress/{username}/{comicId}`.

```json
{
  "comicId": 0,
  "progressPercent": 0,
  "currentPage": 0
}
```

### 3.9 `UpdateProgressRequest`

Used by `PATCH /api/Shelves/update-progress`.

```json
{
  "username": "string",
  "comicId": 0,
  "progressPercent": 0,
  "currentPage": 0
}
```

### 3.10 `CreateCheckoutRequest`

Used by `POST /api/Checkouts`.

```json
{
  "userId": 0,
  "comicId": 0
}
```

### 3.11 `CheckoutResponse`

Returned by checkout endpoints.

```json
{
  "checkoutId": 0,
  "userId": 0,
  "comicId": 0,
  "checkoutDate": "2026-05-13T18:07:20.178Z",
  "dueDate": "2026-05-13T18:07:20.178Z",
  "returnDate": "2026-05-13T18:07:20.178Z",
  "status": "string"
}
```

### 3.12 `MarvelCharacter`

Returned by Marvel character endpoints.

```json
{
  "id": 0,
  "name": "string",
  "alias": "string",
  "description": "string",
  "imagePath": "string"
}
```

### 3.13 `CharacterImage`

Returned by character image endpoints.

```json
{
  "id": 0,
  "alias": "string",
  "imagePath": "string"
}
```

---

## 4. Endpoint Contracts

---

## 4.1 Authentication

### `POST /api/Auth/login`

Authenticates an existing user.

**Request Body**

| Field | Type | Required | Description |
|---|---|---:|---|
| `email` | string | yes | User email |
| `password` | string | yes | User password |

**Example Request**

```json
{
  "email": "string",
  "password": "string"
}
```

**Success Response**

| Status | Body |
|---|---|
| `200 OK` | Login result, token, or authentication status depending on controller implementation |

**Notes**

Swagger currently documents this endpoint with a `text/plain` response media type. The login request body is fully documented, but the exact response body should be verified against the controller if a stricter contract is required.

---

### `POST /api/Auth/signup`

Creates a new user account.

**Request Body**

| Field | Type | Required | Description |
|---|---|---:|---|
| `username` | string | yes | New username |
| `email` | string | yes | New user email |
| `password` | string | yes | New user password |

**Example Request**

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

**Success Response**

**Status:** `200 OK`

```json
{
  "user": {
    "id": 0,
    "username": "string",
    "email": "string"
  },
  "token": "string"
}
```

---

## 4.2 Comics

### `GET /api/Comics`

Returns all comics, optionally filtered by search query and/or genre.

**Query Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `query` | string | no | Search text for comic browsing |
| `genre` | string | no | Genre filter |

**Success Response**

**Status:** `200 OK`

```json
[
  {
    "id": 0,
    "title": "string",
    "author": "string",
    "genre": "string",
    "secondaryGenre": "string",
    "description": "string",
    "coverImagePath": "string",
    "pdfPath": "string",
    "isIReadPick": true,
    "seriesName": "string",
    "volumeNumber": 0,
    "issueNumber": 0,
    "shelf": "string",
    "progressPercent": 0,
    "currentPage": 0
  }
]
```

---

### `GET /api/Comics/{id}`

Returns a single comic by ID.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `id` | int | yes | Comic ID |

**Success Response**

**Status:** `200 OK`

Response body matches the `Comic` model.

---

### `GET /api/Comics/genres`

Returns the list of available comic genres.

**Success Response**

**Status:** `200 OK`

```json
[
  "string"
]
```

---

### `GET /api/Comics/featured`

Returns comics marked as featured or “I Read Pick” items.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/trending`

Returns comics displayed in the trending section of the application.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/recommended`

Returns comics displayed in the recommended section of the application.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/because-you-read`

Returns comics displayed in the “Because You Read” section.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/hidden-gems`

Returns comics displayed in the hidden gems section.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/series/{seriesName}`

Returns comics belonging to a specific comic series.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `seriesName` | string | yes | Series name |

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

## 4.3 Shelves and Reading Progress

### `GET /api/Shelves/{username}/{shelf}`

Returns comics from a specific shelf for a specific user.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `username` | string | yes | Username |
| `shelf` | string | yes | Shelf name/category |

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `POST /api/Shelves/add`

Adds a comic to a user's shelf.

**Request Body**

| Field | Type | Required | Description |
|---|---|---:|---|
| `username` | string | yes | Username |
| `comicId` | int | yes | Comic ID |
| `shelf` | string | yes | Target shelf/category |

**Example Request**

```json
{
  "username": "string",
  "comicId": 0,
  "shelf": "string"
}
```

**Success Response**

**Status:** `200 OK`

---

### `GET /api/Shelves/progress/{username}/{comicId}`

Returns the user's reading progress for a comic.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `username` | string | yes | Username |
| `comicId` | int | yes | Comic ID |

**Success Response**

**Status:** `200 OK`

```json
{
  "comicId": 0,
  "progressPercent": 0,
  "currentPage": 0
}
```

---

### `PATCH /api/Shelves/update-progress`

Updates reading progress and current page for a comic.

**Request Body**

| Field | Type | Required | Description |
|---|---|---:|---|
| `username` | string | yes | Username |
| `comicId` | int | yes | Comic ID |
| `progressPercent` | int | yes | Reading completion percentage |
| `currentPage` | int | yes | Current page number |

**Example Request**

```json
{
  "username": "string",
  "comicId": 0,
  "progressPercent": 0,
  "currentPage": 0
}
```

**Success Response**

**Status:** `200 OK`

---

## 4.4 Checkouts

### `POST /api/Checkouts`

Creates a checkout record for a user and comic.

**Request Body**

| Field | Type | Required | Description |
|---|---|---:|---|
| `userId` | int | yes | User ID |
| `comicId` | int | yes | Comic ID |

**Example Request**

```json
{
  "userId": 0,
  "comicId": 0
}
```

**Success Response**

**Status:** `200 OK`

```json
{
  "checkoutId": 0,
  "userId": 0,
  "comicId": 0,
  "checkoutDate": "2026-05-13T18:07:20.178Z",
  "dueDate": "2026-05-13T18:07:20.178Z",
  "returnDate": "2026-05-13T18:07:20.178Z",
  "status": "string"
}
```

---

### `GET /api/Checkouts/{id}`

Returns a checkout record by ID.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `id` | int | yes | Checkout ID |

**Success Response**

**Status:** `200 OK`

Response body matches the `CheckoutResponse` model.

---

### `GET /api/Checkouts/user/{userId}`

Returns checkout records for a user.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `userId` | int | yes | User ID |

**Query Parameters**

| Parameter | Type | Required | Default | Description |
|---|---|---:|---|---|
| `activeOnly` | boolean | no | `true` | Whether to only return active checkouts |

**Success Response**

**Status:** `200 OK`

```json
[
  {
    "checkoutId": 0,
    "userId": 0,
    "comicId": 0,
    "checkoutDate": "2026-05-13T18:07:20.180Z",
    "dueDate": "2026-05-13T18:07:20.180Z",
    "returnDate": "2026-05-13T18:07:20.180Z",
    "status": "string"
  }
]
```

---

### `PUT /api/Checkouts/{id}/return`

Marks a checkout as returned.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `id` | int | yes | Checkout ID |

**Success Response**

**Status:** `200 OK`

---

## 4.5 Marvel Characters

### `GET /api/marvel-characters`

Returns all Marvel character records.

**Success Response**

**Status:** `200 OK`

```json
[
  {
    "id": 0,
    "name": "string",
    "alias": "string",
    "description": "string",
    "imagePath": "string"
  }
]
```

---

### `GET /api/marvel-characters/{id}`

Returns a Marvel character by ID.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `id` | int | yes | Marvel character ID |

**Success Response**

**Status:** `200 OK`

Response body matches the `MarvelCharacter` model.

---

## 4.6 Character Images

### `GET /api/character-images`

Returns character image records.

**Success Response**

**Status:** `200 OK`

```json
[
  {
    "id": 0,
    "alias": "string",
    "imagePath": "string"
  }
]
```

---

## 4.7 Users

### `GET /api/Users/by-email`

Returns a user record by email.

**Query Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `email` | string | no | User email address |

**Success Response**

**Status:** `200 OK`

```json
{
  "id": 0,
  "username": "string",
  "email": "string"
}
```

---

### `PUT /api/Users/{id}`

Updates a user's profile information.

**Path Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `id` | int | yes | User ID |

**Request Body**

| Field | Type | Required | Description |
|---|---|---:|---|
| `username` | string | yes | Updated username |
| `email` | string | yes | Updated email |
| `password` | string | yes | Updated password |

**Example Request**

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

**Success Response**

**Status:** `200 OK`

---

## 5. Business Rules Captured by the Contract

- Users can sign up and log in through the Auth endpoints.
- The API issues/uses bearer tokens through the implemented authentication flow.
- Comics can be browsed, searched, filtered by genre, viewed by ID, grouped by series, and displayed in curated recommendation sections.
- The comic response model includes static comic metadata plus user-specific shelf/progress fields when applicable.
- Users can add comics to shelves using `username`, `comicId`, and `shelf`.
- Reading progress is tracked by `username` and `comicId`.
- Reading progress stores both `progressPercent` and `currentPage`.
- Checkouts connect users to comics and track checkout date, due date, return date, and status.
- Checkout records can be filtered by user and active status.
- Marvel character data and character image data are currently read-only API resources.
- User profile data can be retrieved by email and updated by user ID.

---

## 6. Current Endpoint Set

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

### Shelves and Reading Progress

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

## 7. Notes

- This contract is based on the current Swagger/OpenAPI output for `Project498.WebApi`.
- This contract intentionally removes earlier planned endpoints that are not currently exposed, including generic `Books`, `Tags`, and generic shelf item routes.
- Swagger remains the source of truth for generated schema names and available routes.
- If controllers, DTOs, or route names change, this file should be updated to match the generated Swagger documentation.
