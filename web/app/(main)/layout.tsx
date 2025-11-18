'use client'

import { Header } from '@/components/layout/header';
import { Footer } from '@/components/layout/footer';
import { SkipToMain } from '@/components/ui/accessibility-improvements';
import { BackendHealthStatus } from '@/components/dev/backend-health';
import { useKeyboardShortcuts } from '@/hooks/useKeyboardShortcuts';

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  // Enable keyboard shortcuts
  useKeyboardShortcuts();

  return (
    <div className="min-h-screen flex flex-col">
      <SkipToMain />
      <Header />
      <main id="main-content" className="flex-1" tabIndex={-1}>
        {children}
      </main>
      <Footer />
      <BackendHealthStatus />
    </div>
  );
}
