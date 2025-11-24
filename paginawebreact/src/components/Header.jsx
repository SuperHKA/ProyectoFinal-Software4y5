import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Home, CreditCard, ArrowLeftRight, User, LogOut, Menu, X, Wifi, WifiOff, Users } from 'lucide-react';
import { useState, useEffect } from 'react';
import apiService, { API_BASE_URL } from '../services/apiService';
import '../styles/Header.css';

const Header = () => {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const [menuOpen, setMenuOpen] = useState(false);
  const [apiConnected, setApiConnected] = useState(null);

  useEffect(() => {
    const checkApiConnection = async () => {
      const isConnected = await apiService.checkConnection();
      setApiConnected(isConnected);
    };

    checkApiConnection();
    const interval = setInterval(checkApiConnection, 30000); // Check every 30 seconds

    return () => clearInterval(interval);
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const toggleMenu = () => {
    setMenuOpen(!menuOpen);
  };

  const isAdmin = !!(user && (user.isAdmin || user.IsAdmin));

  return (
    <header className="site-header">
      <div className="header-container">
        <Link to="/" className="logo-link">
          <img src="/src/assets/logo.png" alt="Banco Fisclase" className="logo-img" />
          <span className="logo-text">Fisclase</span>
        </Link>

        <div className="api-status" title={apiConnected ? `Connected to ${API_BASE_URL}` : `Disconnected from ${API_BASE_URL}`}>
          {apiConnected === null ? (
            <span className="api-status-checking">Checking...</span>
          ) : apiConnected ? (
            <>
              <Wifi size={16} className="api-icon-connected" />
              <span className="api-status-text">API Connected</span>
            </>
          ) : (
            <>
              <WifiOff size={16} className="api-icon-disconnected" />
              <span className="api-status-text">API Disconnected</span>
            </>
          )}
        </div>

        {isAuthenticated && (
          <>
            <button className="mobile-menu-btn" onClick={toggleMenu}>
              {menuOpen ? <X size={24} /> : <Menu size={24} />}
            </button>

            {/* Render admin links when user is admin, otherwise render client links */}
            <nav className={`nav-menu ${menuOpen ? 'active' : ''}`}>
              {isAdmin ? (
                <>
                  <Link to="/admin" className="nav-link" onClick={() => setMenuOpen(false)}>
                    <Home size={18} />
                    <span>Admin</span>
                  </Link>
                  <Link to="/admin/clients" className="nav-link" onClick={() => setMenuOpen(false)}>
                    <Users size={18} />
                    <span>Clientes</span>
                  </Link>
                  <Link to="/admin/accounts" className="nav-link" onClick={() => setMenuOpen(false)}>
                    <CreditCard size={18} />
                    <span>Cuentas</span>
                  </Link>
                </>
              ) : (
                <>
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
                </>
              )}
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
