import { useAuth as useAuthContext } from '@/lib/auth/context';
import { authApi } from '@/lib/api/auth';
import { LoginRequest, RegisterRequest, ForgotPasswordRequest, ResetPasswordRequest } from '@/types/user';
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

  const googleLoginMutation = useMutation({
    mutationFn: authApi.googleLogin,
    onSuccess: (response) => {
      if (response.success && response.data) {
        auth.login(response.data.token, response.data.user);
        toast.success('Successfully logged in with Google!');
        router.push('/');
      } else {
        toast.error(response.message || 'Google login failed');
      }
    },
    onError: (error: any) => {
      toast.error(error.message || 'Google login failed. Please try again.');
    },
  });

  const facebookLoginMutation = useMutation({
    mutationFn: authApi.facebookLogin,
    onSuccess: (response) => {
      if (response.success && response.data) {
        auth.login(response.data.token, response.data.user);
        toast.success('Successfully logged in with Facebook!');
        router.push('/');
      } else {
        toast.error(response.message || 'Facebook login failed');
      }
    },
    onError: (error: any) => {
      toast.error(error.message || 'Facebook login failed. Please try again.');
    },
  });

  const login = (data: LoginRequest) => {
    loginMutation.mutate(data);
  };

  const register = (data: RegisterRequest) => {
    registerMutation.mutate(data);
  };

  const googleLogin = (token: string) => {
    return googleLoginMutation.mutateAsync(token);
  };

  const facebookLogin = (token: string) => {
    return facebookLoginMutation.mutateAsync(token);
  };

  const logout = () => {
    auth.logout();
    toast.success('Logged out successfully');
    router.push('/');
  };

  const forgotPasswordMutation = useMutation({
    mutationFn: (data: ForgotPasswordRequest) => authApi.forgotPassword(data.email),
    onSuccess: (response) => {
      if (response.success) {
        toast.success(response.message || 'If the email exists, a password reset code has been sent.');
      } else {
        toast.error(response.message || 'Failed to send reset code');
      }
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to send reset code. Please try again.');
    },
  });

  const resetPasswordMutation = useMutation({
    mutationFn: (data: ResetPasswordRequest) => authApi.resetPassword(data.email, data.code, data.newPassword),
    onSuccess: (response) => {
      if (response.success) {
        toast.success(response.message || 'Password has been reset successfully.');
        router.push('/login');
      } else {
        toast.error(response.message || 'Failed to reset password');
      }
    },
    onError: (error: any) => {
      toast.error(error.message || 'Failed to reset password. Please try again.');
    },
  });

  const forgotPassword = async (email: string) => {
    return forgotPasswordMutation.mutateAsync({ email });
  };

  const resetPassword = (email: string, code: string, newPassword: string) => {
    resetPasswordMutation.mutate({ email, code, newPassword });
  };

  return {
    ...auth,
    login,
    register,
    googleLogin,
    facebookLogin,
    logout,
    forgotPassword,
    resetPassword,
    isLoggingIn: loginMutation.isPending,
    isRegistering: registerMutation.isPending,
    isGoogleLoggingIn: googleLoginMutation.isPending,
    isFacebookLoggingIn: facebookLoginMutation.isPending,
    isForgotPasswordLoading: forgotPasswordMutation.isPending,
    isResetPasswordLoading: resetPasswordMutation.isPending,
  };
}
