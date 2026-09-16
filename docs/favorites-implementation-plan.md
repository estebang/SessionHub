# Favorites Implementation Plan

## Current-state analysis

SessionHub currently follows a linear React UI -> minimal API endpoint -> service -> EF Core DbContext -> SQLite flow. `Session` and `Speaker` are the only entities. API contracts are records in `Dtos`, read queries use `AsNoTracking()`, and collection ordering is deterministic. The React app keeps all state in `App.tsx`, fetches sessions and speakers on mount, and has no router or test framework. The repository also has no authentication or user model.

The implementation should preserve those boundaries. Use one small `FavoriteService`, a `Favorite` EF entity, DTO responses based on the existing session summary contract, and local React state. Do not introduce a repository layer, state-management library, or full identity system.

## Design decisions

- Scope favorites to an anonymous browser profile identified by a UUID in `X-Client-Id`.
- Persist favorites in SQLite so they survive refreshes and API restarts.
- Model `Favorite` with `ClientId`, `SessionId`, `CreatedAtUtc`, and a `Session` navigation property.
- Use `(ClientId, SessionId)` as the composite primary key. This is both the lookup key and the database-level duplicate guard.
- Use idempotent `PUT` and `DELETE` routes. A repeated add or remove returns `204 No Content`.
- Return favorite sessions from `GET /api/favorites` as the existing `SessionSummaryDto` shape, ordered by `StartTimeUtc` and then `Id`.
- Keep `SessionSummaryDto` free of per-user state; the frontend derives `isFavorite` from the favorites collection.
- Add All sessions/Favorites tabs to the existing Sessions panel rather than adding client-side routing.

## Ordered tasks

### 1. Establish backend test infrastructure

Create `SessionHub.Api.Tests` and add it to `SessionHub.sln`. Reference `SessionHub.Api` and add xUnit, `Microsoft.NET.Test.Sdk`, EF Core SQLite, and ASP.NET Core integration-test dependencies as needed.

Make `Program` discoverable to `WebApplicationFactory<Program>` with `public partial class Program`. Configure integration tests to replace the production DbContext with an isolated temporary SQLite database. Prefer SQLite over EF's in-memory provider so composite keys, foreign keys, and uniqueness behavior are exercised accurately.

Initial tests should describe the API contract before implementation: empty list, successful add/list/remove, unknown session, missing or malformed client ID, repeated add, and concurrent duplicate add.

### 2. Add the Favorite domain model

Create `SessionHub.Api/Models/Favorite.cs` with:

- required `string ClientId`
- `int SessionId`
- required `Session Session` navigation
- `DateTimeOffset CreatedAtUtc`

Add `ICollection<Favorite> Favorites` to `Session` only if it is useful for EF relationship configuration; avoid exposing the collection through DTOs.

### 3. Configure EF Core persistence

Update `ConferenceDbContext` to expose `DbSet<Favorite> Favorites` and configure:

- composite primary key on `(ClientId, SessionId)`
- required `ClientId` with maximum length 36
- required `CreatedAtUtc`
- foreign key from `Favorite.SessionId` to `Session.Id`
- cascade delete when a session is deleted
- an index on `SessionId` for foreign-key maintenance and session-centric cleanup

The composite primary key already supports efficient reads by `ClientId`; do not add a redundant client-only index.

### 4. Generate and verify the migration

Create an EF Core migration named `AddFavorites`. It should create `Favorites` with the composite primary key, session foreign key, `SessionId` index, and cascade behavior, then update `ConferenceDbContextModelSnapshot`.

Verify both directions against a disposable database:

1. Apply all migrations from an empty database.
2. Confirm seeded sessions still load.
3. Insert one favorite and confirm a duplicate key cannot be inserted.
4. Delete a session in test setup and confirm dependent favorites are removed.
5. Roll back the `AddFavorites` migration and confirm only the Favorites table is removed.

### 5. Add favorites service behavior

Create `SessionHub.Api/Services/FavoriteService.cs` and register it as scoped in `Program.cs`. Keep EF access out of endpoint lambdas.

Implement:

- `GetFavoritesAsync(string clientId)`: read-only query joining sessions and speakers, ordered by `StartTimeUtc` then `Id`, projected to `SessionSummaryDto`.
- `AddFavoriteAsync(string clientId, int sessionId)`: verify session existence, return a not-found result when absent, and insert only when the key is not already present.
- `RemoveFavoriteAsync(string clientId, int sessionId)`: remove by composite key when present and otherwise complete successfully.

The pre-insert existence check improves the normal path but does not guarantee uniqueness. Catch only the provider-specific duplicate-key race around `SaveChangesAsync`, verify that the target favorite now exists, and treat it as success; do not swallow unrelated database failures.

Use a small domain-oriented result enum such as `AddFavoriteResult.Added`, `AlreadyExists`, and `SessionNotFound` if it keeps endpoint mapping explicit. Do not return EF entities.

### 6. Map and validate API endpoints

Add a route group at `/api/favorites` in `Program.cs` with endpoints named for OpenAPI:

| Endpoint | Service call | Response |
| --- | --- | --- |
| `GET /api/favorites` | `GetFavoritesAsync` | `200` plus `SessionSummaryDto[]` |
| `PUT /api/favorites/{sessionId:int}` | `AddFavoriteAsync` | `204`; `404` for an unknown session |
| `DELETE /api/favorites/{sessionId:int}` | `RemoveFavoriteAsync` | `204` whether or not the row existed |

Add one focused helper or endpoint filter that reads `X-Client-Id`, validates it as a GUID, normalizes it with format `D`, and returns a `400` validation problem when invalid. Reuse it across all three endpoints. Validate that `sessionId > 0` before calling the service.

Log validation failures and unexpected persistence failures at the API boundary with structured placeholders. Do not log the raw browser identifier or add success-path noise.

### 7. Add frontend client identity and API helpers

Create `SessionHub.Web/src/favoritesApi.ts` to keep header construction and response handling out of the component. It should:

- read a versioned key such as `sessionhub.client-id.v1` from `localStorage`
- validate the stored UUID or replace it with `crypto.randomUUID()`
- send `X-Client-Id` for all favorites calls
- expose typed `getFavorites`, `addFavorite`, and `removeFavorite` functions
- throw a useful error for non-success responses

If `localStorage` is unavailable, retain one generated ID for the page lifetime and surface that favorites will not persist across refreshes. Do not send a request body for add or remove.

### 8. Integrate favorite state into the React UI

Update `App.tsx` to load favorites with the existing sessions and speakers. Maintain:

- favorite session IDs as a `Set<number>` derived from the GET response
- current view: `all` or `favorites`
- pending session IDs to prevent overlapping mutations per session
- a favorites-specific loading/error state so failure does not block browsing all sessions

Implement a mutation handler that waits for API confirmation before changing the favorite set. Derive the visible session list from the current view and favorite IDs. When removing the last visible favorite, show the empty state and ensure the selected session is either a visible session or `null`.

Render an icon button with an accessible name of `Add {session title} to favorites` or `Remove {session title} from favorites`, `aria-pressed`, a tooltip, and a disabled/pending state. Place it where it can be used from both the session list and detail header without nesting a button inside the existing session-selection button; adjust list-item markup accordingly.

Add an All sessions/Favorites segmented control with counts in the Sessions panel header. Do not add explanatory feature text to the page.

### 9. Add styles and responsive behavior

Update `App.css` using the existing class-based visual language. Add stable dimensions for the icon button and segmented control so state changes do not shift layout. Define visible hover, focus, active, disabled, and pressed states with sufficient contrast.

Verify that the new list-item action is reachable by keyboard, does not trigger session selection unintentionally, does not overlap titles on narrow screens, and preserves the current desktop/mobile layout.

### 10. Add frontend test infrastructure and behavior tests

Add Vitest, React Testing Library, `@testing-library/user-event`, and jsdom to `SessionHub.Web`; add `test` and optional `test:coverage` scripts. Keep test setup minimal.

Test the user-visible behavior in `src/App.test.tsx` or focused component tests:

- initial favorite states and Favorites count are loaded from the API
- adding and removing update all representations after success
- switching views filters sessions correctly
- empty Favorites view is rendered
- pending controls are disabled
- failed mutations retain prior state and show an accessible error
- rapid repeated activation cannot issue conflicting requests
- client UUID is reused from `localStorage` and a malformed value is replaced

Mock fetch at the network boundary; assert method, route, and `X-Client-Id` header.

### 11. Run end-to-end and regression validation

Run backend tests, frontend tests, `dotnet build SessionHub.sln`, `npm run lint`, and `npm run build`. Then run the API and web app together and exercise add, refresh, list, remove, empty-state, and API-unavailable flows.

Use two browser contexts to confirm favorite isolation. Use keyboard-only navigation and an accessibility scan. Confirm there are no console errors, failed network calls, duplicate database rows, or regressions in session/speaker browsing.

### 12. Update repository documentation

Update `README.md` to remove Favorites from "Not included," add the three favorites endpoints, describe same-browser persistence, and include test commands. Update `docs/architecture-overview.md` and `docs/folder-structure.md` for the `Favorite` entity, service, API flow, frontend helper, and test projects.

## Affected files

### Existing files to modify

- `SessionHub.sln`: add the backend test project.
- `SessionHub.Api/Program.cs`: register `FavoriteService`, map and validate endpoints, expose `Program` to integration tests.
- `SessionHub.Api/Data/ConferenceDbContext.cs`: add the DbSet and favorite model configuration.
- `SessionHub.Api/Models/Session.cs`: optionally add the internal navigation collection.
- `SessionHub.Api/Migrations/ConferenceDbContextModelSnapshot.cs`: generated favorite schema metadata.
- `SessionHub.Api/SessionHub.Api.csproj`: add testability support only if required by the chosen host setup.
- `SessionHub.Web/src/App.tsx`: load, display, filter, add, and remove favorites.
- `SessionHub.Web/src/App.css`: favorite controls, segmented view control, pending/error/empty states, responsive rules.
- `SessionHub.Web/package.json`: add test scripts and development dependencies.
- `README.md`: document behavior, endpoints, and commands.
- `docs/architecture-overview.md`: document the new entity and request flow.
- `docs/folder-structure.md`: document new production and test files.

### New files to add

- `SessionHub.Api/Models/Favorite.cs`
- `SessionHub.Api/Services/FavoriteService.cs`
- `SessionHub.Api/Migrations/<timestamp>_AddFavorites.cs`
- `SessionHub.Api/Migrations/<timestamp>_AddFavorites.Designer.cs`
- `SessionHub.Api.Tests/SessionHub.Api.Tests.csproj`
- `SessionHub.Api.Tests/FavoriteServiceTests.cs`
- `SessionHub.Api.Tests/FavoritesApiTests.cs`
- `SessionHub.Web/src/favoritesApi.ts`
- `SessionHub.Web/src/favoritesApi.test.ts`
- `SessionHub.Web/src/App.test.tsx`
- `SessionHub.Web/src/test/setup.ts`

File names may be consolidated to match the test structure chosen during implementation, but service and API contract coverage must remain distinct.

## Database changes

Create table `Favorites`:

| Column | SQLite type | Rules |
| --- | --- | --- |
| `ClientId` | `TEXT` | Required, max length 36, composite primary key part 1 |
| `SessionId` | `INTEGER` | Required, composite primary key part 2, FK to `Sessions.Id` |
| `CreatedAtUtc` | `TEXT` | Required, assigned by the API |

Constraints and indexes:

- `PK_Favorites (ClientId, SessionId)` prevents duplicate favorites under concurrency.
- `FK_Favorites_Sessions_SessionId` uses cascade delete.
- `IX_Favorites_SessionId` supports the foreign-key relationship and cleanup.
- No seed favorites are added because client identities are runtime-specific.
- Startup continues to apply migrations through the existing `Database.Migrate()` call.

## API changes

- Add `GET /api/favorites` returning ordered session summaries for the current client.
- Add idempotent `PUT /api/favorites/{sessionId:int}`.
- Add idempotent `DELETE /api/favorites/{sessionId:int}`.
- Require and validate `X-Client-Id` on these routes only.
- Reuse `SessionSummaryDto`; add only internal result types needed to map service outcomes.
- Add OpenAPI metadata and documented `200`, `204`, `400`, `404`, and `500` behavior.
- Keep existing `/api/sessions` and `/api/speakers` contracts unchanged.

## UI changes

- Generate and retain an anonymous browser UUID.
- Load favorites independently alongside current conference data.
- Add accessible favorite icon controls to session list rows and the selected-session header.
- Add All sessions/Favorites segmented views and accurate counts.
- Add favorites loading, empty, pending, and error states.
- Preserve selected-session behavior when filtering and removing favorites.
- Keep existing component and CSS conventions; extract only the API helper unless repeated UI markup clearly warrants a small `FavoriteButton` component.

## Testing requirements

### Backend unit and integration tests

- List returns only the requesting client's favorites in deterministic order.
- Add creates one favorite for a valid session.
- Repeated and concurrent adds retain one row and return success.
- Add of an unknown session returns `404` and creates no row.
- Remove deletes the target favorite without affecting other clients or sessions.
- Repeated remove returns success.
- Missing, malformed, and non-canonicalizable client IDs return `400`.
- Non-positive session IDs are rejected.
- Session deletion cascades to favorites.
- Favorite endpoints return DTOs and do not alter existing endpoint contracts.

### Frontend automated tests

- Client identity generation, validation, storage, and header transmission.
- Initial favorite loading and count.
- Add/remove success, pending state, and failure rollback.
- All/Favorites filtering and empty state.
- Consistent state between list and detail controls.
- Keyboard activation, accessible names, and `aria-pressed` state.

### Manual and production-oriented checks

- Same-browser persistence across refresh and API restart.
- Isolation between separate browser contexts.
- Responsive layout at mobile and desktop widths.
- Keyboard-only workflow and focus visibility.
- API startup with a fresh database and upgrade from the current migration.
- Database inspection confirms no duplicate `(ClientId, SessionId)` values.
- Existing sessions and speakers endpoints and UI still work.

## Risks and mitigations

| Risk | Mitigation |
| --- | --- |
| Anonymous client IDs can be copied or spoofed | Document that this is demo identity, not authorization; require real authentication before storing sensitive user data. |
| Duplicate requests race | Enforce the composite key and handle only the verified duplicate-key outcome as idempotent success. |
| UI and API state diverge after a failure | Update UI state only after confirmation and reload favorites on page load. |
| SQLite write contention | Keep mutations short, avoid unnecessary transactions, and return useful failures; the expected write volume is small. |
| Local storage is cleared or blocked | Create a new identity and degrade to page-lifetime persistence when storage is unavailable. |
| Adding tests increases demo setup | Keep one focused backend project and the standard Vitest/Testing Library frontend setup. |

## Definition of done

The feature is complete when all acceptance criteria in `docs/favorites-feature-spec.md` pass, duplicate protection is verified at the database level, automated backend and frontend tests pass, production builds succeed, accessibility checks pass, and the repository documentation accurately describes the shipped behavior and its anonymous-identity limitation.