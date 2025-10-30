# 🧪 Full-Stack Task Manager (Mono Repo)

This repository contains a .NET 9 Web API backend and a React + Vite frontend. The app supports auth, CRUD tasks, Tailwind CSS v4, and a shadcn-style design system with light/dark theming.

## Projects

- Backend: `backend/Application` (.NET 9 Web API + EF Core)
- Frontend: `frontend` (React 19, Vite 7, Tailwind v4)

## Prerequisites

- Node.js 20+
- .NET SDK 9+
- PostgreSQL 14+ (or update the connection string)

## Backend – Getting Started

1. Navigate to the backend project directory:
   ```bash
   cd backend/Application
   ```
2. Configure the connection string in `appsettings.json` if needed.
3. Run migrations and start the API:
   ```bash
   dotnet ef database update
   dotnet run
   ```
4. The API should be available at `http://localhost:5215` (see `Properties/launchSettings.json`).

## Frontend – Getting Started

1. Navigate to `frontend` and install dependencies:
   ```bash
   cd frontend
   npm install
   ```
2. Create a `.env` file with the API base URL (adjust the port if your API runs elsewhere):
   ```bash
   echo "VITE_API_BASE_URL=http://localhost:5215/api" > .env
   
   # you may also copy from .env.example
   cp .env.example .env
   ```
3. Start the dev server:
   ```bash
   npm run dev
   ```

## Styling and Theme

- Tailwind v4 via the official Vite plugin (`@tailwindcss/vite`).
- Design tokens defined in `frontend/src/index.css` using `@theme` and CSS variables.
- shadcn-inspired UI primitives live in `frontend/src/components/ui`.
- Light/dark mode toggled via `data-theme` on `<html>` with a small helper in `frontend/src/lib/theme.js`.

## Tests

- Unit and integration tests are under `backend/tests`. Run with the standard `dotnet test`.

## Environment Variables

- Frontend: `VITE_API_BASE_URL` (e.g., `http://localhost:5076/api`)
- Backend: configure DB connection string in `Application/appsettings.json`.

## Useful Commands

- Backend
  - `dotnet run` – run API
  - `dotnet ef database update` – apply migrations
  - `dotnet test` – run tests
- Frontend
  - `npm run dev` – start vite dev server
  - `npm run build` – production build
  - `npm run preview` – preview production build

## Notes

- Tailwind editor warnings for `@theme`/`@apply` are expected with some IDEs; the Vite plugin compiles them correctly.
- The frontend persists the auth token to `localStorage` and attaches it as a Bearer token for API calls.
