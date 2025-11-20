'use client'

import { Suspense } from 'react';
import { ResetPasswordForm } from '@/components/features/auth/reset-password-form';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { LoadingSpinner } from '@/components/ui/loading';

function ResetPasswordPageContent() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-[#0f172a] py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <FadeInUp>
          <div className="text-center">
            <h1 className="text-3xl font-bold text-white">The Dish</h1>
            <p className="mt-2 text-gray-300">
              Create a new password
            </p>
          </div>
        </FadeInUp>
        <FadeInUp delay={0.1}>
          <ResetPasswordForm />
        </FadeInUp>
      </div>
    </div>
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={
      <div className="min-h-screen flex items-center justify-center bg-[#0f172a]">
        <LoadingSpinner />
      </div>
    }>
      <ResetPasswordPageContent />
    </Suspense>
  );
}

