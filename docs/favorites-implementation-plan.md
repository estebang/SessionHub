# Favorites implementation plan

## Overview

This plan adds a Favorites capability to the existing SessionHub app without changing the demo’s overall simplicity. The implementation should extend the current EF Core model, add a dedicated API surface, and update the React UI to provide a clear favorite toggle and favorites view.

## Ordered tasks

### 1. Define the data model and uniqueness rule
- Add a new `Favorite` entity to the API layer.
- Define the relationship between `Favorite` and `Session`.
- Enforce uniqueness so each session can appear only once in the favorites set for the current app user model.
- Add a migration for the new table.

### 2. Extend the API contract
- Add endpoints for:
  - `POST /api/favorites` to add a favorite
  - `DELETE /api/favorites/{sessionId}` to remove a favorite
  - `GET /api/favorites` to get all favorites
  - `GET /api/favorites/{sessionId}` or equivalent status endpoint if needed
- Validate requests before persistence.
- Return clear responses for duplicates, missing sessions, and empty results.

### 3. Update the service layer
- Add favorite operations to `SessionService` or a dedicated `FavoriteService`.
- Centralize duplicate checks and validation logic in one place.
- Keep the service class aligned with the existing small application architecture.

### 4. Update EF Core and seed logic
- Add the new `Favorites` table to the `ConferenceDbContext`.
- Configure the session-to-favorite relationship.
- Ensure the migration uses the correct SQLite conventions.
- Add any required indexes that support uniqueness and lookup performance.

### 5. Update the frontend state model
- Extend the React UI state to track the favorite status per session.
- Add a toggle button in the session list or detail panel.
- Add a favorites section or view that lists only favorited sessions.
- Handle loading and empty states cleanly.

### 6. Integrate UI behavior
- On initial load, fetch the favorites list alongside sessions.
- Update the UI when the user favorites or unfavorites a session.
- Prevent duplicate requests by disabling the toggle while a request is pending.
- Show a visual indicator for favorite sessions throughout the app.

### 7. Validate edge cases and finalize the flow
- Verify duplicate prevention
- Verify removal behavior
- Verify favorites list rendering for empty and populated states
- Confirm stale state and error handling are acceptable for the demo

## Affected files

### API files
- `SessionHub.Api/Data/ConferenceDbContext.cs`
- `SessionHub.Api/Models/Session.cs`
- `SessionHub.Api/Models/Speaker.cs`
- `SessionHub.Api/Dtos/SessionDtos.cs` or a new `FavoriteDtos.cs`
- `SessionHub.Api/Services/SessionService.cs` or `FavoriteService.cs`
- `SessionHub.Api/Program.cs`

### Frontend files
- `SessionHub.Web/src/App.tsx`
- `SessionHub.Web/src/App.css`
- `SessionHub.Web/src/index.css` if global presentation changes are needed
- Optionally a new UI component or helper file, if the app grows beyond the single-file view

### Documentation files
- `docs/favorites-feature-spec.md`
- `docs/favorites-implementation-plan.md`
- `docs/favorites-pr-checklist.md`

## Database changes

Add a new table similar to the following conceptual model:

```text
Favorite
- Id (int)
- SessionId (int)
- CreatedAtUtc (DateTimeOffset)
```

Important rules:
- `SessionId` must reference the `Sessions` table.
- `SessionId` should be unique within the single-user favorites model.
- Add a foreign key from `Favorite.SessionId` to `Session.Id`.
- Add a unique index on `SessionId` for duplicate prevention.

If the product later adds user identity, this model can evolve into `UserId + SessionId` with a composite unique index.

## API changes

### Endpoints

- `GET /api/favorites`
  - Returns all favorite sessions.

- `POST /api/favorites`
  - Request body: `{ "sessionId": 123 }`
  - Returns the favorite record or the created session favorite payload.
  - Returns conflict or validation error when the session is already favorited.

- `DELETE /api/favorites/{sessionId}`
  - Removes the favorite for the given session ID.
  - Returns `204 No Content` when successful.
  - Returns `404 Not Found` when no favorite exists.

- `GET /api/favorites/{sessionId}` (optional but useful)
  - Returns favorite status for a specific session.

### Response conventions
- Use standard HTTP success and error status codes.
- Keep payloads simple and aligned with the app’s existing DTO patterns.
- Avoid over-engineering with generic result wrappers unless the project later adopts consistent API conventions.

## UI changes

### Session list and detail
- Add a favorite icon or button next to each session card.
- Show a filled state when the session is favorited.
- Show an empty or outline state when it is not.
- Ensure the button is clearly visible and easy to click on desktop and mobile layouts.

### Favorites section
- Add a dedicated favorites panel or toggleable section.
- Display a message such as “No favorites yet” when empty.
- Render sessions in the same format as the main list.

### State management
- Fetch and maintain a favorite session ID set.
- Use the set to drive visual state across all session cards.
- Refresh favorites after mutation operations.

## Testing requirements

Testing is required for this feature even though the repo currently does not include automated tests. The implementation plan should include the following checks:

### Backend tests
- Add tests for favorite creation success
- Add tests for duplicate favorite rejection
- Add tests for removal success
- Add tests for missing session validation
- Add tests for empty favorites results

### Frontend tests
- Verify a user can favorite a session from the list
- Verify a user can remove a favorite
- Verify the favorites list renders expected entries
- Verify duplicate clicks do not insert duplicate rows
- Verify empty state renders correctly

### Manual validation
- Ensure API and UI stay in sync after mutation
- Confirm all favorite actions work through the browser UI
- Verify the app still loads the base conference schedule correctly when no favorites are selected

## Risks and mitigations

### Risk: duplicate favorite creation
Mitigation: enforce unique constraints in the database and duplicate checks in the service layer.

### Risk: stale UI state
Mitigation: refresh favorite IDs after any mutation and disable the toggle while requests are in flight.

### Risk: missing session cleanup
Mitigation: validate session existence before creating a favorite and clean stale rows in a future data maintenance step.

### Risk: overbuilding the architecture
Mitigation: keep the solution within the current service-layer + EF Core design and avoid introducing a repository abstraction for this small feature.
