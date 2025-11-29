import React, { useState } from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';

const AdminAccounts = () => {
  const [clienteId, setClienteId] = useState('');
  const [cuentas, setCuentas] = useState(null);
  const [error, setError] = useState(null);

  const handleLoad = async () => {
    setError(null);
    setCuentas(null);
    if (!clienteId) { setError('Ingrese clienteId'); return; }
    try {
      const res = await apiService.getCuentasPorCliente(clienteId);
      setCuentas(res.data || []);
    } catch (err) {
      setError(err?.response?.data ?? err.message ?? 'Error');
    }
  };

  return (
    <div>
      <Header />
      <main className="container">
        <h1>Consultar Cuentas por Cliente</h1>
        <div style={{ marginBottom: 12 }}>
          <input placeholder="clienteId" value={clienteId} onChange={e => setClienteId(e.target.value)} />
          <button onClick={handleLoad}>Cargar</button>
        </div>
        {error && <div style={{ color: 'red' }}>{error}</div>}
        {cuentas && (
          <table className="table">
            <thead><tr><th>ID</th><th>Numero</th><th>Tipo</th><th>Saldo</th><th>Activa</th></tr></thead>
            <tbody>
              {cuentas.map(c => (
                <tr key={c.cuenta_id}>
                  <td>{c.cuenta_id}</td>
                  <td>{c.numero_cuenta}</td>
                  <td>{c.tipo}</td>
                  <td>{c.saldo}</td>
                  <td>{c.activa ? 'Sí' : 'No'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </main>
      <Footer />
    </div>
  );
};

export default AdminAccounts;