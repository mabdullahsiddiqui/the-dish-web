import { ReputationLevel } from '@/types/user';

export function getReputationLevelColor(level: ReputationLevel): string {
  switch (level) {
    case 'Bronze':
      return 'text-amber-600';
    case 'Silver':
      return 'text-gray-400';
    case 'Gold':
      return 'text-yellow-400';
    case 'Platinum':
      return 'text-indigo-400';
    case 'Diamond':
      return 'text-cyan-400';
    default:
      return 'text-gray-400';
  }
}

export function getReputationLevelBgColor(level: ReputationLevel): string {
  switch (level) {
    case 'Bronze':
      return 'bg-amber-500/20 border-amber-500/30';
    case 'Silver':
      return 'bg-gray-500/20 border-gray-500/30';
    case 'Gold':
      return 'bg-yellow-500/20 border-yellow-500/30';
    case 'Platinum':
      return 'bg-indigo-500/20 border-indigo-500/30';
    case 'Diamond':
      return 'bg-cyan-500/20 border-cyan-500/30';
    default:
      return 'bg-gray-500/20 border-gray-500/30';
  }
}

export function getReputationLevelDescription(level: ReputationLevel): string {
  switch (level) {
    case 'Bronze':
      return 'New reviewer (0-49 points)';
    case 'Silver':
      return 'Active reviewer (50-149 points)';
    case 'Gold':
      return 'Trusted reviewer (150-299 points)';
    case 'Platinum':
      return 'Expert reviewer (300-499 points)';
    case 'Diamond':
      return 'Top contributor (500+ points)';
    default:
      return 'New reviewer';
  }
}


