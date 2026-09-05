using Microsoft.EntityFrameworkCore;
using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Expenses.DTOs;
using ShakaOrganizationPlatform.Domain.Entities;

namespace ShakaOrganizationPlatform.Application.Expenses.Services;

public class ExpenseService : IExpenseService
{
    private readonly IAppDbContext _context;
    private readonly ITenantService _tenantService;
    private readonly ICurrentUserService _currentUserService;

    public ExpenseService(IAppDbContext context, ITenantService tenantService, ICurrentUserService currentUserService)
    {
        _context = context;
        _tenantService = tenantService;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<ExpenseDto>> GetAllAsync(ExpenseFilterParams filters, CancellationToken cancellationToken = default)
    {
        var query = _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.BudgetLine)
            .Include(e => e.RecordedByUser)
            .AsQueryable();

        if (filters.ProjectId.HasValue)
            query = query.Where(e => e.ProjectId == filters.ProjectId.Value);

        if (filters.BudgetLineId.HasValue)
            query = query.Where(e => e.BudgetLineId == filters.BudgetLineId.Value);

        if (filters.From.HasValue)
            query = query.Where(e => e.ExpenseDate >= filters.From.Value);

        if (filters.To.HasValue)
            query = query.Where(e => e.ExpenseDate <= filters.To.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var expenses = await query
            .OrderByDescending(e => e.ExpenseDate)
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .ToListAsync(cancellationToken);

        var items = expenses.Select(MapToDto).ToList();
        return PagedResult<ExpenseDto>.Create(items, totalCount, filters.Page, filters.PageSize);
    }

    public async Task<ExpenseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await _context.Expenses
            .Include(e => e.Project)
            .Include(e => e.BudgetLine)
            .Include(e => e.RecordedByUser)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return expense == null ? null : MapToDto(expense);
    }

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        var orgId = _tenantService.OrganizationId
            ?? throw new InvalidOperationException("No tenant context.");

        var projectExists = await _context.Projects.AnyAsync(p => p.Id == dto.ProjectId, cancellationToken);
        if (!projectExists)
            throw new KeyNotFoundException($"Project {dto.ProjectId} not found.");

        var expense = new Expense
        {
            OrganizationId = orgId,
            ProjectId = dto.ProjectId,
            BudgetLineId = dto.BudgetLineId,
            Amount = dto.Amount,
            Description = dto.Description,
            ExpenseDate = dto.ExpenseDate,
            RecordedByUserId = _currentUserService.UserId,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUserService.UserId
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(expense.Id, cancellationToken))!;
    }

    public async Task<ExpenseDto> UpdateAsync(int id, UpdateExpenseDto dto, CancellationToken cancellationToken = default)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Expense {id} not found.");

        expense.BudgetLineId = dto.BudgetLineId;
        expense.Amount = dto.Amount;
        expense.Description = dto.Description;
        expense.ExpenseDate = dto.ExpenseDate;
        expense.UpdatedAt = DateTime.UtcNow;
        expense.UpdatedByUserId = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        return (await GetByIdAsync(id, cancellationToken))!;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var expense = await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"Expense {id} not found.");

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            OrganizationId = expense.OrganizationId,
            ProjectId = expense.ProjectId,
            ProjectName = expense.Project?.Name ?? string.Empty,
            BudgetLineId = expense.BudgetLineId,
            BudgetLineCategory = expense.BudgetLine?.Category,
            Amount = expense.Amount,
            Description = expense.Description,
            ExpenseDate = expense.ExpenseDate,
            RecordedByUserId = expense.RecordedByUserId,
            RecordedByUserName = expense.RecordedByUser != null
                ? $"{expense.RecordedByUser.FirstName} {expense.RecordedByUser.LastName}"
                : null,
            CreatedAt = expense.CreatedAt
        };
    }
}
