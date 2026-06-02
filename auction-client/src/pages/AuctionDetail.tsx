import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import type { AuctionDetail } from '../types';
import './AuctionDetail.css';

const pad = (n: number) => n.toString().padStart(2, '0');
const toLocalDatetime = (d: Date) =>
  `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;

export default function AuctionDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { user, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [auction, setAuction] = useState<AuctionDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [bidAmount, setBidAmount] = useState('');
  const [bidError, setBidError] = useState('');
  const [bidSuccess, setBidSuccess] = useState('');

  const [editing, setEditing] = useState(false);
  const [editTitle, setEditTitle] = useState('');
  const [editDescription, setEditDescription] = useState('');
  const [editImageUrl, setEditImageUrl] = useState('');
  const [editEndDate, setEditEndDate] = useState('');
  const [editStartingPrice, setEditStartingPrice] = useState('');
  const [editError, setEditError] = useState('');

  const [undoing, setUndoing] = useState(false);

  const fetchAuction = async () => {
    const { data } = await api.get<AuctionDetail>(`/auctions/${id}`);
    setAuction(data);
  };

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      await fetchAuction();
      setLoading(false);
    };
    load();
  }, [id]);

  const startEdit = () => {
    if (!auction) return;
    setEditTitle(auction.title);
    setEditDescription(auction.description);
    setEditImageUrl(auction.imageUrl || '');
    setEditEndDate(toLocalDatetime(new Date(auction.endDate)));
    setEditStartingPrice(auction.startingPrice.toString());
    setEditError('');
    setEditing(true);
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    setEditError('');
    const hasBids = (auction?.bids.length ?? 0) > 0;
    try {
      await api.put(`/auctions/${id}`, {
        title: editTitle,
        description: editDescription,
        imageUrl: editImageUrl || null,
        endDate: new Date(editEndDate).toISOString(),
        startingPrice: hasBids ? null : parseFloat(editStartingPrice),
      });
      setEditing(false);
      await fetchAuction();
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string } } };
      setEditError(error.response?.data?.message || 'Update failed');
    }
  };

  const handleBid = async (e: React.FormEvent) => {
    e.preventDefault();
    setBidError('');
    setBidSuccess('');
    const amount = parseFloat(bidAmount);
    try {
      const { data } = await api.post(`/auctions/${id}/bids`, { amount });
      setBidSuccess(`Bid of ${data.amount} SEK placed!`);
      setBidAmount('');
      await fetchAuction();
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string } } };
      setBidError(error.response?.data?.message || 'Bid failed');
    }
  };

  const handleUndoBid = async () => {
    setUndoing(true);
    try {
      await api.delete(`/auctions/${id}/bids/latest`);
      await fetchAuction();
    } catch {
      /* ignore */
    }
    setUndoing(false);
  };

  if (loading) return <p className="loading">Loading auction...</p>;
  if (!auction) return <p className="empty">Auction not found</p>;

  const isOwner = user?.id === auction.creatorId;
  const canBid = auction.isOpen && isAuthenticated && !isOwner;
  const latestBid = auction.bids[0];
  const canUndo = auction.isOpen && latestBid && latestBid.bidderId === user?.id;
  const hasBids = auction.bids.length > 0;

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
        {isOwner && auction.isOpen && !editing && (
          <button onClick={startEdit} className="edit-btn">Edit</button>
        )}
      </div>

      {editing ? (
        <form onSubmit={handleUpdate} className="edit-form">
          {editError && <p className="bid-error">{editError}</p>}
          <label>Title</label>
          <input value={editTitle} onChange={(e) => setEditTitle(e.target.value)} required maxLength={100} />

          <label>Description</label>
          <textarea value={editDescription} onChange={(e) => setEditDescription(e.target.value)} maxLength={500} rows={3} />

          <label>Image URL</label>
          <input value={editImageUrl} onChange={(e) => setEditImageUrl(e.target.value)} />

          {hasBids ? (
            <p className="price-locked">Starting price cannot be changed — bids exist ({editStartingPrice} SEK)</p>
          ) : (
            <>
              <label>Starting Price (SEK)</label>
              <input type="number" step="0.01" value={editStartingPrice} onChange={(e) => setEditStartingPrice(e.target.value)} required />
            </>
          )}

          <label>End Date</label>
          <input type="datetime-local" value={editEndDate} onChange={(e) => setEditEndDate(e.target.value)} required />

          <div className="edit-actions">
            <button type="submit">Save Changes</button>
            <button type="button" onClick={() => setEditing(false)} className="cancel-btn">Cancel</button>
          </div>
        </form>
      ) : (
        <>
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
        </>
      )}

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
              {auction.bids.map((bid, i) => (
                <li key={bid.id} className="bid-item">
                  <span className="bid-amount">{bid.amount} SEK</span>
                  <span className="bid-user">{bid.bidderUsername}</span>
                  <span className="bid-time">
                    {new Date(bid.bidTime).toLocaleString()}
                  </span>
                  {i === 0 && canUndo && (
                    <button onClick={handleUndoBid} disabled={undoing} className="undo-btn">
                      {undoing ? '...' : 'Undo'}
                    </button>
                  )}
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
