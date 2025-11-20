import { api } from './client';
import { AuthResponse, LoginRequest, RegisterRequest } from '@/types/user';

export const authApi = {
  // Register new user
  register: async (data: RegisterRequest) => {
    const response = await api.post<AuthResponse>('/users/register', data);
    return response.data;
  },

  // Login user
  login: async (data: LoginRequest) => {
    const response = await api.post<AuthResponse>('/users/login', data);
    return response.data;
  },

  // Google login
  googleLogin: async (token: string) => {
    const response = await api.post<AuthResponse>('/users/auth/google', {
      provider: 'Google',
      token: token,
    });
    return response.data;
  },

  // Facebook login
  facebookLogin: async (token: string) => {
    const response = await api.post<AuthResponse>('/users/auth/facebook', {
      provider: 'Facebook',
      token: token,
    });
    return response.data;
  },

  // Logout (client-side token removal)
  logout: () => {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('user');
    }
  },

  // Validate token (check if user is still authenticated)
  validateToken: async () => {
    const response = await api.get('/users/me');
    return response.data;
  },

  // Forgot password
  forgotPassword: async (email: string) => {
    const response = await api.post<{ success: boolean; message?: string }>('/users/forgot-password', {
      email,
    });
    return response.data;
  },

  // Reset password
  resetPassword: async (email: string, code: string, newPassword: string) => {
    const response = await api.post<{ success: boolean; message?: string }>('/users/reset-password', {
      email,
      code,
      newPassword,
    });
    return response.data;
  },
};

