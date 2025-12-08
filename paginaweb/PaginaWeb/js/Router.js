// Sistema de routing para manejar navegación y protección de rutas
class Router {
    constructor() {
        this.routes = {
            // Rutas públicas
            'Pagina_Principal.html': { protected: false, adminOnly: false },
            'Login.html': { protected: false, adminOnly: false },
            'Register.html': { protected: false, adminOnly: false },
            'quienes-somos.html': { protected: false, adminOnly: false },
            'contactenos.html': { protected: false, adminOnly: false },

            // Rutas protegidas para clientes
            'dashboard.html': { protected: true, adminOnly: false },
            'Profile.html': { protected: true, adminOnly: false },
            'Historial.html': { protected: true, adminOnly: false },
            'transacciones.html': { protected: true, adminOnly: false },
            'transferencias.html': { protected: true, adminOnly: false },
            'notificaciones.html': { protected: true, adminOnly: false },
            'cuentas.html': { protected: true, adminOnly: false },

            // Rutas para administradores
            'admin-dashboard.html': { protected: true, adminOnly: true }
        };

        this.init();
    }

    init() {
        // Verificar ruta actual al cargar la página
        document.addEventListener('DOMContentLoaded', () => {
            this.checkCurrentRoute();
            this.updateNavigation();
        });
    }

    // Obtener nombre de la página actual
    getCurrentPage() {
        const path = window.location.pathname;
        return path.substring(path.lastIndexOf('/') + 1) || 'Pagina_Principal.html';
    }

    // Verificar si la ruta actual es válida
    checkCurrentRoute() {
        const currentPage = this.getCurrentPage();
        const route = this.routes[currentPage];

        if (!route) {
            // Página no encontrada, redirigir a inicio
            this.redirectTo('Pagina_Principal.html');
            return;
        }

        // Verificar si requiere autenticación
        if (route.protected && !authService.isLoggedIn()) {
            this.redirectTo('Login.html');
            return;
        }

        // Verificar si requiere permisos de admin
        if (route.adminOnly && !authService.isAdmin()) {
            this.redirectTo('dashboard.html');
            return;
        }

        // Si el usuario está logueado y trata de acceder a Login/Register, redirigir al dashboard
        if ((currentPage === 'Login.html' || currentPage === 'Register.html') && authService.isLoggedIn()) {
            if (authService.isAdmin()) {
                this.redirectTo('admin-dashboard.html');
            } else {
                this.redirectTo('dashboard.html');
            }
            return;
        }
    }

    // Redirigir a una página
    redirectTo(page) {
        window.location.href = page;
    }

    // Navegar a una página con verificación
    navigateTo(page) {
        const route = this.routes[page];

        if (!route) {
            console.error('Ruta no encontrada:', page);
            return false;
        }

        // Verificar protección de ruta
        if (route.protected && !authService.isLoggedIn()) {
            this.redirectTo('Login.html');
            return false;
        }

        if (route.adminOnly && !authService.isAdmin()) {
            this.redirectTo('dashboard.html');
            return false;
        }

        this.redirectTo(page);
        return true;
    }

    // Actualizar navegación basada en estado de autenticación
    updateNavigation() {
        const isLoggedIn = authService.isLoggedIn();
        const isAdmin = authService.isAdmin();

        // Actualizar botones de cuenta
        this.updateAccountButtons(isLoggedIn, isAdmin);

        // Actualizar información de usuario en header (si existe)
        this.updateUserInfo(isLoggedIn);
    }

    // Actualizar botones de cuenta
    updateAccountButtons(isLoggedIn, isAdmin) {
        const cuentaButton = document.querySelector('.cuenta');
        const cuadroOpciones = document.getElementById('cuadro-opciones');

        if (isLoggedIn) {
            // Usuario logueado
            if (cuentaButton) {
                cuentaButton.querySelector('.fount_cuenta').textContent = authService.currentUser.username;
            }

            if (cuadroOpciones) {
                cuadroOpciones.innerHTML = `
                    <button type="button" class="opcion-btn1" onclick="router.navigateTo('${isAdmin ? 'admin-dashboard.html' : 'dashboard.html'}')">Dashboard</button>
                    <button type="button" class="opcion-btn2" onclick="router.navigateTo('Profile.html')">Perfil</button>
                    <button type="button" class="opcion-btn1" onclick="authService.logout()">Cerrar Sesión</button>
                `;
            }
        } else {
            // Usuario no logueado
            if (cuentaButton) {
                cuentaButton.querySelector('.fount_cuenta').textContent = 'Cuenta';
            }

            if (cuadroOpciones) {
                cuadroOpciones.innerHTML = `
                    <button type="button" class="opcion-btn1" onclick="router.navigateTo('Login.html')">Ingresar</button>
                    <button type="button" class="opcion-btn2" onclick="router.navigateTo('Register.html')">Registrarse</button>
                `;
            }
        }
    }

    // Actualizar información de usuario
    updateUserInfo(isLoggedIn) {
        const userInfo = document.querySelector('.user-info');

        if (userInfo && isLoggedIn) {
            const user = authService.currentUser;
            const avatar = userInfo.querySelector('.user-avatar');
            const name = userInfo.querySelector('span');

            if (avatar) {
                avatar.textContent = user.username.substring(0, 2).toUpperCase();
            }
            if (name) {
                name.textContent = user.username;
            }
        }
    }
}

// Instancia global del router
const router = new Router();