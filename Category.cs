using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;

    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
