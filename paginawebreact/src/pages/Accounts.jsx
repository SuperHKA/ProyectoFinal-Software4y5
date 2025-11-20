import { useState, useEffect } from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';
import { CreditCard, Plus, Eye, EyeOff } from 'lucide-react';
import '../styles/Accounts.css';

const Accounts = () => {
  const [cuentas, setCuentas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showBalances, setShowBalances] = useState({});

  useEffect(() => {
    loadAccounts();
  }, []);

  const normalizeAccount = (r) => ({
    Tipo: r.tipo ?? r.Tipo ?? r.tipo_transaccion ?? '',
    NumCuenta: r.numero_cuenta ?? r.NumCuenta ?? r.numero ?? r.NumCuenta ?? '****-****',
    Saldo: r.saldo_total ?? r.saldo ?? r.Saldo ?? 0,
    NombreCliente: r.nombre ?? r.NombreCliente ?? r.Nombre ?? 'Cliente',
    Activo: r.activo ?? r.Activo ?? true
  });

  const loadAccounts = async () => {
    try {
      const response = await apiService.getResumenClientes();
      const raw = response.data || [];
      const normalized = raw.map(normalizeAccount);
      setCuentas(normalized);
    } catch (error) {
      console.error('Error loading accounts:', error);
    } finally {
      setLoading(false);
    }
  };

  const toggleBalance = (index) => {
    setShowBalances(prev => ({
      ...prev,
      [index]: !prev[index]
    }));
  };

  const getAccountTypeColor = (tipo) => {
    const colors = {
      'AHORRO': 'blue',
      'CORRIENTE': 'green',
      'CREDITO': 'purple',
      'DEBITO': 'orange'
    };
    return colors[tipo] || 'gray';
  };

  if (loading) {
    return (
      <div className="accounts-page">
        <Header />
        <div className="loading-container">
          <div className="spinner"></div>
          <p>Cargando cuentas...</p>
        </div>
        <Footer />
      </div>
    );
  }

  return (
    <div className="accounts-page">
      <Header />
      
      <main className="accounts-content">
        <div className="accounts-header">
          <div>
            <h1>Mis Cuentas</h1>
            <p>Gestiona todas tus cuentas bancarias</p>
          </div>
          <button className="btn-add-account">
            <Plus size={20} />
            <span>Nueva Cuenta</span>
          </button>
        </div>

        <div className="accounts-grid">
          {cuentas.length > 0 ? (
            cuentas.map((cuenta, index) => (
              <div key={index} className={`account-card ${getAccountTypeColor(cuenta.Tipo)}`}>
                <div className="account-card-header">
                  <div className="account-icon">
                    <CreditCard size={32} />
                  </div>
                  <span className="account-type">{cuenta.Tipo}</span>
                </div>
                
                <div className="account-number">
                  <p>Número de Cuenta</p>
                  <h3>{cuenta.NumCuenta || '****-****-****'}</h3>
                </div>
                
                <div className="account-balance">
                  <p>Saldo Disponible</p>
                  <div className="balance-row">
                    <h2>
                      {showBalances[index] 
                        ? `$${cuenta.Saldo?.toLocaleString() || '0.00'}` 
                        : '••••••'
                      }
                    </h2>
                    <button 
                      className="toggle-balance-btn"
                      onClick={() => toggleBalance(index)}
                    >
                      {showBalances[index] ? <EyeOff size={20} /> : <Eye size={20} />}
                    </button>
                  </div>
                </div>
                
                <div className="account-footer">
                  <p className="account-holder">{cuenta.NombreCliente || 'Cliente'}</p>
                  <p className={`account-status ${cuenta.Activo ? 'active' : 'inactive'}`}>
                    {cuenta.Activo ? 'Activa' : 'Inactiva'}
                  </p>
                </div>
              </div>
            ))
          ) : (
            <div className="no-accounts">
              <CreditCard size={64} />
              <h2>No tienes cuentas registradas</h2>
              <p>Solicita tu primera cuenta para comenzar</p>
              <button className="btn-primary">Solicitar Cuenta</button>
            </div>
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default Accounts;
