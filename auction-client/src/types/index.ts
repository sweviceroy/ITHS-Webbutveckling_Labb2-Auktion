export interface User {
  id: string;
  username: string;
  email: string;
  isAdmin: boolean;
  createdAt: string;
}

export interface Auction {
  id: string;
  title: string;
  imageUrl?: string;
  startingPrice: number;
  currentHighestBid?: number;
  endDate: string;
  isOpen: boolean;
  bidCount: number;
  creatorUsername: string;
}

export interface AuctionDetail {
  id: string;
  title: string;
  description: string;
  imageUrl?: string;
  startingPrice: number;
  currentHighestBid?: number;
  startDate: string;
  endDate: string;
  isOpen: boolean;
  creatorId: string;
  creatorUsername: string;
  bids: Bid[];
}

export interface Bid {
  id: string;
  amount: number;
  bidTime: string;
  bidderUsername: string;
  bidderId: string;
}

export interface CreateAuctionRequest {
  title: string;
  description: string;
  imageUrl?: string;
  startingPrice: number;
  startDate: string;
  endDate: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}
