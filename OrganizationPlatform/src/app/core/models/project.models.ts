export interface Project {
  id: number;
  organizationId: number;
  name: string;
  description?: string;
  startDate: string;
  endDate?: string;
  status: string;
  progressPercentage: number;
  projectManagerId?: number;
  projectManagerName?: string;
  createdAt: string;
  members: ProjectMember[];
}

export interface ProjectMember {
  userId: number;
  fullName: string;
  email: string;
  projectRole: string;
  joinedAt: string;
}

export interface CreateProjectRequest {
  name: string;
  description?: string;
  startDate: string;
  endDate?: string;
  projectManagerId?: number;
}

export interface UpdateProjectRequest {
  name: string;
  description?: string;
  startDate: string;
  endDate?: string;
  status: string;
  progressPercentage: number;
  projectManagerId?: number;
}

export interface ProjectFilterParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
  projectManagerId?: number;
}
