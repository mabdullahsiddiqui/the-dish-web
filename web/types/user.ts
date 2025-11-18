export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  joinDate: string;
  reputation: number;
  reviewCount: number;
  isVerified: boolean;
}

export interface AuthResponse {
  token: string;
  user: User;
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

