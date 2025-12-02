// Servicio de autenticación mejorado para manejar login y registro
class AuthService {
    constructor() {
        this.baseURL = 'http://localhost:5202/api';
        this.currentUser = this.getCurrentUser();
    }

    // Obtener usuario actual del localStorage
    getCurrentUser() {
        try {
            const userData = localStorage.getItem('user');
            const token = localStorage.getItem('authToken');
            
            if (userData && token) {
                return JSON.parse(userData);
            }
            return null;
        } catch (error) {
            console.error('Error getting current user:', error);
            return null;
        }
    }

    // Guardar usuario en localStorage
    saveUser(userData, token) {
        try {
            localStorage.setItem('user', JSON.stringify(userData));
            localStorage.setItem('authToken', token);
            this.currentUser = userData;
        } catch (error) {
            console.error('Error saving user:', error);
        }
    }

    // Limpiar datos de usuario
    clearUser() {
        localStorage.removeItem('user');
        localStorage.removeItem('authToken');
        this.currentUser = null;
    }

    // Login
    async login(username, password) {
        try {
            const response = await fetch(`${this.baseURL}/banco/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ username, password }),
            });

            const data = await response.json();

            // EstatusLogin.Success = 0
            if (response.ok && data.status === 0) {
                const userData = {
                    id: data.user.id,
                    username: data.user.username,
                    clienteId: data.user.clienteId,
                    isAdmin: data.user.isAdmin || false
                };

                // Generar un token simple
                const token = btoa(`${username}:${Date.now()}`);
                this.saveUser(userData, token);

                return {
                    success: true,
                    user: userData,
                    message: data.message || 'Login exitoso'
                };
            } else {
                return {
                    success: false,
                    message: data.message || 'Credenciales incorrectas'
                };
            }
        } catch (error) {
            console.error('Login error:', error);
            return {
                success: false,
                message: 'Error de conexión. Intenta de nuevo.'
            };
        }
    }

    // Registro
    async register(userData) {
        try {
            // Preparar datos según el DTO esperado por la API
            const clienteData = {
                username: userData.username,
                password: userData.password,
                nombreCompleto: userData.nombreCompleto,
                email: userData.email,
                telefono: userData.telefono,
                calle: userData.calle || '',
                numero: userData.numero || '',
                ciudad: userData.ciudad || '',
                estadoProvincia: userData.estadoProvincia || '',
                codigoPostal: userData.codigoPostal || '',
                pais: userData.pais || 'México'
            };

            const response = await fetch(`${this.baseURL}/banco/clientes`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(clienteData),
            });

            if (response.ok) {
                return {
                    success: true,
                    message: 'Usuario registrado exitosamente'
                };
            } else {
                const data = await response.json();
                return {
                    success: false,
                    message: data.message || 'Error al registrar usuario'
                };
            }
        } catch (error) {
            console.error('Register error:', error);
            return {
                success: false,
                message: 'Error de conexión. Intenta de nuevo.'
            };
        }
    }

    // Obtener cuentas del usuario actual
    async getCuentas() {
        if (!this.currentUser || !this.currentUser.username) {
            throw new Error('No hay usuario logueado');
        }

        try {
            const response = await fetch(
                `${this.baseURL}/banco/clientes/por-usuario?username=${encodeURIComponent(this.currentUser.username)}`,
                {
                    headers: {
                        'Authorization': `Bearer ${this.getAuthToken()}`
                    }
                }
            );

            if (response.ok) {
                return await response.json();
            } else {
                throw new Error('Error al obtener cuentas');
            }
        } catch (error) {
            console.error('Error getting cuentas:', error);
            throw error;
        }
    }

    // Obtener información completa del cliente (perfil)
    async getClienteInfo() {
        if (!this.currentUser || !this.currentUser.username) {
            throw new Error('No hay usuario logueado');
        }

        try {
            const response = await fetch(
                `${this.baseURL}/banco/clientes/por-usuario?username=${encodeURIComponent(this.currentUser.username)}`,
                {
                    headers: {
                        'Authorization': `Bearer ${this.getAuthToken()}`
                    }
                }
            );

            if (response.ok) {
                const data = await response.json();
                // Devolver el objeto tal cual (la estructura depende de la API)
                return data;
            } else {
                throw new Error('Error al obtener información del cliente');
            }
        } catch (error) {
            console.error('Error getting cliente info:', error);
            throw error;
        }
    }

    // Realizar transferencia
    async transferir(transferData) {
        try {
            const response = await fetch(`${this.baseURL}/banco/transfer/internal`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.getAuthToken()}`
                },
                body: JSON.stringify(transferData),
            });

            const data = await response.json();
            
            if (response.ok) {
                return {
                    success: true,
                    data: data,
                    message: 'Transferencia realizada exitosamente'
                };
            } else {
                return {
                    success: false,
                    message: data.message || 'Error al realizar transferencia'
                };
            }
        } catch (error) {
            console.error('Transfer error:', error);
            return {
                success: false,
                message: 'Error de conexión. Intenta de nuevo.'
            };
        }
    }

    // Logout
    logout() {
        this.clearUser();
        window.location.href = 'Pagina_Principal.html';
    }

    // Verificar si está logueado
    isLoggedIn() {
        return this.currentUser !== null && localStorage.getItem('authToken') !== null;
    }

    // Verificar si es admin
    isAdmin() {
        return this.currentUser && (this.currentUser.isAdmin === true);
    }

    // Obtener token de autorización
    getAuthToken() {
        return localStorage.getItem('authToken');
    }

    // Verificar conexión con la API
    async checkConnection() {
        try {
            const response = await fetch(`${this.baseURL}/banco/health`);
            return response.ok;
        } catch (error) {
            return false;
        }
    }
}

// Instancia global del servicio de autenticación
const authService = new AuthService();
