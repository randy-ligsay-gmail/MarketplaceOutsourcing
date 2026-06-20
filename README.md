# Marketplace Outsourcing API

A **2-tier C# application** for a job marketplace: customers post jobs, contractors submit offers, and customers accept offers.

- **Tier 1:** RESTful HTTP API (ASP.NET Core)
- **Tier 2:** PostgreSQL persistence (Entity Framework Core)

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 17 (local install or Docker)

## Database setup

### Option A — Docker

```powershell
docker compose up -d
```

Default connection string in `appsettings.json`:

```json
"Host=localhost;Port=5432;Database=marketplace_outsourcing;Username=postgres;Password=YOUR_PASSWORD"
```

### Option B — Local PostgreSQL

1. Create the database: `CREATE DATABASE marketplace_outsourcing;`
2. Update `appsettings.json` with your username and password.

Migrations and seed data run automatically on startup.

## Run the API

```powershell
cd MarketplaceOutsourcing
dotnet run
```

Open Swagger UI: [http://localhost:5080/swagger](http://localhost:5080/swagger)

Default URLs (without launch profile): `http://localhost:5000`

## API endpoints

### Customers

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/customers` | List all customers |
| GET | `/customers/{searchTerm}` | Search by **ID** or **partial last name** |
| POST | `/customers` | Create customer |

**Assessment example:**

```http
GET /customers/smi
```

```json
[
  { "id": "...", "name": "Bob", "lastName": "Smith" }
]
```

### Contractors

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/contractors` | List all contractors |
| GET | `/contractors/{searchTerm}` | Search by **ID** or **partial business name** |

### Jobs (CRUD)

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/jobs` | List all jobs |
| GET | `/jobs/search/{term}` | Search **open** jobs by title/description |
| GET | `/jobs/{id}` | Get job by ID |
| POST | `/jobs` | Create job |
| PUT | `/jobs/{id}` | Update open job |
| DELETE | `/jobs/{id}` | Cancel open job |

### Job offers (CRUD + accept)

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/joboffers` | List all offers |
| GET | `/joboffers/{id}` | Get offer by ID |
| GET | `/joboffers/job/{jobId}` | List offers for a job |
| POST | `/joboffers` | Submit offer |
| PUT | `/joboffers/{id}` | Update pending offer price |
| DELETE | `/joboffers/{id}` | Withdraw pending offer |
| POST | `/joboffers/{id}/accept` | Customer accepts offer |

## Project structure

```text
Domain/           Entities, enums, domain rules
Application/      Services + repository interfaces
Infrastructure/   EF Core, PostgreSQL, repositories
Api/              REST controllers, DTOs, mapping
```

## Example requests (PowerShell)

```powershell
# Search customers by last name
Invoke-RestMethod http://localhost:5000/customers/smi

# Search open jobs
Invoke-RestMethod http://localhost:5000/jobs/search/roof

# Create a job offer
Invoke-RestMethod -Method Post -Uri http://localhost:5000/joboffers `
  -ContentType "application/json" `
  -Body '{"jobId":"JOB_GUID","contractorId":"CONTRACTOR_GUID","price":999.00}'

# Accept an offer
Invoke-RestMethod -Method Post -Uri http://localhost:5000/joboffers/OFFER_GUID/accept
```

## EF Core migrations

```powershell
dotnet ef migrations add MigrationName --output-dir Infrastructure/Persistence/Migrations
dotnet ef database update
```

## Notes

- Search uses parameterized EF queries (`ILike`) and indexes on `LastName`, contractor `Name`, and job `Status`.
- DELETE on jobs cancels open jobs (domain rule). DELETE on offers withdraws pending offers.
