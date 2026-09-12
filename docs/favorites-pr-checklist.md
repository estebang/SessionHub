# Favorites PR checklist

## Implementation checklist

- [ ] Add the `Favorite` model and database relationship.
- [ ] Add a unique constraint or unique index to prevent duplicate favorites.
- [ ] Add EF Core migration for the new table.
- [ ] Add a favorite service or extend the session service with favorite operations.
- [ ] Add API endpoints for listing, creating, and deleting favorites.
- [ ] Validate session existence before persisting a favorite.
- [ ] Add duplicate protection in both API and database layers.
- [ ] Add favorite state to the session UI.
- [ ] Add a favorites list or section that shows only favorited sessions.
- [ ] Ensure favorite removes are reflected immediately in the UI.
- [ ] Handle empty favorites states and loading states.
- [ ] Confirm this feature does not break existing session browsing flows.

## Testing checklist

- [ ] Favorite a session successfully.
- [ ] Remove a favorite successfully.
- [ ] Attempt to favorite the same session twice and verify only one record exists.
- [ ] Retrieve the favorites list and verify expected sessions are returned.
- [ ] Confirm the API returns a clean empty list when no favorites exist.
- [ ] Confirm a delete request for a non-existent favorite is handled gracefully.
- [ ] Confirm a favorite request for a missing session returns a validation or not found result.
- [ ] Test frontend duplicate-click protection.
- [ ] Validate the UI shows the correct state after load, add, and remove actions.
- [ ] Verify the app still renders sessions normally when favorites are empty.

## Production readiness checklist

- [ ] Database migration is included and reviewed.
- [ ] Unique index is present to prevent duplicate rows in production data.
- [ ] Error handling and validation messages are understandable to end users.
- [ ] No sensitive or user-specific data is exposed in the API response.
- [ ] The UI state is resilient to slow network responses.
- [ ] Empty and error states are polished enough for demo or real use.
- [ ] The feature keeps the app simple and understandable for a conference demo audience.
- [ ] Documentation is updated to reflect new capability and behavior.
- [ ] No unexpected changes were introduced outside the favorites feature scope.
- [ ] The code is ready for follow-up review before merging.

## Reviewer notes

This checklist assumes the app remains demo-friendly and intentionally avoids user authentication. If the product later adds multi-user behavior, the favorites design should evolve to include a user identity and a composite uniqueness rule.
