import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';
import { CreditCard, Plus, Eye, EyeOff } from 'lucide-react';
import '../styles/Accounts.css';

const Accounts = () => {
  const navigate = useNavigate();
  const [cuentas, setCuentas] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showBalances, setShowBalances] = useState({});

  useEffect(() => {
    loadAccounts();
  }, []);

  const normalizeAccount = (r) => ({
    Tipo: r.tipo ?? r.Tipo ?? r.nombre ?? r.tipo_transaccion ?? '',
    NumCuenta: r.numero_cuenta ?? r.NumCuenta ?? r.numero ?? r.NumeroCuenta ?? '****-****',
    Saldo: r.saldo ?? r.Saldo ?? r.saldo_total ?? 0,
    NombreCliente: r.nombre ?? r.NombreCliente ?? r.Nombre ?? 'Cliente',
    Activo: r.activa ?? r.Activo ?? true,
    Raw: r
  });

  const loadAccounts = async () => {
    try {
      // Use getCuentasPorCliente without id to fetch the logged-in client's accounts
      const response = await apiService.getCuentasPorCliente();
      const raw = response.data || [];
      const normalized = raw.map(normalizeAccount);
      setCuentas(normalized);
    } catch (error) {
      console.error('Error loading accounts:', error);
      // fallback: empty list
      setCuentas([]);
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

  const handleCreateAccount = async () => {
    try {
      const tipo = window.prompt('Tipo de cuenta (AHORRO, CORRIENTE, CREDITO, DEBITO)', 'AHORRO');
      if (!tipo) return;
      const saldoStr = window.prompt('Saldo inicial (numérico)', '0');
      if (saldoStr === null) return;
      const saldo = parseFloat(saldoStr) || 0;
      // generate simple account number
      const numeroCuenta = String(Math.floor(Math.random() * 1e10)).padStart(10, '0');

      const payload = {
        ClienteId: undefined, // apiService will use stored user clienteId when omitted if implemented; include null to be explicit
        Tipo: tipo.toUpperCase(),
        NumeroCuenta: numeroCuenta,
        SaldoInicial: saldo
      };

      await apiService.crearCuentaBancaria(payload);
      alert('Cuenta creada: ' + numeroCuenta);
      loadAccounts();
    } catch (err) {
      console.error('create account error', err);
      alert('Error al crear cuenta: ' + (err?.response?.data ?? err.message ?? err));
    }
  };

  const handleViewTransactions = (cuenta) => {
    // navigate to Transactions and store filter in session so page can read it
    try { sessionStorage.setItem('filterCuenta', cuenta.NumCuenta); } catch {}
    navigate('/transactions');
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
          <button className="btn-add-account" onClick={handleCreateAccount}>
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
                        ? `$${Number(cuenta.Saldo || 0).toLocaleString()}` 
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
                  <div style={{ marginTop: 8 }}>
                    <button onClick={() => handleViewTransactions(cuenta)}>Ver Movimientos</button>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="no-accounts">
              <CreditCard size={64} />
              <h2>No tienes cuentas registradas</h2>
              <p>Solicita tu primera cuenta para comenzar</p>
              <button className="btn-primary" onClick={handleCreateAccount}>Solicitar Cuenta</button>
            </div>
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
};

export default Accounts;
