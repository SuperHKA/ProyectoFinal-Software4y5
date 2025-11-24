import React from 'react';
import Header from '../components/Header';
import Footer from '../components/Footer';

const AdminDashboard = () => {
  return (
    <div>
      <Header />
      <main className="container">
        <h1>Admin Dashboard</h1>
        <p>Administración - panel con estadísticas y acciones administrativas.</p>
        {/* Minimum widgets: totals, recent transactions, blocked users */}
        <section>
          <h2>Estadísticas</h2>
          <div id="admin-stats">Cargando...</div>
        </section>

        <section>
          <h2>Usuarios Bloqueados</h2>
          <div id="admin-blocked-users">Cargando...</div>
        </section>

      </main>
      <Footer />
    </div>
  );
};

export default AdminDashboard;