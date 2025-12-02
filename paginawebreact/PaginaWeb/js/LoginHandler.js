// Manejador espec�fico para la p�gina de login
document.addEventListener('DOMContentLoaded', function() {
    // Verificar si ya est� logueado
    if (authService.isLoggedIn()) {
        if (authService.isAdmin()) {
            console.log('Todavia en desarrollo');
        } else {
            window.location.href = 'dashboard.html';
        }
        return;
    }

    const loginForm = document.getElementById('login-form') || document.querySelector('form');
    
    if (loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const username = document.getElementById('username') ? document.getElementById('username').value.trim() : document.getElementById('username') && document.getElementById('username').value.trim();
            const password = document.getElementById('password').value;
            
            if (!username || !password) {
                showError('Por favor completa todos los campos');
                return;
            }

            showLoading(true);

            try {
                const result = await authService.login(username, password);

                if (result.success) {
                    // Redirigir seg�n el tipo de usuario
                    if (result.user && result.user.isAdmin) {
                        console.log('Todavia en desarrollo');
                    } else {
                        window.location.href = 'dashboard.html';
                    }
                } else {
                    showError(result.message);
                }
            } catch (error) {
                showError('Error al iniciar sesi�n. Intenta de nuevo.');
            } finally {
                showLoading(false);
            }
        });
    }

    function showError(message) {
        // Buscar contenedor de errores existente o crearlo
        let errorDiv = document.querySelector('.error-message');
        
        if (!errorDiv) {
            errorDiv = document.createElement('div');
            errorDiv.className = 'error-message';
            errorDiv.style.cssText = `
                background-color: #fee;
                color: #c33;
                padding: 10px;
                border-radius: 5px;
                margin: 10px 0;
                border: 1px solid #fcc;
                text-align: center;
            `;
            
            const form = document.querySelector('form');
            if (form) {
                form.insertBefore(errorDiv, form.firstChild);
            }
        }
        
        errorDiv.textContent = message;
        errorDiv.style.display = 'block';

        // Ocultar despu�s de 5 segundos
        setTimeout(() => {
            errorDiv.style.display = 'none';
        }, 5000);
    }

    function showLoading(loading) {
        const submitButton = document.querySelector('button[type="submit"]');
        
        if (submitButton) {
            if (loading) {
                submitButton.disabled = true;
                submitButton.textContent = 'Iniciando sesi�n...';
            } else {
                submitButton.disabled = false;
                submitButton.textContent = 'Iniciar Sesi�n';
            }
        }
    }
});