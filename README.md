# Expense Tracker API

A RESTful API built with **ASP.NET Core 8** and **Entity Framework Core** for tracking personal expenses by category — with filtering, pagination, and spending summaries.

## Features

- Full CRUD for expenses and custom categories
- Summary endpoints: total spend, monthly breakdown, per-category totals with percentages
- Filter expenses by date range, category, and amount
- Pagination on list endpoints
- Swagger UI for interactive API exploration
- Integration tests with xUnit and WebApplicationFactory
- SQLite database — zero setup required

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQLite |
| Docs | Swagger / OpenAPI |
| Testing | xUnit + WebApplicationFactory |

## Getting Started

**Prerequisites:** .NET 8 SDK

```bash
# Clone the repo
git clone https://github.com/YOUR_USERNAME/expense-tracker-api
cd expense-tracker-api

# Apply migrations (creates expenses.db automatically)
dotnet ef database update --project ExpenseTracker

# Run the API
dotnet run --project ExpenseTracker
```

Open **http://localhost:5000** to access the Swagger UI.

## API Reference

### Expenses

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/expenses` | List expenses (filterable, paginated) |
| `GET` | `/api/expenses/{id}` | Get a single expense |
| `POST` | `/api/expenses` | Create an expense |
| `PUT` | `/api/expenses/{id}` | Update an expense |
| `DELETE` | `/api/expenses/{id}` | Delete an expense |

**Filter query params:** `startDate`, `endDate`, `categoryId`, `minAmount`, `maxAmount`, `page`, `pageSize`

### Categories

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/categories` | List all categories |
| `POST` | `/api/categories` | Create a custom category |
| `DELETE` | `/api/categories/{id}` | Delete a custom category |

### Summary

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/summary` | Total spend + per-category breakdown |
| `GET` | `/api/summary/monthly` | Month-by-month totals |
| `GET` | `/api/summary/by-category` | Per-category totals with % share |

## Example Requests

```bash
# Create an expense
curl -X POST http://localhost:5000/api/expenses \
  -H "Content-Type: application/json" \
  -d '{"amount": 12.50, "description": "Coffee", "categoryId": 1}'

# Get expenses filtered by date range
curl "http://localhost:5000/api/expenses?startDate=2024-01-01&endDate=2024-01-31"

# Get monthly summary for 2024
curl "http://localhost:5000/api/summary/monthly?year=2024"
```

## Running Tests

```bash
dotnet test
```

The test suite covers all 11 endpoints including edge cases (invalid amounts, duplicate categories, non-existent resources).

## Project Structure

```
ExpenseTracker/
├── Controllers/
│   ├── ExpensesController.cs    # CRUD + filtering
│   ├── CategoriesController.cs  # Category management
│   └── SummaryController.cs     # Aggregations & reports
├── Data/
│   └── AppDbContext.cs          # EF Core context + seeding
├── DTOs/
│   └── ExpenseDtos.cs           # Request/response models
├── Models/
│   ├── Expense.cs
│   └── Category.cs
└── Program.cs

ExpenseTracker.Tests/
└── ExpensesApiTests.cs          # Integration tests
```

## Default Categories

Food · Transport · Utilities · Health · Entertainment · Other

Custom categories can be added via `POST /api/categories`.

## What I Learned

- Designing RESTful APIs with ASP.NET Core 8 minimal hosting model
- EF Core code-first migrations, relationships, and seeding
- LINQ `GroupBy` for real-world data aggregation
- Writing integration tests using `WebApplicationFactory` with in-memory SQLite
- Structuring a .NET project with DTOs to separate API contracts from domain models

---

*Built as part of a .NET learning path. Feedback and PRs welcome!*
