# ShootsNLadders

A small self-hosted daily check-in app. Once a day, each user rates how they feel from 1–10 in three categories (**physical**, **mental** and **emotional**) and can add a short note. The app shows the ratings over time on a graph and will later suggest warning signs like burnout or high mental load.

It's a personal learning project for C# and .NET, built for a handful of users and hosted on a single machine at home.

## Status

Work in progress. The backend is being built first, one piece at a time.

- [x] Core: `Entry` entity, repository and unit-of-work interfaces, `EntryService` business rules
- [x] Infrastructure: EF Core + SQLite (`AppDbContext`, table configuration, `EntryRepository`)
- [x] API project and startup configuration (`Program.cs`)
- [ ] DTOs and `EntriesController` (save and list entries)
- [ ] First database migration
- [ ] Tests

See the roadmap below for what comes after that.

## Tech stack

| Part | Choice |
|---|---|
| Backend | C# / .NET 11, ASP.NET Core Web API (controllers) |
| Database | SQLite through EF Core 11 |
| Frontend (later) | React + Vite + TypeScript as an installable web app (PWA) |
| Hosting (later) | A 2017 MacBook Pro running Ubuntu Server, behind Caddy (HTTPS), DuckDNS and port forwarding |

## Project layout

```
server/
  src/
    ShootsNLadders.Api/             HTTP layer: controllers, DTOs, Program.cs
    ShootsNLadders.Core/            entities, business rules, interfaces (references nothing)
    ShootsNLadders.Infrastructure/  EF Core + SQLite: DbContext, table config, repositories
  tests/                            (not created yet)
```

Dependencies point inwards: **Api → Core ← Infrastructure**. Core doesn't know about EF Core or HTTP.

## Running it locally

**Requirements:** .NET 11 SDK (`dotnet --version` should show 11.x).

1. Create `server/.env` (it's git-ignored) with the database connection string:

   ```bash
   ConnectionStrings__Default=Data Source=shootsnladders.db
   ```

2. Build and run the API:

   ```bash
   cd server/src/ShootsNLadders.Api
   dotnet run
   ```

   The API listens on `http://localhost:5035`. In development, the OpenAPI description is at `http://localhost:5035/openapi/v1.json`.

The app refuses to start if `ConnectionStrings__Default` is missing. There are no endpoints and no database migration yet, so the app starts but doesn't do anything useful.

## Configuration

- `appsettings.json` (committed): settings that are the same everywhere and not secret, like logging levels.
- `server/.env` (git-ignored): per-machine values and secrets, like the connection string. Loaded with the DotNetEnv package in development. In production, systemd's `EnvironmentFile=` loads it instead.

## Roadmap

1. Skeleton and file layout
2. Save and list daily entries ← *in progress*
3. Edit and delete past days
4. Graph data: ratings over a date range
5. Accounts and invite codes
6. Rule-based insights (burnout, mental load)
7. Daily push reminders
8. Deployment to the home server
9. Later: offline logging, backups, ML.NET predictions

## Docs for contributors

- [`CLAUDE.md`](CLAUDE.md): project rules and every design decision made so far
- [`redflags.md`](redflags.md): known limitations and planned fixes
