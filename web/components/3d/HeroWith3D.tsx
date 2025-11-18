'use client';

import { ParticleBackground } from './ParticleBackground';
import { FadeInUp } from '../animations/FadeInUp';
import { Button3D } from '../ui/Button3D';

interface HeroWith3DProps {
  title?: string;
  subtitle?: string;
  ctaText?: string;
  onCtaClick?: () => void;
  particleColor?: string;
}

export function HeroWith3D({
  title = 'Welcome to the Future',
  subtitle = 'Experience the next generation of web design',
  ctaText = 'Get Started',
  onCtaClick,
  particleColor = '#6366f1',
}: HeroWith3DProps) {
  return (
    <div className="relative w-full h-screen overflow-hidden">
      {/* 3D Background */}
      <ParticleBackground
        particleCount={5000}
        color={particleColor}
        speed={0.001}
      />

      {/* Content */}
      <div className="relative z-10 flex items-center justify-center h-full">
        <div className="text-center px-4">
          <FadeInUp>
            <h1 className="text-5xl md:text-7xl font-bold text-white mb-6">
              {title}
            </h1>
          </FadeInUp>
          <FadeInUp delay={0.2}>
            <p className="text-lg md:text-xl text-gray-300 mb-8 max-w-2xl mx-auto">
              {subtitle}
            </p>
          </FadeInUp>
          <FadeInUp delay={0.4}>
            <Button3D onClick={onCtaClick} glow>
              {ctaText}
            </Button3D>
          </FadeInUp>
        </div>
      </div>
    </div>
  );
}

