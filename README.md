# Retailer.POS - .NET 8 Full Template
- Namespace: Retailer.POS
- Database: RetailerDB (LocalDB)
- Auth: JWT-based for API
- Projects:
  - Retailer.Api (Web API, EF Core, JWT auth)
  - Retailer.Web (Razor Pages UI, demo client)

**Notes**
- The template includes a SQL script `database/create_tables.sql` you can run against LocalDB to create schema and seed data.
- EF Migrations are not executed here; add them locally with `dotnet ef migrations add InitialCreate` if you want code-first migrations.
- To run locally:
  1. Open solution in VS or `dotnet sln` tools.
  2. Update connection string in `Retailer.Api/appsettings.json` if needed.
  3. Run `dotnet restore` then run projects.
  4. API JWT secret is set in appsettings.json (for demo). For production, store secrets securely.

