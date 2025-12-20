'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Settings, User, Bell, Lock, Trash2, Save, X } from 'lucide-react';
import { Button3D } from '@/components/ui/Button3D';
import { GlassCard } from '@/components/cards/GlassCard';
import { FadeInUp } from '@/components/animations/FadeInUp';
import { useAuth } from '@/hooks/useAuth';
import { cn } from '@/lib/utils/cn';
import toast from 'react-hot-toast';

const DIETARY_OPTIONS = [
  { value: 'Halal', label: 'Halal', icon: '🕌' },
  { value: 'Kosher', label: 'Kosher', icon: '✡️' },
  { value: 'Vegan', label: 'Vegan', icon: '🌱' },
  { value: 'Vegetarian', label: 'Vegetarian', icon: '🥗' },
  { value: 'Gluten-Free', label: 'Gluten-Free', icon: '🌾' },
  { value: 'Dairy-Free', label: 'Dairy-Free', icon: '🥛' },
  { value: 'Nut-Free', label: 'Nut-Free', icon: '🥜' },
  { value: 'Keto', label: 'Keto-Friendly', icon: '🥑' },
];

const profileSchema = z.object({
  firstName: z.string().min(1, 'First name is required'),
  lastName: z.string().min(1, 'Last name is required'),
  email: z.string().email('Invalid email address'),
});

const passwordSchema = z.object({
  currentPassword: z.string().min(8, 'Password must be at least 8 characters'),
  newPassword: z.string().min(8, 'Password must be at least 8 characters'),
  confirmPassword: z.string(),
}).refine((data) => data.newPassword === data.confirmPassword, {
  message: "Passwords don't match",
  path: ['confirmPassword'],
});

type ProfileFormData = z.infer<typeof profileSchema>;
type PasswordFormData = z.infer<typeof passwordSchema>;

export default function SettingsPage() {
  const { user, isAuthenticated, isLoading } = useAuth();
  const router = useRouter();
  const [activeTab, setActiveTab] = useState<'profile' | 'preferences' | 'security' | 'notifications'>('profile');
  const [selectedDietaryPreferences, setSelectedDietaryPreferences] = useState<string[]>([]);
  const [isSaving, setIsSaving] = useState(false);

  const profileForm = useForm<ProfileFormData>({
    resolver: zodResolver(profileSchema),
    defaultValues: {
      firstName: user?.firstName || '',
      lastName: user?.lastName || '',
      email: user?.email || '',
    },
  });

  const passwordForm = useForm<PasswordFormData>({
    resolver: zodResolver(passwordSchema),
  });

  useEffect(() => {
    if (!isAuthenticated && !isLoading) {
      router.push('/login');
    }
  }, [isAuthenticated, isLoading, router]);

  useEffect(() => {
    if (user) {
      profileForm.reset({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
      });
      // TODO: Load user's dietary preferences from backend
      // setSelectedDietaryPreferences(user.dietaryPreferences || []);
    }
  }, [user, profileForm]);

  if (isLoading) {
    return (
      <div className="min-h-screen bg-[#0f172a]">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-700 rounded w-1/3 mb-6"></div>
            <div className="h-64 bg-gray-700 rounded"></div>
          </div>
        </div>
      </div>
    );
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  const toggleDietaryPreference = (value: string) => {
    setSelectedDietaryPreferences((prev) =>
      prev.includes(value) ? prev.filter((p) => p !== value) : [...prev, value]
    );
  };

  const onProfileSubmit = async (data: ProfileFormData) => {
    setIsSaving(true);
    try {
      // TODO: Call API to update user profile
      await new Promise((resolve) => setTimeout(resolve, 1000));
      toast.success('Profile updated successfully!');
    } catch (error) {
      toast.error('Failed to update profile');
    } finally {
      setIsSaving(false);
    }
  };

  const onPasswordSubmit = async (data: PasswordFormData) => {
    setIsSaving(true);
    try {
      // TODO: Call API to change password
      await new Promise((resolve) => setTimeout(resolve, 1000));
      toast.success('Password changed successfully!');
      passwordForm.reset();
    } catch (error) {
      toast.error('Failed to change password');
    } finally {
      setIsSaving(false);
    }
  };

  const saveDietaryPreferences = async () => {
    setIsSaving(true);
    try {
      // TODO: Call API to update dietary preferences
      await new Promise((resolve) => setTimeout(resolve, 1000));
      toast.success('Dietary preferences saved!');
    } catch (error) {
      toast.error('Failed to save dietary preferences');
    } finally {
      setIsSaving(false);
    }
  };

  const tabs = [
    { id: 'profile' as const, label: 'Profile', icon: User },
    { id: 'preferences' as const, label: 'Dietary Preferences', icon: Settings },
    { id: 'security' as const, label: 'Security', icon: Lock },
    { id: 'notifications' as const, label: 'Notifications', icon: Bell },
  ];

  return (
    <div className="min-h-screen bg-[#0f172a]">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <FadeInUp>
          <div className="mb-8">
            <h1 className="text-3xl font-bold text-white mb-2">Settings</h1>
            <p className="text-gray-300">Manage your account settings and preferences</p>
          </div>
        </FadeInUp>

        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Sidebar Tabs */}
          <FadeInUp delay={0.1}>
            <GlassCard className="p-4 lg:col-span-1">
              <nav className="space-y-2">
                {tabs.map((tab) => {
                  const Icon = tab.icon;
                  return (
                    <button
                      key={tab.id}
                      onClick={() => setActiveTab(tab.id)}
                      className={cn(
                        'w-full flex items-center space-x-3 px-4 py-3 rounded-lg transition-all text-left',
                        activeTab === tab.id
                          ? 'bg-indigo-500/20 text-indigo-400 border border-indigo-500/50'
                          : 'text-gray-300 hover:bg-white/5'
                      )}
                    >
                      <Icon className="w-5 h-5" />
                      <span className="font-medium">{tab.label}</span>
                    </button>
                  );
                })}
              </nav>
            </GlassCard>
          </FadeInUp>

          {/* Content Area */}
          <div className="lg:col-span-3 space-y-6">
            {/* Profile Tab */}
            {activeTab === 'profile' && (
              <FadeInUp delay={0.2}>
                <GlassCard className="p-6">
                  <h2 className="text-xl font-bold text-white mb-6">Profile Information</h2>
                  <form onSubmit={profileForm.handleSubmit(onProfileSubmit)} className="space-y-4">
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-300 mb-2">
                          First Name
                        </label>
                        <input
                          {...profileForm.register('firstName')}
                          className="w-full bg-white/5 border border-white/10 rounded-lg p-3 text-white placeholder:text-gray-500 focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                        />
                        {profileForm.formState.errors.firstName && (
                          <p className="text-red-400 text-sm mt-1">
                            {profileForm.formState.errors.firstName.message}
                          </p>
                        )}
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-300 mb-2">
                          Last Name
                        </label>
                        <input
                          {...profileForm.register('lastName')}
                          className="w-full bg-white/5 border border-white/10 rounded-lg p-3 text-white placeholder:text-gray-500 focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                        />
                        {profileForm.formState.errors.lastName && (
                          <p className="text-red-400 text-sm mt-1">
                            {profileForm.formState.errors.lastName.message}
                          </p>
                        )}
                      </div>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-300 mb-2">
                        Email Address
                      </label>
                      <input
                        {...profileForm.register('email')}
                        disabled
                        className="w-full bg-white/5 border border-white/10 rounded-lg p-3 text-gray-400 cursor-not-allowed"
                      />
                      <p className="text-xs text-gray-500 mt-1">
                        Email cannot be changed. Contact support if needed.
                      </p>
                    </div>

                    <div className="flex justify-end space-x-3 pt-4">
                      <Button3D
                        type="button"
                        variant="outline"
                        onClick={() => profileForm.reset()}
                      >
                        <X className="w-4 h-4 mr-2" />
                        Cancel
                      </Button3D>
                      <Button3D type="submit" disabled={isSaving} glow>
                        <Save className="w-4 h-4 mr-2" />
                        {isSaving ? 'Saving...' : 'Save Changes'}
                      </Button3D>
                    </div>
                  </form>
                </GlassCard>
              </FadeInUp>
            )}

            {/* Dietary Preferences Tab */}
            {activeTab === 'preferences' && (
              <FadeInUp delay={0.2}>
                <GlassCard className="p-6">
                  <h2 className="text-xl font-bold text-white mb-2">Dietary Preferences</h2>
                  <p className="text-sm text-gray-300 mb-6">
                    Select your dietary preferences to get personalized restaurant recommendations
                  </p>

                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6">
                    {DIETARY_OPTIONS.map((option) => (
                      <label
                        key={option.value}
                        className={cn(
                          'flex items-center space-x-3 p-4 rounded-lg border cursor-pointer transition-all',
                          selectedDietaryPreferences.includes(option.value)
                            ? 'bg-indigo-500/20 border-indigo-500'
                            : 'bg-white/5 border-transparent hover:bg-white/10'
                        )}
                      >
                        <input
                          type="checkbox"
                          checked={selectedDietaryPreferences.includes(option.value)}
                          onChange={() => toggleDietaryPreference(option.value)}
                          className="sr-only"
                        />
                        <span className="text-2xl">{option.icon}</span>
                        <span className="text-white font-medium">{option.label}</span>
                        {selectedDietaryPreferences.includes(option.value) && (
                          <div className="ml-auto w-5 h-5 bg-indigo-500 rounded-full flex items-center justify-center">
                            <X className="w-3 h-3 text-white" />
                          </div>
                        )}
                      </label>
                    ))}
                  </div>

                  <div className="flex justify-end">
                    <Button3D onClick={saveDietaryPreferences} disabled={isSaving} glow>
                      <Save className="w-4 h-4 mr-2" />
                      {isSaving ? 'Saving...' : 'Save Preferences'}
                    </Button3D>
                  </div>
                </GlassCard>
              </FadeInUp>
            )}

            {/* Security Tab */}
            {activeTab === 'security' && (
              <FadeInUp delay={0.2}>
                <GlassCard className="p-6">
                  <h2 className="text-xl font-bold text-white mb-6">Change Password</h2>
                  <form onSubmit={passwordForm.handleSubmit(onPasswordSubmit)} className="space-y-4">
                    <div>
                      <label className="block text-sm font-medium text-gray-300 mb-2">
                        Current Password
                      </label>
                      <input
                        {...passwordForm.register('currentPassword')}
                        type="password"
                        className="w-full bg-white/5 border border-white/10 rounded-lg p-3 text-white placeholder:text-gray-500 focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                      />
                      {passwordForm.formState.errors.currentPassword && (
                        <p className="text-red-400 text-sm mt-1">
                          {passwordForm.formState.errors.currentPassword.message}
                        </p>
                      )}
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-300 mb-2">
                        New Password
                      </label>
                      <input
                        {...passwordForm.register('newPassword')}
                        type="password"
                        className="w-full bg-white/5 border border-white/10 rounded-lg p-3 text-white placeholder:text-gray-500 focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                      />
                      {passwordForm.formState.errors.newPassword && (
                        <p className="text-red-400 text-sm mt-1">
                          {passwordForm.formState.errors.newPassword.message}
                        </p>
                      )}
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-300 mb-2">
                        Confirm New Password
                      </label>
                      <input
                        {...passwordForm.register('confirmPassword')}
                        type="password"
                        className="w-full bg-white/5 border border-white/10 rounded-lg p-3 text-white placeholder:text-gray-500 focus:ring-2 focus:ring-indigo-500 focus:border-transparent outline-none"
                      />
                      {passwordForm.formState.errors.confirmPassword && (
                        <p className="text-red-400 text-sm mt-1">
                          {passwordForm.formState.errors.confirmPassword.message}
                        </p>
                      )}
                    </div>

                    <div className="flex justify-end space-x-3 pt-4">
                      <Button3D
                        type="button"
                        variant="outline"
                        onClick={() => passwordForm.reset()}
                      >
                        Cancel
                      </Button3D>
                      <Button3D type="submit" disabled={isSaving} glow>
                        {isSaving ? 'Changing...' : 'Change Password'}
                      </Button3D>
                    </div>
                  </form>

                  {/* Delete Account Section */}
                  <div className="mt-8 pt-8 border-t border-red-500/20">
                    <h3 className="text-lg font-bold text-red-400 mb-2">Danger Zone</h3>
                    <p className="text-sm text-gray-300 mb-4">
                      Once you delete your account, there is no going back. Please be certain.
                    </p>
                    <Button3D
                      variant="outline"
                      onClick={() => {
                        if (confirm('Are you sure you want to delete your account? This action cannot be undone.')) {
                          toast.success('Account deletion requested. Contact support to complete.');
                        }
                      }}
                      className="!border-red-500 !text-red-400 hover:!bg-red-500/10"
                    >
                      <Trash2 className="w-4 h-4 mr-2" />
                      Delete Account
                    </Button3D>
                  </div>
                </GlassCard>
              </FadeInUp>
            )}

            {/* Notifications Tab */}
            {activeTab === 'notifications' && (
              <FadeInUp delay={0.2}>
                <GlassCard className="p-6">
                  <h2 className="text-xl font-bold text-white mb-6">Notification Preferences</h2>
                  <div className="space-y-4">
                    {[
                      { label: 'Email notifications for new reviews', key: 'emailReviews' },
                      { label: 'Email notifications for helpful votes', key: 'emailVotes' },
                      { label: 'Email notifications for friend activity', key: 'emailFriends' },
                      { label: 'Marketing emails and updates', key: 'emailMarketing' },
                    ].map((item) => (
                      <label key={item.key} className="flex items-center justify-between p-4 rounded-lg bg-white/5 hover:bg-white/10 cursor-pointer transition-all">
                        <span className="text-white">{item.label}</span>
                        <input
                          type="checkbox"
                          className="w-5 h-5 rounded accent-indigo-500"
                          defaultChecked
                        />
                      </label>
                    ))}
                  </div>

                  <div className="flex justify-end mt-6">
                    <Button3D onClick={() => toast.success('Notification preferences saved!')} glow>
                      <Save className="w-4 h-4 mr-2" />
                      Save Preferences
                    </Button3D>
                  </div>
                </GlassCard>
              </FadeInUp>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
