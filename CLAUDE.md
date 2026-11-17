# CLAUDE.md

## Rules for working on this project

- **Start every response with "jacob".**
- **Never write code before discussing it with jacob**: explain what you plan to write and the possible alternatives, then wait for approval.
- **Git is jacob's job.** Do not commit, branch, push or otherwise change git history unless jacob says so. Commit and push only when jacob explicitly asks, never automatically after a step. Never add a "Co-Authored-By: Claude" line or any other AI attribution to commits or PRs.
- **Backend first.** Do not build any frontend or UI until jacob says the design is decided.
- **Small steps.** Build bottom-up, one feature at a time. Write one file at a time (or a few only if they're the same kind, e.g. two DTOs) and wait for jacob's approval before the next. Keep plans high-level and decide details only when building each step.
- This is a personal learning project (C# / .NET practice). Explain the patterns you use.

## What the app is

A daily check-in: each user rates how they feel from 1–10 in three categories (physical, mental, emotional), with an optional note. The app shows a graph over time and, later, suggestions about burnout and mental load.

## Stack (decided)

- Backend: C#, .NET 11 (SDK 11.0.100), ASP.NET Core Web API with **controllers** (not minimal APIs)
- Database: SQLite through EF Core
- Frontend (later, design not decided yet): React + Vite + TypeScript as an installable web app (PWA). Phones are a mix of iPhone and Android.
- Insights: rule-based first, possibly ML.NET later

## Hosting (decided, not set up yet)

A 2017 MacBook Pro (i5), wiped and running Ubuntu Server as a dedicated server. Public access through router port forwarding, DuckDNS and Let's Encrypt HTTPS (likely through Caddy). Before deploying, check that the ISP gives a real public IP (not CGNAT).

## Feature roadmap

1. Skeleton: rough file layout only
2. Entries: save and list daily entries
3. Edit and delete past days
4. Graph data: ratings over a date range
5. Accounts and invite codes
6. Rule-based insights
7. Daily push reminders
8. Deployment to the MacBook server
9. Later: offline logging, backups, ML.NET predictions

## Project layout (layered / clean architecture)

```
server/
  src/
    ShootsNLadders.Api/             HTTP layer: controllers, DTOs, Program.cs, DI setup
      Controllers/
      Dtos/                       request/response shapes (never expose entities directly)
    ShootsNLadders.Core/            domain: entities, interfaces, business rules; no dependencies on other projects
      Common/                     shared building blocks (Result, Error)
      Entities/
      Interfaces/                 e.g. repository interfaces, implemented in Infrastructure
      Services/                   business logic (entries, later insights)
        Inputs/                   input records services receive (e.g. CreateEntryInput)
    ShootsNLadders.Infrastructure/  EF Core + SQLite: DbContext, migrations, repository implementations
      Data/
      Repositories/
  tests/
    ShootsNLadders.Core.Tests/      unit tests for business rules
    ShootsNLadders.Api.Tests/       integration tests against the API
```

Dependency direction: Api → Core ← Infrastructure (Api also references Infrastructure only to register it in DI). Core references nothing.

Only the Core project exists so far (`ShootsNLadders.Core.csproj`); the other folders hold `.gitkeep` placeholders until their projects are created. "ShootsNLadders" is the project name (from the folder name "Shoots and laders").

## Design decisions

- **Backend is the source of truth.** Never trust the client: validate every input on the server (DTO validation attributes) and back it up with database constraints.
- **Entries:** one entry per day (unique on date; per user once accounts exist). Ratings are whole numbers 1–10. `Date` is a `DateOnly`, sent by the client and validated by the server (no future dates). No `UserId` until step 5 (added then with a migration).
- **Total** (physical + mental + emotional) is a read-only property on the `Entry` entity (`Total => Physical + Mental + Emotional`), computed at runtime and never stored in the database (EF Core ignores it). See `redflags.md` for the possible future fix (SQLite generated column).
- **Data access:** specific repositories (no generic `IRepository<T>`) + Unit of Work. Interfaces in Core, EF Core implementations in Infrastructure. Repository methods only stage changes; services call `IUnitOfWork.SaveChangesAsync()` once per operation (AppDbContext implements `IUnitOfWork`). Async methods take an optional `CancellationToken`; lists are returned as `IReadOnlyList<T>`; lookups return null when not found.
- **Service results:** services return a `Result<T>` (success value or an `Error` with a type: Validation, NotFound, Conflict) instead of throwing for expected failures. Controllers map error types to HTTP status codes. `Result`/`Error` live in `Core/Common/`.
- **Services have interfaces** (e.g. `IEntryService`) in `Core/Interfaces/`; controllers depend on the interface.
- **Time:** never call `DateTime.UtcNow` directly; inject .NET's `TimeProvider` so tests can use `FakeTimeProvider`.
- **Future dates:** rejected if after UTC today + 1 day (see `redflags.md`; per-user time zones in step 5).
- **Member order in classes:** data first (fields, then properties), then constructors, then methods. Group with `// ---- Data ----` / `// ---- Construction ----` comments where it helps.
- **Tests:** xUnit.
- **Trying the API:** no Swagger UI. Use Bruno (installed) and the VS Code REST Client extension (`.http` files, installed).

## Known limitations

Track known limitations and possible future fixes in `redflags.md`. Add an entry whenever we knowingly choose a simpler option.

## Tools

- Editor: VS Code
- Bruno 4.3 in /Applications; REST Client extension (humao.rest-client) in VS Code

## Commands

None yet. Add build, run and test commands here once the project exists.
