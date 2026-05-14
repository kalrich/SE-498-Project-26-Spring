# Marvel•ous Reads — API Contracts

**Project:** SE-498 Capstone · Spring 2026  
**System:** `Project498.WebApi`  
**Base Path:** `/api`  
**Content Type:** `application/json`  
**Last Updated:** 2026-05-13

---

## 1. Purpose

This document defines the current endpoint-level API contracts for Marvel•ous Reads. It is based on the current controllers, DTOs, and Swagger output for `Project498.WebApi`.

For architecture-level API details, see `api-spec.md`.

---

## 2. Global Rules

- JSON uses camelCase when serialized by ASP.NET Core.
- IDs are integers.
- Dates are ISO 8601 date/time strings.
- Most successful operations return `200 OK` unless otherwise noted.
- Protected endpoints require `Authorization: Bearer <token>`.
- Swagger is available at `/swagger` in development.

### Common Status Codes

| Code | Meaning |
|---|---|
| `200 OK` | Request succeeded |
| `204 No Content` | Request succeeded with no response body |
| `400 Bad Request` | Invalid request body or business validation failure |
| `401 Unauthorized` | Missing or invalid bearer token |
| `404 Not Found` | Requested user/comic/resource was not found |
| `500 Internal Server Error` | Server-side failure |

---

## 3. Shared Models

### User

```json
{
  "id": 1,
  "username": "josh",
  "email": "josh@example.com"
}
```

> Current internal `User` model contains a `password` field. API responses should avoid exposing it where possible.

### Comic

```json
{
  "id": 1,
  "title": "Human Torch Vol. 1 Issue 2",
  "author": "Marvel",
  "genre": "Action",
  "secondaryGenre": "Adventure",
  "description": "Comic description",
  "coverImagePath": "/images/covers/human-torch-2.jpg",
  "pdfPath": "/comics/human-torch-2.pdf",
  "isIReadPick": true,
  "seriesName": "Human Torch",
  "volumeNumber": 1,
  "issueNumber": 2,
  "shelf": "Reading",
  "progressPercent": 35,
  "currentPage": 7,
  "isFavorite": true,
  "averageRating": 4.5,
  "reviewCount": 2,
  "isCheckedOut": true,
  "activeCheckoutDueDate": "2026-05-20T18:00:00Z"
}
```

### AddToShelfRequest

```json
{
  "username": "josh",
  "comicId": 1,
  "shelf": "Reading"
}
```

### UpdateProgressRequest

```json
{
  "username": "josh",
  "comicId": 1,
  "progressPercent": 50,
  "currentPage": 12
}
```

### ReadingProgressResponse

```json
{
  "comicId": 1,
  "progressPercent": 50,
  "currentPage": 12
}
```

### CreateCheckoutRequest

```json
{
  "userId": 1,
  "comicId": 1
}
```

### CheckoutResponse

```json
{
  "checkoutId": 1,
  "userId": 1,
  "comicId": 1,
  "checkoutDate": "2026-05-13T18:00:00Z",
  "dueDate": "2026-05-20T18:00:00Z",
  "returnDate": null,
  "status": "Active",
  "comicTitle": "Human Torch Vol. 1 Issue 2",
  "coverImagePath": "/images/covers/human-torch-2.jpg",
  "isOverdue": false
}
```

### FavoriteRequest

```json
{
  "userId": 1,
  "comicId": 1
}
```

### FavoriteStatusResponse

```json
{
  "userId": 1,
  "comicId": 1,
  "isFavorite": true
}
```

### ComicReviewRequest

```json
{
  "userId": 1,
  "comicId": 1,
  "rating": 5,
  "comment": "Great issue."
}
```

### ComicReviewResponse

```json
{
  "id": 1,
  "userId": 1,
  "comicId": 1,
  "username": "josh",
  "comicTitle": "Human Torch Vol. 1 Issue 2",
  "coverImagePath": "/images/covers/human-torch-2.jpg",
  "rating": 5,
  "comment": "Great issue.",
  "createdAt": "2026-05-13T18:00:00Z",
  "updatedAt": "2026-05-13T18:00:00Z"
}
```

### ReadingHistoryResponse

```json
{
  "comicId": 1,
  "title": "Human Torch Vol. 1 Issue 2",
  "coverImagePath": "/images/covers/human-torch-2.jpg",
  "currentPage": 12,
  "progressPercent": 50,
  "lastReadAt": "2026-05-13T18:00:00Z"
}
```

### MarvelCharacter

```json
{
  "id": 1,
  "name": "Peter Parker",
  "alias": "Spider-Man",
  "description": "Character description",
  "imagePath": "/images/characters/spider-man.jpg"
}
```

### CharacterImage

```json
{
  "id": 1,
  "alias": "Spider-Man",
  "imagePath": "/images/characters/spider-man-alt.jpg"
}
```

---

## 4. Authentication

### `POST /api/Auth/login`

Authenticates a user.

**Request**

```json
{
  "email": "josh@example.com",
  "password": "password123"
}
```

**Success Response — `200 OK`**

```json
{
  "user": {
    "id": 1,
    "username": "josh",
    "email": "josh@example.com"
  },
  "token": "jwt-token"
}
```

---

### `POST /api/Auth/signup`

Creates a new user account.

**Request**

```json
{
  "username": "josh",
  "email": "josh@example.com",
  "password": "password123"
}
```

**Success Response — `200 OK`**

```json
{
  "user": {
    "id": 1,
    "username": "josh",
    "email": "josh@example.com"
  },
  "token": "jwt-token"
}
```

---

## 5. Comics

### `GET /api/Comics`

Returns comics with optional filters.

**Query Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `query` | string | no | Search title, author, description, or series name |
| `genre` | string | no | Match primary or secondary genre |
| `status` | string | no | `checkedout` or `available`; requires `userId` |
| `userId` | int | no | Adds user-specific favorite/checkout state |

**Success Response — `200 OK`**

Array of `Comic` objects.

---

### `GET /api/Comics/{id}`

Returns one comic by ID.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `id` | int | yes |

**Query Parameters**

| Parameter | Type | Required | Description |
|---|---|---:|---|
| `userId` | int | no | Adds user-specific favorite/checkout state |

**Success Response — `200 OK`**

Single `Comic` object.

**Errors**

- `404 Not Found` if comic does not exist

---

### `GET /api/Comics/genres`

Returns distinct comic genres from primary and secondary genre fields.

**Success Response — `200 OK`**

```json
[
  "Action",
  "Adventure"
]
```

---

### Comic Collection Endpoints

Each returns an array of `Comic` objects.

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/Comics/featured` | Comics marked as I Read Picks |
| `GET` | `/api/Comics/trending` | Trending-style comic collection |
| `GET` | `/api/Comics/recommended` | Recommended-style comic collection |
| `GET` | `/api/Comics/because-you-read` | Because-you-read-style comic collection |
| `GET` | `/api/Comics/hidden-gems` | Hidden-gems-style comic collection |

---

### `GET /api/Comics/series/{seriesName}`

Returns comics in a specific series ordered by volume and issue number.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `seriesName` | string | yes |

**Success Response — `200 OK`**

Array of `Comic` objects.

---

## 6. Shelves and Reading Progress

### `GET /api/Shelves/{username}/{shelf}`

Returns comics from a user's shelf.

**Path Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `username` | string | yes |
| `shelf` | string | yes |

**Success Response — `200 OK`**

Array of `Comic` objects.

---

### `POST /api/Shelves/add`

Adds a comic to a user's shelf.

**Request**

```json
{
  "username": "josh",
  "comicId": 1,
  "shelf": "Reading"
}
```

**Success Response — `200 OK`**

---

### `GET /api/Shelves/progress/{username}/{comicId}`

Returns a user's progress for one comic.

**Success Response — `200 OK`**

```json
{
  "comicId": 1,
  "progressPercent": 50,
  "currentPage": 12
}
```

---

### `PATCH /api/Shelves/update-progress`

Updates reading progress and current page.

**Request**

```json
{
  "username": "josh",
  "comicId": 1,
  "progressPercent": 50,
  "currentPage": 12
}
```

**Success Response — `200 OK`**

---

## 7. Checkouts

### `POST /api/Checkouts`

Creates a checkout record.

**Request**

```json
{
  "userId": 1,
  "comicId": 1
}
```

**Success Response — `200 OK`**

`CheckoutResponse`

---

### `GET /api/Checkouts/{id}`

Returns checkout by checkout ID.

---

### `GET /api/Checkouts/user/{userId}`

Returns checkout records for a user.

**Query Parameters**

| Parameter | Type | Required | Default |
|---|---|---:|---|
| `activeOnly` | boolean | no | `true` |

---

### `PUT /api/Checkouts/{id}/return`

Marks a checkout as returned.

**Success Response — `200 OK`**

---

## 8. Favorites

### `GET /api/Favorites/user/{userId}`

Returns favorite comics for a user.

**Success Response — `200 OK`**

Array of `Comic` objects with `isFavorite` set to `true`.

---

### `GET /api/Favorites/user/{userId}/comic/{comicId}`

Returns favorite status for one user/comic pair.

**Success Response — `200 OK`**

```json
{
  "userId": 1,
  "comicId": 1,
  "isFavorite": true
}
```

---

### `POST /api/Favorites`

Adds a comic to favorites.

**Request**

```json
{
  "userId": 1,
  "comicId": 1
}
```

**Success Response — `200 OK`**

`FavoriteStatusResponse` with `isFavorite: true`.

---

### `DELETE /api/Favorites/user/{userId}/comic/{comicId}`

Removes a comic from favorites.

**Success Response — `200 OK`**

`FavoriteStatusResponse` with `isFavorite: false`.

---

## 9. Comic Reviews

### `GET /api/ComicReviews/comic/{comicId}`

Returns all reviews for a comic.

**Success Response — `200 OK`**

Array of `ComicReviewResponse` objects.

---

### `GET /api/ComicReviews/user/{userId}/comic/{comicId}`

Returns the user's review for a comic, or `null` if no review exists.

---

### `GET /api/ComicReviews/user/{userId}`

Returns all reviews written by a user.

---

### `POST /api/ComicReviews`

Creates or updates a user's review for a comic.

**Request**

```json
{
  "userId": 1,
  "comicId": 1,
  "rating": 5,
  "comment": "Great issue."
}
```

**Success Response — `200 OK`**

`ComicReviewResponse`

**Validation**

- `rating` must be between `1` and `5`
- one review is stored per user/comic pair

---

### `DELETE /api/ComicReviews/user/{userId}/comic/{comicId}`

Deletes the user's review for a comic.

**Success Response — `204 No Content`**

---

## 10. Reading History

### `GET /api/ReadingHistory/user/{userId}`

Returns recently read comics for a user ordered by most recent reading activity.

**Success Response — `200 OK`**

Array of `ReadingHistoryResponse` objects.

---

## 11. Marvel Characters

### `GET /api/marvel-characters`

Returns all Marvel characters.

### `GET /api/marvel-characters/{id}`

Returns one Marvel character by ID.

---

## 12. Character Images

### `GET /api/character-images`

Returns all character image records.

---

## 13. Users

### `GET /api/Users/by-email`

Returns a user by email.

**Query Parameters**

| Parameter | Type | Required |
|---|---|---:|
| `email` | string | no |

---

### `PUT /api/Users/{id}`

Updates profile information for a user.

**Request**

```json
{
  "username": "josh",
  "email": "josh@example.com",
  "password": "newPasswordOrNull"
}
```

**Success Response — `200 OK`**

---

## 14. Business Rules Captured by the Contract

- Comics can be searched by title, author, description, or series.
- Comics can be filtered by genre and user checkout status.
- Comic series are ordered by volume and issue number.
- Shelf/progress state is user-specific.
- Favorites are unique per user/comic.
- Reading history is unique per user/comic and ordered by `LastReadAt`.
- Reviews are unique per user/comic.
- Review ratings must be between 1 and 5.
- Checkout records can be active or returned.
- Active checkouts can be filtered by user.
