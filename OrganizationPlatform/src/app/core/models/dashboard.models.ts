export interface DashboardSummary {
  totalProjects: number;
  activeProjects: number;
  completedProjects: number;
  totalTasks: number;
  overdueTasks: number;
  upcomingTasks: number;
  totalBudget: number;
  totalExpenses: number;
  remainingBudget: number;
  budgetUtilizationPercentage: number;
  totalUsers: number;
  activeUsers: number;
  projectProgress: ProjectProgress[];
  recentActivities: RecentActivity[];
}

export interface ProjectProgress {
  projectId: number;
  projectName: string;
  status: string;
  progressPercentage: number;
  budgetUtilization: number;
  totalTasks: number;
  completedTasks: number;
}

export interface RecentActivity {
  action: string;
  entityName: string;
  userName?: string;
  occurredAt: string;
}
