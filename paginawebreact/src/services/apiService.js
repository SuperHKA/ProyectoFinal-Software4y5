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

function getStoredUser() {
  try {
    const raw = localStorage.getItem('user');
    if (!raw) return null;
    return JSON.parse(raw);
  } catch {
    return null;
  }
}

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
      // return response regardless of status so caller can inspect codes like 401/403/404
      const res = await apiClient.post('/api/banco/login-admin', payload, { validateStatus: () => true });
      // normalize empty 204/205
      if (res.status === 204 || res.status === 205) {
        return { data: { Status: 'Success', Message: '', User: null }, status: res.status };
      }
      return res;
    } catch (err) {
      // network or unexpected error
      console.error('loginAdmin network error:', err.message || err);
      // rethrow so callers can handle
      throw err;
    }
  },

  // Auth: normal user login
  login: async (credentials) => {
    const payload = {
      Username: credentials?.Username ?? credentials?.username ?? '',
      Password: credentials?.Password ?? credentials?.password ?? ''
    };

    if (!payload.Username || !payload.Password) {
      return Promise.reject(new Error('Username and Password are required'));
    }

    try {
      const res = await apiClient.post('/api/banco/login', payload, { validateStatus: () => true });
      if (res.status === 204 || res.status === 205) {
        return { data: { Status: 'Success', Message: '', User: null }, status: res.status };
      }
      return res;
    } catch (err) {
      console.error('login network error:', err.message || err);
      throw err;
    }
  },

  // new helper: check if username is admin
  isAdmin: async (username) => {
    if (!username) return false;
    try {
      const res = await apiClient.get('/api/banco/is-admin', { params: { username } , validateStatus: () => true });
      if (!res || res.status >= 400) return false;
      // Expect { username, isAdmin }
      // normalize: server may return User object in login response, but this endpoint returns { isAdmin }
      return !!(res.data && (res.data.isAdmin === true || res.data.IsAdmin === true));
    } catch (err) {
      console.error('isAdmin check failed:', err.message || err);
      return false;
    }
  },

  // get accounts for a client with enforcement: non-admins can only request their own clienteId
  getCuentasPorCliente: async (clienteId) => {
    const storedUser = getStoredUser();
    const targetId = clienteId ?? (storedUser ? (storedUser.clienteId ?? storedUser.ClienteId ?? null) : null);

    if (!storedUser) {
      return Promise.reject({ response: { status: 401, data: 'Not authenticated' } });
    }

    // If the logged user is not admin, ensure they only request their own clienteId
    const isAdmin = !!(storedUser.isAdmin || storedUser.IsAdmin);
    const userClienteId = storedUser.clienteId ?? storedUser.ClienteId ?? null;

    if (!isAdmin) {
      if (targetId == null) {
        return Promise.reject({ response: { status: 403, data: 'Forbidden: no client id available' } });
      }
      if (userClienteId == null || String(targetId) !== String(userClienteId)) {
        return Promise.reject({ response: { status: 403, data: 'Forbidden: cannot access other client accounts' } });
      }
    }

    if (targetId == null) {
      return Promise.reject({ response: { status: 400, data: 'clienteId required' } });
    }

    return apiClient.get(`/api/banco/clientes/${encodeURIComponent(targetId)}/cuentas`);
  },

  // new helper: get accounts by username (calls server endpoint I added)
  getCuentasPorUsuario: async (username) => {
    if (!username) return Promise.reject(new Error('username required'));
    try {
      return await apiClient.get('/api/banco/clientes/por-usuario', { params: { username } });
    } catch (err) {
      throw err;
    }
  },

  // NEW: Get client dashboard data (use for non-admin users)
  getDashboardCliente: async (username) => {
    if (!username) return Promise.reject(new Error('username required'));
    try {
      return await apiClient.get('/api/banco/dashboard/usuario', { params: { username } });
    } catch (err) {
      throw err;
    }
  },

  // NEW: Get client transactions (use for non-admin users)
  getTransaccionesCliente: async (username, limit = 20) => {
    if (!username) return Promise.reject(new Error('username required'));
    try {
      return await apiClient.get('/api/banco/transacciones/usuario', { params: { username, limit } });
    } catch (err) {
      throw err;
    }
  },

  // Statistics and Dashboard (ADMIN ONLY)
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

  // NEW: Debug endpoint
  debugUser: async (username) => {
    if (!username) return Promise.reject(new Error('username required'));
    try {
      return await apiClient.get('/api/banco/debug/user', { params: { username } });
    } catch (err) {
      throw err;
    }
  },

  // NEW: Get all accounts for transfer dropdowns
  getCuentas: () => 
      apiClient.get('/api/banco/cuentas'),

  // NEW: Get all clients for transfer dropdowns
  getClientesParaTransferencia: () => 
      apiClient.get('/api/banco/clientes-transferencia'),
};

// add transfer methods
apiService.transferInternal = (payload) => apiClient.post('/api/banco/transfer/internal', payload);
apiService.transferExternal = (payload) => apiClient.post('/api/banco/transfer/external', payload);

export default apiService;
export { API_BASE_URL };
