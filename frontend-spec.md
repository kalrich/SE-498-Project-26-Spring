# Marvel•ous Reads — Website Frontend Specification

**Project:** SE-498 Capstone · Spring  
**Stack:** Razor Views (.cshtml) · ASP.NET Core MVC · Bootstrap/CSS · C#  
**Last Updated:** 2026-05-13

---

## 1. Overview

Marvel•ous Reads is a server-rendered ASP.NET Core MVC web application for browsing, discovering, and checking out Marvel comic content. The frontend is built with Razor Views (`.cshtml`) and communicates with `Project498.WebApi` through typed service classes registered with `HttpClient`.

The current implementation focuses on comic discovery, recommendations, checkout tracking, profile management, and character browsing. There is no separate React/Vue frontend application.

---

## 2. Scope

### Included

- Public landing page
- Login page
- Sign up page
- Explore Comics page
- Comic detail page
- Recommendations page
- My Checkouts page
- My Profile page
- Marvel Characters page
- DC Characters page using an external DC API service
- Authenticated navbar state
- Cookie-based web session authentication
- API-backed comic, checkout, auth, character, and profile data
- Responsive card-based UI with a Marvel-themed visual style
- Empty/error states such as no active checkouts or no DC characters found

### Excluded / Not Currently Implemented

- Tags management pages
- Admin content management panel
- OAuth/SSO login
- Password reset flow
- Social features such as following users or sharing shelves
- Client-side SPA framework

---

## 3. Functional Requirements

1. The frontend shall provide a public landing page with the Marvel•ous Reads brand, tagline, and calls to action for login and sign up.
2. The frontend shall provide a login form with email and password fields.
3. The frontend shall provide a sign up form with username, email, password, and confirm password fields.
4. The frontend shall show different navigation links depending on authentication state.
5. Authenticated users shall see links for Explore, Recommendations, DC Characters, Marvel Characters, My Checkouts, Logout, and a profile avatar.
6. The Explore page shall display comics in a card grid and support searching by title, author, or description.
7. Comic cards shall show cover image, title, description, genre badges, and Marvel Comics labeling.
8. The comic detail page shall show comic metadata, cover art, genre badges, checkout status, and checkout actions.
9. The Recommendations page shall display curated comic recommendations and support searching/clearing results.
10. The My Checkouts page shall display currently checked-out comics or an empty state if none exist.
11. The My Profile page shall show username, email, reading activity summary, and an edit account form.
12. The Marvel Characters page shall display character cards using data from the Web API.
13. The DC Characters page shall attempt to display external DC character data and show a user-friendly empty state if the DC API returns no characters.
14. User-facing errors and empty states shall be shown as readable UI messages rather than raw exceptions.

---

## 4. Non-Functional Requirements

1. **Usability:** Main flows should be accessible through the navbar and should use clear page headings and buttons.
2. **Responsiveness:** Card grids and forms should remain usable across desktop and smaller screen widths.
3. **Performance:** Standard pages should load quickly under local Docker development conditions.
4. **Security:** Authentication state is managed server-side using ASP.NET Core cookie authentication and session data. Tokens are not manually handled by frontend JavaScript.
5. **Maintainability:** WebServer controllers should delegate API calls to typed services rather than directly embedding HTTP logic in views.
6. **Reliability:** Failed external calls, such as the DC Characters API being unavailable, should show an empty/error state instead of crashing the page.

---

## 5. Technology Stack

| Concern | Current Choice |
|---|---|
| Frontend rendering | Razor Views (`.cshtml`) |
| Web framework | ASP.NET Core MVC |
| Styling | Custom Marvel-themed CSS / Bootstrap-style layout |
| Auth state | ASP.NET Core cookie authentication + session |
| API communication | Typed `HttpClient` services |
| Backend API | `Project498.WebApi` |
| External API | DC Comics API service |
| Static assets | `wwwroot` images/css |

---

## 6. Current Pages

### 6.1 Landing Page

**Route:** `/`

**Purpose:** Public entry page for unauthenticated users.

**Features:**

- Brand title: Marvel•ous Reads
- Tagline: “Your one stop shop for all your Marvel reads”
- Login and Sign Up buttons
- Public navbar with Explore, Login, and Sign Up links

---

### 6.2 Login

**Route:** `/Auth/Login`

**Purpose:** Authenticates existing users.

**Features:**

- Email field
- Password field
- Login button
- Forgot password placeholder link
- Sign up link for new users

---

### 6.3 Sign Up

**Route:** `/Auth/Signup`

**Purpose:** Registers new users.

**Features:**

- Username field
- Email field
- Password field
- Confirm password field
- Create Account button
- Login link for existing users

---

### 6.4 Explore Comics

**Route:** `/Comics` or `/Explore` depending on controller routing

**Purpose:** Main comic browsing page.

**Features:**

- Search by title, author, or description
- Clear search button
- Comic card grid
- Comic cover art
- Comic title and description
- Genre/secondary genre badges

---

### 6.5 Comic Detail

**Route:** `/Comics/Details/{id}` or equivalent details route

**Purpose:** Displays one comic and allows checkout interaction.

**Features:**

- Large cover image
- Comic title
- Genre badges
- Description
- Checkout Comic button
- My Checkouts button
- Availability/status panel

---

### 6.6 Recommendations

**Route:** `/Recommendations`

**Purpose:** Displays curated comic recommendations.

**Features:**

- Search recommendations input
- Recommended For You comic card grid
- Clear search button
- Recommendation cards based on popular picks and current library data

---

### 6.7 My Checkouts

**Route:** `/Checkouts` or `/MyCheckouts`

**Purpose:** Displays active user checkouts.

**Features:**

- Active checkout list
- Empty state when no comics are checked out
- Browse Comics button
- Return/checkout status support when active records exist

---

### 6.8 My Profile

**Route:** profile/avatar route or `/Users/Profile`

**Purpose:** Displays and updates account information.

**Features:**

- Avatar circle with user initial
- Username
- Email
- Total books/comics read summary
- Edit account form
- Save Changes button

---

### 6.9 Marvel Characters

**Route:** `/MarvelCharacters`

**Purpose:** Displays Golden Age Marvel character data from the Web API.

**Features:**

- Character card grid
- Character image
- Name
- Alias
- Description
- View Details button

---

### 6.10 DC Characters

**Route:** `/DcCharacters`

**Purpose:** Displays DC character data from an external service if available.

**Features:**

- External API-backed character section
- Empty state message when no characters are found or the DC API is unavailable

---

## 7. Navigation

### Unauthenticated Navbar

```text
Marvel•ous Reads | Explore | Login | Sign Up
```

### Authenticated Navbar

```text
Marvel•ous Reads | Explore | Recommendations | DC Characters | Marvel Characters | My Checkouts | Logout | Profile Avatar
```

---

## 8. WebServer Service Integration

The frontend uses typed services registered in `Program.cs`:

| Service Interface | Implementation | Purpose |
|---|---|---|
| `IAuthService` | `AuthApiService` | Login/signup through Web API |
| `IComicService` | `ComicApiService` | Comic browsing/details/recommendations |
| `ICheckoutService` | `CheckoutService` | Checkout and return workflows |
| `IDcCharacterService` | `DcCharacterService` | External DC character API |
| `ICharacterImageService` | `CharacterImageService` | Character image records |
| `IMarvelCharacterService` | `MarvelCharacterService` | Marvel character data |

Most API-backed services use a `BearerTokenHandler` to attach the authenticated user's token to outgoing API requests.

---

## 9. Authentication Behavior

Authentication is configured using ASP.NET Core cookie authentication.

- Login path: `/Auth/Login`
- Logout path: `/Auth/Logout`
- Access denied path: `/Auth/Login`

The WebServer also uses session state and `IHttpContextAccessor` to maintain user-specific request context.

---

## 10. Styling and Branding

The visual design uses a Marvel-inspired theme:

- Red accent colors for brand and action buttons
- Cream/light background
- Large bold typography
- Rounded content cards
- Comic cover-heavy card grid layouts
- Profile avatar circle using the user's initial

---

## 11. Future Improvements

- Add checkout due-date warnings
- Add role-based admin tools for managing comic data
- Add password reset support
