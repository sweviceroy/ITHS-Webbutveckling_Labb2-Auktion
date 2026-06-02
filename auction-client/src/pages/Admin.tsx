import { useState, useEffect } from 'react';
import api from '../services/api';
import type { User, Auction } from '../types';
import './Admin.css';

export default function Admin() {
  const [users, setUsers] = useState<User[]>([]);
  const [auctions, setAuctions] = useState<Auction[]>([]);
  const [tab, setTab] = useState<'users' | 'auctions'>('users');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      const [userRes, auctionRes] = await Promise.all([
        api.get('/admin/users'),
        api.get('/admin/auctions'),
      ]);
      setUsers(userRes.data);
      setAuctions(auctionRes.data);
      setLoading(false);
    };
    load();
  }, []);

  const toggleUser = async (id: string) => {
    const res = await api.put(`/admin/users/${id}/deactivate`);
    const activated = res.data.message === 'User activated';
    setUsers((prev) =>
      prev.map((u) => (u.id === id ? { ...u, isActive: activated } : u))
    );
  };

  const toggleAuction = async (id: string) => {
    const res = await api.put(`/admin/auctions/${id}/deactivate`);
    const activated = res.data.message === 'Auction activated';
    setAuctions((prev) =>
      prev.map((a) => (a.id === id ? { ...a, isActive: activated } : a))
    );
  };

  if (loading) return <p className="loading">Loading admin panel...</p>;

  return (
    <div className="admin-page">
      <h1>Admin Panel</h1>

      <div className="admin-tabs">
        <button
          className={tab === 'users' ? 'active' : ''}
          onClick={() => setTab('users')}
        >
          Users ({users.length})
        </button>
        <button
          className={tab === 'auctions' ? 'active' : ''}
          onClick={() => setTab('auctions')}
        >
          Auctions ({auctions.length})
        </button>
      </div>

      {tab === 'users' && (
        <table className="admin-table">
          <thead>
            <tr>
              <th>Username</th>
              <th>Email</th>
              <th>Admin</th>
              <th>Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id} className={!u.isActive ? 'inactive' : ''}>
                <td>{u.username}</td>
                <td>{u.email}</td>
                <td>{u.isAdmin ? 'Yes' : 'No'}</td>
                <td>{u.isActive ? 'Active' : 'Deactivated'}</td>
                <td>
                  {!u.isAdmin && (
                    <button
                      onClick={() => toggleUser(u.id)}
                      className={u.isActive ? 'deactivate-btn' : 'activate-btn'}
                    >
                      {u.isActive ? 'Deactivate' : 'Activate'}
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {tab === 'auctions' && (
        <table className="admin-table">
          <thead>
            <tr>
              <th>Title</th>
              <th>Seller</th>
              <th>Price</th>
              <th>Status</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            {auctions.map((a) => (
              <tr key={a.id} className={!a.isActive ? 'inactive' : ''}>
                <td>{a.title}</td>
                <td>{a.creatorUsername}</td>
                <td>{a.currentHighestBid ?? a.startingPrice} SEK</td>
                <td>{a.isActive ? (a.isOpen ? 'Open' : 'Ended') : 'Hidden'}</td>
                <td>
                  <button
                    onClick={() => toggleAuction(a.id)}
                    className={a.isActive ? 'deactivate-btn' : 'activate-btn'}
                  >
                    {a.isActive ? 'Hide' : 'Show'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}
