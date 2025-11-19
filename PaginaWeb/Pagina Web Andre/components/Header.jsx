import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Home, CreditCard, ArrowLeftRight, User, LogOut, Menu, X } from 'lucide-react';
import { useState } from 'react';
import '../styles/Header.css';

const Header = () => {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const toggleMenu = () => {
    setMenuOpen(!menuOpen);
  };

  return (
    <header className="site-header">
      <div className="header-container">
        <Link to="/" className="logo-link">
          <img src="/src/assets/logo.png" alt="Banco Fisclase" className="logo-img" />
          <span className="logo-text">Fisclase</span>
        </Link>

        {isAuthenticated && (
          <>
            <button className="mobile-menu-btn" onClick={toggleMenu}>
              {menuOpen ? <X size={24} /> : <Menu size={24} />}
            </button>

            <nav className={`nav-menu ${menuOpen ? 'active' : ''}`}>
              <Link to="/dashboard" className="nav-link" onClick={() => setMenuOpen(false)}>
                <Home size={18} />
                <span>Inicio</span>
              </Link>
              <Link to="/accounts" className="nav-link" onClick={() => setMenuOpen(false)}>
                <CreditCard size={18} />
                <span>Cuentas</span>
              </Link>
              <Link to="/transactions" className="nav-link" onClick={() => setMenuOpen(false)}>
                <ArrowLeftRight size={18} />
                <span>Transacciones</span>
              </Link>
              <Link to="/profile" className="nav-link" onClick={() => setMenuOpen(false)}>
                <User size={18} />
                <span>Perfil</span>
              </Link>
            </nav>

            <div className="user-section">
              <span className="user-name">{user?.username}</span>
              <button onClick={handleLogout} className="logout-btn">
                <LogOut size={18} />
                <span>Salir</span>
              </button>
            </div>
          </>
        )}

        {!isAuthenticated && (
          <div className="auth-buttons">
            <Link to="/login" className="btn-login">Iniciar sesión</Link>
            <Link to="/register" className="btn-register">Registrarse</Link>
          </div>
        )}
      </div>
    </header>
  );
};

export default Header;
