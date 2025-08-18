# Time Registration (Check ind/ud)

A minimal time registration system with:
- API: ASP.NET Core 8 + EF Core
- Frontend: Angular (standalone components, forms)
- Feature: Single action to Check in / Check out using phone number

Monorepo layout
- api/ — ASP.NET Core Web API + EF Core
- frontend/ — Angular app

## Quick start (Windows)

Prereqs
- .NET SDK 8.0+
- Node.js 20+ and npm
- VS Code recommended

Install and run
1) API
   - Open a terminal in repo root:
     - cd api
     - dotnet restore
     - dotnet build
     - dotnet ef database update (if EF tools installed; otherwise see “Database & migrations”)
     - dotnet run
   - API runs on http://localhost:5169 (as used by the Angular app)

2) Frontend
   - Open a second terminal in repo root:
     - cd frontend
     - npm install
     - npm start (or: npx ng serve)
   - App runs on http://localhost:4200

VS Code
- .vscode/ has launch/tasks to run and debug both projects.

## How it works

High level flow
- Users are identified by an 8-digit phone number.
- Signup: create a user (name + phone).
- Login page: one button toggles between Check in and Check out:
  1) GET /api/checkin/status/{phone} to know if the user is currently checked in.
  2) If checked in: POST /api/checkout/byphone/{phone}
  3) If not checked in: POST /api/checkin/byphone/{phone}
- UI shows success feedback; form validation messages only appear when the user interacts with invalid fields.

Frontend (Angular)
- Pages
  - /signup: Create user
  - / (root): Check ind/ud
- Styling
  - Shared glassmorphism look; floating labels; responsive layout.
- Validation
  - Name: at least two words (letters incl. accents and - ')
  - Phone: exactly 8 digits; only numbers allowed; paste cleans non-digits
- Accessibility
  - Aria attributes on inputs and error messages
- UX details
  - While submitting: loading spinner on button
  - On success: forms reset to clear touched/dirty state (prevents error hints from flashing)

API (ASP.NET Core)
- Controllers (routes under /api):
  - UserController: POST /api/user — create a user
  - CheckInController:
    - GET /api/checkin/status/{phone}
    - POST /api/checkin/byphone/{phone}
  - CheckOutController: POST /api/checkout/byphone/{phone}
  - RegistrationController: historical registrations (see code)
- Data
  - EF Core with repositories pattern in src/Repositories
  - Connection string configured in appsettings*.json

## Endpoints overview

- POST /api/user
  - Body: { "name": "John Doe", "tlf": "12345678" }
  - 201 or 409 if phone already exists
- GET /api/checkin/status/{phone}
  - Returns current status { isCheckedIn: boolean, ... }
- POST /api/checkin/byphone/{phone}
- POST /api/checkout/byphone/{phone}

Note: See Controllers for exact response shapes.

## Database & migrations

- Ensure a database provider is configured in api/appsettings.Development.json.
- Create/Update DB:
  - Install EF Core tools if needed:
    - dotnet tool install --global dotnet-ef
  - From api/:
    - dotnet ef database update
- Create a new migration:
  - dotnet ef migrations add <name>
  - dotnet ef database update

## Configuration

Frontend → API URL
- Current code uses absolute URLs (http://localhost:5169).
- Recommended: use Angular proxy or environment variables so builds don’t hardcode URLs.
  - Option A (proxy, already present: frontend/proxy.conf.json):
    - In HttpClient calls, use relative paths (e.g., /api/user) and run ng serve with proxy.
  - Option B: environment files (environment.ts / environment.prod.ts) with baseApiUrl.

CORS
- If running on different ports, ensure API allows the frontend origin or use the Angular proxy.

## Scripts

Frontend (from frontend/)
- npm start → ng serve
- npm run build → production build
- npm test → unit tests (if configured)

API (from api/)
- dotnet build
- dotnet run
- dotnet publish -c Release

## Implementing new features

Example: Add “View my registrations”
1) API
   - Add method in IRegistrationRepo and implementation in RegistrationRepo.
   - Add endpoint in RegistrationController (e.g., GET /api/registration/byphone/{phone}).
   - Add migration if schema changes; update DB.

2) Frontend
   - Create a standalone component (e.g., registrations) and a route.
   - Add a service or use HttpClient directly to call /api/registration/byphone/{phone}.
   - Reuse existing UI patterns (container, .field, .text-input, floating labels).
   - Validate inputs; show errors only on touched/invalid; show loading and error states.

3) Wiring & UX
   - Keep API URLs centralized (proxy or environment).
   - Provide clear success/error messages; reset forms on success to clear touched state.

4) Testing & Quality
   - Add Angular unit tests for the new component and services.
   - Consider API integration tests (new test project) for new endpoints.

## Production

- Frontend build:
  - cd frontend
  - npm run build
  - Output in frontend/dist — host behind a static server or reverse proxy
- API publish:
  - cd api
  - dotnet publish -c Release -o publish
  - Deploy publish/ to your host (IIS, container, etc.)
- Reverse proxy setup:
  - Serve Angular at / and proxy /api to the API.

## Troubleshooting

- CORS errors: use Angular proxy in dev or enable CORS for http://localhost:4200 in API.
- Port conflicts: change API/Frontend ports or stop conflicting processes.
- EF errors: ensure migrations are applied; connection string is reachable.

## Code style & lint

- Frontend: Angular ESLint (if configured), follow existing patterns for forms and styling.
- API: .NET analyzers; keep DI in Extensions/DependencyInjection.cs; repositories thin and testable.

## Security (recommended baseline)
- Server-side validation of phone (8 digits) in addition to frontend.
- Rate limiting per IP + phone.
- Headers: Strict-Transport-Security, X-Content-Type-Options, Content-Security-Policy (define when deploying to production).
- Input sanitization (even if simple) and EF parameterization (already default).
- Database user with least privileges.

## Observability (incremental)
| Phase | Item | Value |
|-------|------|-------|
| Phase 1 | Structured logging (Serilog / console JSON) | Consistent debugging |
| Phase 2 | Metrics (Prometheus/OpenTelemetry) | Basic SLOs |
| Phase 3 | Distributed tracing (OpenTelemetry) | Faster root cause |
| Phase 4 | Feature flags (e.g., LD / OpenFeature) | Safer deploys |

## Suggested Roadmap (high → low priority)
1. Remove unnecessary dependencies (AngularJS already removed).
2. Centralize API base URL via environments/proxy (avoid hard‑coded strings).
3. Structured logging + correlation id.
4. Critical unit tests (rule: prevent double check-in).
5. Paginated history endpoint + filters (date / min duration).
6. CSV / PDF export (batch).
7. Internationalization (i18n).
8. Advanced accessibility (keyboard shortcuts + high contrast mode).
9. Audit trail (user change log) / event sourcing (if requirements grow).
10. Cache & scalability (Redis) for high-frequency status checks.

## Code Standards
- Frontend: Prefer pure helper functions for testability. Avoid complex logic inside components.
- API: Thin controllers → delegate to services/domain. Repositories hold no business rules.
- Naming: Async repository methods end with Async.

## Contributing
See [ARCHITECTURE.md](./ARCHITECTURE.md) and open small PRs including risk / rollback notes.
