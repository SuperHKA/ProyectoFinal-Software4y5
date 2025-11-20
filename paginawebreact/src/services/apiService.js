import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5202';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor para agregar el token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor para manejar errores
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('authToken');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

const apiService = {
  // Auth endpoints
  loginAdmin: async (credentials) => {
    const payload = {
      Username: credentials?.Username ?? credentials?.username ?? '',
      Password: credentials?.Password ?? credentials?.password ?? ''
    };

    if (!payload.Username || !payload.Password) {
      return Promise.reject(new Error('Username and Password are required'));
    }

    try {
      const res = await apiClient.post('/api/banco/login-admin', payload);
      if (res.status === 204 || res.status === 205) {
        return { data: { Status: 'Success', Message: '', User: null }, status: res.status };
      }
      return res;
    } catch (err) {
      console.error('loginAdmin error:', err.response?.status, err.response?.data ?? err.message);
      throw err;
    }
  },
  
  // Statistics and Dashboard
  getEstadisticas: () => 
      apiClient.get('/api/banco/estadisticas'),
  
  getMovimientosDelDia: () => 
      apiClient.get('/api/banco/movimientos-del-dia'),
  
  getTransaccionesRecientes: (limit = 20) => 
      apiClient.get(`/api/banco/transacciones-recientes?limit=${limit}`),
  
  getAnalisis: () => 
      apiClient.get('/api/banco/analisis'),
  
  // Client management
  getClientes: () => 
      apiClient.get('/api/banco/clientes'),
  
  crearCliente: (clienteData) => 
      apiClient.post('/api/banco/clientes', clienteData),
  
  actualizarCliente: (id, clienteData) => 
      apiClient.put(`/api/banco/clientes/${id}`, clienteData),
  
  getResumenClientes: () => 
      apiClient.get('/api/banco/resumen-clientes'),
  
  // Account management
  crearCuentaBancaria: (cuentaData) => 
      apiClient.post('/api/banco/cuentas', cuentaData),
  
  // User management
  getUsuariosBloqueados: () => 
      apiClient.get('/api/banco/usuarios-bloqueados'),
  
  // Note: controller expects POST /api/banco/usuarios/{id}/bloquear with body { Bloquear }
  bloquearDesbloquearUsuario: (usuarioId, bloquear) => 
      apiClient.post(`/api/banco/usuarios/${usuarioId}/bloquear`, { Bloquear: !!bloquear }),
  
  // Note: controller expects POST /api/banco/usuarios/{id}/cambiar-contrasena with body { Password }
  cambiarContrasena: (usuarioId, nuevaContrasena) => 
      apiClient.post(`/api/banco/usuarios/${usuarioId}/cambiar-contrasena`, { Password: nuevaContrasena }),
  
  // Controller expects POST /api/banco/usuarios/admin
  crearAdministrador: (adminData) => 
      apiClient.post('/api/banco/usuarios/admin', adminData),
  
  // Notifications
  getNotificaciones: () => 
      apiClient.get('/api/banco/notificaciones-sistema'),
  
  // Health check
  healthCheck: () => 
      apiClient.get('/api/banco/health'),
  
  // Reports - controller expects /api/banco/reporte?desde=...&hasta=...
  getReporteTransacciones: (desde, hasta) => {
    const desdeStr = typeof desde === 'string' ? encodeURIComponent(desde) : encodeURIComponent(new Date(desde).toISOString());
    const hastaStr = typeof hasta === 'string' ? encodeURIComponent(hasta) : encodeURIComponent(new Date(hasta).toISOString());
    return apiClient.get(`/api/banco/reporte?desde=${desdeStr}&hasta=${hastaStr}`);
  },
  
  // Connection check
  checkConnection: async () => {
    try {
        await apiClient.get('/api/banco/health', { timeout: 5000 });
      return true;
    } catch (error) {
      return false;
    }
  },

  // Transfer endpoints
  transferInternal: (payload) => apiClient.post('/api/banco/transfer/internal', payload),
  transferExternal: (payload) => apiClient.post('/api/banco/transfer/external', payload),
};

export default apiService;
export { API_BASE_URL };
