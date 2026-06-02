import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import Logo from '../assets/Logo.jpg';
import Avatar from '../assets/sweviceroy.jpg';
import './Navbar.css';

export default function Navbar() {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);

  const handleLogout = () => {
    logout();
    setMenuOpen(false);
    navigate('/');
  };

  return (
    <nav className="navbar">
      <div className="navbar-inner">
        <Link to="/" className="navbar-brand">
          <img src={Logo} alt="AuctionHouse" className="nav-logo" />
        </Link>

        <button className="nav-hamburger" onClick={() => setMenuOpen(!menuOpen)}>
          {menuOpen ? '\u2715' : '\u2630'}
        </button>

        <div className={`navbar-links ${menuOpen ? 'open' : ''}`}>
          {isAuthenticated ? (
            <>
              <Link to="/auctions/create" className="nav-link" onClick={() => setMenuOpen(false)}>Create Auction</Link>
              <Link to="/change-password" className="nav-link" onClick={() => setMenuOpen(false)}>Password</Link>
              {user?.isAdmin && (
                <Link to="/admin" className="nav-link" onClick={() => setMenuOpen(false)}>Admin</Link>
              )}
              <span className="nav-user">{user?.username}</span>
              <button onClick={handleLogout} className="nav-btn">Logout</button>
            </>
          ) : (
            <>
              <Link to="/login" className="nav-link" onClick={() => setMenuOpen(false)}>Login</Link>
              <Link to="/register" className="nav-link" onClick={() => setMenuOpen(false)}>Register</Link>
            </>
          )}
        </div>

        <img src={Avatar} alt="Profile" className="nav-avatar" />
      </div>
    </nav>
  );
}
