import React, { createContext, useContext, useState, useEffect } from 'react';
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

  // login can accept either (credentialsObject) or (username, password)
  const login = async (usernameOrCredentials, password) => {
    setLoading(true);
    try {
      const credentials = typeof usernameOrCredentials === 'string'
        ? { Username: usernameOrCredentials, Password: password }
        : usernameOrCredentials;

      const res = await apiService.loginAdmin(credentials);
      const data = res?.data ?? null;

      if (!data && (res.status === 204 || res.status === 205)) {
        setLoading(false);
        return { success: true };
      }

      const status = typeof data?.Status === 'number' ? data.Status : data?.Status;
      if (status === 0 || status === 'Success') {
        // Normalize user object
        const rawUser = data?.User ?? null;
        const normalized = rawUser ? { id: rawUser.Id ?? rawUser.id, username: rawUser.Username ?? rawUser.username } : null;
        // If API returns Token, persist it
        if (data?.Token) setToken(data.Token);
        setUser(normalized);
        if (normalized) localStorage.setItem('user', JSON.stringify(normalized));
        setLoading(false);
        return { success: true };
      }

      setLoading(false);
      return { success: false, message: data?.Message ?? 'Login failed' };
    } catch (err) {
      setLoading(false);
      throw err;
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
    localStorage.removeItem('user');
    localStorage.removeItem('authToken');
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
