'use client'

import { useState } from 'react';
import { Edit, Trash2, MoreHorizontal } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Review } from '@/types/review';
import { useReviews } from '@/hooks/useReviews';
import { useAuth } from '@/hooks/useAuth';
import { useRouter } from 'next/navigation';

interface ReviewActionsProps {
  review: Review;
  showLabel?: boolean;
}

export function ReviewActions({ review, showLabel = false }: ReviewActionsProps) {
  const { user } = useAuth();
  const { useDeleteReview } = useReviews();
  const deleteReviewMutation = useDeleteReview();
  const router = useRouter();
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

  // Only show actions if the review belongs to the current user
  if (!user || review.userId !== user.id) {
    return null;
  }

  const handleEdit = () => {
    router.push(`/reviews/${review.id}/edit`);
  };

  const handleDelete = () => {
    if (showDeleteConfirm) {
      // Confirm deletion
      deleteReviewMutation.mutate(review.id, {
        onSuccess: () => {
          setShowDeleteConfirm(false);
        }
      });
    } else {
      setShowDeleteConfirm(true);
    }
  };

  if (showDeleteConfirm) {
    return (
      <Card className="border-red-200 bg-red-50">
        <CardContent className="p-4">
          <div className="flex items-center justify-between">
            <div className="text-sm">
              <p className="font-medium text-red-800">Delete this review?</p>
              <p className="text-red-600">This action cannot be undone.</p>
            </div>
            <div className="flex space-x-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setShowDeleteConfirm(false)}
                disabled={deleteReviewMutation.isPending}
              >
                Cancel
              </Button>
              <Button
                variant="destructive"
                size="sm"
                onClick={handleDelete}
                disabled={deleteReviewMutation.isPending}
              >
                {deleteReviewMutation.isPending ? 'Deleting...' : 'Delete'}
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="flex items-center space-x-2">
      {showLabel && <span className="text-sm text-gray-500">Actions:</span>}
      <Button
        variant="ghost"
        size="sm"
        onClick={handleEdit}
        className="flex items-center space-x-1"
      >
        <Edit className="w-4 h-4" />
        <span>Edit</span>
      </Button>
      <Button
        variant="ghost"
        size="sm"
        onClick={handleDelete}
        className="flex items-center space-x-1 text-red-600 hover:text-red-800"
      >
        <Trash2 className="w-4 h-4" />
        <span>Delete</span>
      </Button>
    </div>
  );
}

interface ReviewActionsDropdownProps {
  review: Review;
}

export function ReviewActionsDropdown({ review }: ReviewActionsDropdownProps) {
  const [isOpen, setIsOpen] = useState(false);
  const { user } = useAuth();

  // Only show actions if the review belongs to the current user
  if (!user || review.userId !== user.id) {
    return null;
  }

  return (
    <div className="relative">
      <Button
        variant="ghost"
        size="sm"
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center space-x-1"
      >
        <MoreHorizontal className="w-4 h-4" />
      </Button>

      {isOpen && (
        <Card className="absolute right-0 top-8 z-10 w-32">
          <CardContent className="p-2">
            <div className="space-y-1">
              <ReviewActions review={review} />
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}

