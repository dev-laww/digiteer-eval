# 🧪 Frontend (React + Vite + Tailwind v4)

React 19 app built with Vite 7. Uses Tailwind v4 via the official Vite plugin and shadcn-style UI primitives. Light/dark theme is supported.

## Quick Start

```bash
cd frontend
npm install
echo "VITE_API_BASE_URL=http://localhost:5076/api" > .env
npm run dev
```

## Scripts

- `npm run dev` – start the Vite dev server
- `npm run build` – build for production
- `npm run preview` – preview the production build
- `npm run lint` – run ESLint

## Configuration

- API base URL is read from `VITE_API_BASE_URL` (see `.env`).
- Tailwind v4 is enabled via `@tailwindcss/vite` in `vite.config.js`.
- Global styles, tokens and theme definitions live in `src/index.css`:
  - `@import "tailwindcss"` – Tailwind base/utilities
  - `@theme` – maps CSS variables to Tailwind tokens (`border-border`, `bg-background`, etc.)
  - `:root` – default light theme variables
  - `[data-theme="dark"]` – dark theme overrides

## Theme

- The theme is toggled by setting `document.documentElement.dataset.theme` to `light` or `dark`.
- Helpers live in `src/lib/theme.js` (`initTheme`, `toggleTheme`).
- The header includes a theme toggle using lucide-react icons.

## UI

- shadcn-inspired components are in `src/components/ui` and consume Tailwind tokens
- Page-level composition uses these primitives without modifying the components themselves

## Folders

- `src/components` – UI primitives and page components
- `src/pages` – route pages (Login, Register, Tasks)
- `src/hooks` – API hooks (React Query)
- `src/api/axios.js` – axios instance (attaches Bearer token from localStorage)
- `src/store/auth` – simple auth store (Zustand)

## Notes

- Some editors warn about `@apply`/`@theme`; compilation is handled by the Tailwind Vite plugin.
- For mobile viewport height quirks, the layout can use `min-h-dvh` or `100svh`; current layout uses a standard flex column with full-width pages.