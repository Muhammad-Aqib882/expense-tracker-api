using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.DTOs;

// ── Expense DTOs ──────────────────────────────────────────────────────────────

public class CreateExpenseDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    public DateOnly? Date { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    [Required]
    public int CategoryId { get; set; }
}

public class UpdateExpenseDto
{
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal? Amount { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    public DateOnly? Date { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int? CategoryId { get; set; }
}

public class ExpenseResponseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class ExpenseQueryParams
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

// ── Category DTOs ─────────────────────────────────────────────────────────────

public class CreateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public int ExpenseCount { get; set; }
}

// ── Summary DTOs ──────────────────────────────────────────────────────────────

public class SummaryResponseDto
{
    public decimal TotalSpend { get; set; }
    public int TotalExpenses { get; set; }
    public decimal AverageExpense { get; set; }
    public string TopCategory { get; set; } = string.Empty;
    public List<CategorySummaryDto> ByCategory { get; set; } = new();
}

public class CategorySummaryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class MonthlySummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
}
