import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { useNavigate } from 'react-router-dom';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';
import { TrendingUp, Users, Activity, DollarSign } from 'lucide-react';
import '../styles/Dashboard.css';

const Dashboard = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [estadisticas, setEstadisticas] = useState(null);
  const [movimientos, setMovimientos] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Si el usuario es admin, redirigir al AdminDashboard
    if (user?.isAdmin) {
      navigate('/admin-dashboard');
      return;
    }
    
    loadDashboardData();
  }, [user, navigate]);

  const normalizeStats = (raw = {}) => ({
    SaldoTotal: Number(raw.saldo_total ?? raw.SaldoTotal ?? 0),
    TransaccionesHoy: Number(raw.transacciones_hoy ?? raw.TransaccionesHoy ?? 0),
    ClientesActivos: 1, // El cliente mismo
    CuentasTotales: Number(raw.total_cuentas ?? raw.CuentasTotales ?? 0)
  });

  const normalizeMovimiento = (m) => ({
    Tipo: m.tipo_transaccion ?? m.tipo ?? m.Tipo ?? '',
    Fecha: m.fecha ?? m.Fecha ?? null,
    Monto: m.monto != null ? Number(m.monto) : (m.Monto != null ? Number(m.Monto) : 0),
    Descripcion: m.descripcion ?? m.Descripcion ?? '',
    NumCuenta: m.cuenta_origen ?? m.cuenta_origen_numero ?? m.cuenta_origen_num ?? m.cuentaDestino ?? m.cuenta_destino ?? null
  });

  const loadDashboardData = async () => {
    if (!user?.username) {
      setLoading(false);
      return;
    }

    try {
      console.log('Loading dashboard for user:', user); // Debug

      // Cliente: cargar datos específicos del cliente
      const [estadisticasRes, movimientosRes] = await Promise.all([
        apiService.getDashboardCliente(user.username),
        apiService.getTransaccionesCliente(user.username, 10) // Últimas 10 transacciones
      ]);

      console.log('Dashboard data received:', { estadisticasRes, movimientosRes }); // Debug

      const rawStats = (estadisticasRes?.data && estadisticasRes.data[0]) ? estadisticasRes.data[0] : {};
      setEstadisticas(normalizeStats(rawStats));

      const rawMov = movimientosRes?.data || [];
      setMovimientos(rawMov.map(normalizeMovimiento));
    } catch (error) {
      console.error('Error loading dashboard:', error);
      // Para clientes sin datos, mostrar valores por defecto
      setEstadisticas({
        SaldoTotal: 0,
        TransaccionesHoy: 0,
        ClientesActivos: 1,
        CuentasTotales: 0
      });
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="dashboard-page">
        <Header />
        <div className="loading-container">
          <div className="spinner"></div>
          <p>Cargando...</p>
        </div>
        <Footer />
      </div>
    );
  }

  return (
    <div className="dashboard-page">
      <Header />
      
      <main className="dashboard-content">
        <div className="dashboard-header">
          <h1>Bienvenido, {user?.username}</h1>
          <p>Mi Panel Personal</p>
        </div>

        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-icon blue">
              <DollarSign size={32} />
            </div>
            <div className="stat-info">
              <h3>Mi Saldo Total</h3>
              <p className="stat-value">${estadisticas?.SaldoTotal?.toLocaleString() || '0.00'}</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon green">
              <TrendingUp size={32} />
            </div>
            <div className="stat-info">
              <h3>Mis Transacciones Hoy</h3>
              <p className="stat-value">{estadisticas?.TransaccionesHoy || 0}</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon purple">
              <Users size={32} />
            </div>
            <div className="stat-info">
              <h3>Mi Perfil</h3>
              <p className="stat-value">Activo</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon orange">
              <Activity size={32} />
            </div>
            <div className="stat-info">
              <h3>Mis Cuentas</h3>
              <p className="stat-value">{estadisticas?.CuentasTotales || 0}</p>
            </div>
          </div>
        </div>

        <div className="dashboard-sections">
          <section className="recent-transactions">
            <h2>Mis Últimas Transacciones</h2>
            <div className="transactions-list">
              {movimientos.length > 0 ? (
                movimientos.map((mov, index) => (
                  <div key={index} className="transaction-item">
                    <div className="transaction-info">
                      <p className="transaction-type">{mov.Tipo}</p>
                      <p className="transaction-date">{mov.Fecha ? new Date(mov.Fecha).toLocaleString() : ''}</p>
                    </div>
                    <p className={`transaction-amount ${mov.Monto > 0 ? 'positive' : 'negative'}`}>
                      ${Math.abs(mov.Monto).toLocaleString()}
                    </p>
                  </div>
                ))
              ) : (
                <p className="no-data">No tienes transacciones recientes</p>
              )}
            </div>
          </section>

          <section className="quick-actions">
            <h2>Acciones Rápidas</h2>
            <div className="actions-grid">
              <a href="/transfer" className="action-card">
                <TrendingUp size={24} />
                <span>Transferir</span>
              </a>
              <a href="/accounts" className="action-card">
                <DollarSign size={24} />
                <span>Ver Cuentas</span>
              </a>
              <a href="/transactions" className="action-card">
                <Activity size={24} />
                <span>Historial</span>
              </a>
              <a href="/profile" className="action-card">
                <Users size={24} />
                <span>Perfil</span>
              </a>
            </div>
          </section>
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default Dashboard;
