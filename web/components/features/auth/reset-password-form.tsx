'use client'

import { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { Button3D } from '@/components/ui/Button3D';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { GlassCard } from '@/components/cards/GlassCard';
import { useAuth } from '@/hooks/useAuth';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { Eye, EyeOff } from 'lucide-react';

const resetPasswordSchema = z.object({
  email: z.string().email('Please enter a valid email address'),
  code: z.string().length(6, 'Code must be exactly 6 digits').regex(/^\d+$/, 'Code must contain only numbers'),
  newPassword: z.string()
    .min(8, 'Password must be at least 8 characters')
    .regex(/[A-Z]/, 'Password must contain at least one uppercase letter')
    .regex(/[a-z]/, 'Password must contain at least one lowercase letter')
    .regex(/[0-9]/, 'Password must contain at least one number')
    .regex(/[^A-Za-z0-9]/, 'Password must contain at least one special character'),
  confirmPassword: z.string(),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: 'Passwords do not match',
  path: ['confirmPassword'],
});

type ResetPasswordFormData = z.infer<typeof resetPasswordSchema>;

export function ResetPasswordForm() {
  const { resetPassword, isResetPasswordLoading } = useAuth();
  const searchParams = useSearchParams();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<ResetPasswordFormData>({
    resolver: zodResolver(resetPasswordSchema),
  });

  // Pre-fill email from query param if available
  useEffect(() => {
    const email = searchParams.get('email');
    if (email) {
      setValue('email', email);
    }
  }, [searchParams, setValue]);

  const onSubmit = async (data: ResetPasswordFormData) => {
    resetPassword(data.email, data.code, data.newPassword);
  };

  return (
    <GlassCard className="w-full max-w-md p-8 shadow-2xl">
      <FadeInUp>
        <div className="space-y-6">
          <div className="text-center space-y-2">
            <h2 className="text-2xl font-bold text-white">Reset Password</h2>
            <p className="text-gray-300 text-sm">
              Enter the 6-digit code sent to your email and your new password.
            </p>
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="email" className="text-gray-200">Email</Label>
              <Input
                id="email"
                type="email"
                placeholder="Enter your email"
                className="bg-gray-900/50 border-gray-700 text-gray-400 placeholder-gray-500 cursor-not-allowed opacity-75"
                readOnly
                disabled
                {...register('email')}
              />
              {errors.email && (
                <p className="text-sm text-red-400">{errors.email.message}</p>
              )}
              <p className="text-xs text-gray-400">
                Email cannot be changed. If you need to reset a different email, go back to the forgot password page.
              </p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="code" className="text-gray-200">Reset Code</Label>
              <Input
                id="code"
                type="text"
                placeholder="000000"
                maxLength={6}
                className="bg-gray-800/50 border-gray-700 text-white placeholder-gray-400 text-center text-2xl tracking-widest font-mono"
                {...register('code')}
              />
              {errors.code && (
                <p className="text-sm text-red-400">{errors.code.message}</p>
              )}
              <p className="text-xs text-gray-400">
                Enter the 6-digit code sent to your email
              </p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="newPassword" className="text-gray-200">New Password</Label>
              <div className="relative">
                <Input
                  id="newPassword"
                  type={showPassword ? 'text' : 'password'}
                  placeholder="Create a new password"
                  className="bg-gray-800/50 border-gray-700 text-white placeholder-gray-400 pr-10"
                  {...register('newPassword')}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-300"
                >
                  {showPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
              {errors.newPassword && (
                <p className="text-sm text-red-400">{errors.newPassword.message}</p>
              )}
              <p className="text-xs text-gray-400">
                Must be at least 8 characters with uppercase, lowercase, number, and special character
              </p>
            </div>

            <div className="space-y-2">
              <Label htmlFor="confirmPassword" className="text-gray-200">Confirm Password</Label>
              <div className="relative">
                <Input
                  id="confirmPassword"
                  type={showConfirmPassword ? 'text' : 'password'}
                  placeholder="Confirm your new password"
                  className="bg-gray-800/50 border-gray-700 text-white placeholder-gray-400 pr-10"
                  {...register('confirmPassword')}
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-300"
                >
                  {showConfirmPassword ? <EyeOff size={20} /> : <Eye size={20} />}
                </button>
              </div>
              {errors.confirmPassword && (
                <p className="text-sm text-red-400">{errors.confirmPassword.message}</p>
              )}
            </div>

            <Button3D
              type="submit"
              variant="primary"
              className="w-full"
              disabled={isResetPasswordLoading}
            >
              {isResetPasswordLoading ? 'Resetting...' : 'Reset Password'}
            </Button3D>
          </form>

          <div className="text-sm text-center text-gray-300 pt-4 border-t border-gray-700 space-y-2">
            <div>
              Don't have a code?{' '}
              <Link href="/forgot-password" className="text-indigo-400 hover:text-indigo-300 hover:underline font-medium">
                Request a new one
              </Link>
            </div>
            <div>
              Need to reset a different email?{' '}
              <Link href="/forgot-password" className="text-indigo-400 hover:text-indigo-300 hover:underline font-medium">
                Go back
              </Link>
            </div>
          </div>
        </div>
      </FadeInUp>
    </GlassCard>
  );
}

