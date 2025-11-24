import React, { useEffect, useState } from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';

const AdminClients = () => {
  const [clients, setClients] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let mounted = true;
    apiService.getClientes()
      .then(res => { if (mounted) setClients(res.data || []); })
      .catch(() => { if (mounted) setClients([]); })
      .finally(() => { if (mounted) setLoading(false); });
    return () => { mounted = false; };
  }, []);

  return (
    <div>
      <Header />
      <main className="container">
        <h1>Clientes</h1>
        {loading ? <p>Cargando...</p> : (
          <table className="table">
            <thead><tr><th>ID</th><th>Usuario</th><th>Nombre</th><th>Email</th><th>Admin</th><th>Bloqueado</th></tr></thead>
            <tbody>
              {clients.map(c => (
                <tr key={c.usuario_id}>
                  <td>{c.usuario_id}</td>
                  <td>{c.username}</td>
                  <td>{c.nombre_completo}</td>
                  <td>{c.email}</td>
                  <td>{c.es_admin ? 'Sí' : 'No'}</td>
                  <td>{c.bloqueado ? 'Sí' : 'No'}</td>
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

export default AdminClients;