import { cn } from "@/lib/utils/cn"

function Skeleton({
  className,
  ...props
}: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      className={cn("animate-pulse rounded-md bg-gray-200", className)}
      {...props}
    />
  )
}

interface PlaceCardSkeletonProps {
  count?: number;
}

function PlaceCardSkeleton({ count = 1 }: PlaceCardSkeletonProps) {
  return (
    <>
      {[...Array(count)].map((_, i) => (
        <div key={i} className="border rounded-lg overflow-hidden">
          <Skeleton className="h-48 w-full" />
          <div className="p-4 space-y-3">
            <Skeleton className="h-4 w-3/4" />
            <Skeleton className="h-4 w-1/2" />
            <div className="flex items-center space-x-2">
              <Skeleton className="h-4 w-16" />
              <Skeleton className="h-4 w-12" />
            </div>
            <div className="flex space-x-1">
              <Skeleton className="h-6 w-12" />
              <Skeleton className="h-6 w-16" />
              <Skeleton className="h-6 w-14" />
            </div>
          </div>
        </div>
      ))}
    </>
  );
}

interface ReviewSkeletonProps {
  count?: number;
}

function ReviewSkeleton({ count = 1 }: ReviewSkeletonProps) {
  return (
    <>
      {[...Array(count)].map((_, i) => (
        <div key={i} className="border-b pb-6 last:border-b-0">
          <div className="flex items-center space-x-3 mb-3">
            <Skeleton className="h-8 w-8 rounded-full" />
            <div className="space-y-1">
              <Skeleton className="h-4 w-24" />
              <Skeleton className="h-3 w-16" />
            </div>
          </div>
          <div className="space-y-2 mb-3">
            <Skeleton className="h-4 w-full" />
            <Skeleton className="h-4 w-3/4" />
            <Skeleton className="h-4 w-1/2" />
          </div>
          <div className="flex justify-between">
            <Skeleton className="h-4 w-32" />
            <Skeleton className="h-4 w-20" />
          </div>
        </div>
      ))}
    </>
  );
}

export { Skeleton, PlaceCardSkeleton, ReviewSkeleton }

