# Frontend (Angular)

Main flow:
- /signup creates user (name + 8‑digit phone)
- / root toggles Check in / Check out with a single button

## Run in development
```bash
npm start          # ng serve with proxy (if configured)
```

## Proposed Structure
```
src/
  app/
    features/
      signup/
      presence/
    shared/
      validators/
      ui/
    core/
      interceptors/
      config/
```

## Style & Accessibility
- Global SCSS defines CSS variables (colors, spacing).
- Automatic light/dark via prefers-color-scheme.
- Inputs with floating labels + aria-describedby for error messages.

## Best Practices
- Keep components thin; move logic to services/helpers.
- Avoid any; use explicit types.
- HttpClient: always type responses (e.g., Observable<CheckStatusDto>).
- Future interceptor: attach X-Correlation-Id if present in localStorage.

## Validation
- Name: minimum two words, accented characters allowed.
- Phone: exactly 8 digits (paste cleanup removes non-digits).

## Scripts
```bash
npm start      # dev
npm run build  # production
```

## Next Increments
1. Extract baseApiUrl to environment files.
2. Add status service with short-lived cache (avoid repeated GETs).
3. Implement i18n (Angular built-in or Transloco).
4. E2E tests (Playwright).

