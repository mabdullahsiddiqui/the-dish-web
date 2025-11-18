import { useAuth as useAuthContext } from '@/lib/auth/context';
import { authApi } from '@/lib/api/auth';
import { LoginRequest, RegisterRequest } from '@/types/user';
import { useMutation } from '@tanstack/react-query';
import { useRouter } from 'next/navigation';
import toast from 'react-hot-toast';

export function useAuth() {
  const auth = useAuthContext();
  const router = useRouter();

  const loginMutation = useMutation({
    mutationFn: authApi.login,
    onSuccess: (response) => {
      if (response.success && response.data) {
        auth.login(response.data.token, response.data.user);
        toast.success('Successfully logged in!');
        router.push('/');
      } else {
        toast.error(response.message || 'Login failed');
      }
    },
    onError: (error: any) => {
      toast.error(error.message || 'Login failed. Please try again.');
    },
  });

  const registerMutation = useMutation({
    mutationFn: authApi.register,
    onSuccess: (response) => {
      if (response.success && response.data) {
        auth.login(response.data.token, response.data.user);
        toast.success('Welcome to The Dish! Your account has been created.');
        router.push('/');
      } else {
        toast.error(response.message || 'Registration failed');
      }
    },
    onError: (error: any) => {
      toast.error(error.message || 'Registration failed. Please try again.');
    },
  });

  const login = (data: LoginRequest) => {
    loginMutation.mutate(data);
  };

  const register = (data: RegisterRequest) => {
    registerMutation.mutate(data);
  };

  const logout = () => {
    auth.logout();
    toast.success('Logged out successfully');
    router.push('/');
  };

  return {
    ...auth,
    login,
    register,
    logout,
    isLoggingIn: loginMutation.isPending,
    isRegistering: registerMutation.isPending,
  };
}
