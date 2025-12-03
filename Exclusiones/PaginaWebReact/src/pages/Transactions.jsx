import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';
import { ArrowUpRight, ArrowDownLeft, Search, Filter } from 'lucide-react';
import '../styles/Transactions.css';

const Transactions = () => {
  const { user } = useAuth();
  const [transacciones, setTransacciones] = useState([]);
  const [filteredTransacciones, setFilteredTransacciones] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterType, setFilterType] = useState('all');

  useEffect(() => {
    loadTransactions();
  }, []);

  useEffect(() => {
    filterTransactions();
  }, [searchTerm, filterType, transacciones]);

  const normalize = (t) => ({
    Tipo: t.tipo_transaccion ?? t.Tipo ?? t.tipo ?? '',
    Fecha: t.fecha ?? t.Fecha ?? null,
    Monto: t.monto != null ? Number(t.monto) : (t.Monto != null ? Number(t.Monto) : 0),
    Descripcion: t.descripcion ?? t.Descripcion ?? '',
    NumCuenta: t.cuenta_origen ?? t.cuenta_origen_numero ?? t.cuenta_origen_num ?? t.NumCuenta ?? '',
    Estado: t.estado ?? t.Estado ?? 'Completada'
  });

  const loadTransactions = async () => {
    try {
      let response;
      
      if (user?.isAdmin) {
        // Admin: cargar todas las transacciones del sistema
        response = await apiService.getTransaccionesRecientes(50);
      } else {
        // Cliente: cargar solo las transacciones del cliente
        response = await apiService.getTransaccionesCliente(user?.username, 50);
      }
      
      const raw = response.data || [];
      const normalized = raw.map(normalize);
      setTransacciones(normalized);
      setFilteredTransacciones(normalized);
    } catch (error) {
      console.error('Error loading transactions:', error);
      setTransacciones([]);
      setFilteredTransacciones([]);
    } finally {
      setLoading(false);
    }
  };

  const filterTransactions = () => {
    let filtered = [...transacciones];

    if (searchTerm) {
      filtered = filtered.filter(t => 
        t.Tipo?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        t.Descripcion?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        t.NumCuenta?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    if (filterType !== 'all') {
      filtered = filtered.filter(t => t.Tipo === filterType);
    }

    setFilteredTransacciones(filtered);
  };

  const getTransactionIcon = (tipo) => {
    return tipo?.toLowerCase().includes('dep') || tipo?.toLowerCase().includes('ing')
      ? <ArrowDownLeft className="icon-income" />
      : <ArrowUpRight className="icon-expense" />;
  };

  const getTransactionClass = (monto) => {
    return monto > 0 ? 'positive' : 'negative';
  };

  if (loading) {
    return (
      <div className="transactions-page">
        <Header />
        <div className="loading-container">
          <div className="spinner"></div>
          <p>Cargando transacciones...</p>
        </div>
        <Footer />
      </div>
    );
  }

  return (
    <div className="transactions-page">
      <Header />
      
      <main className="transactions-content">
        <div className="transactions-header">
          <div>
            <h1>{user?.isAdmin ? 'Todas las Transacciones' : 'Mis Transacciones'}</h1>
            <p>{user?.isAdmin ? 'Historial completo del sistema' : 'Historial de mis movimientos'}</p>
          </div>
        </div>

        <div className="transactions-filters">
          <div className="search-box">
            <Search size={20} />
            <input
              type="text"
              placeholder={user?.isAdmin ? "Buscar en todas las transacciones..." : "Buscar en mis transacciones..."}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>

          <div className="filter-box">
            <Filter size={20} />
            <select value={filterType} onChange={(e) => setFilterType(e.target.value)}>
              <option value="all">Todas</option>
              <option value="Depósito">Depósitos</option>
              <option value="Retiro">Retiros</option>
              <option value="Transferencia">Transferencias</option>
              <option value="Transferencia Interna">Transferencia Interna</option>
              <option value="Transferencia Externa">Transferencia Externa</option>
              <option value="Pago">Pagos</option>
            </select>
          </div>
        </div>

        <div className="transactions-list">
          {filteredTransacciones.length > 0 ? (
            filteredTransacciones.map((transaccion, index) => (
              <div key={index} className="transaction-card">
                <div className="transaction-icon">
                  {getTransactionIcon(transaccion.Tipo)}
                </div>
                
                <div className="transaction-details">
                  <h3>{transaccion.Tipo || 'Transacción'}</h3>
                  <p className="transaction-description">
                    {transaccion.Descripcion || 'Sin descripción'}
                  </p>
                  <p className="transaction-date">
                    {transaccion.Fecha ? new Date(transaccion.Fecha).toLocaleString('es-MX', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                      hour: '2-digit',
                      minute: '2-digit'
                    }) : 'Fecha no disponible'}
                  </p>
                  {transaccion.Estado && (
                    <p className="transaction-status">Estado: {transaccion.Estado}</p>
                  )}
                </div>
                
                <div className="transaction-amount">
                  <p className={`amount ${getTransactionClass(transaccion.Monto)}`}>
                    {transaccion.Monto > 0 ? '+' : ''}
                    ${Math.abs(transaccion.Monto).toLocaleString('es-MX', {
                      minimumFractionDigits: 2,
                      maximumFractionDigits: 2
                    })}
                  </p>
                  <p className="transaction-account">
                    Cuenta: {transaccion.NumCuenta || '****'}
                  </p>
                </div>
              </div>
            ))
          ) : (
            <div className="no-transactions">
              <ArrowUpRight size={64} />
              <h2>No hay transacciones</h2>
              <p>{user?.isAdmin 
                ? 'No se encontraron transacciones con los filtros seleccionados'
                : 'No tienes transacciones con los filtros seleccionados'
              }</p>
            </div>
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default Transactions;
