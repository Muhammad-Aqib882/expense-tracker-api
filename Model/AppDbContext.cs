using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraint on category name
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        // Seed default categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Food",          IsDefault = true },
            new Category { Id = 2, Name = "Transport",     IsDefault = true },
            new Category { Id = 3, Name = "Utilities",     IsDefault = true },
            new Category { Id = 4, Name = "Health",        IsDefault = true },
            new Category { Id = 5, Name = "Entertainment", IsDefault = true },
            new Category { Id = 6, Name = "Other",         IsDefault = true }
        );
    }
}
