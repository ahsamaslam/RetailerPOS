# GitHub Copilot Instructions

## Purpose
These directives constrain Copilot-generated code so it respects the modular architecture of this Retailer POS solution and targets .NET 8 Razor Pages.

## Solution Topology
1. **AuthModule**
   - Owns companies, users, roles, permissions.
   - Exposes token-based identity endpoints only; no business logic.
2. **Retailer.API**
   - Hosts all business modules (Sales, Purchases, Inventory, Reports, etc.).
   - Requires valid JWT/Bearer tokens issued by `AuthModule`.
3. **Retailer_Web (Razor Pages)**
   - UI layer consuming `Retailer.API`.
   - Never talks directly to data stores.

## Mandatory Guardrails
- Always authenticate by calling `AuthModule` first; never reimplement identity or bypass token issuance.
- All business operations must call `Retailer.API`. Razor Pages must not contain business logic beyond orchestration/view models.
- Prefer Razor Pages conventions (PageModel classes, handler methods, tag helpers) over MVC controllers or Blazor components.
- Reuse existing DTOs/contracts when interacting with the API; do not invent new shapes unless extending the shared contract.
- Tokens must be stored/handled via secure mechanisms (e.g., server-side session, secure cookies). Never expose secrets in client-side code.

## Auth & Authorization
- Use standard OAuth2/OpenID Connect flows appropriate for server-rendered apps.
- Propagate user context via the JWT; downstream modules derive permissions from claims issued by `AuthModule`.
- Enforce per-company scoping (multi-tenant) at the API level; Razor Pages should set the company context but not enforce rules themselves.

## API Interaction
- Use typed `HttpClient` or generated clients registered via dependency injection in Razor Pages.
- Handle non-200 responses explicitly (retry policy, user feedback, logging).
- Batch or paginate list calls; never load large result sets into the UI without server-side filtering.

## Razor Pages Guidance
- Keep `.cshtml` markup thin—push logic to the accompanying `PageModel`.
- Follow existing layout/partial structure (`_Layout`, `_SuperAdminLayout`, `_EmptyLayout`, `_GlobalLoader`).
- Use tag helpers/components already present before introducing new ones.

## Data & Validation
- Validation rules live server-side in `Retailer.API`; mirror only lightweight checks in Razor Pages.
- Never access the Auth or Retailer databases from the web project.

## Observability & Errors
- Log authentication failures in `AuthModule`, business errors in `Retailer.API`, and UI issues in `Retailer_Web`.
- Return user-friendly messages from Razor Pages while preserving detailed logs server-side.

## Testing Expectations
- Add/extend unit tests per module (identity, business logic, UI handlers).
- For Razor Pages, cover PageModel handlers with integration tests that mock API clients.

## Non-Negotiable
- No new technology stacks unless they integrate seamlessly with .NET 8 Razor Pages and existing modules.
- All new endpoints require explicit authorization policies.
- Shared constants, enums, and error codes belong in a shared library so each module stays consistent.