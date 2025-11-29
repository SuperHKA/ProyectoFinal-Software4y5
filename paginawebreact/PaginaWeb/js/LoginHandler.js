// Manejador específico para la página de login
document.addEventListener('DOMContentLoaded', function() {
    // Verificar si ya está logueado
    if (authService.isLoggedIn()) {
        if (authService.isAdmin()) {
            console.log('Todavia en desarrollo');
        } else {
            window.location.href = 'dashboard.html';
        }
        return;
    }

    const loginForm = document.querySelector('form[action="/login"]');
    
    if (loginForm) {
        loginForm.addEventListener('submit', async function(e) {
            e.preventDefault();

            const email = document.getElementById('email').value.trim();
            const password = document.getElementById('password').value;
            
            if (!email || !password) {
                showError('Por favor completa todos los campos');
                return;
            }

            showLoading(true);

            try {
                const result = await authService.login(email, password);

                if (result.success) {
                    // Redirigir según el tipo de usuario
                    if (result.user.isAdmin) {
                        console.log('Todavia en desarrollo');
                    } else {
                        window.location.href = 'dashboard.html';
                    }
                } else {
                    showError(result.message);
                }
            } catch (error) {
                showError('Error al iniciar sesión. Intenta de nuevo.');
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

        // Ocultar después de 5 segundos
        setTimeout(() => {
            errorDiv.style.display = 'none';
        }, 5000);
    }

    function showLoading(loading) {
        const submitButton = document.querySelector('button[type="submit"]');
        
        if (submitButton) {
            if (loading) {
                submitButton.disabled = true;
                submitButton.textContent = 'Iniciando sesión...';
            } else {
                submitButton.disabled = false;
                submitButton.textContent = 'Iniciar Sesión';
            }
        }
    }
});