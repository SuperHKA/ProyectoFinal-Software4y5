import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';
import { TrendingUp, Users, Activity, DollarSign } from 'lucide-react';
import '../styles/Dashboard.css';

const Dashboard = () => {
  const { user } = useAuth();
  const [estadisticas, setEstadisticas] = useState(null);
  const [movimientos, setMovimientos] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    try {
      const [estadisticasRes, movimientosRes] = await Promise.all([
        apiService.getEstadisticas(),
        apiService.getMovimientosDelDia()
      ]);
      
      setEstadisticas(estadisticasRes.data[0] || {});
      setMovimientos(movimientosRes.data || []);
    } catch (error) {
      console.error('Error loading dashboard:', error);
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
          <p>Panel de Control</p>
        </div>

        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-icon blue">
              <DollarSign size={32} />
            </div>
            <div className="stat-info">
              <h3>Saldo Total</h3>
              <p className="stat-value">${estadisticas?.SaldoTotal?.toLocaleString() || '0.00'}</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon green">
              <TrendingUp size={32} />
            </div>
            <div className="stat-info">
              <h3>Transacciones Hoy</h3>
              <p className="stat-value">{estadisticas?.TransaccionesHoy || 0}</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon purple">
              <Users size={32} />
            </div>
            <div className="stat-info">
              <h3>Clientes Activos</h3>
              <p className="stat-value">{estadisticas?.ClientesActivos || 0}</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon orange">
              <Activity size={32} />
            </div>
            <div className="stat-info">
              <h3>Cuentas Totales</h3>
              <p className="stat-value">{estadisticas?.CuentasTotales || 0}</p>
            </div>
          </div>
        </div>

        <div className="dashboard-sections">
          <section className="recent-transactions">
            <h2>Movimientos del Día</h2>
            <div className="transactions-list">
              {movimientos.length > 0 ? (
                movimientos.map((mov, index) => (
                  <div key={index} className="transaction-item">
                    <div className="transaction-info">
                      <p className="transaction-type">{mov.Tipo}</p>
                      <p className="transaction-date">{new Date(mov.Fecha).toLocaleString()}</p>
                    </div>
                    <p className={`transaction-amount ${mov.Monto > 0 ? 'positive' : 'negative'}`}>
                      ${Math.abs(mov.Monto).toLocaleString()}
                    </p>
                  </div>
                ))
              ) : (
                <p className="no-data">No hay movimientos registrados hoy</p>
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
