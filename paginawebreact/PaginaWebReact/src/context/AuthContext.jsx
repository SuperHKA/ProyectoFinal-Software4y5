import React, { createContext, useContext, useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import apiService from '../services/apiService';

const AuthContext = createContext(undefined);

export const AuthProvider = ({ children }) => {
  const navigate = useNavigate();

  const [loading, setLoading] = useState(false);

  const [user, setUser] = useState(() => {
    const raw = localStorage.getItem('user');
    if (!raw) return null;
    try {
      return JSON.parse(raw);
    } catch {
      return null;
    }
  });
  const [token, setToken] = useState(() => localStorage.getItem('authToken'));

  useEffect(() => {
    if (token) localStorage.setItem('authToken', token);
    else localStorage.removeItem('authToken');
  }, [token]);

  // inactivity timeout (10 minutes)
  const INACTIVITY_TIMEOUT_MS = 10 * 60 * 1000; // 10 minutes
  const inactivityTimerRef = useRef(null);
  const activityEvents = ['mousemove', 'mousedown', 'keydown', 'scroll', 'touchstart', 'click'];

  const clearInactivityTimer = () => {
    if (inactivityTimerRef.current) {
      clearTimeout(inactivityTimerRef.current);
      inactivityTimerRef.current = null;
    }
  };

  const startInactivityTimer = () => {
    clearInactivityTimer();
    inactivityTimerRef.current = setTimeout(() => {
      // auto logout on inactivity
      console.info('User inactive, logging out');
      handleAutoLogout();
    }, INACTIVITY_TIMEOUT_MS);
  };

  const resetInactivityTimer = () => {
    if (user) startInactivityTimer();
  };

  // called when inactivity timeout fires
  const handleAutoLogout = () => {
    setUser(null);
    setToken(null);
    try { localStorage.removeItem('user'); } catch {};
    try { localStorage.removeItem('authToken'); } catch {};
    // navigate to login page
    try { navigate('/login'); } catch {}
  };

  useEffect(() => {
    // attach listeners when authenticated
    const onActivity = () => resetInactivityTimer();

    if (user) {
      // start timer and add listeners
      startInactivityTimer();
      activityEvents.forEach((ev) => window.addEventListener(ev, onActivity));
    }

    return () => {
      // cleanup listeners and timers
      activityEvents.forEach((ev) => window.removeEventListener(ev, onActivity));
      clearInactivityTimer();
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user]);

  // login can accept either (credentialsObject) or (username, password)
  const login = async (usernameOrCredentials, password) => {
    setLoading(true);
    try {
      const credentials = typeof usernameOrCredentials === 'string'
        ? { Username: usernameOrCredentials, Password: password }
        : usernameOrCredentials;

      if (!credentials?.Username || !credentials?.Password) {
        setLoading(false);
        return { success: false, message: 'Username and Password required' };
      }

      // Always attempt the normal login endpoint first and require server-side admin flag check after auth
      let res;
      try {
        res = await apiService.login(credentials);
      } catch (err) {
        // network or unexpected
        setLoading(false);
        console.error('login network error', err);
        return { success: false, message: err?.message ?? 'Network error' };
      }

      const status = res?.status ?? (res && res.data ? 200 : 500);
      const data = res?.data ?? res;

      if (status >= 400) {
        setLoading(false);
        if (status === 401) return { success: false, message: 'Credenciales incorrectas' };
        if (status === 423) return { success: false, message: 'Cuenta bloqueada' };
        if (status === 404) return { success: false, message: 'Usuario no encontrado' };
        // if server returned forbidden, do not auto-try admin endpoint to avoid privilege escalation
        const msg = (data && (data.message || data.Message)) || `Login failed (${status})`;
        return { success: false, message: msg };
      }

      // data may include User object from API -> prefer server-provided role/ids
      let isAdmin = false;
      let userId = null;
      let clienteId = null;

      if (data && data.User) {
        const u = data.User;
        // support different casings
        isAdmin = !!(u.IsAdmin ?? u.isAdmin ?? u.es_admin);
        userId = u.Id ?? u.id ?? u.usuario_id;
        clienteId = u.ClienteId ?? u.clienteId ?? u.cliente_id ?? null;
      }

      // ALWAYS call isAdmin endpoint to ensure we have the correct admin status
      try {
        const adminCheck = await apiService.isAdmin(credentials.Username);
        isAdmin = adminCheck; // Override with server response
      } catch (e) {
        console.warn('isAdmin check failed after login, treating as non-admin', e);
        isAdmin = false;
      }

      // If clienteId is still missing, try to resolve it by fetching clients list and matching username
      if (clienteId == null && !isAdmin) { // Only for non-admin users
        try {
          console.log('Trying to resolve clienteId for non-admin user:', credentials.Username);
          const clienteIdRes = await apiService.getCuentasPorUsuario(credentials.Username);
          console.log('getCuentasPorUsuario response:', clienteIdRes);
          
          // This endpoint should work, but let's also try a direct approach
          if (!clienteId && data && data.User && data.User.ClienteId) {
            clienteId = data.User.ClienteId;
            console.log('Using clienteId from server User data:', clienteId);
          }
        } catch (e) {
          // ignore - leave clienteId null
          console.warn('Could not resolve clienteId after login', e);
        }
      }

      const userInfo = { username: credentials.Username.trim(), isAdmin, id: userId, clienteId };
      setUser(userInfo);
      try { localStorage.setItem('user', JSON.stringify(userInfo)); } catch { }

      // Debug logging
      console.log('Login successful:', { userInfo, serverData: data });

      // if API returned a token include it
      if (data && (data.token || data.Token)) {
        const t = data.token ?? data.Token;
        setToken(t);
      }

      // start/reset inactivity timer on successful login
      startInactivityTimer();

      setLoading(false);
      return { success: true, data, user: userInfo };
    } catch (err) {
      setLoading(false);
      console.error('login error', err);
      return { success: false, message: err?.message ?? 'Error during login' };
    }
  };

  const register = async (userData) => {
    setLoading(true);
    try {
      // Expect userData to have username, password, nombreCompleto, email, telefono
      const payload = {
        Username: userData.username,
        Nombre: userData.nombreCompleto || userData.nombre || '',
        Email: userData.email,
        Telefono: userData.telefono,
        Password: userData.password
      };

      const res = await apiService.crearCliente(payload);
      const ok = res?.data === true || res?.status === 200;
      setLoading(false);
      return { success: !!ok, message: ok ? null : (res?.data || 'Registration failed') };
    } catch (err) {
      console.error('register error', err);
      setLoading(false);
      return { success: false, message: err?.response?.data ?? err.message };
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    try { localStorage.removeItem('user'); } catch {}
    try { localStorage.removeItem('authToken'); } catch {}
    clearInactivityTimer();
    navigate('/login');
  };
    
  const isAuthenticated = !!user;

  const value = { user, token, login, logout, register, isAuthenticated, loading };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export default AuthContext;
