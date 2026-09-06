using Microsoft.Extensions.DependencyInjection;
using ShakaOrganizationPlatform.Application.AuditLogs.Services;
using ShakaOrganizationPlatform.Application.Budgets.Services;
using ShakaOrganizationPlatform.Application.Dashboard.Services;
using ShakaOrganizationPlatform.Application.Expenses.Services;
using ShakaOrganizationPlatform.Application.Notifications.Services;
using ShakaOrganizationPlatform.Application.Organizations.Services;
using ShakaOrganizationPlatform.Application.Permissions.Services;
using ShakaOrganizationPlatform.Application.Projects.Services;
using ShakaOrganizationPlatform.Application.Reports.Services;
using ShakaOrganizationPlatform.Application.Roles.Services;
using ShakaOrganizationPlatform.Application.Tasks.Services;
using ShakaOrganizationPlatform.Application.Users.Services;

namespace ShakaOrganizationPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IBudgetService, BudgetService>();
        services.AddScoped<IBudgetRequestService, BudgetRequestService>();
        services.AddScoped<IExpenseService, ExpenseService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        return services;
    }
}
