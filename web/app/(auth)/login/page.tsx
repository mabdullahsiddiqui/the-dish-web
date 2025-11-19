import { LoginForm } from '@/components/features/auth/login-form';
import { FadeInUp } from '@/components/animations/FadeInUp';

export default function LoginPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-[#0f172a] py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <FadeInUp>
          <div className="text-center">
            <h1 className="text-3xl font-bold text-white">The Dish</h1>
            <p className="mt-2 text-gray-300">
              Discover great restaurants and share your experiences
            </p>
          </div>
        </FadeInUp>
        <FadeInUp delay={0.1}>
          <LoginForm />
        </FadeInUp>
      </div>
    </div>
  );
}

