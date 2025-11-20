'use client'

import { useState, useEffect } from 'react';
import { GoogleLogin } from '@react-oauth/google';
import { Button3D } from '@/components/ui/Button3D';
import { useAuth } from '@/hooks/useAuth';
import toast from 'react-hot-toast';

declare global {
  interface Window {
    FB: any;
    fbAsyncInit: () => void;
  }
}

export function SocialLoginButtons() {
  const { googleLogin } = useAuth();
  const [isLoading, setIsLoading] = useState<'google' | 'facebook' | null>(null);
  // const [fbReady, setFbReady] = useState(false);
  const [mounted, setMounted] = useState(false);
  const [googleClientId, setGoogleClientId] = useState<string | undefined>(undefined);
  // const [facebookAppId, setFacebookAppId] = useState<string | undefined>(undefined);
  
  // Only read env vars on client side to avoid hydration mismatch
  useEffect(() => {
    setMounted(true);
    setGoogleClientId(process.env.NEXT_PUBLIC_GOOGLE_CLIENT_ID);
    // setFacebookAppId(process.env.NEXT_PUBLIC_FACEBOOK_APP_ID);
  }, []);

  // Initialize Facebook SDK - COMMENTED OUT (Facebook login not needed)
  // useEffect(() => {
  //   if (typeof window !== 'undefined' && !window.FB) {
  //     window.fbAsyncInit = function () {
  //       window.FB.init({
  //         appId: process.env.NEXT_PUBLIC_FACEBOOK_APP_ID || '',
  //         cookie: true,
  //         xfbml: true,
  //         version: 'v18.0',
  //       });
  //       setFbReady(true);
  //     };

  //     (function (d, s, id) {
  //       var js: any,
  //         fjs: any = d.getElementsByTagName(s)[0];
  //       if (d.getElementById(id)) return;
  //       js = d.createElement(s);
  //       js.id = id;
  //       js.src = 'https://connect.facebook.net/en_US/sdk.js';
  //       fjs.parentNode.insertBefore(js, fjs);
  //     })(document, 'script', 'facebook-jssdk');
  //   } else if (window.FB) {
  //     setFbReady(true);
  //   }
  // }, []);

  const handleGoogleSuccess = async (credentialResponse: any) => {
    setIsLoading('google');
    try {
      // credentialResponse.credential is the ID token
      await googleLogin(credentialResponse.credential);
    } catch (error: any) {
      toast.error(error.message || 'Google login failed');
    } finally {
      setIsLoading(null);
    }
  };

  const handleGoogleError = () => {
    toast.error('Google login failed. Please try again.');
    setIsLoading(null);
  };

  // Facebook login handler - COMMENTED OUT (Facebook login not needed)
  // const handleFacebookLogin = () => {
  //   if (!fbReady || !window.FB) {
  //     toast.error('Facebook SDK is not ready. Please refresh the page.');
  //     return;
  //   }

  //   setIsLoading('facebook');
  //   window.FB.login(
  //     async (response: any) => {
  //       if (response.authResponse) {
  //         try {
  //           await facebookLogin(response.authResponse.accessToken);
  //         } catch (error: any) {
  //           toast.error(error.message || 'Facebook login failed');
  //         } finally {
  //           setIsLoading(null);
  //         }
  //       } else {
  //         toast.error('Facebook login was cancelled or failed');
  //         setIsLoading(null);
  //       }
  //     },
  //     { scope: 'email,public_profile' }
  //   );
  // };

  // Don't render until mounted to avoid hydration mismatch
  if (!mounted) {
    return (
      <div className="space-y-3">
        <div className="w-full h-10 bg-gray-800/50 rounded animate-pulse"></div>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      {googleClientId ? (
        <div className="w-full flex justify-center">
          <div style={{ width: '100%', maxWidth: '400px' }}>
            <GoogleLogin
              onSuccess={handleGoogleSuccess}
              onError={handleGoogleError}
              useOneTap={false}
              theme="filled_black"
              size="large"
              text="signin_with"
              shape="rectangular"
            />
          </div>
        </div>
      ) : (
        <Button3D
          variant="outline"
          className="w-full opacity-50 cursor-not-allowed"
          disabled
          title="Google login not configured. Add NEXT_PUBLIC_GOOGLE_CLIENT_ID to .env.local"
        >
          <div className="flex items-center justify-center gap-2">
            <svg className="w-5 h-5" viewBox="0 0 24 24">
              <path
                fill="currentColor"
                d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
              />
              <path
                fill="currentColor"
                d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
              />
              <path
                fill="currentColor"
                d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
              />
              <path
                fill="currentColor"
                d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
              />
            </svg>
            Continue with Google (Not Configured)
          </div>
        </Button3D>
      )}

      {/* Facebook login button - COMMENTED OUT (Facebook login not needed) */}
      {/* {facebookAppId ? (
        <Button3D
          variant="outline"
          className="w-full"
          onClick={handleFacebookLogin}
          disabled={isLoading === 'facebook' || !fbReady}
        >
          {isLoading === 'facebook' ? (
            'Connecting...'
          ) : (
            <div className="flex items-center justify-center gap-2">
              <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
                <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z" />
              </svg>
              Continue with Facebook
            </div>
          )}
        </Button3D>
      ) : (
        <Button3D
          variant="outline"
          className="w-full opacity-50 cursor-not-allowed"
          disabled
          title="Facebook login not configured. Add NEXT_PUBLIC_FACEBOOK_APP_ID to .env.local"
        >
          <div className="flex items-center justify-center gap-2">
            <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 24 24">
              <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z" />
            </svg>
            Continue with Facebook (Not Configured)
          </div>
        </Button3D>
      )} */}
    </div>
  );
}

