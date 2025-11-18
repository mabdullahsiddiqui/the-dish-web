'use client';

import { useEffect, useRef } from 'react';
import * as THREE from 'three';

interface InteractiveMeshProps {
  geometry?: 'box' | 'sphere' | 'torus';
  color?: string;
  className?: string;
  onHover?: () => void;
  onClick?: () => void;
}

export function InteractiveMesh({
  geometry = 'box',
  color = '#6366f1',
  className = '',
  onHover,
  onClick,
}: InteractiveMeshProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const sceneRef = useRef<{
    scene: THREE.Scene;
    camera: THREE.PerspectiveCamera;
    renderer: THREE.WebGLRenderer;
    mesh: THREE.Mesh;
    animationId: number;
  } | null>(null);
  const isHoveredRef = useRef(false);

  useEffect(() => {
    if (!containerRef.current) return;

    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(75, 1, 0.1, 1000);
    const renderer = new THREE.WebGLRenderer({
      antialias: true,
      alpha: true,
      powerPreference: 'high-performance',
    });

    const size = 200;
    renderer.setSize(size, size);
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    containerRef.current.appendChild(renderer.domElement);

    // Create geometry based on prop
    let shapeGeometry: THREE.BufferGeometry;
    switch (geometry) {
      case 'sphere':
        shapeGeometry = new THREE.SphereGeometry(1, 32, 32);
        break;
      case 'torus':
        shapeGeometry = new THREE.TorusGeometry(1, 0.4, 16, 100);
        break;
      default:
        shapeGeometry = new THREE.BoxGeometry(1, 1, 1);
    }

    const material = new THREE.MeshStandardMaterial({
      color: color,
      metalness: 0.7,
      roughness: 0.3,
    });
    const mesh = new THREE.Mesh(shapeGeometry, material);
    scene.add(mesh);

    // Add lighting
    const ambientLight = new THREE.AmbientLight(0xffffff, 0.5);
    scene.add(ambientLight);

    const pointLight = new THREE.PointLight(0xffffff, 1, 100);
    pointLight.position.set(0, 0, 5);
    scene.add(pointLight);

    camera.position.z = 3;

    // Mouse interaction
    const handleMouseMove = (event: MouseEvent) => {
      if (!containerRef.current) return;
      const rect = containerRef.current.getBoundingClientRect();
      const x = ((event.clientX - rect.left) / rect.width) * 2 - 1;
      const y = -((event.clientY - rect.top) / rect.height) * 2 + 1;

      mesh.rotation.y = x * 0.5;
      mesh.rotation.x = y * 0.5;
    };

    const handleMouseEnter = () => {
      isHoveredRef.current = true;
      if (onHover) onHover();
    };

    const handleMouseLeave = () => {
      isHoveredRef.current = false;
    };

    const handleClick = () => {
      if (onClick) onClick();
    };

    containerRef.current.addEventListener('mousemove', handleMouseMove);
    containerRef.current.addEventListener('mouseenter', handleMouseEnter);
    containerRef.current.addEventListener('mouseleave', handleMouseLeave);
    containerRef.current.addEventListener('click', handleClick);

    // Animation
    let lastTime = 0;
    const targetFPS = 60;
    const frameInterval = 1000 / targetFPS;

    const animate = (currentTime: number) => {
      const animationId = requestAnimationFrame(animate);

      const deltaTime = currentTime - lastTime;

      if (deltaTime >= frameInterval) {
        if (!isHoveredRef.current) {
          mesh.rotation.x += 0.01;
          mesh.rotation.y += 0.01;
        }

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
      mesh,
      animationId: 0,
    };

    // Cleanup
    return () => {
      if (containerRef.current) {
        containerRef.current.removeEventListener('mousemove', handleMouseMove);
        containerRef.current.removeEventListener('mouseenter', handleMouseEnter);
        containerRef.current.removeEventListener('mouseleave', handleMouseLeave);
        containerRef.current.removeEventListener('click', handleClick);
      }
      if (sceneRef.current) {
        cancelAnimationFrame(sceneRef.current.animationId);
        shapeGeometry.dispose();
        material.dispose();
        renderer.dispose();
        if (containerRef.current && renderer.domElement.parentNode) {
          containerRef.current.removeChild(renderer.domElement);
        }
      }
    };
  }, [geometry, color, onHover, onClick]);

  return (
    <div
      ref={containerRef}
      className={`cursor-pointer ${className}`}
      style={{ width: 200, height: 200 }}
      role="button"
      tabIndex={0}
      aria-label="Interactive 3D mesh"
    />
  );
}

