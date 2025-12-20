import { useMutation } from '@tanstack/react-query';
import { photosApi } from '@/lib/api/photos';
import toast from 'react-hot-toast';

export function usePhotos() {
  const useUploadPhoto = () => {
    return useMutation({
      mutationFn: photosApi.upload,
      onError: (error: any) => {
        toast.error(error.message || 'Failed to upload photo');
      },
    });
  };

  return {
    useUploadPhoto,
  };
}
