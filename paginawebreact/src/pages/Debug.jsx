import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import Header from '../components/Header';
import Footer from '../components/Footer';
import apiService from '../services/apiService';

const Debug = () => {
  const { user, isAuthenticated } = useAuth();
  const [apiData, setApiData] = useState({});
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (user) {
      loadDebugData();
    } else {
      setLoading(false);
    }
  }, [user]);

  const loadDebugData = async () => {
    try {
      const data = {};
      
      // NEW: Use the dedicated debug endpoint
      if (user?.username) {
        try {
          data.userDebug = await apiService.debugUser(user.username);
        } catch (e) {
          data.userDebugError = e.message;
        }
      }
      
      // Test admin check
      if (user?.username) {
        try {
          data.isAdminCheck = await apiService.isAdmin(user.username);
        } catch (e) {
          data.isAdminError = e.message;
        }
      }

      // Test dashboard data
      try {
        data.dashboardData = await apiService.getDashboardCliente(user.username);
      } catch (e) {
        data.dashboardError = e.message;
      }

      // Test accounts
      try {
        data.accountsData = await apiService.getCuentasPorUsuario(user.username);
      } catch (e) {
        data.accountsError = e.message;
      }

      // Test transactions
      try {
        data.transactionsData = await apiService.getTransaccionesCliente(user.username, 5);
      } catch (e) {
        data.transactionsError = e.message;
      }

      setApiData(data);
    } catch (error) {
      console.error('Debug data error:', error);
    } finally {
      setLoading(false);
    }
  };

  if (!isAuthenticated) {
    return (
      <div>
        <Header />
        <main style={{ padding: '2rem' }}>
          <h1>Debug - No autenticado</h1>
          <p>Debes iniciar sesión para usar esta página.</p>
        </main>
        <Footer />
      </div>
    );
  }

  return (
    <div>
      <Header />
      <main style={{ padding: '2rem', fontFamily: 'monospace' }}>
        <h1>Página de Diagnóstico</h1>
        
        <section style={{ marginBottom: '2rem', padding: '1rem', border: '1px solid #ddd', borderRadius: '8px' }}>
          <h2>Usuario Actual (Frontend)</h2>
          <pre>{JSON.stringify(user, null, 2)}</pre>
        </section>

        <section style={{ marginBottom: '2rem', padding: '1rem', border: '1px solid #ddd', borderRadius: '8px' }}>
          <h2>Estado de Autenticación</h2>
          <p>Autenticado: {isAuthenticated ? 'Sí' : 'No'}</p>
          <p>Es Admin (Frontend): {user?.isAdmin ? 'Sí' : 'No'}</p>
        </section>

        {loading ? (
          <p>Cargando datos de API...</p>
        ) : (
          <section style={{ marginBottom: '2rem', padding: '1rem', border: '1px solid #ddd', borderRadius: '8px' }}>
            <h2>Respuestas de la API</h2>
            
            <h3>Debug Completo del Usuario</h3>
            {apiData.userDebugError ? (
              <p style={{ color: 'red' }}>Error: {apiData.userDebugError}</p>
            ) : (
              <pre style={{ backgroundColor: '#f5f5f5', padding: '1rem', borderRadius: '4px' }}>
                {JSON.stringify(apiData.userDebug?.data, null, 2)}
              </pre>
            )}
            
            <h3>Verificación de Admin</h3>
            {apiData.isAdminError ? (
              <p style={{ color: 'red' }}>Error: {apiData.isAdminError}</p>
            ) : (
              <p>Es Admin (API): {apiData.isAdminCheck ? 'Sí' : 'No'}</p>
            )}

            <h3>Dashboard Data</h3>
            {apiData.dashboardError ? (
              <p style={{ color: 'red' }}>Error: {apiData.dashboardError}</p>
            ) : (
              <pre>{JSON.stringify(apiData.dashboardData?.data, null, 2)}</pre>
            )}

            <h3>Cuentas</h3>
            {apiData.accountsError ? (
              <p style={{ color: 'red' }}>Error: {apiData.accountsError}</p>
            ) : (
              <pre>{JSON.stringify(apiData.accountsData?.data, null, 2)}</pre>
            )}

            <h3>Transacciones</h3>
            {apiData.transactionsError ? (
              <p style={{ color: 'red' }}>Error: {apiData.transactionsError}</p>
            ) : (
              <pre>{JSON.stringify(apiData.transactionsData?.data, null, 2)}</pre>
            )}
          </section>
        )}

        <section style={{ marginBottom: '2rem', padding: '1rem', border: '1px solid #ddd', borderRadius: '8px' }}>
          <h2>LocalStorage</h2>
          <pre>{localStorage.getItem('user')}</pre>
        </section>
      </main>
      <Footer />
    </div>
  );
};

export default Debug;