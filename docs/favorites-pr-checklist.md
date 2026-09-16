# Favorites Pull Request Checklist

## Implementation checklist

### Data and domain

- [ ] `Favorite` contains only `ClientId`, `SessionId`, `CreatedAtUtc`, and its session navigation.
- [ ] `ConferenceDbContext` exposes and configures Favorites.
- [ ] `(ClientId, SessionId)` is a composite primary key or equivalent unique constraint.
- [ ] `ClientId` is required and limited to the canonical GUID length.
- [ ] The session foreign key and cascade-delete behavior are configured.
- [ ] The migration and model snapshot contain only the intended schema changes.
- [ ] No runtime-specific favorites are seeded.

### Service and API

- [ ] Favorites logic is in a focused service, not endpoint lambdas or the React app.
- [ ] Read queries use `AsNoTracking()` and deterministic ordering.
- [ ] API responses use DTOs rather than EF entities.
- [ ] `GET /api/favorites` returns the current client's favorite session summaries.
- [ ] `PUT /api/favorites/{sessionId}` is idempotent and returns `404` for an unknown session.
- [ ] `DELETE /api/favorites/{sessionId}` is idempotent.
- [ ] All favorites routes require and normalize a valid `X-Client-Id` GUID.
- [ ] Non-positive session IDs are rejected.
- [ ] Concurrent duplicate insertion is handled without hiding unrelated database errors.
- [ ] Favorites endpoints include OpenAPI names and response metadata.
- [ ] Logs are structured, low-noise, and do not contain raw client IDs.
- [ ] Existing session and speaker API contracts remain unchanged.

### Web UI

- [ ] A versioned `localStorage` key retains the browser client UUID.
- [ ] Missing or malformed stored IDs are replaced with `crypto.randomUUID()`.
- [ ] Every favorites API request includes `X-Client-Id`.
- [ ] All sessions and Favorites views are available with accurate counts.
- [ ] Favorite controls appear in the session list and selected-session details.
- [ ] Controls have icons, tooltips, accessible names, and correct `aria-pressed` values.
- [ ] List markup does not nest one button inside another.
- [ ] Pending mutations disable only the affected session's controls.
- [ ] UI state changes only after server confirmation and remains unchanged on failure.
- [ ] Favorites loading, empty, and error states are explicit.
- [ ] Removing the last visible favorite leaves a coherent selection and empty state.
- [ ] New styling follows existing CSS conventions and works at mobile and desktop widths.
- [ ] No unnecessary router, repository, state library, or identity framework was introduced.

### Documentation

- [ ] `README.md` lists the favorites behavior, endpoints, persistence scope, and test commands.
- [ ] Favorites is removed from the README's "Not included" list.
- [ ] Architecture and folder-structure docs reflect the new model, service, frontend helper, and tests.
- [ ] Anonymous browser identity and lack of cross-device synchronization are documented.
- [ ] Public API behavior matches `docs/favorites-feature-spec.md`.

## Testing checklist

### Automated backend

- [ ] Fresh-database migrations apply successfully.
- [ ] Upgrade from the current schema applies successfully.
- [ ] Empty favorites returns `200` with an empty array.
- [ ] Add, list, and remove happy paths pass.
- [ ] Favorite lists are isolated by client ID.
- [ ] Favorite lists are ordered by session start time and then ID.
- [ ] Repeated add produces one row and succeeds.
- [ ] Concurrent add produces one row and succeeds.
- [ ] Unknown-session add returns `404` without writing a row.
- [ ] Repeated remove succeeds without writing or throwing.
- [ ] Missing and malformed client IDs return `400`.
- [ ] Invalid session IDs are rejected.
- [ ] Deleting a session cascades to its favorites.
- [ ] Existing sessions and speakers tests or smoke checks still pass.

### Automated frontend

- [ ] Client UUID generation, reuse, and malformed-value replacement are tested.
- [ ] API helper tests assert routes, methods, and `X-Client-Id` headers.
- [ ] Initial favorite state and count render correctly.
- [ ] All/Favorites switching filters sessions correctly.
- [ ] Add and remove update list and detail controls consistently.
- [ ] Pending state prevents conflicting rapid actions.
- [ ] API failure keeps the previous state and renders an accessible error.
- [ ] Empty Favorites state renders correctly.
- [ ] Favorite controls support keyboard activation and expose accessible state.

### Build and manual verification

- [ ] `dotnet test SessionHub.sln` passes.
- [ ] `dotnet build SessionHub.sln` passes without new warnings.
- [ ] `npm test` passes.
- [ ] `npm run lint` passes.
- [ ] `npm run build` passes.
- [ ] Add -> refresh -> Favorites view -> remove works end to end.
- [ ] Favorites survive API restart in the same browser.
- [ ] Separate browser contexts have independent favorites.
- [ ] The UI works at representative mobile and desktop widths without overlap.
- [ ] Keyboard-only navigation and visible focus states work.
- [ ] Browser console and network panel show no unexpected errors.
- [ ] Database inspection confirms no duplicate client/session pairs.

## Production readiness checklist

### Data safety and deployment

- [ ] Migration upgrade and rollback are tested against a copy of the current schema.
- [ ] SQLite database backup and restore steps are documented and exercised.
- [ ] The deployment process applies the migration before serving favorites traffic.
- [ ] Foreign keys are enabled in the deployed SQLite configuration.
- [ ] Database file permissions and persistence location are correct for the host.
- [ ] Expected database growth and retention of anonymous client records are accepted.

### Security and privacy

- [ ] Product owners accept that `X-Client-Id` is anonymous correlation, not authentication.
- [ ] No sensitive or personal data is associated with a client ID.
- [ ] Client IDs are validated and are not written to normal success logs.
- [ ] CORS remains restricted to approved web origins.
- [ ] HTTPS is enforced in the production environment.
- [ ] Abuse controls or rate limits are considered before internet exposure.
- [ ] A move to authenticated user IDs is planned before favorites hold private data or require cross-device access.

### Reliability and operations

- [ ] API failures return consistent problem responses without implementation details.
- [ ] Unexpected favorites persistence failures are logged with route and session context.
- [ ] Health checks or deployment smoke tests cover a favorites read and mutation.
- [ ] Concurrent-write behavior is tested at expected load.
- [ ] Monitoring can distinguish validation, not-found, conflict/race, and server failures.
- [ ] Rollback behavior is documented if the UI deploys before or after the API.

### Release acceptance

- [ ] Every acceptance criterion in `docs/favorites-feature-spec.md` is demonstrated.
- [ ] Product and accessibility review are complete.
- [ ] API documentation reflects the final contract and status codes.
- [ ] No unrelated application or schema changes are included in the pull request.
- [ ] Release notes state that favorites are scoped to one browser profile and can be lost when site data is cleared.