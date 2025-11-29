// Servicio de autenticación para manejar login y registro
class AuthService {
    constructor() {
        this.baseURL = 'http://localhost:5202';
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
            const response = await fetch(`${this.baseURL}/banco/login-admin`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ username, password }),
            });

            const data = await response.json();

            if (response.ok && data.success) {
                const userData = {
                    username: username,
                    isAdmin: data.isAdmin || false,
                    token: data.token
                };

                this.saveUser(userData, data.token);

                return {
                    success: true,
                    user: userData,
                    message: data.message
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
            const response = await fetch(`${this.baseURL}/banco/clientes`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(userData),
            });

            const data = await response.json();

            if (response.ok) {
                return {
                    success: true,
                    message: 'Usuario registrado exitosamente'
                };
            } else {
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
}

// Instancia global del servicio de autenticación
const authService = new AuthService();