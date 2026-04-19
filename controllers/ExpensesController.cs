using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExpensesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ExpensesController(AppDbContext db) => _db = db;

    // GET /api/expenses
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ExpenseResponseDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] ExpenseQueryParams query)
    {
        var q = _db.Expenses.Include(e => e.Category).AsQueryable();

        if (query.StartDate.HasValue)
            q = q.Where(e => e.Date >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            q = q.Where(e => e.Date <= query.EndDate.Value);

        if (query.CategoryId.HasValue)
            q = q.Where(e => e.CategoryId == query.CategoryId.Value);

        if (query.MinAmount.HasValue)
            q = q.Where(e => e.Amount >= query.MinAmount.Value);

        if (query.MaxAmount.HasValue)
            q = q.Where(e => e.Amount <= query.MaxAmount.Value);

        var totalCount = await q.CountAsync();

        var items = await q
            .OrderByDescending(e => e.Date)
            .ThenByDescending(e => e.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(e => MapToDto(e))
            .ToListAsync();

        return Ok(new PagedResult<ExpenseResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        });
    }

    // GET /api/expenses/{id}
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseResponseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _db.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense is null)
            return NotFound(new { message = $"Expense with id {id} not found." });

        return Ok(MapToDto(expense));
    }

    // POST /api/expenses
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponseDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
    {
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            return BadRequest(new { message = $"Category with id {dto.CategoryId} does not exist." });

        var expense = new Expense
        {
            Amount      = dto.Amount,
            Description = dto.Description,
            Date        = dto.Date ?? DateOnly.FromDateTime(DateTime.UtcNow),
            Notes       = dto.Notes,
            CategoryId  = dto.CategoryId,
            CreatedAt   = DateTime.UtcNow,
            UpdatedAt   = DateTime.UtcNow
        };

        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync();

        await _db.Entry(expense).Reference(e => e.Category).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = expense.Id }, MapToDto(expense));
    }

    // PUT /api/expenses/{id}
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ExpenseResponseDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto)
    {
        var expense = await _db.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense is null)
            return NotFound(new { message = $"Expense with id {id} not found." });

        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value);
            if (!categoryExists)
                return BadRequest(new { message = $"Category with id {dto.CategoryId} does not exist." });
            expense.CategoryId = dto.CategoryId.Value;
        }

        if (dto.Amount.HasValue)      expense.Amount      = dto.Amount.Value;
        if (dto.Description != null)  expense.Description = dto.Description;
        if (dto.Date.HasValue)        expense.Date        = dto.Date.Value;
        if (dto.Notes != null)        expense.Notes       = dto.Notes;

        expense.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _db.Entry(expense).Reference(e => e.Category).LoadAsync();

        return Ok(MapToDto(expense));
    }

    // DELETE /api/expenses/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var expense = await _db.Expenses.FindAsync(id);

        if (expense is null)
            return NotFound(new { message = $"Expense with id {id} not found." });

        _db.Expenses.Remove(expense);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // ── Helper ────────────────────────────────────────────────────────────────

    private static ExpenseResponseDto MapToDto(Expense e) => new()
    {
        Id           = e.Id,
        Amount       = e.Amount,
        Description  = e.Description,
        Date         = e.Date,
        Notes        = e.Notes,
        CreatedAt    = e.CreatedAt,
        CategoryId   = e.CategoryId,
        CategoryName = e.Category?.Name ?? string.Empty
    };
}
