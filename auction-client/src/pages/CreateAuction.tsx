import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import './CreateAuction.css';

export default function CreateAuction() {
  const navigate = useNavigate();
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError('');
    const formData = new FormData(e.currentTarget);

    const body = {
      title: formData.get('title') as string,
      description: formData.get('description') as string,
      imageUrl: (formData.get('imageUrl') as string) || undefined,
      startingPrice: parseFloat(formData.get('startingPrice') as string),
      startDate: new Date(formData.get('startDate') as string).toISOString(),
      endDate: new Date(formData.get('endDate') as string).toISOString(),
    };

    try {
      const { data } = await api.post('/auctions', body);
      navigate(`/auctions/${data.id}`);
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string } } };
      setError(error.response?.data?.message || 'Failed to create auction');
    }
  };

  const now = new Date();
  const pad = (n: number) => n.toString().padStart(2, '0');
  const formatLocal = (d: Date) =>
    `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
  const defaultStart = formatLocal(now);
  const defaultEnd = formatLocal(new Date(now.getTime() + 7 * 24 * 60 * 60 * 1000));

  return (
    <div className="create-auction-page">
      <form onSubmit={handleSubmit} className="create-auction-form">
        <h1>Create Auction</h1>
        {error && <p className="form-error">{error}</p>}

        <label>Title</label>
        <input name="title" required maxLength={100} />

        <label>Description</label>
        <textarea name="description" rows={4} maxLength={500} />

        <label>Image URL (optional)</label>
        <input name="imageUrl" type="url" placeholder="https://..." />

        <label>Starting Price (SEK)</label>
        <input name="startingPrice" type="number" step="0.01" min="1" required />

        <label>Start Date</label>
        <input name="startDate" type="datetime-local" defaultValue={defaultStart} required />

        <label>End Date</label>
        <input name="endDate" type="datetime-local" defaultValue={defaultEnd} required />

        <button type="submit">Create Auction</button>
      </form>
    </div>
  );
}
