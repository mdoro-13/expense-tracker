using Api.Domain.Entities;
using Api.Infrastructure.Data.Configs;
using Microsoft.EntityFrameworkCore;
#pragma warning disable CS8618

namespace Api.Infrastructure.Data;

public class DataContext : DbContext
{
	public DataContext()
	{

	}

	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{

	}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        new BudgetConfig().Configure(modelBuilder.Entity<Budget>());
        new CategoryConfig().Configure(modelBuilder.Entity<Category>());
        new ExpenseConfig().Configure(modelBuilder.Entity<Expense>());
        new SpendingLimitConfig().Configure(modelBuilder.Entity<SpendingLimit>());
    }

    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<SpendingLimit> SpendingLimits { get; set; }


}
