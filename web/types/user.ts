export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  externalProvider?: 'Email' | 'Google' | 'Facebook';
  joinDate: string;
  reputation: number;
  reviewCount: number;
  isVerified: boolean;
}

export interface AuthResponse {
  success: boolean;
  data?: {
    token: string;
    user: User;
  };
  message?: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  newPassword: string;
}

