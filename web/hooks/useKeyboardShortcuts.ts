import { useEffect } from 'react';
import { useRouter } from 'next/navigation';

export function useKeyboardShortcuts() {
  const router = useRouter();

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      // Only trigger shortcuts if not typing in an input/textarea
      const target = event.target as HTMLElement;
      if (
        target.tagName === 'INPUT' ||
        target.tagName === 'TEXTAREA' ||
        target.contentEditable === 'true'
      ) {
        return;
      }

      // Handle keyboard shortcuts
      if (event.ctrlKey || event.metaKey) {
        switch (event.key) {
          case 'k':
          case 'K':
            // Focus search input (Ctrl+K)
            event.preventDefault();
            const searchInput = document.querySelector('input[placeholder*="Search"]') as HTMLInputElement;
            if (searchInput) {
              searchInput.focus();
              searchInput.select();
            }
            break;
          case '/':
            // Focus search input (Ctrl+/)
            event.preventDefault();
            const searchInput2 = document.querySelector('input[placeholder*="Search"]') as HTMLInputElement;
            if (searchInput2) {
              searchInput2.focus();
              searchInput2.select();
            }
            break;
        }
      }

      // Handle single key shortcuts
      switch (event.key) {
        case 'Escape':
          // Close modals, menus, etc.
          const closeButtons = document.querySelectorAll('[data-keyboard-close]');
          closeButtons.forEach(button => (button as HTMLElement).click());
          break;
        case '?':
          // Show keyboard shortcuts help (future implementation)
          if (!event.shiftKey) return;
          event.preventDefault();
          console.log('Keyboard shortcuts help');
          break;
      }
    };

    // Add event listener
    document.addEventListener('keydown', handleKeyDown);

    // Cleanup
    return () => {
      document.removeEventListener('keydown', handleKeyDown);
    };
  }, [router]);
}

// Keyboard shortcuts information
export const KEYBOARD_SHORTCUTS = [
  { key: 'Ctrl+K', description: 'Focus search' },
  { key: 'Ctrl+/', description: 'Focus search' },
  { key: 'Escape', description: 'Close dialogs' },
  { key: 'Shift+?', description: 'Show shortcuts' },
] as const;

