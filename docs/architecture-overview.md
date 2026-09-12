# Architecture overview

SessionHub uses a small and readable architecture designed to be understandable during a conference demo.

## Layers

### Presentation
The React app is responsible for:
- displaying a list of sessions
- showing session detail cards
- rendering speaker profiles
- calling the backend APIs

### Application
The ASP.NET Core API hosts the service layer and exposes endpoints. The service layer is intentionally lightweight and acts as the application boundary for the data access needs of the app.

### Domain
The domain model is intentionally small:
- Session
- Speaker
- SessionLevel enum

### Data
Entity Framework Core handles persistence with SQLite. The DbContext manages the relationship between sessions and speakers and seeds the database with realistic conference content.

## Why this architecture works for the demo

- It is easy to explain in under 10 minutes
- It supports all required features without unnecessary abstraction
- The repository pattern is not used because the app is small and the service layer already keeps the data access responsibilities clear
- DTOs keep the API contract simple and intentional
- Database seeding helps tell a complete story without requiring extra infrastructure

## Request flow

```text
React UI -> API endpoint -> SessionService -> DbContext -> SQLite
```

This produces a clean, linear flow that attendees can easily follow.
