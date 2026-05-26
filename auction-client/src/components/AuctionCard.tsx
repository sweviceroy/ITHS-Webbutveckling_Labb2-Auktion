import { Link } from 'react-router-dom';
import type { Auction } from '../types';
import './AuctionCard.css';

interface Props {
  auction: Auction;
}

export default function AuctionCard({ auction }: Props) {
  const hoursLeft = Math.max(0, Math.floor(
    (new Date(auction.endDate).getTime() - Date.now()) / (1000 * 60 * 60)
  ));

  return (
    <Link to={`/auctions/${auction.id}`} className="auction-card">
      <div className="auction-card-image">
        {auction.imageUrl ? (
          <img src={auction.imageUrl} alt={auction.title} />
        ) : (
          <div className="auction-card-placeholder">No Image</div>
        )}
      </div>
      <div className="auction-card-body">
        <h3>{auction.title}</h3>
        <p className="auction-card-price">
          {auction.currentHighestBid
            ? `${auction.currentHighestBid} SEK`
            : `Starts at ${auction.startingPrice} SEK`}
        </p>
        <p className="auction-card-meta">
          {auction.bidCount} bid{auction.bidCount !== 1 ? 's' : ''} &middot; {hoursLeft}h left
        </p>
        <p className="auction-card-seller">by {auction.creatorUsername}</p>
      </div>
    </Link>
  );
}
