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
};

