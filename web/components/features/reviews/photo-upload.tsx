'use client'

import { useState, useRef } from 'react';
import { Camera, X, Upload, AlertCircle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import toast from 'react-hot-toast';

interface PhotoUploadProps {
  onPhotosChange: (photos: File[]) => void;
  maxPhotos?: number;
  maxSize?: number; // in MB
}

interface PhotoPreview {
  file: File;
  url: string;
  id: string;
}

export function PhotoUpload({ 
  onPhotosChange, 
  maxPhotos = 5, 
  maxSize = 10 
}: PhotoUploadProps) {
  const [photos, setPhotos] = useState<PhotoPreview[]>([]);
  const [isDragOver, setIsDragOver] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const acceptedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/webp'];

  const validateFile = (file: File): string | null => {
    if (!acceptedTypes.includes(file.type)) {
      return 'Only JPEG, PNG, and WebP images are allowed';
    }
    
    if (file.size > maxSize * 1024 * 1024) {
      return `File size must be less than ${maxSize}MB`;
    }
    
    return null;
  };

  const processFiles = (files: FileList | File[]) => {
    const fileArray = Array.from(files);
    const validFiles: File[] = [];
    
    // Check if adding these files would exceed the limit
    if (photos.length + fileArray.length > maxPhotos) {
      toast.error(`You can only upload up to ${maxPhotos} photos`);
      return;
    }

    fileArray.forEach((file) => {
      const error = validateFile(file);
      if (error) {
        toast.error(`${file.name}: ${error}`);
      } else {
        validFiles.push(file);
      }
    });

    if (validFiles.length > 0) {
      const newPreviews: PhotoPreview[] = validFiles.map((file) => ({
        file,
        url: URL.createObjectURL(file),
        id: Math.random().toString(36).substring(2),
      }));

      const updatedPhotos = [...photos, ...newPreviews];
      setPhotos(updatedPhotos);
      onPhotosChange(updatedPhotos.map(p => p.file));
    }
  };

  const removePhoto = (id: string) => {
    const photoToRemove = photos.find(p => p.id === id);
    if (photoToRemove) {
      URL.revokeObjectURL(photoToRemove.url);
    }
    
    const updatedPhotos = photos.filter(p => p.id !== id);
    setPhotos(updatedPhotos);
    onPhotosChange(updatedPhotos.map(p => p.file));
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files) {
      processFiles(files);
    }
    // Reset the input so the same file can be selected again if needed
    e.target.value = '';
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
    
    const files = e.dataTransfer.files;
    if (files) {
      processFiles(files);
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    setIsDragOver(false);
  };

  return (
    <div className="space-y-4">
      {/* Upload Area */}
      <div
        className={`border-2 border-dashed rounded-lg p-6 text-center transition-colors ${
          isDragOver
            ? 'border-blue-400 bg-blue-50'
            : photos.length >= maxPhotos
            ? 'border-gray-200 bg-gray-50'
            : 'border-gray-300 hover:border-gray-400'
        }`}
        onDrop={handleDrop}
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
      >
        <input
          ref={fileInputRef}
          type="file"
          accept={acceptedTypes.join(',')}
          multiple
          onChange={handleFileSelect}
          className="hidden"
          disabled={photos.length >= maxPhotos}
        />
        
        {photos.length >= maxPhotos ? (
          <div className="text-gray-500">
            <AlertCircle className="w-8 h-8 mx-auto mb-2" />
            <p className="text-sm">Maximum {maxPhotos} photos allowed</p>
          </div>
        ) : (
          <div className="text-gray-600">
            <Camera className="w-8 h-8 mx-auto mb-2 text-gray-400" />
            <p className="text-sm font-medium mb-1">
              Drag and drop photos here, or click to select
            </p>
            <p className="text-xs text-gray-500">
              JPEG, PNG, WebP up to {maxSize}MB each • Max {maxPhotos} photos
            </p>
            <Button
              type="button"
              variant="outline"
              size="sm"
              className="mt-3"
              onClick={() => fileInputRef.current?.click()}
            >
              <Upload className="w-4 h-4 mr-2" />
              Choose Photos
            </Button>
          </div>
        )}
      </div>

      {/* Photo Previews */}
      {photos.length > 0 && (
        <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
          {photos.map((photo) => (
            <Card key={photo.id} className="relative group">
              <CardContent className="p-2">
                <div className="aspect-square relative overflow-hidden rounded">
                  <img
                    src={photo.url}
                    alt="Review photo preview"
                    className="w-full h-full object-cover"
                  />
                  <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-20 transition-all flex items-center justify-center">
                    <Button
                      type="button"
                      variant="destructive"
                      size="sm"
                      className="opacity-0 group-hover:opacity-100 transition-opacity"
                      onClick={() => removePhoto(photo.id)}
                    >
                      <X className="w-4 h-4" />
                    </Button>
                  </div>
                </div>
                <p className="text-xs text-gray-500 mt-1 truncate">
                  {photo.file.name}
                </p>
                <p className="text-xs text-gray-400">
                  {(photo.file.size / 1024 / 1024).toFixed(1)}MB
                </p>
              </CardContent>
            </Card>
          ))}
        </div>
      )}
      
      {/* Upload Guidelines */}
      {photos.length === 0 && (
        <div className="text-xs text-gray-500 space-y-1">
          <p>• Photos help other diners see what to expect</p>
          <p>• Take photos of food, ambiance, or menu items</p>
          <p>• Please respect others' privacy and restaurant policies</p>
        </div>
      )}
    </div>
  );
}

