import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import api from '../services/api';
import AuctionCard from '../components/AuctionCard';
import type { Auction } from '../types';
import './Home.css';

export default function Home() {
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchParams, setSearchParams] = useSearchParams();
  const query = searchParams.get('q') || '';
  const includeClosed = searchParams.get('closed') === 'true';

  useEffect(() => {
    const fetchAuctions = async () => {
      setLoading(true);
      const params: Record<string, string> = {};
      if (query) params.title = query;
      if (includeClosed) params.includeClosed = 'true';
      const { data } = await api.get<Auction[]>('/auctions', { params });
      setAuctions(data);
      setLoading(false);
    };
    fetchAuctions();
  }, [query, includeClosed]);

  const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const q = (formData.get('q') as string).trim();
    setSearchParams(q ? { q } : {});
  };

  return (
    <div className="home">
      <div className="home-hero">
        <h1>Discover Auctions</h1>
        <form onSubmit={handleSearch} className="search-form" role="search">
          <input
            name="q"
            type="search"
            defaultValue={query}
            placeholder="Search auctions by title..."
            className="search-input"
          />
          <button type="submit" className="search-btn">Search</button>
        </form>
        <label className="closed-toggle">
          <input
            type="checkbox"
            checked={includeClosed}
            onChange={(e) => setSearchParams((prev) => {
              const next = new URLSearchParams(prev);
              if (e.target.checked) next.set('closed', 'true');
              else next.delete('closed');
              return next;
            })}
          />
          Include closed auctions
        </label>
      </div>

      {loading ? (
        <p className="loading">Loading auctions...</p>
      ) : auctions.length === 0 ? (
        <p className="empty">No auctions found.</p>
      ) : (
        <div className="auction-grid">
          {auctions.map((auction) => (
            <AuctionCard key={auction.id} auction={auction} />
          ))}
        </div>
      )}
    </div>
  );
}
