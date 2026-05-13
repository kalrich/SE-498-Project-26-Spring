# Marvel•ous Reads — API Contracts

**Project:** SE-498 Capstone · Spring  
**System:** Project498.WebApi  
**Version:** v1  
**Base Path:** `/api`  
**Content Type:** `application/json`

## 1. Purpose

This document defines the current API contracts for the Marvel•ous Reads REST API. It is based on the current Swagger/OpenAPI documentation and should be used by the Website Backend when communicating with `Project498.WebApi`.

The API currently supports authentication, comic browsing, recommendation lists, shelf management, reading progress tracking, checkout/return workflows, Marvel character data, character images, and user profile lookup/update.

## 2. Global Rules

### Authentication

Swagger includes an `Authorize` option, but the currently documented endpoints do not show required authorization on each operation. The frontend should follow the implemented authentication flow and include a bearer token where required by the backend.

```http
Authorization: Bearer <token>
```

### Standard Response Rules

- JSON uses `camelCase`
- IDs are integers
- Dates use ISO 8601 strings
- Most documented successful responses return `200 OK`
- Request and response bodies are sent as `application/json`

### Common Status Codes

| Code | Meaning |
|---|---|
| `200 OK` | Request succeeded |
| `400 Bad Request` | Invalid or malformed request |
| `401 Unauthorized` | Missing/invalid credentials or token |
| `404 Not Found` | Resource was not found |
| `409 Conflict` | Duplicate resource or business rule conflict |
| `500 Internal Server Error` | Server-side error |

---

## 3. Resource Models

### User

```json
{
  "id": 0,
  "username": "string",
  "email": "string",
  "password": "string"
}
```

### AuthResponse

Used by signup/login-style authentication responses.

```json
{
  "user": {
    "id": 0,
    "username": "string",
    "email": "string",
    "password": "string"
  },
  "token": "string"
}
```

### Comic

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

### AddToShelfRequest

```json
{
  "username": "string",
  "comicId": 0,
  "shelf": "string"
}
```

### ReadingProgressResponse

```json
{
  "comicId": 0,
  "progressPercent": 0,
  "currentPage": 0
}
```

### UpdateProgressRequest

```json
{
  "username": "string",
  "comicId": 0,
  "progressPercent": 0,
  "currentPage": 0
}
```

### CreateCheckoutRequest

```json
{
  "userId": 0,
  "comicId": 0
}
```

### CheckoutResponse

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

### MarvelCharacter

```json
{
  "id": 0,
  "name": "string",
  "alias": "string",
  "description": "string",
  "imagePath": "string"
}
```

### CharacterImage

```json
{
  "id": 0,
  "alias": "string",
  "imagePath": "string"
}
```

### LoginRequest

```json
{
  "email": "string",
  "password": "string"
}
```

### SignupRequest

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

### UpdateProfileRequest

```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

---

## 4. Endpoint Contracts

---

## 4.1 Authentication

### `POST /api/Auth/login`

Authenticates a user.

**Request Body**

| Field | Type | Required |
|---|---|---:|
| `email` | string | yes |
| `password` | string | yes |

**Example Request**

```json
{
  "email": "string",
  "password": "string"
}
```

**Success Response**

**Status:** `200 OK`

Swagger documents a plain-text response media type. The implementation may return a token, user data, or login status depending on controller logic.

---

### `POST /api/Auth/signup`

Creates a new user account.

**Request Body**

| Field | Type | Required |
|---|---|---:|
| `username` | string | yes |
| `email` | string | yes |
| `password` | string | yes |

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

**Example Response**

```json
{
  "user": {
    "id": 0,
    "username": "string",
    "email": "string",
    "password": "string"
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
| `query` | string | no | Search query |
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

Returns one comic by ID.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `id` | int | yes |

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

Returns featured comics.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/trending`

Returns trending comics.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/recommended`

Returns recommended comics.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/because-you-read`

Returns comics recommended based on previous reading activity.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/hidden-gems`

Returns hidden gem comics.

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `GET /api/Comics/series/{seriesName}`

Returns comics belonging to a specific series.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `seriesName` | string | yes |

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

## 4.3 Shelves

### `GET /api/Shelves/{username}/{shelf}`

Returns comics from a specific user's shelf.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `username` | string | yes |
| `shelf` | string | yes |

**Success Response**

**Status:** `200 OK`

Response body is an array of `Comic` objects.

---

### `POST /api/Shelves/add`

Adds a comic to a user's shelf.

**Request Body**

| Field | Type | Required |
|---|---|---:|
| `username` | string | yes |
| `comicId` | int | yes |
| `shelf` | string | yes |

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

Returns a user's reading progress for a comic.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `username` | string | yes |
| `comicId` | int | yes |

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

Updates reading progress for a comic.

**Request Body**

| Field | Type | Required |
|---|---|---:|
| `username` | string | yes |
| `comicId` | int | yes |
| `progressPercent` | int | yes |
| `currentPage` | int | yes |

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

| Field | Type | Required |
|---|---|---:|
| `userId` | int | yes |
| `comicId` | int | yes |

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

Returns a checkout by ID.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `id` | int | yes |

**Success Response**

**Status:** `200 OK`

Response body matches the `CheckoutResponse` model.

---

### `GET /api/Checkouts/user/{userId}`

Returns checkout records for a user.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `userId` | int | yes |

**Query Parameters**

| Parameter | Type | Required | Default |
|---|---|---:|---|
| `activeOnly` | boolean | yes | `true` |

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

| Parameter | Type | Required |
|---|---|---:|
| `id` | int | yes |

**Success Response**

**Status:** `200 OK`

---

## 4.5 Marvel Characters

### `GET /api/marvel-characters`

Returns all Marvel characters.

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

Returns one Marvel character by ID.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `id` | int | yes |

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

Returns a user by email.

**Query Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `email` | string | no |

**Success Response**

**Status:** `200 OK`

```json
{
  "id": 0,
  "username": "string",
  "email": "string",
  "password": "string"
}
```

---

### `PUT /api/Users/{id}`

Updates a user's profile information.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `id` | int | yes |

**Request Body**

| Field | Type | Required |
|---|---|---:|
| `username` | string | yes |
| `email` | string | yes |
| `password` | string | yes |

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
- Comics can be browsed, searched, filtered by genre, viewed by ID, and grouped by series.
- Comic recommendation sections are separated into featured, trending, recommended, because-you-read, and hidden-gems endpoints.
- Users can add comics to shelves using a username, comic ID, and shelf name.
- Reading progress is tracked by username and comic ID.
- Reading progress stores both `progressPercent` and `currentPage`.
- Checkouts connect users to comics and track checkout date, due date, return date, and status.
- Checkout records can be filtered by user and active status.
- Marvel character data and character image data are read-only API resources in the current contract.
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

## 7. Notes

This contract replaces earlier planned endpoints that are not currently shown in Swagger, including generic `Books`, `Tags`, and generic shelf item endpoints. The current contract should stay aligned with Swagger/OpenAPI. If the API implementation changes, this document should be updated to match the generated Swagger documentation.