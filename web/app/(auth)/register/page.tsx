import { RegisterForm } from '@/components/features/auth/register-form';

export default function RegisterPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-gray-900">The Dish</h1>
          <p className="mt-2 text-gray-600">
            Join our community of food lovers
          </p>
        </div>
        <RegisterForm />
      </div>
    </div>
  );
}

