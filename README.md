# Marvel•ous Reads

> A Marvel + Books themed web application — SE-498 Software Engineering Capstone, Spring 2026

Marvel•ous Reads is a full-stack comic and book tracking platform themed around Marvel Comics and characters. Users can browse comics, organize reading shelves, track progress, and receive recommendations.

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

---

# Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET 10) |
| Web Framework | ASP.NET Core MVC |
| REST API | ASP.NET Core Web API |
| ORM | Entity Framework Core + Npgsql |
| Database | PostgreSQL 16 |
| Authentication | Cookie Auth + JWT |
| Styling | Bootstrap 5 |
| Testing | xUnit |
| API Docs | Swagger/OpenAPI |
| Containerization | Docker + Docker Compose |
| IDE | JetBrains Rider |

---

# Architecture

```txt
Browser
  └── Project498.WebServer
        └── Project498.WebApi
              └── PostgreSQL 16 (Docker Container)
```

The WebServer acts as a Backend-for-Frontend (BFF) and communicates with the REST API for comic and user data.

---

# Comic Organization System

Comics are organized using:

- `SeriesName`
- `VolumeNumber`
- `IssueNumber`

Example:

```txt
Human Torch
Volume 1
Issue 2
```

Each comic has a unique database ID.

User-specific comic data is stored in the `UserComics` table, allowing:

- unique reading progress per user
- personalized shelves
- independent user libraries

---

# Containerization

The REST API and PostgreSQL database run inside Docker containers using Docker Compose.

The project uses PostgreSQL 16 for compatibility and stable local development.

---

# Prerequisites

Install the following before running the project:

- [.NET 10 SDK](https://dotnet.microsoft.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Git

---

# Running the Project

## 1. Clone Repository

```bash
git clone git@github.com:kalrich/SE-498-Project-26-Spring.git
```

---

## 2. Navigate Into Source Folder

```bash
cd SE-498-Project-26-Spring/src
```

---

# Start Docker Containers

This starts:

- PostgreSQL container
- Web API container

```bash
docker compose up --build
```

---

# Load Database Schema + Seed Data

Open a new terminal window while Docker is still running.

Run:

```bash
docker exec -it src-db-1 psql -U postgres -d project498
```

You should now see:

```txt
project498=#
```

---

# Load schema.sql

Paste the contents of:

```txt
Project498.WebApi/Database/schema.sql
```

into the PostgreSQL shell.

You should see:

```txt
CREATE TABLE
CREATE TABLE
CREATE TABLE
```

---

# Load seed.sql

Paste the contents of:

```txt
Project498.WebApi/Database/seed.sql
```

into the PostgreSQL shell.

You should see:

```txt
INSERT 0 3
INSERT 0 9
INSERT 0 9
```

---

# Access Swagger API

Swagger UI:

```txt
http://localhost:8080/swagger
```

---

# Available API Endpoints

| Endpoint | Description |
|---|---|
| `GET /api/comics` | Retrieve all comics |
| `GET /api/comics/{id}` | Retrieve comic by ID |
| `GET /api/comics/series/{seriesName}` | Retrieve comics by series |
| `GET /api/comics/genres` | Retrieve genres |
| `GET /api/comics/featured` | Retrieve featured comics |

---

# Running the WebServer

Open another terminal:

```bash
cd SE-498-Project-26-Spring/src/Project498.WebServer
```

If using the Dockerized API:

```bash
ApiBaseUrl=http://localhost:8080/ dotnet run
```

If using a locally running API:

```bash
dotnet run
```

The website will typically run at:

```txt
http://localhost:5150
```

---

# Verify Setup

After startup:

- Swagger should load at `http://localhost:8080/swagger`
- `GET /api/comics` should return seeded comic JSON
- WebServer login should function correctly
- Series endpoints should return ordered comic issues

---

# Running Tests

From the `/src` directory:

```bash
dotnet test
```

---

# Database Design

## Comics Table

Stores:

- comic metadata
- series organization
- issue numbers
- comic file references

---

## UserComics Table

Stores:

- shelf placement
- reading progress
- user-specific comic relationships

Example:

```txt
User 1 → Comic 3 → 35% complete
```

This prevents one user's reading activity from affecting another user's library.

---

# Repository Structure

```txt
SE-498-Project-26-Spring/
├── src/
│   ├── Project498.WebApi/
│   ├── Project498.WebApi.Tests/
│   ├── Project498.WebServer/
│   ├── Project498.WebServer.Tests/
│   └── compose.yaml
├── docs/
│   ├── api-spec.md
│   ├── backend-spec.md
│   ├── frontend-spec.md
│   └── wireframes/
├── .github/
│   └── workflows/
└── README.md
```

---

# Branches

| Branch | Purpose |
|---|---|
| `main` | Stable deployable branch |
| `name` | Team member development branches |
| `docs` | Documentation and wireframes |

---

# Future Improvements

- Automatic DB migrations
- Cloud deployment
- Comic recommendation engine
- Ratings and favorites
- Admin upload dashboard
- Full-text search
- OAuth authentication