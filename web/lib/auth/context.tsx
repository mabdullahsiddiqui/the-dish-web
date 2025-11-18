'use client'

import React, { createContext, useContext, useEffect, useState, ReactNode } from 'react';
import { User } from '@/types/user';
import { authApi } from '@/lib/api/auth';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (token: string, user: User) => void;
  logout: () => void;
  updateUser: (user: User) => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Initialize auth state from localStorage
  useEffect(() => {
    const initializeAuth = async () => {
      try {
        // Only access localStorage in the browser
        if (typeof window === 'undefined') {
          setIsLoading(false);
          return;
        }

        const storedToken = localStorage.getItem('auth_token');
        const storedUser = localStorage.getItem('user');

        if (storedToken && storedUser) {
          const parsedUser = JSON.parse(storedUser);
          setToken(storedToken);
          setUser(parsedUser);

          // Optionally validate token with server
          try {
            await authApi.validateToken();
          } catch (error) {
            // Token is invalid, clear auth state
            localStorage.removeItem('auth_token');
            localStorage.removeItem('user');
            setToken(null);
            setUser(null);
          }
        }
      } catch (error) {
        console.error('Error initializing auth:', error);
      } finally {
        setIsLoading(false);
      }
    };

    initializeAuth();
  }, []);

  const login = (newToken: string, newUser: User) => {
    if (typeof window !== 'undefined') {
      localStorage.setItem('auth_token', newToken);
      localStorage.setItem('user', JSON.stringify(newUser));
    }
    setToken(newToken);
    setUser(newUser);
  };

  const logout = () => {
    authApi.logout();
    setToken(null);
    setUser(null);
  };

  const updateUser = (newUser: User) => {
    if (typeof window !== 'undefined') {
      localStorage.setItem('user', JSON.stringify(newUser));
    }
    setUser(newUser);
  };

  const value: AuthContextType = {
    user,
    token,
    isLoading,
    isAuthenticated: !!user && !!token,
    login,
    logout,
    updateUser,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
