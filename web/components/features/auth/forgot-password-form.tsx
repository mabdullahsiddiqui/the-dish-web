'use client'

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { Button3D } from '@/components/ui/Button3D';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { GlassCard } from '@/components/cards/GlassCard';
import { useAuth } from '@/hooks/useAuth';
import { FadeInUp } from '@/components/animations/FadeInUp';

const forgotPasswordSchema = z.object({
  email: z.string().email('Please enter a valid email address'),
});

type ForgotPasswordFormData = z.infer<typeof forgotPasswordSchema>;

export function ForgotPasswordForm() {
  const { forgotPassword, isForgotPasswordLoading } = useAuth();
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ForgotPasswordFormData>({
    resolver: zodResolver(forgotPasswordSchema),
  });

  const onSubmit = async (data: ForgotPasswordFormData) => {
    setIsSubmitting(true);
    try {
      await forgotPassword(data.email);
      // Navigate to reset password page with email as query parameter
      router.push(`/reset-password?email=${encodeURIComponent(data.email)}`);
    } catch (error) {
      setIsSubmitting(false);
    }
  };

  return (
    <GlassCard className="w-full max-w-md p-8 shadow-2xl">
      <FadeInUp>
        <div className="space-y-6">
          <div className="text-center space-y-2">
            <h2 className="text-2xl font-bold text-white">Forgot Password?</h2>
            <p className="text-gray-300 text-sm">
              Enter your email address and we'll send you a code to reset your password.
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

              <Button3D
                type="submit"
                variant="primary"
                className="w-full"
                disabled={isForgotPasswordLoading || isSubmitting}
              >
                {isForgotPasswordLoading || isSubmitting ? 'Sending...' : 'Send Reset Code'}
              </Button3D>
            </form>

          <div className="text-sm text-center text-gray-300 pt-4 border-t border-gray-700">
            Remember your password?{' '}
            <Link href="/login" className="text-indigo-400 hover:text-indigo-300 hover:underline font-medium">
              Sign in
            </Link>
          </div>
        </div>
      </FadeInUp>
    </GlassCard>
  );
}

