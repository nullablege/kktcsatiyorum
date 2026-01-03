# Repository Guidelines (agents.md)

## Agent Behavior (mandatory)
- Each prompt/request is handled as a **single scoped task**.
- After completing the requested change(s) and committing, **stop**. Do not continue with follow-up improvements, refactors, cleanup, or “next steps” unless explicitly requested.
- Do **only** what is asked. If something is missing/unclear, prefer a minimal, safe implementation over speculative additions.

## Code Comment Policy (mandatory)
- Do **not** add code comments (`//`, `/* */`, `///`) unless the user explicitly asks for comments.
- Prefer self-explanatory naming and small, clear methods over comments.

## Minimal Change Policy (mandatory)
When editing an existing page/file:
- Change **only** the lines/blocks that must change to satisfy the request.
- Do **not** reformat, reorder, rename, or “clean up” unrelated code/markup.
- Preserve existing structure, indentation style, and patterns already used in that file.

## Project Structure & Module Organization
- `KKTCSatiyorum.sln` is the solution entry.
- `KKTCSatiyorum/` is the ASP.NET Core MVC app (controllers, views, areas, wwwroot).
- `Areas/Admin` and `Areas/Member` hold role-specific UI.
- `Views/` holds public MVC views.
- `wwwroot/` is static assets; `wwwroot/FE/` contains HTML prototypes (source templates).
- `BusinessLayer/` contains services, DTOs, validators, mappings, caching abstractions.
- `DataAccessLayer/` contains EF Core context, repositories, configurations, migrations.
- `EntityLayer/` contains entities and enums (no web/view models, no DTOs).

### Placement rules (important)
- DTOs: `BusinessLayer/Features/<Feature>/DTOs/...`
- ViewModels: `KKTCSatiyorum/(Areas/<Area>/)Models/...`
- Do not place ViewModels inside Controllers.
- Do not place public DTOs / PagedResult types in EntityLayer.

## Consistency With Existing Project Conventions (mandatory)
While writing code:
- Match the existing solution’s patterns and conventions (service/repo usage, naming, folder placement).
- Use the project’s existing variable names and terminology where applicable (avoid introducing new naming styles).
- Keep code compatible with the current architecture and layers (MVC thin controllers, BL services, DAL repos).
- Do not introduce new abstractions/libraries unless explicitly requested.

## FE Prototype Usage (`wwwroot/FE`)
- FE HTML prototypes are the source template for Razor views. Keep markup as close as possible.
- Convert static blocks to dynamic using Razor (`@Model`, `foreach`) without rewriting the layout.
- Prefer TagHelpers for forms and selects:
  - Use `<select asp-for="..." asp-items="...">` instead of manually rendering `<option>` to avoid RZ1031.
- Do NOT use `Html.Raw` for user-provided content (XSS risk). Use encoded output, `white-space: pre-line` if needed.
- Apply **Minimal Change Policy** strictly: only touch the markup required for the requested change.

## Build, Test, and Development Commands
- `dotnet build KKTCSatiyorum.sln`
- `dotnet run --project KKTCSatiyorum`
- `dotnet watch --project KKTCSatiyorum`

### EF migrations
- `dotnet ef migrations add <Name> --project DataAccessLayer --startup-project KKTCSatiyorum`
- `dotnet ef database update --project DataAccessLayer --startup-project KKTCSatiyorum`

## Coding Style & Engineering Practices
- .NET 8, nullable enabled; follow existing conventions (4 spaces, Allman braces).
- Controllers stay thin: no business rules, no heavy data access.
- Business rules live in BusinessLayer services; data access stays in DAL repositories.
- Use async/await end-to-end; accept `CancellationToken ct = default` on public service methods.
- EF Core:
  - Read queries: `AsNoTracking()` + projection when possible.
  - Write operations: use tracked entities or explicit insert/update methods; avoid mutating `AsNoTracking` entities.
  - Handle unique constraint conflicts via `DbUpdateException` (2601/2627) and return `Result` conflicts.
- Comments: do not add comments (see Code Comment Policy).

## Security
- No `Html.Raw` for user content.
- POST endpoints must use AntiForgery (default MVC form helpers or `[ValidateAntiForgeryToken]`).
- Keep secrets out of repo. Use `appsettings.Development.json` locally if needed.

## Caching
- Use `ICacheService` for caching public listing detail / category attributes where applicable.
- Invalidate caches on write operations that change public output (approve/reject, update listing, update attributes/options).

## Testing Guidelines
- No dedicated test projects exist yet.
- If adding tests: `tests/<Project>.Tests` and run `dotnet test`.

## Commit & Pull Request Guidelines
- Use Conventional Commits aligned with MASTER_PLAN.md:
  - `feat(admin): ...`, `feat(member): ...`, `feat: ...`
  - `fix(...): ...`, `refactor(...): ...`
- Keep commits scoped to one plan step when possible.
- PR/summary should mention: migrations added, cache invalidation changes, UI template used (FE file name).
- Do not commit build logs/artifacts (add to `.gitignore` instead).
- After the commit for the requested task, stop (see Agent Behavior).

## UI Integration Master Plan (summary)
- Source of truth: TASKS.md (full phases, status, deliverables, exit criteria).
- Phases: 0 Inventory, 1 Layouts, 2 Auth, 3 Public, 4 Member, 5 Admin, 6 Missing+Errors, 7 Routes, 8 UI polish.
- Always follow: TagHelpers only, layout/partials only, preserve FE structure/classes, keep assets under ~/FE.