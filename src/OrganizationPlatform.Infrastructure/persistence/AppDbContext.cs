using Microsoft.EntityFrameworkCore;
using OrganizationPlat.src.Domain;
using organizationPlatform.Domain.Entities;
namespace organizationPlatform.Infrastructure.Persistence;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
public DbSet<User> Users => Set<User>();
public DbSet<TaskItem> TaskItems => Set<TaskItem>();
public DbSet<Expense> Expenses => Set<Expense>();
public DbSet<BudgetLine> BudgetLines => Set<BudgetLine>();
public DbSet<Budget> Budgets => Set<Budget>();
//public DbSet<Expense> Expenses => Set<Expense>();
}