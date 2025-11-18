import { useEffect } from 'react';

// Skip to main content link for screen readers
export function SkipToMain() {
  return (
    <a
      href="#main-content"
      className="sr-only focus:not-sr-only focus:absolute focus:top-0 focus:left-0 bg-blue-600 text-white px-4 py-2 z-50 rounded-br"
    >
      Skip to main content
    </a>
  );
}

// Announce page changes to screen readers
export function PageAnnouncement({ message }: { message: string }) {
  useEffect(() => {
    const announcement = document.createElement('div');
    announcement.setAttribute('aria-live', 'polite');
    announcement.setAttribute('aria-atomic', 'true');
    announcement.className = 'sr-only';
    announcement.textContent = message;
    
    document.body.appendChild(announcement);
    
    // Remove after announcement
    setTimeout(() => {
      document.body.removeChild(announcement);
    }, 1000);
  }, [message]);

  return null;
}

// Focus management utility
export function useFocusManagement() {
  const focusElement = (selector: string, delay = 0) => {
    setTimeout(() => {
      const element = document.querySelector(selector) as HTMLElement;
      if (element) {
        element.focus();
      }
    }, delay);
  };

  const focusFirstError = () => {
    const firstError = document.querySelector('[aria-invalid="true"]') as HTMLElement;
    if (firstError) {
      firstError.focus();
    }
  };

  return {
    focusElement,
    focusFirstError,
  };
}

