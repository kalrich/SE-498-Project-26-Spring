# Marvel•ous Reads

> Marvel-themed comic discovery and reading tracker — SE-498 Software Engineering Capstone, Spring 2026

Marvel•ous Reads is a full-stack web application for browsing Marvel-style comics, organizing a personal comic library, tracking reading progress, checking comics out and returning them, favoriting comics, writing reviews, and exploring Marvel character content.

The project is implemented as an ASP.NET Core MVC WebServer that communicates with an ASP.NET Core Web API backed by PostgreSQL.

---

# Team

| Name | Email |
|---|---|
| Kalin Richardson | kalrichardson@chapman.edu |
| Joshua Fisher | joshfisher@chapman.edu |
| Alexandra Fomina | fomina@chapman.edu |

---

# Project Links

| Resource | Link |
|---|---|
| GitHub Repository | https://github.com/kalrich/SE-498-Project-26-Spring |
| Jira Board | https://marvel-books.atlassian.net/jira/software/projects/SCRUM/boards/1/backlog |
| Figma Wireframes | https://www.figma.com/design/VsPh5szOQsMp7U9XpHzCJE/Marvel-Books |
| Team 3 DC API | https://github.com/LaurelLatt/Team3-DCBooks |

---

# Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET 10) |
| Frontend | ASP.NET Core MVC + Razor Views |
| REST API | ASP.NET Core Web API |
| ORM | Entity Framework Core + Npgsql |
| Database | PostgreSQL 16 |
| Authentication | WebServer Cookie Auth + WebApi JWT Bearer Support |
| Styling | Bootstrap 5 + Custom CSS |
| Testing | xUnit |
| API Docs | Swagger/OpenAPI |
| Containerization | Docker + Docker Compose |
| IDE | JetBrains Rider |

---

# Architecture

```txt
Browser
  |
  v
Project498.WebServer
ASP.NET Core MVC + Razor Views
Cookie/session browser experience
Typed HttpClient API services
  |
  +----> Project498.WebApi
  |         |
  |         v
  |      PostgreSQL 16
  |      Docker Container
  |
  +----> Team3-DCBooks API
```

The WebServer acts as a Backend-for-Frontend (BFF) and communicates with the REST API for comic and user data.

The project also integrates with Team 3’s external DC Comics API to retrieve DC-related comic and character data.

---

# Current Features

## User Accounts

- Sign up
- Login
- Logout
- Profile lookup and profile updates

---

## Comic Browsing

- Home page discovery sections
- Explore/search page
- Genre filtering
- Availability filtering
- Comic detail pages
- Series-based comic organization using:
    - `SeriesName`
    - `VolumeNumber`
    - `IssueNumber`

Example:

```txt
Human Torch
Volume 1
Issue 2
```

---

## Shelves and Reading Progress

- Add comics to shelves
- Track reading progress percentage
- Track current reading page
- Reader page for continuing comics
- Reading history records ordered by recent activity

User-specific comic data is stored in the `UserComics` table, allowing:

- unique reading progress per user
- personalized shelves
- independent user libraries

---

## Checkouts

- Checkout flow for comics
- Active checkout list
- Checkout confirmation page
- Due date and overdue status support
- Return workflow

---

## Favorites and Reviews

- Favorite/unfavorite comics
- Retrieve a user's favorite comics
- Add or update a review for a comic
- Retrieve reviews by comic or by user
- Average rating support

---

## Character Content

- Marvel character list and detail pages
- Character image records
- DC character service integration through Team 3's REST API
- Cross-project API communication using `DcCharacterService`

---

# Containerization

The REST API and PostgreSQL database run inside Docker containers using Docker Compose.

The project uses PostgreSQL 16 for stable local development and persistent relational storage.

---

# Prerequisites

Install the following before running the project:

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

To be able to run DC External API need to follow instructions on this link

- Team 3 DC API running locally/configured
- https://github.com/LaurelLatt/Team3-DCBooks/blob/main/README.md

---

# Running the Project

## 1. Clone the Repository

```bash
git clone git@github.com:kalrich/SE-498-Project-26-Spring.git
cd SE-498-Project-26-Spring
```

---

## 2. Start the Development Environment

Run:

```bash
./tools/start-dev.sh
```

The startup script automatically:

- starts PostgreSQL
- starts the WebApi container
- initializes the database schema
- loads seed data
- configures environment variables
- launches the local development environment

---

# Access Points

| Service | URL |
|---|---|
| Web Application | http://localhost:5150 |
| Swagger API Docs | http://localhost:8082/swagger |

---

# External API Integration

Marvel•ous Reads integrates with Team 3's DC Comics API project:

https://github.com/LaurelLatt/Team3-DCBooks

The WebServer includes a configured `DcCharacterService`:

```csharp
builder.Services.AddHttpClient<IDcCharacterService, DcCharacterService>(client =>
{
    client.BaseAddress = new Uri(dcApiUrl);
});
```

Configuration:

```txt
DcComicsApiUrl=http://localhost:5100
```

This integration demonstrates inter-team REST API communication and external service consumption.

---

# Main API Endpoint Groups

| Endpoint Group | Purpose |
|---|---|
| `/api/Auth` | Login and signup |
| `/api/Comics` | Comic browsing, filtering, detail, featured/trending/recommended sections |
| `/api/Shelves` | Shelf management and reading progress |
| `/api/Checkouts` | Checkout and return workflow |
| `/api/Favorites` | Favorite comic management |
| `/api/ComicReviews` | Comic reviews and ratings |
| `/api/ReadingHistory` | Recently read comic history |
| `/api/marvel-characters` | Marvel character list/detail data |
| `/api/character-images` | Character image records |
| `/api/Users` | User lookup and profile updates |

Full request/response documentation is available in:

```txt
api_contracts.md
```

---

# Verify Setup

After startup:

- Swagger loads at `http://localhost:8082/swagger`
- `GET /api/Comics` returns seeded comic JSON
- Login/signup works correctly
- Explore/search displays comics
- Comic details load correctly
- Shelves, reader progress, favorites, reviews, and checkouts function for logged-in users
- DC integration loads when the Team 3 API is running

---

# Running Tests

From the `/src` directory:

```bash
dotnet test
```

---

# Database Design

Current database tables:

- `Users`
- `Comics`
- `UserComics`
- `Checkouts`
- `FavoriteComics`
- `ReadingHistories`
- `ComicReviews`
- `MarvelCharacters`
- `CharacterImages`

---

## Important Relationships

```txt
Users 1 ──< UserComics >── 1 Comics
Users 1 ──< Checkouts >── 1 Comics
Users 1 ──< FavoriteComics >── 1 Comics
Users 1 ──< ReadingHistories >── 1 Comics
Users 1 ──< ComicReviews >── 1 Comics
```

`UserComics`, `FavoriteComics`, `ReadingHistories`, and `ComicReviews` enforce unique user/comic state relationships.

---

# Repository Structure

```txt
SE-498-Project-26-Spring/
├── src/
│   ├── Project498.WebApi/
│   ├── Project498.WebApi.Tests/
│   ├── Project498.WebServer/
│   ├── tools/
│   └── compose.yaml
├── docs/
│   ├── api-spec.md
│   ├── api_contracts.md
│   ├── backend-spec.md
│   ├── frontend-spec.md
│   └── database-schema.md
├── .github/
│   └── workflows/
└── README.md
```

---

# Documentation Files

| File | Purpose |
|---|---|
| `README.md` | Project overview and setup instructions |
| `api-spec.md` | System-level REST API architecture/specification |
| `api_contracts.md` | Endpoint-level request/response contracts |
| `backend-spec.md` | Website backend/WebServer specification |
| `frontend-spec.md` | Razor UI/frontend specification |
| `database-schema.md` | Database schema and relationships |

---

# Branches

| Branch | Purpose |
|---|---|
| `main` | Stable deployable branch |
| `name` | Team member development branches |
| `docs` | Documentation and wireframes |

---

# Future Improvements

- Automatic database migrations during startup
- Cloud deployment
- Admin dashboard for comic and character management
- Full-text search
- Pagination for large comic/character lists
- Stronger DTO separation for sensitive fields
- OAuth or SSO authentication
- Expanded recommendation logic