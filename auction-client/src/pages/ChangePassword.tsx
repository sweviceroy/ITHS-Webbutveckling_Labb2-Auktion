import { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import api from '../services/api';
import './ChangePassword.css';

export default function ChangePassword() {
  const { logout } = useAuth();
  const [oldPassword, setOldPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
      await api.put('/auth/password', { oldPassword, newPassword });
      setSuccess('Password changed successfully');
      setOldPassword('');
      setNewPassword('');
      setTimeout(() => logout(), 2000);
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string } } };
      setError(error.response?.data?.message || 'Failed to change password');
    }
  };

  return (
    <div className="change-password-page">
      <form onSubmit={handleSubmit} className="change-password-form">
        <h1>Change Password</h1>
        {error && <p className="auth-error">{error}</p>}
        {success && <p className="auth-success">{success}</p>}
        <label>Current Password</label>
        <input
          type="password"
          value={oldPassword}
          onChange={(e) => setOldPassword(e.target.value)}
          required
        />
        <label>New Password</label>
        <input
          type="password"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
          required
          minLength={6}
          placeholder="At least 6 characters"
        />
        <button type="submit">Change Password</button>
      </form>
    </div>
  );
}
