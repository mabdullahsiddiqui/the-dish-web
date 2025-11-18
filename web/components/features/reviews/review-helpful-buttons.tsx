'use client'

import { useState } from 'react';
import { ThumbsUp, ThumbsDown } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Review } from '@/types/review';
import { useReviews } from '@/hooks/useReviews';
import { useAuth } from '@/hooks/useAuth';

interface ReviewHelpfulButtonsProps {
  review: Review;
}

export function ReviewHelpfulButtons({ review }: ReviewHelpfulButtonsProps) {
  const { isAuthenticated } = useAuth();
  const { useMarkReviewHelpful } = useReviews();
  const markHelpfulMutation = useMarkReviewHelpful();

  // Local state for optimistic updates
  const [optimisticVote, setOptimisticVote] = useState<boolean | undefined>(review.userHelpfulVote);
  const [optimisticCounts, setOptimisticCounts] = useState({
    helpful: review.helpfulCount,
    notHelpful: review.notHelpfulCount,
  });

  const handleVote = (isHelpful: boolean) => {
    if (!isAuthenticated) return;

    // Optimistic update
    const previousVote = optimisticVote;
    setOptimisticVote(isHelpful);
    
    // Update counts optimistically
    const newCounts = { ...optimisticCounts };
    
    if (previousVote === undefined) {
      // New vote
      if (isHelpful) {
        newCounts.helpful += 1;
      } else {
        newCounts.notHelpful += 1;
      }
    } else if (previousVote !== isHelpful) {
      // Changing vote
      if (previousVote) {
        newCounts.helpful -= 1;
        newCounts.notHelpful += 1;
      } else {
        newCounts.notHelpful -= 1;
        newCounts.helpful += 1;
      }
    } else {
      // Same vote - remove it
      if (isHelpful) {
        newCounts.helpful -= 1;
      } else {
        newCounts.notHelpful -= 1;
      }
      setOptimisticVote(undefined);
    }
    
    setOptimisticCounts(newCounts);

    // Make API call
    markHelpfulMutation.mutate(
      { id: review.id, isHelpful },
      {
        onError: () => {
          // Revert optimistic update on error
          setOptimisticVote(previousVote);
          setOptimisticCounts({
            helpful: review.helpfulCount,
            notHelpful: review.notHelpfulCount,
          });
        },
      }
    );
  };

  if (!isAuthenticated) {
    return (
      <div className="flex items-center justify-between text-sm text-gray-500">
        <div>
          {optimisticCounts.helpful} people found this helpful
        </div>
        <div className="text-xs text-gray-400">
          Sign in to vote
        </div>
      </div>
    );
  }

  return (
    <div className="flex items-center justify-between text-sm">
      <div className="text-gray-500">
        {optimisticCounts.helpful} people found this helpful
        {optimisticCounts.notHelpful > 0 && (
          <span className="ml-2">
            • {optimisticCounts.notHelpful} found it not helpful
          </span>
        )}
      </div>
      
      <div className="flex items-center space-x-2">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => handleVote(true)}
          disabled={markHelpfulMutation.isPending}
          className={`flex items-center space-x-1 ${
            optimisticVote === true 
              ? 'text-green-600 bg-green-50' 
              : 'text-gray-600 hover:text-green-600'
          }`}
        >
          <ThumbsUp className="w-4 h-4" />
          <span>Helpful</span>
          {optimisticVote === true && (
            <span className="text-xs">✓</span>
          )}
        </Button>
        
        <Button
          variant="ghost"
          size="sm"
          onClick={() => handleVote(false)}
          disabled={markHelpfulMutation.isPending}
          className={`flex items-center space-x-1 ${
            optimisticVote === false 
              ? 'text-red-600 bg-red-50' 
              : 'text-gray-600 hover:text-red-600'
          }`}
        >
          <ThumbsDown className="w-4 h-4" />
          <span>Not Helpful</span>
          {optimisticVote === false && (
            <span className="text-xs">✓</span>
          )}
        </Button>
      </div>
    </div>
  );
}

