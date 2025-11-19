import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

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
  loginAdmin: (credentials) => 
    apiClient.post('/banco/login-admin', credentials),
  
  // Statistics and Dashboard
  getEstadisticas: () => 
    apiClient.get('/banco/estadisticas'),
  
  getMovimientosDelDia: () => 
    apiClient.get('/banco/movimientos-del-dia'),
  
  getTransaccionesRecientes: (limit = 20) => 
    apiClient.get(`/banco/transacciones-recientes?limit=${limit}`),
  
  getAnalisis: () => 
    apiClient.get('/banco/analisis'),
  
  // Client management
  getClientes: () => 
    apiClient.get('/banco/clientes'),
  
  crearCliente: (clienteData) => 
    apiClient.post('/banco/clientes', clienteData),
  
  actualizarCliente: (id, clienteData) => 
    apiClient.put(`/banco/clientes/${id}`, clienteData),
  
  getResumenClientes: () => 
    apiClient.get('/banco/resumen-clientes'),
  
  // Account management
  crearCuentaBancaria: (cuentaData) => 
    apiClient.post('/banco/cuentas', cuentaData),
  
  // User management
  getUsuariosBloqueados: () => 
    apiClient.get('/banco/usuarios-bloqueados'),
  
  bloquearDesbloquearUsuario: (usuarioId, bloquear) => 
    apiClient.post('/banco/usuarios/bloquear-desbloquear', { usuarioId, bloquear }),
  
  cambiarContrasena: (usuarioId, nuevaContrasena) => 
    apiClient.post('/banco/usuarios/cambiar-contrasena', { usuarioId, nuevaContrasena }),
  
  crearAdministrador: (adminData) => 
    apiClient.post('/banco/administradores', adminData),
  
  // Notifications
  getNotificaciones: () => 
    apiClient.get('/banco/notificaciones-sistema'),
  
  // Health check
  healthCheck: () => 
    apiClient.get('/banco/health'),
  
  // Reports
  getReporteTransacciones: (desde, hasta) => 
    apiClient.get(`/banco/reportes/transacciones?desde=${desde}&hasta=${hasta}`),
};

export default apiService;
