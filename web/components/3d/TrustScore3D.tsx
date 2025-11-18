'use client';

import { useEffect, useRef } from 'react';
import * as THREE from 'three';

interface TrustScore3DProps {
  score: number;
  size?: number;
  className?: string;
}

export function TrustScore3D({
  score,
  size = 200,
  className = '',
}: TrustScore3DProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const sceneRef = useRef<{
    scene: THREE.Scene;
    camera: THREE.PerspectiveCamera;
    renderer: THREE.WebGLRenderer;
    ring: THREE.Mesh;
    animationId: number;
  } | null>(null);

  useEffect(() => {
    if (!containerRef.current) return;

    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(75, size / size, 0.1, 1000);
    const renderer = new THREE.WebGLRenderer({
      antialias: true,
      alpha: true,
      powerPreference: 'high-performance',
    });

    renderer.setSize(size, size);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    containerRef.current.appendChild(renderer.domElement);

    // Create ring geometry
    const geometry = new THREE.TorusGeometry(1, 0.1, 16, 100);
    const material = new THREE.MeshStandardMaterial({
      color:
        score >= 80
          ? 0x10b981
          : score >= 60
          ? 0xf59e0b
          : 0xef4444,
      metalness: 0.7,
      roughness: 0.3,
    });
    const ring = new THREE.Mesh(geometry, material);
    scene.add(ring);

    // Add lighting
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
    scene.add(ambientLight);

    const pointLight = new THREE.PointLight(0xffffff, 1, 100);
    pointLight.position.set(0, 0, 5);
    scene.add(pointLight);

    camera.position.z = 3;

    // Animation
    let lastTime = 0;
    const targetFPS = 60;
    const frameInterval = 1000 / targetFPS;

    const animate = (currentTime: number) => {
      const animationId = requestAnimationFrame(animate);

      const deltaTime = currentTime - lastTime;

      if (deltaTime >= frameInterval) {
        ring.rotation.x += 0.01;
        ring.rotation.y += 0.01;

        renderer.render(scene, camera);
        lastTime = currentTime - (deltaTime % frameInterval);
      }

      if (sceneRef.current) {
        sceneRef.current.animationId = animationId;
      }
    };

    animate(0);

    // Store references
    sceneRef.current = {
      scene,
      camera,
      renderer,
      ring,
      animationId: 0,
    };

    // Cleanup
    return () => {
      if (sceneRef.current) {
        cancelAnimationFrame(sceneRef.current.animationId);
        geometry.dispose();
        material.dispose();
        renderer.dispose();
        if (containerRef.current && renderer.domElement.parentNode) {
          containerRef.current.removeChild(renderer.domElement);
        }
      }
    };
  }, [score, size]);

  return (
    <div className={`relative ${className}`}>
      <div
        ref={containerRef}
        className="w-full h-full"
        style={{ width: size, height: size }}
        aria-hidden="true"
      />
      <div className="absolute inset-0 flex items-center justify-center pointer-events-none">
        <div className="text-center">
          <div className="text-4xl font-bold text-white">{score}</div>
          <div className="text-sm text-gray-300">Trust Score</div>
        </div>
      </div>
      <div className="sr-only">
        Trust score visualization: {score} out of 100
      </div>
    </div>
  );
}

