using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Expense>  Expenses   => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User>     Users      => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

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
