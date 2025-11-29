import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';
import { TrendingUp, Users, Activity, DollarSign, Shield, AlertTriangle } from 'lucide-react';
import '../styles/Dashboard.css';
import '../styles/AdminDashboard.css';

const AdminDashboard = () => {
  const { user } = useAuth();
  const [estadisticas, setEstadisticas] = useState(null);
  const [movimientos, setMovimientos] = useState([]);
  const [usuariosBloqueados, setUsuariosBloqueados] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (user?.isAdmin) {
      loadAdminData();
    }
  }, [user]);

  const normalizeStats = (raw = {}) => ({
    SaldoTotal: Number(raw.saldo_total_sistema ?? raw.saldo_total ?? raw.SaldoTotal ?? 0),
    TransaccionesHoy: Number(raw.transacciones_hoy ?? raw.transaccionesHoy ?? raw.TransaccionesHoy ?? 0),
    ClientesActivos: Number(raw.total_clientes ?? raw.clientes_activos ?? raw.ClientesActivos ?? 0),
    CuentasTotales: Number(raw.total_cuentas ?? raw.cuentas_totales ?? raw.CuentasTotales ?? 0)
  });

  const normalizeMovimiento = (m) => ({
    Tipo: m.tipo_transaccion ?? m.tipo ?? m.Tipo ?? '',
    Fecha: m.fecha ?? m.Fecha ?? null,
    Monto: m.monto != null ? Number(m.monto) : (m.Monto != null ? Number(m.Monto) : 0),
    Descripcion: m.descripcion ?? m.Descripcion ?? '',
    NumCuenta: m.cuenta_origen ?? m.cuenta_origen_numero ?? m.cuenta_origen_num ?? null
  });

  const loadAdminData = async () => {
    try {
      const [estadisticasRes, movimientosRes, bloqueadosRes] = await Promise.all([
        apiService.getEstadisticas(),
        apiService.getMovimientosDelDia(),
        apiService.getUsuariosBloqueados()
      ]);

      const rawStats = (estadisticasRes?.data && estadisticasRes.data[0]) ? estadisticasRes.data[0] : {};
      setEstadisticas(normalizeStats(rawStats));

      const rawMov = movimientosRes?.data || [];
      setMovimientos(rawMov.map(normalizeMovimiento));

      const rawBloqueados = bloqueadosRes?.data || [];
      setUsuariosBloqueados(rawBloqueados);
    } catch (error) {
      console.error('Error loading admin dashboard:', error);
    } finally {
      setLoading(false);
    }
  };

  if (!user?.isAdmin) {
    return (
      <div className="dashboard-page">
        <Header />
        <main className="dashboard-content">
          <div className="error-message">
            <Shield size={64} />
            <h2>Acceso Denegado</h2>
            <p>No tienes permisos para acceder a esta página.</p>
          </div>
        </main>
        <Footer />
      </div>
    );
  }

  if (loading) {
    return (
      <div className="dashboard-page">
        <Header />
        <div className="loading-container">
          <div className="spinner"></div>
          <p>Cargando panel administrativo...</p>
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
          <div>
            <h1>Panel de Administración</h1>
            <p>Bienvenido, {user?.username} - Vista completa del sistema</p>
          </div>
        </div>

        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-icon blue">
              <DollarSign size={32} />
            </div>
            <div className="stat-info">
              <h3>Saldo Total del Sistema</h3>
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
            <h2>Movimientos del Día del Sistema</h2>
            <div className="transactions-list">
              {movimientos.length > 0 ? (
                movimientos.slice(0, 10).map((mov, index) => (
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
                <p className="no-data">No hay movimientos registrados hoy</p>
              )}
            </div>
          </section>

          <section className="blocked-users">
            <h2>
              <AlertTriangle size={20} style={{ marginRight: '8px' }} />
              Usuarios Bloqueados
            </h2>
            <div className="blocked-users-list">
              {usuariosBloqueados.length > 0 ? (
                usuariosBloqueados.slice(0, 5).map((usuario, index) => (
                  <div key={index} className="blocked-user-item">
                    <div className="user-info">
                      <p className="username">{usuario.username || 'Usuario'}</p>
                      <p className="client-name">{usuario.cliente_nombre || 'Sin nombre'}</p>
                    </div>
                    <div className="attempts">
                      <span className="attempts-count">{usuario.intentos_fallidos || 0} intentos</span>
                    </div>
                  </div>
                ))
              ) : (
                <p className="no-data">No hay usuarios bloqueados</p>
              )}
            </div>
          </section>
        </div>

        <div className="admin-actions">
          <h2>Acciones Administrativas</h2>
          <div className="actions-grid">
            <a href="/admin/clients" className="action-card admin">
              <Users size={24} />
              <span>Gestionar Clientes</span>
            </a>
            <a href="/admin/reports" className="action-card admin">
              <Activity size={24} />
              <span>Reportes y Análisis</span>
            </a>
            <a href="/admin/transactions" className="action-card admin">
              <TrendingUp size={24} />
              <span>Transacciones del Sistema</span>
            </a>
            <a href="/admin/settings" className="action-card admin">
              <Shield size={24} />
              <span>Configuración del Sistema</span>
            </a>
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default AdminDashboard;