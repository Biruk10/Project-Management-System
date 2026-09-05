export interface Task {
  id: number;
  organizationId: number;
  projectId: number;
  projectName: string;
  title: string;
  description?: string;
  assignedToUserId?: number;
  assignedToUserName?: string;
  priority: string;
  status: string;
  dueDate?: string;
  completionPercentage: number;
  createdAt: string;
}

export interface CreateTaskRequest {
  projectId: number;
  title: string;
  description?: string;
  assignedToUserId?: number;
  priority: string;
  dueDate?: string;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  assignedToUserId?: number;
  priority: string;
  status: string;
  dueDate?: string;
  completionPercentage: number;
}

export interface TaskFilterParams {
  page?: number;
  pageSize?: number;
  search?: string;
  projectId?: number;
  assignedToUserId?: number;
  status?: string;
  priority?: string;
  overdue?: boolean;
}
