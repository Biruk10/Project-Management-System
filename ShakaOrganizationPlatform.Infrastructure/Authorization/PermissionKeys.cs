namespace ShakaOrganizationPlatform.Infrastructure.Authorization;

public static class PermissionKeys
{
    public const string ProjectCreate = "Project.Create";
    public const string ProjectRead = "Project.Read";
    public const string ProjectUpdate = "Project.Update";
    public const string ProjectDelete = "Project.Delete";

    public const string TaskCreate = "Task.Create";
    public const string TaskRead = "Task.Read";
    public const string TaskUpdate = "Task.Update";
    public const string TaskDelete = "Task.Delete";
    public const string TaskAssign = "Task.Assign";

    public const string BudgetCreate = "Budget.Create";
    public const string BudgetRead = "Budget.Read";
    public const string BudgetApprove = "Budget.Approve";

    public const string ExpenseCreate = "Expense.Create";
    public const string ExpenseRead = "Expense.Read";

    public const string ReportView = "Report.View";
    public const string ReportExport = "Report.Export";

    public const string UserCreate = "User.Create";
    public const string UserRead = "User.Read";
    public const string UserUpdate = "User.Update";
    public const string UserDelete = "User.Delete";

    public const string RoleCreate = "Role.Create";
    public const string RoleRead = "Role.Read";
    public const string RoleUpdate = "Role.Update";
    public const string RoleDelete = "Role.Delete";

    public const string OrganizationRead = "Organization.Read";
    public const string OrganizationUpdate = "Organization.Update";

    public const string AuditLogRead = "AuditLog.Read";
    public const string NotificationRead = "Notification.Read";
}
