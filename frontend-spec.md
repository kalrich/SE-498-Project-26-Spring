# Marvel-ous Reads — Website Frontend Specification

**Project:** SE-498 Capstone · Spring
**Stack:** Razor Views (.cshtml) · Bootstrap 5 · C# (ASP.NET Core MVC)
**Last Updated:** 2026-03-16

---

## 1. Overview

The Marvel-ous Reads frontend is a set of server-rendered Razor Views served by the Website Backend (ASP.NET Core MVC). There is no separate frontend application — the C# backend controllers fetch data, apply logic, and pass view models to `.cshtml` templates which render the HTML. The site is styled with Bootstrap 5 and structured around five core pages.

---

## 2. Scope

**Included:**
- Six core pages: Home, Sign Up, Login, My Shelves, Item Detail, Recommendations
- Bootstrap 5 styling with a Marvel-themed color scheme
- Razor Views (`.cshtml`) served by the ASP.NET Core MVC backend
- User account creation and login
- Error states for all user-facing failure cases (form validation errors, empty states, item not found)
- Responsive layout usable on mobile (≥ 375px) and desktop (≥ 1024px)
- Wireframes for all six pages committed to the repository

**Excluded:**
- Admin or content management panel
- User profile settings or avatar upload
- Social features (following other users, sharing shelves)
- Third-party OAuth or SSO login
- Password reset flow (future phase)
- A/B testing or localization (future phase)

---

## 3. Functional Requirements

1. The frontend shall provide a registration form with fields for username, email, password, and confirm password; submitting shall call the backend registration endpoint and redirect to the home page on success.
2. The frontend shall provide a login form; invalid credentials shall display a visible error message using a Bootstrap `alert-danger` component.
3. Authenticated users shall see "My Shelves" and "Recommendations" in the navbar; unauthenticated users shall see "Sign Up" and "Log In" instead.
4. The home page shall display a search bar and at least two featured item card grids (one for books, one for comics) loaded from the backend.
5. The search results page shall display matching books and comics by title, with a type badge on each result, and a "no matches" message if the query returns nothing.
6. The item detail page shall display title, type, author/creator, description, tags, and type-specific fields (ISBN for books, story arc for comics); authenticated users shall see an "Add to Shelf" control.
7. The My Shelves page shall display at least three tabs (To Read, Reading, Finished) with items listed under each; each item shall show a progress bar and action buttons (Update, Move, Remove).
8. The Recommendations page shall display a ranked card grid of suggested items with matched tags shown on each card; an empty state message shall appear if the user has no shelf data.
9. All form submissions that result in an error shall display an inline message — no raw exception output shall be visible to the user.

---

## 4. Non-Functional Requirements

1. **Performance:** Pages shall load and render in under 2 seconds on a broadband connection.
2. **Usability:** All error states (form errors, empty shelves, item not found, failed API call) shall show a human-readable Bootstrap alert or empty state message — never a blank page or stack trace.
3. **Responsiveness:** All pages shall be usable on screen widths from 375px (mobile) to 1440px (desktop) using Bootstrap's responsive grid.
4. **Accessibility:** Interactive elements (buttons, form inputs, nav links) shall include appropriate `aria-label` attributes where the text label alone is insufficient.
5. **Browser support:** The site shall function correctly in the latest stable versions of Chrome, Firefox, and Safari.
6. **Security:** No sensitive data (tokens, passwords) shall be stored in the browser. All auth state is managed server-side via the ASP.NET Core cookie.

---

## 5. Technology Stack

| Concern | Choice |
|---|---|
| Views | Razor Views (`.cshtml`) — server-rendered by ASP.NET Core MVC |
| Styles | Bootstrap 5 (CDN) |
| Icons | Bootstrap Icons (CDN) |
| JavaScript | Bootstrap bundle only (for modals, dropdowns, tabs) — no custom JS required |
| Auth | ASP.NET Core cookie authentication — `[Authorize]` attribute protects routes |

Bootstrap CDN links (to be included in the base layout):

```html
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet">
<link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
```

---

## 6. Wireframes

Wireframes are tracked in `/docs/wireframes/` as PNG screenshots committed to GitHub. A Figma link is also stored in the wireframes README.

See: [`/docs/wireframes/README.md`](./wireframes/README.md)

---

## 7. Pages

### 4.1 Home (`/`)

**Purpose:** Entry point and discovery hub.

**Components:**
- Navbar (logo + nav links + login/register buttons if not authenticated, or username + logout if authenticated)
- Hero section with site tagline and a prominent search bar
- "Featured Books" — a Bootstrap card grid (3–4 cards) showing recent or popular books
- "Featured Comics" — same treatment for comics
- Footer with links to About, GitHub, and team names

**Behavior:**
- Search bar is an HTML form that submits to `GET /search?q=<query>&type=all` — `SearchController` handles the query and passes results to a view
- `HomeController.Index()` calls the API for featured items and passes them as a view model
- Unauthenticated users can browse but cannot add items to shelves — "Add to Shelf" links are hidden by `@if (User.Identity.IsAuthenticated)` and the route is protected by `[Authorize]`

**Wireframe:** `docs/wireframes/home.png`

---

### 4.2 Sign Up (`/account/register`)

**Purpose:** New user registration.

**Components:**
- Centered card form (Bootstrap `col-md-5 mx-auto`)
- Fields: Username, Email, Password, Confirm Password
- Submit button: "Create Account"
- Link to login page below the form

**Validation:**
- Server-side: model validation via C# Data Annotations (`[Required]`, `[EmailAddress]`, `[MinLength(8)]`, `[Compare("Password")]`) in the view model
- Client-side: Bootstrap `is-invalid` classes applied by the Razor tag helper when ModelState has errors — no custom JavaScript needed

**Behavior:**
- On submit: `POST /account/register` with form data
- On success: redirect to `/` (logged in)
- On error: display inline error messages using Bootstrap `is-invalid` classes

**Wireframe:** `docs/wireframes/signup.png`

---

### 4.3 Login (`/account/login`)

**Purpose:** Existing user authentication.

**Components:**
- Centered card form (same layout as Sign Up)
- Fields: Email, Password
- Submit button: "Sign In"
- Link to registration page
- "Forgot password?" link (placeholder — not implemented in Sprint 1)

**Behavior:**
- On submit: `POST /account/login` with credentials
- Backend validates via Basic Auth, returns session cookie or token
- On success: redirect to `/` or the previously requested page
- On failure: display "Invalid email or password" banner using Bootstrap `alert-danger`

**Wireframe:** `docs/wireframes/login.png`

---

### 4.4 My Shelves (`/shelves`)

**Purpose:** Display and manage the user's reading lists.

**Requires authentication.** Unauthenticated access redirects to `/account/login`.

**Components:**
- Tab bar (Bootstrap `nav-tabs`) with one tab per shelf: To Read, Reading, Finished, and any custom shelves
- Active tab shows a table or card list of items on that shelf with:
  - Cover image (thumbnail)
  - Title and author/creator
  - Item type badge (Book / Comic — Bootstrap `badge`)
  - Progress bar (`progressPercent`) using Bootstrap `progress`
  - Notes (truncated, expandable)
  - Action buttons: "Update Progress", "Move to Shelf", "Remove"
- "Create New Shelf" button opens a Bootstrap modal with a name input

**Behavior:**
- `ShelvesController.Index()` calls the API for all user shelves and renders the default active tab
- Each tab link navigates to `GET /shelves/{shelfId}` — `ShelvesController.Detail()` fetches and renders that shelf's items
- Progress update and Remove are HTML form POSTs to the controller (using `asp-action` tag helpers); the controller calls the API then redirects back
- Moving an item uses a form POST to `ShelvesController.MoveItem()` which calls the API add + remove in sequence

**Wireframe:** `docs/wireframes/shelves.png`

---

### 4.5 Item Detail (`/items/{id}`)

**Purpose:** View full details for a book or comic and manage shelf membership.

**Components:**
- Two-column layout: cover image (left) + metadata (right)
- Metadata: title, type badge, author/creator, published date, description, average rating (Bootstrap star icons or numeric), tags as Bootstrap `badge` pills
- "Add to Shelf" button — opens a Bootstrap dropdown or modal to pick the target shelf
- If item is already on a shelf: show current shelf name + progress bar + "Update" and "Remove" buttons
- Comic-only fields: Marvel character, story arc, issue number, series, universe
- Book-only fields: ISBN, page count, format, publisher
- "You might also like" section: 3 recommendation cards based on shared tags (fetched from backend)

**Behavior:**
- `ItemsController.Detail(id)` calls the API for the item and the user's shelves, passes both as a view model
- Add to shelf, Update, and Remove are HTML form POSTs to `ShelvesController` (using `asp-controller` / `asp-action` tag helpers) — controller calls the API then redirects back to the item detail page

**Wireframe:** `docs/wireframes/item-detail.png`

---

### 4.6 Recommendations (`/recommendations`)

**Purpose:** Personalized item suggestions based on the user's shelf activity.

**Requires authentication.**

**Components:**
- Page heading: "Recommended for You"
- Short explanation: "Based on tags from your shelves"
- Card grid (Bootstrap `row row-cols-1 row-cols-md-3`) — up to 12 recommendation cards
- Each card shows: cover image, title, type badge, matched tags (up to 3 shown), score (optional, shown as a small percentage or hidden)
- "Add to Shelf" button on each card

**Behavior:**
- `RecommendationsController.Index()` calls `RecommendationService` which calls the API, scores items, and passes the ranked list as a view model
- Empty state is handled in the view with `@if (!Model.Any())` — shows a prompt to add items to shelves
- Add to shelf: HTML form POST to `ShelvesController`, same as Item Detail

**Wireframe:** `docs/wireframes/recommendations.png`

---

## 8. Navigation & Layout

### 5.1 Navbar

Persistent across all pages. Uses Bootstrap `navbar navbar-expand-lg`.

**Unauthenticated:**
```
[Marvel-ous Reads logo]  Books  Comics  [Sign Up]  [Log In]
```

**Authenticated:**
```
[Marvel-ous Reads logo]  Books  Comics  My Shelves  Recommendations  [👤 kalin ▾]
                                                                     ↳ Log Out
```

### 5.2 Base Layout Template

All pages extend a shared base template (`_Layout.cshtml` or `base.html`) that includes:
- `<head>` with Bootstrap CDN, Bootstrap Icons CDN, and a custom `site.css` for theme overrides
- Navbar partial
- `<main>` content block
- Footer partial

---

## 9. Theme & Branding

- **Primary color:** Deep red (`#C0392B`) — Marvel red
- **Secondary color:** Dark navy (`#1A1A2E`) — comic book dark
- **Accent:** Gold (`#F4D03F`) — highlights and badges
- **Font:** System sans-serif stack (Bootstrap default) — no external font imports required
- **Logo:** Text-based logo in the navbar using the color scheme above

Custom CSS overrides live in `wwwroot/css/site.css` (or equivalent static path).

---

## 10. Authentication Behavior

Authentication is handled entirely server-side in C# — there is no client-side credential storage or JavaScript auth code.

**Login flow:**
1. The login form POSTs `email` and `password` to `AccountController.Login()`
2. The controller validates the password against the bcrypt hash in the web server DB
3. On success, `HttpContext.SignInAsync()` issues an encrypted auth cookie
4. The browser sends this cookie automatically on every subsequent request — no JS required

**Route protection:**
All controllers and actions that require a logged-in user are decorated with `[Authorize]`. ASP.NET Core's auth middleware intercepts unauthenticated requests and redirects automatically to `/account/login?returnUrl=...`.

**Logout:**
`AccountController.Logout()` calls `HttpContext.SignOutAsync()`, which clears the cookie, then redirects to `/`.

**Razor conditional rendering:**
Views check `User.Identity.IsAuthenticated` to show/hide elements like the navbar's "My Shelves" link or the "Add to Shelf" button:

```csharp
@if (User.Identity.IsAuthenticated)
{
    <a class="nav-link" asp-controller="Shelves" asp-action="Index">My Shelves</a>
}
```

---

## 11. Responsive Design

The site must be usable on screens ≥ 375px wide (mobile) and ≥ 1024px (desktop).

Bootstrap breakpoints in use:

- Card grids: `col-12 col-sm-6 col-md-4` for item cards
- Shelves layout: full-width tabs, table collapses to card list on mobile
- Item detail: stacked single-column on mobile, two-column on `md+`

---

## 12. Future Phases

- Localize UI labels (genre names, shelf names) via `?locale=es` query param
- A/B test two visual layouts for the Recommendations page
- Add a "Dark Mode" toggle in the navbar (Bootstrap `data-bs-theme`)
- Add a `/admin` panel (hidden behind a role flag) for managing items and tags
