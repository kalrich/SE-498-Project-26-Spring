# Marvel•ous Reads

> A Marvel + Books themed web application — SE-498 Software Engineering Capstone, Spring 2026

Marvel•ous Reads is a full-stack book tracking and discovery platform themed around Marvel Comics and characters. Users can browse books and comics, manage personal reading shelves, and receive personalised recommendations based on their reading history.

---

## Team

| Name | Email |
|---|---|
| Kalin Richardson  | kalrichardson@chapman.edu |
| Joshua Fisher | joshfisher@chapman.edu |
| Alexandra Fomina | fomina@chapman.edu |

---

## Project Links

| Resource | Link |
|---|---|
| **GitHub Repository** | https://github.com/kalrich/SE-498-Project-26-Spring |
| **Jira Board** | https://marvel-books.atlassian.net/jira/software/projects/SCRUM/boards/1/backlog?atlOrigin=eyJpIjoiNDExOTYzMzg2NWRlNDU0YzhjNmUwNTJiOTc4MDRjYzkiLCJwIjoiaiJ9 |
| **Figma Wireframes** | https://www.figma.com/design/VsPh5szOQsMp7U9XpHzCJE/Marvel-Books?node-id=0-1&t=QNceDbpoARzMJDQ5-1 |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET 10) |
| Web Framework | ASP.NET Core MVC (Razor Views) |
| REST API | ASP.NET Core Web API |
| ORM | Entity Framework Core + Npgsql |
| Database | PostgreSQL 16 (Docker) |
| Authentication | JWT Bearer (API) · ASP.NET Core Cookie Auth (Web) |
| Password Hashing | BCrypt |
| Frontend Styling | Bootstrap 5 (CDN) |
| Testing | xUnit |
| HTTP Integration Tests | httpyac |
| API Docs | Swagger / OpenAPI |
| Containerisation | Docker + Docker Compose |
| CI/CD | GitHub Actions |
| IDE | JetBrains Rider |

---

## Architecture

```
Browser
  └── Project498.WebServer  (ASP.NET Core MVC · Razor Views · Cookie Auth)
        └── Project498.WebApi  (REST API · JWT Bearer)
              └── PostgreSQL (Docker)
```

The website backend acts as a BFF (Backend for Frontend) — it handles browser sessions, applies business logic (recommendation scoring, shelf constraints), and proxies authenticated requests to the API.

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker + Docker Compose
- JetBrains Rider (or VS Code with C# extension)

### Run Locally

```bash
# Clone the repo
git clone git@github.com:kalrich/SE-498-Project-26-Spring.git
cd SE-498-Project-26-Spring/src

# Start Postgres + API
docker compose up --build

# Run tests
dotnet test
```

Swagger UI available at: `http://localhost:8080/swagger`

---

## Repository Structure

```
SE-498-Project-26-Spring/
├── src/
│   ├── Project498.WebApi/          # REST API (JWT auth, EF Core, Swagger)
│   ├── Project498.WebApi.Tests/    # xUnit unit tests for API
│   ├── Project498.WebServer/       # Website BFF (Razor Views, Cookie auth)
│   ├── Project498.WebServer.Tests/ # xUnit unit tests for web server
│   └── compose.yaml                # Docker Compose (API + WebServer + Postgres DBs)
├── docs/
│   ├── api-spec.md                 # REST API specification
│   ├── backend-spec.md             # Website backend (BFF) specification
│   ├── frontend-spec.md            # Frontend (Razor Views) specification
│   └── wireframes/
│       ├── screenshots of all frames in Figma
├── .github/
│   └── workflows/
│       └── main.yml                # CI: dotnet test + httpyac integration tests
└── README.md
```

---

## Branches

| Branch | Purpose |
|---|---|
| `main` | Stable, deployable code — merges via PR only |
| `name` | One branch per Team Member |
| `docs` | For all documentation and screenshots |

---

## Wireframes

Greyscale, low-fidelity wireframes.

**Figma file:** https://www.figma.com/design/jDMxAmvSgfj2xzEWyogByz/Marvel-Books---Test

---

## Specifications

| Document | Description |
|---|---|
| [`docs/api-spec.md`](docs/api-spec.md) | REST API — endpoints, auth, DB schema, error format |
| [`docs/backend-spec.md`](docs/backend-spec.md) | BFF Web Server — session auth, business logic, routing |
| [`docs/frontend-spec.md`](docs/frontend-spec.md) | Razor Views — pages, layout, navigation, auth behaviour |

Each spec follows: Overview → Scope → Functional Requirements → Non-Functional Requirements → Technical Detail.

---
