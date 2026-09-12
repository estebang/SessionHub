# Folder structure

```text
SessionHub/
├── README.md
├── SessionHub.sln
├── docs/
│   ├── architecture-overview.md
│   └── folder-structure.md
├── SessionHub.Api/
│   ├── Data/
│   │   └── ConferenceDbContext.cs
│   ├── Dtos/
│   │   └── SessionDtos.cs
│   ├── Models/
│   │   ├── Session.cs
│   │   └── Speaker.cs
│   ├── Services/
│   │   └── SessionService.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   ├── SessionHub.Api.csproj
│   └── SessionHub.Api.http
├── SessionHub.Web/
│   ├── src/
│   │   ├── App.css
│   │   ├── App.tsx
│   │   ├── index.css
│   │   ├── main.tsx
│   │   └── vite-env.d.ts
│   ├── package.json
│   ├── tsconfig.json
│   ├── vite.config.ts
│   └── index.html
└── .gitignore
```

## Why this structure works

- Domain and data logic stay in the API project
- DTOs are isolated to the API contract
- The React app focuses only on the frontend experience
- Documentation sits beside the project for easy conference storytelling
