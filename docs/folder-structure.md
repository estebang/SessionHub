# Folder structure

```text
SessionHub/
├── .github/
│   ├── copilot-instructions.md
│   └── workflows/
│       └── deploy.yml
├── README.md
├── SessionHub.sln
├── docs/
│   ├── architecture-overview.md
│   ├── azure-deployment.md
│   ├── deployment-notes.md
│   ├── demo-prompt.txt
│   ├── favorites-feature-spec.md
│   ├── favorites-implementation-plan.md
│   ├── favorites-pr-checklist.md
│   ├── folder-structure.md
│   ├── production-readiness-review.md
│   └── test-coverage-summary.md
├── infra/
│   ├── main.bicep
│   ├── README.md
│   └── parameters/
│       └── dev.bicepparam
├── SessionHub.Api/
│   ├── Data/
│   │   └── ConferenceDbContext.cs
│   ├── Dtos/
│   │   ├── FavoriteDtos.cs
│   │   └── SessionDtos.cs
│   ├── Migrations/
│   │   ├── 20260912213331_InitialCreate.cs
│   │   ├── 20260912213331_InitialCreate.Designer.cs
│   │   ├── 20260912215138_AddFavorites.cs
│   │   ├── 20260912215138_AddFavorites.Designer.cs
│   │   └── ConferenceDbContextModelSnapshot.cs
│   ├── Models/
│   │   ├── Favorite.cs
│   │   ├── Session.cs
│   │   └── Speaker.cs
│   ├── Services/
│   │   └── SessionService.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.Production.json
│   ├── Program.cs
│   ├── SessionHub.Api.csproj
│   └── SessionHub.Api.http
├── SessionHub.Tests/
│   ├── FavoritesIntegrationTests.cs
│   ├── FavoritesServiceTests.cs
│   ├── SessionHub.Tests.csproj
│   └── UnitTest1.cs
├── SessionHub.Web/
│   ├── src/
│   │   ├── App.css
│   │   ├── App.tsx
│   │   ├── index.css
│   │   └── main.tsx
│   ├── tests/
│   │   └── favorites.spec.ts
│   ├── package.json
│   ├── tsconfig.json
│   ├── vite.config.ts
│   ├── package-lock.json
│   └── index.html
└── .gitignore
```

## Why this structure works

- Domain and data logic stay in the API project
- DTOs are isolated to the API contract
- The React app focuses only on the frontend experience
- Tests stay in a separate project so API behavior can be validated without coupling test code to production code
- Infrastructure and deployment automation are kept separate from application code
- Documentation sits beside the project for easy conference storytelling
