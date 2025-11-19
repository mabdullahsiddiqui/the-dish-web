'use client'

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import Link from 'next/link';
import { Button3D } from '@/components/ui/Button3D';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { GlassCard } from '@/components/cards/GlassCard';
import { useAuth } from '@/hooks/useAuth';
import { LoginRequest } from '@/types/user';
import { Eye, EyeOff } from 'lucide-react';

const loginSchema = z.object({
  email: z.string().email('Please enter a valid email address'),
  password: z.string().min(1, 'Password is required'),
  rememberMe: z.boolean().optional(),
});

type LoginFormData = z.infer<typeof loginSchema>;

export function LoginForm() {
  const { login, isLoggingIn } = useAuth();
  const [error, setError] = useState<string>('');
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    setError('');
    try {
      await login({
        email: data.email,
        password: data.password,
      });
    } catch (err: any) {
      setError(err.message || 'Login failed. Please try again.');
    }
  };

  return (
    <GlassCard className="w-full max-w-md p-8 shadow-2xl">
      <div className="space-y-6">
        <div className="text-center space-y-2">
          <h2 className="text-2xl font-bold text-white">Welcome back</h2>
          <p className="text-gray-300 text-sm">
            Sign in to your account to continue
          </p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="email" className="text-gray-200">Email</Label>
            <Input
              id="email"
              type="email"
              placeholder="Enter your email"
              className="bg-gray-800/50 border-gray-700 text-white placeholder-gray-400"
              {...register('email')}
            />
            {errors.email && (
              <p className="text-sm text-red-400">{errors.email.message}</p>
            )}
          </div>
          
          <div className="space-y-2">
            <Label htmlFor="password" className="text-gray-200">Password</Label>
            <div className="relative">
              <Input
                id="password"
                type={showPassword ? 'text' : 'password'}
                placeholder="Enter your password"
                className="bg-gray-800/50 border-gray-700 text-white placeholder-gray-400 pr-10"
                {...register('password')}
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-300"
              >
                {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
              </button>
            </div>
            {errors.password && (
              <p className="text-sm text-red-400">{errors.password.message}</p>
            )}
          </div>

          <div className="flex items-center space-x-2">
            <input
              type="checkbox"
              id="rememberMe"
              className="h-4 w-4 rounded border-gray-600 bg-gray-800/50 text-indigo-500 accent-indigo-500"
              {...register('rememberMe')}
            />
            <Label htmlFor="rememberMe" className="text-sm font-normal text-gray-300">
              Remember me
            </Label>
          </div>

          {error && (
            <div className="text-sm text-red-400 bg-red-500/10 border border-red-500/30 p-3 rounded-md">
              {error}
            </div>
          )}

          <Button3D
            type="submit"
            variant="primary"
            className="w-full"
            disabled={isLoggingIn}
          >
            {isLoggingIn ? 'Signing in...' : 'Sign In'}
          </Button3D>
        </form>

        <div className="text-sm text-center text-gray-300 pt-4 border-t border-gray-700">
          Don't have an account?{' '}
          <Link href="/register" className="text-indigo-400 hover:text-indigo-300 hover:underline font-medium">
            Sign up
          </Link>
        </div>
      </div>
    </GlassCard>
  );
}

