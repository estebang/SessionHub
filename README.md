# SessionHub

SessionHub is a demo conference session planner built for a VSLive San Diego talk. It is intentionally small, approachable, and realistic enough to show the full lifecycle of a modern app: API, data model, UI, and deployment story.

## Goals

- Browse conference sessions
- View session details
- View speaker information
- Demonstrate a clean, demo-friendly architecture
- Show the progression from starter app to deployed application across multiple branches

## Tech stack

### Backend
- ASP.NET Core 9 Web API
- Entity Framework Core
- SQLite

### Frontend
- React
- TypeScript
- Vite

## Included features

- Browse conference sessions
- View session details
- View speaker information
- Favorite and unfavorite sessions
- Browse favorites from a dedicated favorites view

## Not included

This demo is intentionally scoped to a simpler app and does not include:

- Personal schedules
- Tests
- GitHub Actions
- Infrastructure as Code

## Suggested branch flow

```text
START
 |
 v
demo/00-starter
 |
 v
demo/01-plan-complete
 |
 v
demo/02-feature-complete
 |
 v
demo/03-tests-complete
 |
 v
demo/04-production-ready
 |
 v
demo/05-deployed
```

## Run locally

### 1. Start the API

```bash
cd SessionHub.Api
dotnet restore
dotnet run
```

The API runs on:
- https://localhost:7208
- http://localhost:5120

### 2. Start the frontend

```bash
cd SessionHub.Web
npm install
npm run dev
```

The React app runs at:
- http://localhost:5173

## API endpoints

- GET /api/sessions
- GET /api/sessions/{id}
- GET /api/speakers
- GET /api/speakers/{id}
- GET /api/favorites
- POST /api/favorites
- DELETE /api/favorites/{sessionId}

## Favorites feature

The app includes a lightweight favorites flow for the demo user:

- favorite a session from the session list or details panel
- remove a favorite from the same UI
- show a favorites-only view with a session count
- prevent duplicate favorites by enforcing a unique index on the session id

## Database

The app uses SQLite and creates the database automatically on startup with EF Core migrations. Seed data includes 15 sessions and 10 speakers. Favorite records are stored in a dedicated table with a unique index on the session id to prevent duplicates.

## Architecture overview

See the project docs in the docs folder for a simple architectural breakdown and folder-by-folder explanation.

## Azure deployment

SessionHub is prepared for Azure deployment with:

- Azure App Service for the API
- Azure Application Insights for telemetry
- Azure SQL for relational data persistence in place of the local SQLite database
- GitHub Actions using OIDC federation rather than stored Azure credentials

See [docs/azure-deployment.md](docs/azure-deployment.md) and [infra/README.md](infra/README.md) for architecture details, infrastructure templates, and deployment guidance.
