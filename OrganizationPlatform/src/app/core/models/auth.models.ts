export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterOrganizationRequest {
  name: string;
  email: string;
  logoUrl?: string;
  address?: string;
  phone?: string;
  timeZone?: string;
  adminFirstName: string;
  adminLastName: string;
  adminEmail: string;
  adminPassword: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserProfile;
  organization: OrganizationSummary;
}

export interface UserProfile {
  id: number;
  organizationId: number;
  firstName: string;
  lastName: string;
  email: string;
  roles: string[];
}

export interface OrganizationSummary {
  id: number;
  name: string;
  email: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}
