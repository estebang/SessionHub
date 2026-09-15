# Copilot Instructions for SessionHub

Use these rules when generating or editing code in this repository.

## Architecture principles

- Keep the app simple and demo-friendly: React UI -> API endpoints -> service layer -> EF Core DbContext -> SQLite.
- Preserve clear boundaries:
  - `SessionHub.Web` handles presentation and API calls only.
  - `SessionHub.Api` handles domain, data access, and API contracts.
- Prefer lightweight services over extra abstraction layers; do not introduce repository/unit-of-work patterns unless explicitly requested.
- Keep DTOs as the API contract and keep EF entities internal to the API/data layer.

## Naming conventions

- Use `PascalCase` for C# types, methods, enums, and public properties.
- Use `camelCase` for local variables and method parameters.
- Use private fields with leading underscore (for example `_dbContext`).
- Keep async methods suffixed with `Async`.
- In TypeScript/React, use `PascalCase` for component/type names and `camelCase` for variables/functions.
- Prefer explicit, domain-oriented names (`SessionService`, `SpeakerDto`, `GetSessionsAsync`) over generic names.

## API design conventions

- Keep endpoints under `/api/*` and follow existing route style:
  - `GET /api/sessions`
  - `GET /api/sessions/{id:int}`
  - `GET /api/speakers`
  - `GET /api/speakers/{id:int}`
  - `GET /api/favorites`
  - `POST /api/favorites`
  - `DELETE /api/favorites/{sessionId:int}`
- For single-resource lookups, return `404` when missing (`Results.NotFound()`).
- Return DTOs, not EF entities.
- For read-only queries, use `AsNoTracking()`.
- Keep database query order deterministic (for example by start time or last name).

## React component conventions

- Prefer functional components with hooks.
- Keep state minimal and derive computed values with `useMemo` when it improves clarity.
- Fetch API data inside `useEffect`; handle loading and empty states explicitly.
- Keep UI code readable and practical; avoid over-engineering with premature abstractions.
- Keep styles in existing CSS files and follow the current class-based styling approach.

## Testing expectations

- New business logic should include backend unit tests in the test project when available.
- New UI behavior should include frontend tests (component or Playwright) when test infrastructure exists.
- Focus tests on user-visible behavior and API/service contracts.
- Cover happy path plus at least one failure/edge case for new behavior.

## Logging expectations

- Log meaningful events at API boundaries and failure points.
- Use structured logging with placeholders; do not build log messages with string concatenation.
- Do not log secrets, tokens, or sensitive personal data.
- Keep logging noise low in normal success paths.

## Documentation expectations

- Update `README.md` when setup, run steps, or public behavior changes.
- Update files in `docs/` when architecture or folder conventions change.
- Keep docs concise, practical, and aligned with the demo storytelling goal.
- Include endpoint changes in API documentation sections when routes/contracts change.