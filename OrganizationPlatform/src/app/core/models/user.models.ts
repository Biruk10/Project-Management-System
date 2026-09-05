export interface User {
  id: number;
  organizationId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  isActive: boolean;
  status: string;
  lastLoginAt?: string;
  createdAt: string;
  roles: string[];
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  roleIds: number[];
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface UserFilterParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean;
}
