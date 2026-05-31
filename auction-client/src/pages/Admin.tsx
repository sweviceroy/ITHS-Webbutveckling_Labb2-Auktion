import { useState, useEffect } from 'react';
import api from '../services/api';
import type { User, Auction } from '../types';
import './Admin.css';

export default function Admin() {
  const [users, setUsers] = useState<(User & { isActive?: boolean })[]>([]);
  const [auctions, setAuctions] = useState<(Auction & { isActive?: boolean })[]>([]);
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
    await api.put(`/admin/users/${id}/deactivate`);
    setUsers((prev) =>
      prev.map((u) => (u.id === id ? { ...u, isActive: !u.isActive } : u))
    );
  };

  const toggleAuction = async (id: string) => {
    await api.put(`/admin/auctions/${id}/deactivate`);
    setAuctions((prev) =>
      prev.map((a) => (a.id === id ? { ...a, isActive: !a.isActive } : a))
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
              <tr key={u.id} className={u.isActive === false ? 'inactive' : ''}>
                <td>{u.username}</td>
                <td>{u.email}</td>
                <td>{u.isAdmin ? 'Yes' : 'No'}</td>
                <td>{u.isActive !== false ? 'Active' : 'Deactivated'}</td>
                <td>
                  {!u.isAdmin && (
                    <button
                      onClick={() => toggleUser(u.id)}
                      className={u.isActive !== false ? 'deactivate-btn' : 'activate-btn'}
                    >
                      {u.isActive !== false ? 'Deactivate' : 'Activate'}
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
              <tr key={a.id} className={a.isActive === false ? 'inactive' : ''}>
                <td>{a.title}</td>
                <td>{a.creatorUsername}</td>
                <td>{a.currentHighestBid ?? a.startingPrice} SEK</td>
                <td>{a.isActive !== false ? (a.isOpen ? 'Open' : 'Ended') : 'Hidden'}</td>
                <td>
                  <button
                    onClick={() => toggleAuction(a.id)}
                    className={a.isActive !== false ? 'deactivate-btn' : 'activate-btn'}
                  >
                    {a.isActive !== false ? 'Hide' : 'Show'}
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
