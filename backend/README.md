# Backend

This backend is organized into two top-level folders for clarity:

- `application`: the ASP.NET Core Web API project
- `tests`: unit and integration tests for the API

## Common commands

From the `backend` folder:

```bash
# Run the API
cd Application
 dotnet run

# Run EF migrations
cd Application
dotnet ef database update

# Run all tests
cd ..
 dotnet test
```

See `backend/application/README.md` for API details and usage.

## Solution generation (Premake)

Generate a solution that includes the API and both test projects using Premake (it calls `dotnet` under the hood).

1) Install Premake 5
   - macOS (Homebrew): `brew install premake`
   - Windows: download premake5 from the official site and put it on PATH

2) From the `backend` folder, run:
```bash
premake5 gendsln
```

This creates `TaskManager.sln` in the `backend` folder containing:
- `Application/task-manager-api.csproj`
- `tests/TaskManager.UnitTests/TaskManager.UnitTests.csproj`
- `tests/TaskManager.IntegrationTests/TaskManager.IntegrationTests.csproj`

Open `TaskManager.sln` (located in `backend`) in Visual Studio or JetBrains Rider.
