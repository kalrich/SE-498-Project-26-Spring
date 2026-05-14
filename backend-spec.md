# Marvel•ous Reads — Website Backend Specification

**Project:** SE-498 Capstone · Spring  
**Stack:** ASP.NET Core MVC · C# · Razor Views · Typed HttpClient · Cookie Auth  
**Last Updated:** 2026-05-13

---

## 1. Overview

The Website Backend is the ASP.NET Core MVC application that serves the user-facing Marvel•ous Reads website. It renders Razor Views, manages browser session/authentication state, and communicates with `Project498.WebApi` through typed HTTP client services.

Unlike the earlier planned architecture, the current WebServer does not maintain its own separate PostgreSQL database for mirrored users, preferences, or token caching. The primary persistent data lives in `Project498.WebApi` and its PostgreSQL database.

---

## 2. Responsibilities

- Serve Razor View pages to the browser
- Manage cookie-based authentication for the website
- Store user session data with ASP.NET Core session
- Register typed API service clients
- Forward authenticated API requests using a `BearerTokenHandler`
- Render comic, character, checkout, recommendation, profile, login, and signup views
- Display user-friendly empty/error states

---

## 3. Current Architecture

```text
Browser
   |
   | HTML form posts / page requests
   v
Project498.WebServer
ASP.NET Core MVC + Razor Views
Cookie Auth + Session
Typed HttpClient Services
   |
   | REST API calls with bearer token when needed
   v
Project498.WebApi
ASP.NET Core Web API + EF Core
   |
   v
PostgreSQL database
```

The WebServer also calls an external/local DC Comics API through `IDcCharacterService`.

```text
Project498.WebServer ---> DC Comics API
```

---

## 4. Scope

### Included

- Razor page rendering
- Login and signup UI
- Cookie authentication
- Session support
- API-backed services for auth, comics, checkouts, Marvel characters, and character images
- External DC character service integration
- Authenticated and unauthenticated navbar behavior
- Profile page and update workflow
- Recommendations page
- My Checkouts page

### Excluded / Not Current

- HTTP Basic Auth as the main browser authentication method
- Separate WebServer PostgreSQL database
- Local `web_users` table
- Local `user_preferences` table
- Local bcrypt validation database
- Token refresh database table
- Backend-computed tag-overlap recommendation scoring
- Generic Books or Tags pages

---

## 5. Functional Requirements

1. The backend shall serve the public landing page.
2. The backend shall serve login and signup forms.
3. The backend shall authenticate users using API-backed auth services and establish a cookie-authenticated web session.
4. The backend shall use session state and HTTP context to store/access the current user's authentication context.
5. The backend shall display authenticated navigation links after login.
6. The backend shall call `Project498.WebApi` to retrieve comics, comic details, recommendations, checkout records, Marvel characters, and character images.
7. The backend shall call the configured DC Comics API for DC character data.
8. The backend shall provide checkout actions by calling the Web API checkout endpoints.
9. The backend shall provide profile lookup/update functionality through the Web API users endpoints.
10. The backend shall show user-friendly empty states when no checkouts or external API data are available.

---

## 6. Non-Functional Requirements

1. **Security:** Browser authentication uses ASP.NET Core cookie authentication. API tokens should be handled server-side and attached to outbound API requests by `BearerTokenHandler`.
2. **Maintainability:** Controllers should use service interfaces rather than direct `HttpClient` logic.
3. **Reliability:** Failed API calls should be handled gracefully in controllers/services.
4. **Usability:** Errors and empty states should be displayed in the UI rather than exposing stack traces.
5. **Configurability:** API base URLs should come from configuration, with development defaults.
6. **Container Awareness:** The frontend/backend should be able to communicate with the containerized Web API using the configured API base URL.

---

## 7. Technology Stack

| Concern | Current Choice |
|---|---|
| Framework | ASP.NET Core MVC |
| Views | Razor Views (`.cshtml`) |
| Auth | ASP.NET Core Cookie Authentication |
| Session | ASP.NET Core Session |
| API communication | Typed `HttpClient` |
| Token forwarding | `BearerTokenHandler` |
| Internal API | `Project498.WebApi` |
| External API | DC Comics API service |
| Testing | xUnit where applicable |

---

## 8. WebServer Configuration

Current `Program.cs` registers:

- MVC controllers with views
- Session
- HTTP context accessor
- `BearerTokenHandler`
- Typed HTTP clients for API-backed services
- Cookie authentication
- Authorization middleware
- Static files
- Routing

### Configured API URLs

| Setting | Default |
|---|---|
| `ApiBaseUrl` | `http://localhost:5272/` |
| `DcComicsApiUrl` | `http://localhost:5100` |

---

## 9. Registered Services

| Interface | Implementation | Base URL |
|---|---|---|
| `IAuthService` | `AuthApiService` | `ApiBaseUrl` |
| `IComicService` | `ComicApiService` | `ApiBaseUrl` |
| `ICheckoutService` | `CheckoutService` | `ApiBaseUrl` |
| `IDcCharacterService` | `DcCharacterService` | `DcComicsApiUrl` |
| `ICharacterImageService` | `CharacterImageService` | `ApiBaseUrl` |
| `IMarvelCharacterService` | `MarvelCharacterService` | `ApiBaseUrl` |

---

## 10. Authentication Flow

1. User submits login/signup form from the WebServer UI.
2. WebServer calls the appropriate auth endpoint in `Project498.WebApi`.
3. On success, WebServer creates a cookie-authenticated session.
4. User navigates authenticated pages with the browser cookie.
5. WebServer service calls use `BearerTokenHandler` to attach bearer tokens to API requests when needed.
6. Logout clears the web authentication session.

Configured cookie paths:

| Purpose | Path |
|---|---|
| Login | `/Auth/Login` |
| Logout | `/Auth/Logout` |
| Access denied | `/Auth/Login` |

---

## 11. WebServer Routes / Pages

| Page | Purpose |
|---|---|
| Landing page | Public introduction page |
| Login | Existing user authentication |
| Sign Up | Account creation |
| Explore | Browse/search comics |
| Comic Detail | View one comic and checkout status |
| Recommendations | Curated comic recommendations |
| DC Characters | External DC character data |
| Marvel Characters | API-backed Marvel character data |
| My Checkouts | Current checkout records |
| My Profile | View/update account information |

---

## 12. Relationship to API Specification

The WebServer should rely on the current API contract for:

- Auth endpoints
- Comics endpoints
- Shelves/progress endpoints
- Checkout endpoints
- Marvel character endpoints
- Character image endpoint
- Users profile endpoints

The WebServer should not assume old endpoints such as `/api/books`, `/api/tags`, or `/api/recommendations` unless they are re-added to Swagger.

---

## 13. Error Handling

The backend should convert API failures into user-friendly UI states, including:

- Invalid login message
- Signup failure message
- No active checkouts
- No DC characters found
- Comic not found
- Checkout unavailable/failure state

Raw exception output should not be displayed to users.

---

## 14. Future Improvements

- Add stronger route-level `[Authorize]` usage for protected pages
- Improve token lifetime handling
- Add automated integration tests for WebServer-to-WebApi flows
- Add clearer service-level error objects
- Add dedicated health checks for API connectivity
