import { ChevronLeft, ChevronRight, MoreHorizontal } from 'lucide-react';
import { Button } from '@/components/ui/button';

interface PaginationProps {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  showEdges?: boolean;
}

export function Pagination({ 
  currentPage, 
  totalPages, 
  onPageChange,
  showEdges = true 
}: PaginationProps) {
  const generatePages = () => {
    const pages: (number | string)[] = [];
    
    if (totalPages <= 7) {
      // Show all pages if 7 or fewer
      for (let i = 1; i <= totalPages; i++) {
        pages.push(i);
      }
    } else {
      // Show first page
      if (showEdges) pages.push(1);
      
      // Show dots if there's a gap
      if (currentPage > 3 && showEdges) pages.push('...');
      
      // Show pages around current
      const start = Math.max(showEdges ? 2 : 1, currentPage - 1);
      const end = Math.min(showEdges ? totalPages - 1 : totalPages, currentPage + 1);
      
      for (let i = start; i <= end; i++) {
        pages.push(i);
      }
      
      // Show dots if there's a gap
      if (currentPage < totalPages - 2 && showEdges) pages.push('...');
      
      // Show last page
      if (showEdges) pages.push(totalPages);
    }
    
    return pages;
  };

  if (totalPages <= 1) return null;

  const pages = generatePages();

  return (
    <nav className="flex items-center justify-center space-x-1" aria-label="Pagination">
      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
        className="flex items-center space-x-1"
      >
        <ChevronLeft className="w-4 h-4" />
        <span className="sr-only sm:not-sr-only">Previous</span>
      </Button>
      
      {pages.map((page, index) => (
        typeof page === 'number' ? (
          <Button
            key={page}
            variant={currentPage === page ? 'default' : 'outline'}
            size="sm"
            onClick={() => onPageChange(page)}
            className="min-w-[2.5rem]"
          >
            {page}
          </Button>
        ) : (
          <div key={index} className="flex items-center px-2">
            <MoreHorizontal className="w-4 h-4 text-gray-400" />
          </div>
        )
      ))}
      
      <Button
        variant="outline"
        size="sm"
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages}
        className="flex items-center space-x-1"
      >
        <span className="sr-only sm:not-sr-only">Next</span>
        <ChevronRight className="w-4 h-4" />
      </Button>
    </nav>
  );
}

