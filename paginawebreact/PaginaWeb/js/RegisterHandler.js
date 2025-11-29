// Manejador específico para la página de registro
document.addEventListener('DOMContentLoaded', function () {
    // Verificar si ya está logueado
    if (authService.isLoggedIn()) {
        if (authService.isAdmin()) {
            console.log('Todavia en desarrollo');
        } else {
            window.location.href = 'dashboard.html';
        }
        return;
    }

    const registerForm = document.querySelector('form[action="/submit"]');

    if (registerForm) {
        registerForm.addEventListener('submit', async function (e) {
            e.preventDefault();

            const formData = {
                username: document.getElementById('usuario').value.trim(),
                password: document.getElementById('contraseña').value,
                confirmPassword: document.getElementById('confirmar-contraseña').value,
                nombreCompleto: document.getElementById('nombre-completo').value.trim(),
                email: document.getElementById('email').value.trim(),
                telefono: document.getElementById('telefono').value.trim(),
                calle: document.getElementById('direccion').value.trim(),
                numero: '',
                ciudad: '',
                estadoProvincia: '',
                codigoPostal: '',
                pais: 'México'
            };

            // Validaciones
            const validation = validateForm(formData);
            if (!validation.valid) {
                showError(validation.message);
                return;
            }

            showLoading(true);

            try {
                // Remover confirmPassword antes de enviar
                const { confirmPassword, ...userData } = formData;

                const result = await authService.register(userData);

                if (result.success) {
                    showSuccess('Cuenta creada exitosamente. Redirigiendo al login...');
                    setTimeout(() => {
                        window.location.href = 'Login.html';
                    }, 2000);
                } else {
                    showError(result.message);
                }
            } catch (error) {
                showError('Error al registrar usuario. Intenta de nuevo.');
            } finally {
                showLoading(false);
            }
        });
    }

    function validateForm(data) {
        if (!data.username || !data.password || !data.nombreCompleto || !data.email) {
            return { valid: false, message: 'Por favor completa todos los campos obligatorios' };
        }

        if (data.password.length < 6) {
            return { valid: false, message: 'La contraseña debe tener al menos 6 caracteres' };
        }

        if (data.password !== data.confirmPassword) {
            return { valid: false, message: 'Las contraseñas no coinciden' };
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(data.email)) {
            return { valid: false, message: 'El correo electrónico no es válido' };
        }

        return { valid: true };
    }

    function showError(message) {
        showMessage(message, 'error');
    }

    function showSuccess(message) {
        showMessage(message, 'success');
    }

    function showMessage(message, type) {
        let messageDiv = document.querySelector('.message');

        if (!messageDiv) {
            messageDiv = document.createElement('div');
            messageDiv.className = 'message';

            const form = document.querySelector('form');
            if (form) {
                form.insertBefore(messageDiv, form.firstChild);
            }
        }

        messageDiv.style.cssText = `
            padding: 10px;
            border-radius: 5px;
            margin: 10px 0;
            border: 1px solid;
            text-align: center;
            ${type === 'error' ?
                'background-color: #fee; color: #c33; border-color: #fcc;' :
                'background-color: #efe; color: #3c3; border-color: #cfc;'
            }
        `;

        messageDiv.textContent = message;
        messageDiv.style.display = 'block';

        if (type === 'error') {
            setTimeout(() => {
                messageDiv.style.display = 'none';
            }, 5000);
        }
    }

    function showLoading(loading) {
        const submitButton = document.querySelector('button[type="submit"]');

        if (submitButton) {
            if (loading) {
                submitButton.disabled = true;
                submitButton.textContent = 'Creando cuenta...';
            } else {
                submitButton.disabled = false;
                submitButton.textContent = 'Enviar';
            }
        }
    }
});