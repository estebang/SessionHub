# Favorites Feature Specification

## User story

As a conference attendee, I want to save sessions that interest me, remove sessions I no longer want to save, and view my saved sessions together so that I can quickly return to the talks I am considering.

## Scope

The feature adds server-persisted favorites to the existing anonymous SessionHub experience. A favorite belongs to one browser profile and one session. It is not a personal schedule and does not reserve a seat, detect schedule conflicts, or synchronize across browsers or devices.

## Acceptance criteria

### Favorite a session

- Every session exposes a favorite control in the session list and detail view.
- Activating the control for an unfavorited session persists the favorite through the API.
- The control indicates the saved state after the API confirms success.
- The state remains after a page refresh in the same browser profile.
- Favoriting one session does not change another session's favorite state.

### Remove a favorite

- Activating the control for a favorited session removes the persisted favorite through the API.
- The control indicates the unsaved state after the API confirms success.
- Removing an already-absent favorite is safe and leaves the session unfavorited.

### View all favorites

- The Sessions panel provides `All sessions` and `Favorites` views.
- The Favorites view shows only sessions favorited by the current browser profile.
- Favorite sessions use the same session information and deterministic start-time order as the existing session list.
- The Favorites view has a distinct empty state when no sessions are saved.
- Selecting a favorite displays its existing session details and speaker information.
- Favorite state is consistent in the All sessions view, Favorites view, and detail view.

### Prevent duplicate favorites

- Repeated or concurrent requests for the same browser profile and session result in one database row.
- The API treats adding an existing favorite as a successful idempotent operation.
- Duplicate prevention is enforced by a database composite primary key or unique constraint, not only by UI state or a pre-insert query.

### Failure behavior

- A missing or invalid browser client identifier is rejected with `400 Bad Request`.
- Favoriting a session that does not exist returns `404 Not Found`.
- API failures do not display a favorite state that was not confirmed by the server.
- While a favorite mutation is pending, that session's favorite control is disabled to prevent conflicting UI actions.
- A failed mutation restores the last confirmed state and presents a concise, accessible error message.

## Assumptions

- SessionHub has no authentication or user entity. For this feature, a "user" means one anonymous browser profile.
- The web app creates a UUID with `crypto.randomUUID()`, stores it under a versioned `localStorage` key, and sends it in the `X-Client-Id` request header for every favorites request.
- The client identifier is stable across refreshes but is lost when site storage is cleared. Incognito profiles, browsers, and devices have independent favorites.
- The API stores the client identifier as a canonical GUID string. It is an opaque correlation value, not authentication or authorization.
- Favorites are persisted in the existing SQLite database and survive API restarts.
- Session data remains read-only through the current application. If a session is deleted administratively, its favorites are deleted by foreign-key cascade.
- The Favorites view is a filter within the existing single-page interface; adding a router is unnecessary.
- The expected conference data set is small, so the first version does not require pagination.

## Edge cases

| Scenario | Expected behavior |
| --- | --- |
| The browser has no client identifier | Generate and persist a UUID before the first favorites request. |
| Stored client identifier is malformed | Replace it with a new UUID; the API also rejects malformed values. |
| No favorites exist | `GET /api/favorites` returns `200 OK` with `[]`; the UI shows the Favorites empty state. |
| The same session is added repeatedly | Each `PUT` succeeds idempotently and only one row exists. |
| Two add requests race | The database uniqueness constraint wins; both requests resolve to the favorited state. |
| Remove is repeated or races with another remove | Each `DELETE` returns `204 No Content`; no row remains. |
| Add and remove overlap for one session | The UI permits only one pending mutation per session; the final confirmed request determines the displayed state. |
| Session ID is zero, negative, or non-numeric | Route matching or validation rejects the request with `400 Bad Request` or `404 Not Found`; no row is written. |
| Session is deleted after being favorited | Cascade delete removes the favorite; the next favorites read omits it. |
| Favorites load fails while sessions load | All sessions remain usable; favorite controls show a retryable error state rather than assuming no favorites. |
| Mutation fails because the API is unavailable | Keep the last server-confirmed state and allow retry. |
| User clears browser storage | A new browser identity is created and the previous profile's favorites are no longer addressable from that browser. |
| Favorite view is active and its last item is removed | Show the Favorites empty state and clear the selected session if it is no longer in the visible list. |

## Validation rules

### Client identifier

- `X-Client-Id` is required on all `/api/favorites` endpoints.
- It must parse as a GUID and be normalized to the canonical `D` format before querying or storing it.
- The persisted value is required and limited to 36 characters.
- Do not log the raw identifier at normal success paths.

### Session identifier

- `sessionId` must be a positive integer.
- `PUT /api/favorites/{sessionId}` verifies that the session exists before inserting a favorite.
- A favorite must reference an existing session through a foreign key.

### Favorite integrity

- `(ClientId, SessionId)` uniquely identifies a favorite and is the table's composite primary key.
- `CreatedAtUtc` is assigned by the server in UTC when the row is first created and is not changed by an idempotent repeat.
- The API never accepts a client-supplied creation timestamp or favorite entity body.
- Read queries use `AsNoTracking()` and return DTOs rather than EF entities.

## API contract summary

| Method and route | Success | Validation and errors |
| --- | --- | --- |
| `GET /api/favorites` | `200 OK` with `SessionSummaryDto[]`, ordered by session start time | `400` for missing or invalid `X-Client-Id` |
| `PUT /api/favorites/{sessionId:int}` | `204 No Content`, whether newly added or already present | `400` for invalid input; `404` when the session does not exist |
| `DELETE /api/favorites/{sessionId:int}` | `204 No Content`, whether present or already absent | `400` for invalid input or client identifier |

The endpoints use desired-state semantics: `PUT` means the session must be a favorite and `DELETE` means it must not be a favorite. This keeps retries safe while the database key guarantees physical uniqueness.

## Out of scope

- Sign-in, named user accounts, and cross-device synchronization
- Sharing or exporting favorites
- Personal schedules, reminders, seat reservations, or conflict detection
- Favorite counts, popularity ranking, and analytics
- Offline mutation queues
- Pagination or server-side filtering of the favorites collection