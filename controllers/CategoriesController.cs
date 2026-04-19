using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;

    public CategoriesController(AppDbContext db) => _db = db;

    // GET /api/categories
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponseDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _db.Categories
            .Select(c => new CategoryResponseDto
            {
                Id           = c.Id,
                Name         = c.Name,
                IsDefault    = c.IsDefault,
                ExpenseCount = c.Expenses.Count
            })
            .OrderBy(c => c.Name)
            .ToListAsync();

        return Ok(categories);
    }

    // POST /api/categories
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var exists = await _db.Categories
            .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

        if (exists)
            return Conflict(new { message = $"A category named '{dto.Name}' already exists." });

        var category = new Category { Name = dto.Name, IsDefault = false };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new CategoryResponseDto
        {
            Id           = category.Id,
            Name         = category.Name,
            IsDefault    = category.IsDefault,
            ExpenseCount = 0
        });
    }

    // DELETE /api/categories/{id}
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.Categories
            .Include(c => c.Expenses)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
            return NotFound(new { message = $"Category with id {id} not found." });

        if (category.IsDefault)
            return BadRequest(new { message = "Default categories cannot be deleted." });

        if (category.Expenses.Any())
            return BadRequest(new { message = "Cannot delete a category that has expenses. Reassign them first." });

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
