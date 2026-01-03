# KKTCSatiyorum UI Integration Master Plan

Last updated: 2026-01-03

## Goal
- Replace all temporary UI with FE design language.
- Fix all redirect/route/link breaks (login/register, member/admin menus, public links).
- Add missing pages in the same FE design system (no placeholders that break UX flow).

## Non-Goals
- No backend refactor or new architecture changes.
- No new libraries or abstractions unless explicitly requested.

## Hard Rules
- No hardcoded href/action. Use TagHelpers everywhere.
- No copy/paste header/footer/sidebar. Use layouts + partials/viewcomponents.
- Preserve FE HTML structure and class names to avoid CSS/JS breakage.
- Keep assets under `~/FE` for now.

## Decisions
- Admin entrypoint: `Areas/Admin/Home/Index`
- Asset base: keep `~/FE` paths
- Static content pages are real views/routes; content filled later

## Definition of Done
- Public/Auth/Member/Admin pages share the same FE visual language.
- Login/Register redirect to correct dashboards; ReturnUrl is safe.
- No broken links in header/footer/sidebars.
- 404/403/500 and AccessDenied are FE-consistent.

## Phase 0 - Inventory and Mapping
Tasks
- List FE templates under `wwwroot/FE`.
- Map each FE page to MVC view/controller/area.
- Build nav/action link route map.
- List missing pages and required view models.

Deliverables
- FE -> MVC map
- Route map
- Missing pages backlog
- ViewModel short list

Exit Criteria
- Every FE page has a target MVC location.
- Broken link sources are known.
- Missing pages are prioritized.

Status
- Done

FE -> MVC Map (current)
- `FE/index.html` -> `Views/Home/Index.cshtml` (Public)
- `FE/login.html` -> `Views/Account/Login.cshtml` (Auth)
- `FE/register.html` -> `Views/Account/Register.cshtml` (Auth)
- `FE/search-results.html` -> `Views/Listings/Index.cshtml` (Public)
- `FE/listing-detail.html` -> `Views/Listings/Detail.cshtml` (Public)
- `FE/dashboard.html` -> `Areas/Member/Views/Dashboard/Index.cshtml` (Member)
- `FE/create-listing.html` -> `Areas/Member/Views/MyListings/Create.cshtml` (Member)
- `FE/admin/index.html` -> `Areas/Admin/Views/Home/Index.cshtml` (Admin)
- `FE/admin/moderation.html` -> `Areas/Admin/Views/Moderasyon/Index.cshtml` (Admin)
- `FE/admin/categories.html` -> `Areas/Admin/Views/Kategori/Index.cshtml` (Admin)

## Phase 1 - Asset Strategy and Layout Skeletons
Tasks
- Confirm asset strategy (keep `~/FE`).
- Create layouts: `_PublicLayout`, `_AuthLayout`, `_MemberLayout`, `_AdminLayout`.
- Align CSS/JS references.
- Plan shared UI via partials/viewcomponents.

Deliverables
- All layouts created and wired.

Exit Criteria
- All pages render with correct layout.
- No CSS/JS 404s.

Status
- In progress

## Phase 2 - Authentication Integration
Tasks
- Replace Login/Register with FE markup using TagHelpers.
- Validate antiforgery flow.
- Redirects: ReturnUrl -> local, else Admin/Member dashboard.
- Logout is POST with antiforgery.

Deliverables
- FE-aligned auth views
- Correct redirect logic

Exit Criteria
- Admin -> Admin dashboard, Member -> Member dashboard.
- ReturnUrl is safe.

Status
- Done

## Phase 3 - Public Pages
Tasks
- Replace `Views/Home/Index.cshtml` with FE `index.html`.
- Bind featured/latest listings (ViewModel).
- Plan header/category menu reuse.

Deliverables
- FE-aligned home page
- Public UI uses FE structure

Exit Criteria
- Public pages in FE design language
- Data binding stable

Status
- In progress

## Phase 4 - Member Area
Tasks
- Integrate FE dashboard and member pages.
- Use `_MemberLayout` and fix sidebar links.
- Keep view models under `Areas/Member/Models`.

Deliverables
- FE-aligned member dashboard and pages
- Working member navigation

Exit Criteria
- Member menu has no broken links
- Member flow has no 404s

Status
- Not started

## Phase 5 - Admin Area
Tasks
- Integrate FE admin templates.
- Standardize admin links with `asp-area="Admin"`.

Deliverables
- FE-aligned admin pages

Exit Criteria
- Admin navigation stable
- No broken admin links

Status
- Not started

## Phase 6 - Missing Pages and Error States
Tasks
- Create missing pages in FE style.
- Create FE-aligned 403/404/500 pages.

Deliverables
- Missing pages complete
- Error UI consistent

Exit Criteria
- No missing pages
- Error views look consistent

Status
- Not started

## Phase 7 - Route and Link Stabilization
Tasks
- Replace hardcoded links with TagHelpers.
- Verify area routes and redirects.

Deliverables
- Stable routing and links

Exit Criteria
- No broken links
- ReturnUrl flows correct

Status
- In progress

## Phase 8 - UI Stabilization
Tasks
- Responsive checks and spacing alignment.
- Normalize any off-style pages.

Deliverables
- Unified visual language

Exit Criteria
- UI feels production-ready

Status
- Not started

## Immediate Tasks (1-3)
1) Replace `Views/Home/Index.cshtml` with FE `index.html` and wire TagHelper links. (Done)
2) Introduce `_AuthLayout` and migrate auth views to it. (Done)
3) Introduce `_MemberLayout` and point Member views to it. (Done)

## Test Strategy
- Build after each phase: `dotnet build KKTCSatiyorum.sln`.
- Manual smoke: home -> login/register -> role redirect -> member dashboard -> listings -> admin home.
- Link checks: header/footer/sidebar/breadcrumbs.
- Form checks: validation + antiforgery.
- Playwright E2E can be added later for login/member/admin flows.

## Latest Build
- 2026-01-03: `dotnet build KKTCSatiyorum.sln` timed out because `KKTCSatiyorum` process locked output DLLs. Stop the running app and re-run build.
