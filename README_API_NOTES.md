# Diller – Backend ↔ Angular Essentials

Only what you need to understand and extend the integration between the ASP.NET Core API and the Angular frontend.

---

## 1. Minimal Architecture Flow

Angular Component → (calls) Shared Service → HttpClient → ApiUrlService builds URL → ASP.NET Controller → Repository Interface → Repository Implementation → DbContext → Database (PostgreSQL)

---

## 2. DbContext (EF Core)

Defines the sets (tables) your app uses.

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) {}

    public DbSet<User> Users => Set<User>();
    public DbSet<Registration> Registrations => Set<Registration>();
    // Add new entities as needed

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent configuration if required
        base.OnModelCreating(modelBuilder);
    }
}
```

Rules:
- Every new entity => add a DbSet.
- Run a migration when model changes.
- Connection string lives in appsettings.*.json.

---

## 3. Automatic Repository Registration (DependencyInjection.cs)

All classes ending in `Repo` get auto-registered with their interfaces (Scoped lifetime).

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(c => c.Where(t => t.Name.EndsWith("Repo")))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}
```

Requirements:
- Interface: IUserRepo
- Implementation: UserRepo
- Name must end with `Repo`.

---

## 4. Program.cs (Minimal Working Setup)

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddRepositories()
    .AddControllers();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAngular", p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowAngular");
app.MapControllers();
app.Run();
```

Key points:
- CORS allows the Angular dev origin only.
- No Swagger / extras here (kept minimal).
- Controllers are auto-discovered.

---

## 5. Angular Environment File

Central place for the backend base URL.

`frontend/src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  baseApiUrl: 'https://localhost:5001/api'
};
```

Switch value in production build (`environment.prod.ts`).

---

## 6. ApiUrlService (Frontend)

Builds safe URLs (no double slashes, consistent base).

```typescript
@Injectable({ providedIn: 'root' })
export class ApiUrlService {
  private readonly baseUrl = environment.baseApiUrl;
  url(path: string): string {
    return `${this.baseUrl.replace(/\/$/, '')}/${path.replace(/^\//, '')}`;
  }
}
```

Usage example:
```typescript
this.http.get(apiUrl.url('checkin/status/12345678'));
```

---

## 7. Shared Check-In Service

All HTTP logic for check-in / check-out is centralized here.

`frontend/src/app/shared/services/checkin.service.ts`
```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiUrlService } from '../../core/api-url.service';

// Tipos de resposta esperados
interface StatusResponse { isCheckedIn: boolean; }
interface ActionResponse { name: string; }

@Injectable({ providedIn: 'root' })
export class CheckinService {
  constructor(private http: HttpClient, private apiUrl: ApiUrlService) {}

  getStatus(phone: string): Observable<StatusResponse> {
    return this.http.get<StatusResponse>(this.apiUrl.url(`checkin/status/${phone}`));
  }

  checkinByPhone(phone: string): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.apiUrl.url(`checkin/byphone/${phone}`), {});
  }

  checkoutByPhone(phone: string): Observable<ActionResponse> {
    return this.http.post<ActionResponse>(this.apiUrl.url(`checkout/byphone/${phone}`), {});
  }

  // List all registrations (used by admin dashboard)
  getAllRegistrations(): Observable<any> {
    return this.http.get(this.apiUrl.url('checkin'));
  }

  // Delete a registration by id
  deleteRegistration(id: string): Observable<any> {
    return this.http.delete(this.apiUrl.url(`checkin/${id}`));
  }
}
```

Why a shared service:
- Single place to change endpoints.
- Typed responses.
- Reusable across multiple components.

---

## 8. Component Usage (Login Example)

```typescript
this.checkinService.getStatus(this.phone).subscribe({
  next: res => {
    if (res.isCheckedIn) {
      this.checkout();
    } else {
      this.checkin();
    }
  },
  error: err => {
    this.message = 'Error contacting server';
  }
});
```

---

## 9. What is subscribe()?

In Angular, HttpClient methods return an Observable.

Key facts:
- An Observable is a lazy description of an async operation (here: HTTP).
- Nothing happens until you call `subscribe()`.
- `subscribe()` lets you define handlers:
  - `next`: response data
  - `error`: network/server error
  - `complete`: called when the Observable finishes (HTTP completes automatically after one emission)
- For single HTTP calls you usually do NOT need to manually unsubscribe (they complete after one value).

Mental model:
- Define intent (Observable) → trigger execution (subscribe) → receive result → update UI.

---

## 10. Adding a New API + Frontend Method

Backend:
1. Add Model + DbSet (if persistent).
2. Add Interface (e.g. `IProjectRepo`) + Implementation (`ProjectRepo`).
3. Auto-registered because name ends with `Repo`.
4. Add Controller endpoint (e.g. `GET /api/projects`).

Frontend:
1. Add method in a service:
   ```typescript
   getProjects() { return this.http.get<Project[]>(this.api.url('projects')); }
   ```
2. In component:
   ```typescript
   this.projectService.getProjects().subscribe(projects => this.items = projects);
   ```

---

## 11. Minimal Command Reference

Backend:
```bash
dotnet restore
dotnet ef migrations add Name   # when model changes
dotnet ef database update
dotnet run
```

Frontend:
```bash
npm install
ng serve
```

---

## 12. Essentials Cheat Sheet

| Topic | Remember |
|-------|----------|
| Base URL | Only in environment files |
| URLs | Always use ApiUrlService |
| Repositories | Must end with `Repo` |
| DI | Auto via scanning |
| HTTP in Angular | Service returns Observable, component subscribes |
| Unsubscribe | Not needed for single HTTP calls |
| CORS | Allow only required origins |
| Migrations | Run after model changes |

---

## 13. End-to-End Example (Check-In Flow)

1. User enters phone.
2. Component sanitizes phone and calls `getStatus()`.
3. API controller queries repo via DbContext.
4. Response `{ isCheckedIn: true/false }` returned.
5. Component decides: call `checkinByPhone()` or `checkoutByPhone()`.
6. UI updates state + shows message.

---

## 14. When Something Fails

| Symptom | Likely Cause | Fix |
|---------|--------------|-----|
| 404 from Angular | Wrong `baseApiUrl` | Check environment.ts |
| CORS error | Origin blocked | Update CORS policy |
| Repo not injected | Naming mismatch | Ensure ends with `Repo` + interface |
| DB errors | Migration missing | Add & apply migration |
| Double slashes | Manual URL concat | Use ApiUrlService |

---

Done. This file intentionally keeps only core integration concepts—no extras.
