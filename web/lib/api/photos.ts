import { api } from './client';

export const photosApi = {
  upload: async (file: File) => {
    const formData = new FormData();
    formData.append('file', file);

    const response = await api.post<{ url: string }>('/photos/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};
