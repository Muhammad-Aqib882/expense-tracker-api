using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SummaryController : ControllerBase
{
    private readonly AppDbContext _db;

    public SummaryController(AppDbContext db) => _db = db;

    // GET /api/summary?startDate=2024-01-01&endDate=2024-12-31
    [HttpGet]
    [ProducesResponseType(typeof(SummaryResponseDto), 200)]
    public async Task<IActionResult> GetSummary(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate)
    {
        var q = _db.Expenses.Include(e => e.Category).AsQueryable();

        if (startDate.HasValue) q = q.Where(e => e.Date >= startDate.Value);
        if (endDate.HasValue)   q = q.Where(e => e.Date <= endDate.Value);

        var expenses = await q.ToListAsync();

        if (!expenses.Any())
            return Ok(new SummaryResponseDto());

        var totalSpend = expenses.Sum(e => e.Amount);

        var byCategory = expenses
            .GroupBy(e => new { e.CategoryId, e.Category.Name })
            .Select(g => new CategorySummaryDto
            {
                CategoryId   = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Total        = g.Sum(e => e.Amount),
                Count        = g.Count(),
                Percentage   = totalSpend > 0
                    ? Math.Round(g.Sum(e => e.Amount) / totalSpend * 100, 1)
                    : 0
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        return Ok(new SummaryResponseDto
        {
            TotalSpend      = totalSpend,
            TotalExpenses   = expenses.Count,
            AverageExpense  = Math.Round(totalSpend / expenses.Count, 2),
            TopCategory     = byCategory.FirstOrDefault()?.CategoryName ?? "N/A",
            ByCategory      = byCategory
        });
    }

    // GET /api/summary/monthly
    [HttpGet("monthly")]
    [ProducesResponseType(typeof(List<MonthlySummaryDto>), 200)]
    public async Task<IActionResult> GetMonthly(
        [FromQuery] int? year)
    {
        var q = _db.Expenses.AsQueryable();

        if (year.HasValue)
            q = q.Where(e => e.Date.Year == year.Value);

        var expenses = await q.ToListAsync();

        var monthly = expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new MonthlySummaryDto
            {
                Year      = g.Key.Year,
                Month     = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1)
                                .ToString("MMMM yyyy"),
                Total     = g.Sum(e => e.Amount),
                Count     = g.Count()
            })
            .OrderByDescending(m => m.Year)
            .ThenByDescending(m => m.Month)
            .ToList();

        return Ok(monthly);
    }

    // GET /api/summary/by-category
    [HttpGet("by-category")]
    [ProducesResponseType(typeof(List<CategorySummaryDto>), 200)]
    public async Task<IActionResult> GetByCategory(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate)
    {
        var q = _db.Expenses.Include(e => e.Category).AsQueryable();

        if (startDate.HasValue) q = q.Where(e => e.Date >= startDate.Value);
        if (endDate.HasValue)   q = q.Where(e => e.Date <= endDate.Value);

        var expenses = await q.ToListAsync();
        var totalSpend = expenses.Sum(e => e.Amount);

        var result = expenses
            .GroupBy(e => new { e.CategoryId, e.Category.Name })
            .Select(g => new CategorySummaryDto
            {
                CategoryId   = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                Total        = g.Sum(e => e.Amount),
                Count        = g.Count(),
                Percentage   = totalSpend > 0
                    ? Math.Round(g.Sum(e => e.Amount) / totalSpend * 100, 1)
                    : 0
            })
            .OrderByDescending(c => c.Total)
            .ToList();

        return Ok(result);
    }
}
