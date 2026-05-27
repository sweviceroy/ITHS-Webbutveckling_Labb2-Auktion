import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import type { AuctionDetail } from '../types';
import './AuctionDetail.css';

export default function AuctionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [auction, setAuction] = useState<AuctionDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [bidAmount, setBidAmount] = useState('');
  const [bidError, setBidError] = useState('');
  const [bidSuccess, setBidSuccess] = useState('');

  useEffect(() => {
    const fetchAuction = async () => {
      setLoading(true);
      const { data } = await api.get<AuctionDetail>(`/auctions/${id}`);
      setAuction(data);
      setLoading(false);
    };
    fetchAuction();
  }, [id]);

  const handleBid = async (e: React.FormEvent) => {
    e.preventDefault();
    setBidError('');
    setBidSuccess('');
    const amount = parseFloat(bidAmount);
    try {
      const { data } = await api.post(`/auctions/${id}/bids`, { amount });
      setBidSuccess(`Bid of ${data.amount} SEK placed!`);
      setBidAmount('');
      const { data: refreshed } = await api.get<AuctionDetail>(`/auctions/${id}`);
      setAuction(refreshed);
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string } } };
      setBidError(error.response?.data?.message || 'Bid failed');
    }
  };

  if (loading) return <p className="loading">Loading auction...</p>;
  if (!auction) return <p className="empty">Auction not found</p>;

  const isOwner = user?.id === auction.creatorId;
  const canBid = auction.isOpen && isAuthenticated && !isOwner;

  return (
    <div className="auction-detail">
      <button onClick={() => navigate(-1)} className="back-btn">&larr; Back</button>

      {auction.imageUrl && (
        <img src={auction.imageUrl} alt={auction.title} className="auction-detail-image" />
      )}

      <div className="auction-detail-header">
        <h1>{auction.title}</h1>
        <span className={`status-badge ${auction.isOpen ? 'open' : 'closed'}`}>
          {auction.isOpen ? 'Open' : 'Closed'}
        </span>
      </div>

      <p className="auction-description">{auction.description || 'No description'}</p>

      <div className="auction-info-grid">
        <div>
          <label>Current Price</label>
          <p className="price">
            {auction.currentHighestBid ?? auction.startingPrice} SEK
          </p>
        </div>
        <div>
          <label>Starting Price</label>
          <p>{auction.startingPrice} SEK</p>
        </div>
        <div>
          <label>Seller</label>
          <p>{auction.creatorUsername}</p>
        </div>
        <div>
          <label>Ends</label>
          <p>{new Date(auction.endDate).toLocaleString()}</p>
        </div>
      </div>

      {canBid && (
        <form onSubmit={handleBid} className="bid-form">
          <input
            type="number"
            step="0.01"
            min={(auction.currentHighestBid ?? auction.startingPrice) + 1}
            value={bidAmount}
            onChange={(e) => setBidAmount(e.target.value)}
            placeholder="Enter bid amount"
            required
          />
          <button type="submit">Place Bid</button>
          {bidError && <p className="bid-error">{bidError}</p>}
          {bidSuccess && <p className="bid-success">{bidSuccess}</p>}
        </form>
      )}

      {!isAuthenticated && auction.isOpen && (
        <p className="login-prompt">Log in to place a bid.</p>
      )}

      {isOwner && auction.isOpen && (
        <p className="owner-notice">This is your auction. You cannot bid on it.</p>
      )}

      {auction.isOpen ? (
        <div className="bid-history">
          <h2>Bid History ({auction.bids.length})</h2>
          {auction.bids.length === 0 ? (
            <p>No bids yet.</p>
          ) : (
            <ul>
              {auction.bids.map((bid) => (
                <li key={bid.id} className="bid-item">
                  <span className="bid-amount">{bid.amount} SEK</span>
                  <span className="bid-user">{bid.bidderUsername}</span>
                  <span className="bid-time">
                    {new Date(bid.bidTime).toLocaleString()}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>
      ) : (
        <div className="closed-result">
          <h2>Auction Closed</h2>
          {auction.currentHighestBid ? (
            <p>
              <strong>Winner:</strong> {auction.bids[0]?.bidderUsername} with{' '}
              <span className="price">{auction.currentHighestBid} SEK</span>
            </p>
          ) : (
            <p>No bids were placed on this auction.</p>
          )}
        </div>
      )}
    </div>
  );
}
