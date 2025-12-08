document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    // Configurar eventos
    setupProfileEvents();
    
    // Cargar datos del usuario
    await loadUserProfile();
    
    // Configurar validaciones
    setupValidations();
    
    // Configurar cambio de contraseña
    setupPasswordChange();
});

let originalUserData = {};
let hasUnsavedChanges = false;

async function loadUserProfile() {
    try {
        showLoading(true);
        
        const data = await authService.getClienteInfo();
        const cliente = data.cliente || data;
        
        if (cliente) {
            originalUserData = { ...cliente };
            populateProfileForm(cliente);
            updateProfileHeader(cliente);
        } else {
            throw new Error('No se pudo cargar la información del usuario');
        }
        
    } catch (error) {
        console.error('Error loading profile:', error);
        showError('Error al cargar el perfil');
        showFallbackProfile();
    } finally {
        showLoading(false);
    }
}

function populateProfileForm(cliente) {
    // Información personal
    setFieldValue('profileName', cliente.nombreCompleto || cliente.nombre || '');
    setFieldValue('profileEmail', cliente.email || '');
    setFieldValue('profilePhone', cliente.telefono || '');
    setFieldValue('profileUsername', cliente.username || '');
    setFieldValue('profileBirthDate', formatDateForInput(cliente.fechaNacimiento));
    
    // Dirección
    setFieldValue('profileAddress', cliente.direccion || '');
    setFieldValue('profileCity', cliente.ciudad || '');
    setFieldValue('profileState', cliente.estado || '');
    setFieldValue('profileCountry', cliente.pais || 'Panamá');
    setFieldValue('profilePostalCode', cliente.codigoPostal || '');
    
    // Información adicional
    setFieldValue('profileOccupation', cliente.ocupacion || '');
    setFieldValue('profileIncome', cliente.ingresos || '');
    setFieldValue('profileDocument', cliente.documento || '');
    
    // Configuraciones
    updateSecuritySettings(cliente);
}

function setFieldValue(fieldId, value) {
    const field = document.getElementById(fieldId);
    if (field) {
        field.value = value || '';
    }
}

function updateProfileHeader(cliente) {
    // Actualizar avatar y nombre en el header
    const avatar = document.querySelector('.profile-avatar');
    const profileName = document.querySelector('.profile-name');
    const profileId = document.querySelector('.profile-id');
    
    if (avatar && cliente) {
        const fullName = cliente.nombreCompleto || cliente.nombre || cliente.username;
        const initials = fullName.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
        avatar.textContent = initials;
    }
    
    if (profileName) {
        profileName.textContent = cliente.nombreCompleto || cliente.nombre || cliente.username;
    }
    
    if (profileId) {
        profileId.textContent = `ID de Cliente: #${cliente.clienteId || 'N/A'}`;
    }
    
    // Actualizar badge de verificación
    updateVerificationBadge(cliente);
    
    // Actualizar header de usuario global
    updateGlobalUserInfo(cliente);
}

function updateVerificationBadge(cliente) {
    const statusBadge = document.querySelector('.status-badge');
    if (!statusBadge) return;
    
    const isVerified = cliente.verificado !== false;
    statusBadge.className = `status-badge ${isVerified ? 'verified' : 'unverified'}`;
    statusBadge.innerHTML = isVerified ? 
        '<i class="fas fa-check-circle"></i> Cuenta Verificada' : 
        '<i class="fas fa-exclamation-triangle"></i> Pendiente Verificación';
}

function updateGlobalUserInfo(cliente) {
    // Actualizar información en header global y sidebar
    const userNameElements = document.querySelectorAll('.user-info span');
    userNameElements.forEach(el => {
        el.textContent = cliente.nombreCompleto || cliente.nombre || cliente.username;
    });

    const userAvatars = document.querySelectorAll('.user-avatar');
    userAvatars.forEach(avatar => {
        const fullName = cliente.nombreCompleto || cliente.nombre || cliente.username;
        const initials = fullName.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
        avatar.textContent = initials;
    });
}

function setupProfileEvents() {
    // Botón de edición
    const editBtn = document.getElementById('editProfileBtn');
    if (editBtn) {
        editBtn.addEventListener('click', toggleEditMode);
    }
    
    // Configuración de notificaciones
    setupNotificationSettings();
    
    // Detectar cambios en formularios
    setupChangeDetection();
}

function setupPasswordChange() {
    const changePasswordBtn = document.getElementById('changePasswordBtn');
    if (changePasswordBtn) {
        changePasswordBtn.addEventListener('click', showPasswordChangeModal);
    }
}

function showPasswordChangeModal() {
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.innerHTML = `
        <div class="modal-content">
            <div class="modal-header">
                <h3>Cambiar Contraseña</h3>
                <button class="close-modal" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <form id="passwordChangeForm">
                    <div class="form-group">
                        <label for="currentPassword">Contraseña Actual *</label>
                        <div class="password-input">
                            <input type="password" id="currentPassword" required>
                            <button type="button" class="toggle-password" onclick="togglePasswordVisibility('currentPassword')">
                                <i class="fas fa-eye"></i>
                            </button>
                        </div>
                        <div class="validation-message"></div>
                    </div>
                    
                    <div class="form-group">
                        <label for="newPassword">Nueva Contraseña *</label>
                        <div class="password-input">
                            <input type="password" id="newPassword" required>
                            <button type="button" class="toggle-password" onclick="togglePasswordVisibility('newPassword')">
                                <i class="fas fa-eye"></i>
                            </button>
                        </div>
                        <div class="validation-message"></div>
                        <div class="password-strength">
                            <div class="strength-bar">
                                <div class="strength-fill"></div>
                            </div>
                            <span class="strength-text">Ingresa una contraseña</span>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label for="confirmPassword">Confirmar Nueva Contraseña *</label>
                        <div class="password-input">
                            <input type="password" id="confirmPassword" required>
                            <button type="button" class="toggle-password" onclick="togglePasswordVisibility('confirmPassword')">
                                <i class="fas fa-eye"></i>
                            </button>
                        </div>
                        <div class="validation-message"></div>
                    </div>
                    
                    <div class="password-requirements">
                        <h4>Requisitos de contraseña:</h4>
                        <ul>
                            <li id="req-length" class="requirement">
                                <i class="fas fa-times"></i> Mínimo 8 caracteres
                            </li>
                            <li id="req-uppercase" class="requirement">
                                <i class="fas fa-times"></i> Al menos una mayúscula
                            </li>
                            <li id="req-lowercase" class="requirement">
                                <i class="fas fa-times"></i> Al menos una minúscula
                            </li>
                            <li id="req-number" class="requirement">
                                <i class="fas fa-times"></i> Al menos un número
                            </li>
                            <li id="req-special" class="requirement">
                                <i class="fas fa-times"></i> Al menos un carácter especial
                            </li>
                        </ul>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn-secondary" onclick="closeModal()">Cancelar</button>
                <button type="button" class="btn-primary" onclick="submitPasswordChange()">
                    <i class="fas fa-key"></i> Cambiar Contraseña
                </button>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    modal.style.display = 'flex';
    
    // Configurar validaciones de contraseña
    setupPasswordValidations();
}

function setupPasswordValidations() {
    const newPasswordField = document.getElementById('newPassword');
    const confirmPasswordField = document.getElementById('confirmPassword');
    
    if (newPasswordField) {
        newPasswordField.addEventListener('input', function() {
            validatePasswordStrength(this.value);
        });
    }
    
    if (confirmPasswordField) {
        confirmPasswordField.addEventListener('input', function() {
            validatePasswordMatch();
        });
    }
}

function validatePasswordStrength(password) {
    const requirements = {
        length: password.length >= 8,
        uppercase: /[A-Z]/.test(password),
        lowercase: /[a-z]/.test(password),
        number: /\d/.test(password),
        special: /[!@#$%^&*(),.?":{}|<>]/.test(password)
    };
    
    // Actualizar indicadores visuales
    Object.keys(requirements).forEach(req => {
        const element = document.getElementById(`req-${req}`);
        if (element) {
            const icon = element.querySelector('i');
            if (requirements[req]) {
                element.classList.add('valid');
                icon.className = 'fas fa-check';
            } else {
                element.classList.remove('valid');
                icon.className = 'fas fa-times';
            }
        }
    });
    
    // Calcular fortaleza
    const strength = Object.values(requirements).filter(Boolean).length;
    updatePasswordStrengthBar(strength);
    
    return Object.values(requirements).every(Boolean);
}

function updatePasswordStrengthBar(strength) {
    const strengthFill = document.querySelector('.strength-fill');
    const strengthText = document.querySelector('.strength-text');
    
    if (!strengthFill || !strengthText) return;
    
    const percentage = (strength / 5) * 100;
    strengthFill.style.width = `${percentage}%`;
    
    const strengthLevels = {
        0: { text: 'Muy débil', color: '#dc3545', class: 'very-weak' },
        1: { text: 'Débil', color: '#fd7e14', class: 'weak' },
        2: { text: 'Regular', color: '#ffc107', class: 'fair' },
        3: { text: 'Buena', color: '#20c997', class: 'good' },
        4: { text: 'Fuerte', color: '#28a745', class: 'strong' },
        5: { text: 'Muy fuerte', color: '#198754', class: 'very-strong' }
    };
    
    const level = strengthLevels[strength] || strengthLevels[0];
    strengthFill.style.backgroundColor = level.color;
    strengthText.textContent = level.text;
    strengthText.className = `strength-text ${level.class}`;
}

function validatePasswordMatch() {
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    
    const confirmField = document.getElementById('confirmPassword');
    const validationDiv = confirmField?.parentNode?.parentNode?.querySelector('.validation-message');
    
    if (confirmPassword && newPassword !== confirmPassword) {
        confirmField?.classList.add('invalid');
        if (validationDiv) {
            validationDiv.textContent = 'Las contraseñas no coinciden';
            validationDiv.className = 'validation-message error';
        }
        return false;
    } else if (confirmPassword) {
        confirmField?.classList.remove('invalid');
        confirmField?.classList.add('valid');
        if (validationDiv) {
            validationDiv.textContent = 'Las contraseñas coinciden';
            validationDiv.className = 'validation-message success';
        }
        return true;
    }
    
    return false;
}

async function submitPasswordChange() {
    const currentPassword = document.getElementById('currentPassword').value;
    const newPassword = document.getElementById('newPassword').value;
    const confirmPassword = document.getElementById('confirmPassword').value;
    
    // Validaciones
    if (!currentPassword) {
        showError('Contraseña actual requerida');
        return;
    }
    
    if (!validatePasswordStrength(newPassword)) {
        showError('La nueva contraseña no cumple con los requisitos');
        return;
    }
    
    if (!validatePasswordMatch()) {
        return;
    }
    
    try {
        showLoading(true);
        
        const result = await changeUserPassword(currentPassword, newPassword);
        
        if (result.success) {
            closeModal();
            showSuccess('Contraseña cambiada exitosamente');
            
            // Opcional: cerrar sesión para re-autenticar
            setTimeout(() => {
                if (confirm('Contraseña cambiada. ¿Deseas cerrar sesión para mayor seguridad?')) {
                    authService.logout();
                }
            }, 2000);
        } else {
            showError(result.message);
        }
        
    } catch (error) {
        console.error('Error changing password:', error);
        showError('Error al cambiar contraseña');
    } finally {
        showLoading(false);
    }
}

async function changeUserPassword(currentPassword, newPassword) {
    try {
        const user = authService.getCurrentUser();
        if (!user) {
            throw new Error('Usuario no autenticado');
        }

        const response = await fetch(`${authService.baseURL}/banco/usuarios/cambiar-contrasena`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authService.getAuthToken()}`
            },
            body: JSON.stringify({
                usuarioId: user.id,
                contrasenaActual: currentPassword,
                nuevaContrasena: newPassword
            }),
        });

        if (response.ok) {
            return { success: true };
        } else {
            const data = await response.json();
            return { success: false, message: data.message || 'Error al cambiar contraseña' };
        }
        
    } catch (error) {
        throw error;
    }
}

function setupNotificationSettings() {
    // Configurar toggles de notificaciones
    const notificationToggles = document.querySelectorAll('.notification-toggle');
    
    notificationToggles.forEach(toggle => {
        toggle.addEventListener('change', function() {
            saveNotificationPreference(this.dataset.type, this.checked);
        });
    });
}

function saveNotificationPreference(type, enabled) {
    // Guardar preferencia en localStorage o enviar a API
    const preferences = getNotificationPreferences();
    preferences[type] = enabled;
    localStorage.setItem('notification_preferences', JSON.stringify(preferences));
    
    showToast(`Notificaciones ${enabled ? 'activadas' : 'desactivadas'} para ${type}`);
}

function getNotificationPreferences() {
    try {
        return JSON.parse(localStorage.getItem('notification_preferences')) || {};
    } catch {
        return {};
    }
}

function setupChangeDetection() {
    // Detectar cambios en los campos del formulario
    const formFields = document.querySelectorAll('input, select, textarea');
    
    formFields.forEach(field => {
        field.addEventListener('change', () => {
            hasUnsavedChanges = true;
        });
    });
    
    // Advertir antes de cerrar si hay cambios sin guardar
    window.addEventListener('beforeunload', function(e) {
        if (hasUnsavedChanges) {
            e.preventDefault();
            e.returnValue = '';
            return 'Tienes cambios sin guardar. ¿Estás seguro de que quieres salir?';
        }
    });
}

function setupValidations() {
    // Las validaciones se configuran cuando se crea el formulario de edición
    console.log('Validations setup completed');
}

function updateSecuritySettings() {
    // Configurar ajustes de seguridad
    console.log('Security settings updated');
}

function toggleEditMode() {
    // Por simplicidad, redirigir a una página de edición o mostrar un modal
    showEditModal();
}

function showEditModal() {
    alert('Función de edición disponible próximamente. Use el modal de cambio de contraseña como ejemplo.');
}

// Funciones auxiliares
function formatDateForInput(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toISOString().split('T')[0];
}

function showFallbackProfile() {
    // Mostrar datos de ejemplo si la API falla
    const fallbackData = {
        nombreCompleto: 'Usuario Demo',
        email: 'usuario@demo.com',
        telefono: '+507 6000-0000',
        username: 'usuario_demo',
        clienteId: '12345'
    };
    
    originalUserData = fallbackData;
    populateProfileForm(fallbackData);
    updateProfileHeader(fallbackData);
}

function togglePasswordVisibility(fieldId) {
    const field = document.getElementById(fieldId);
    const button = field?.parentNode?.querySelector('.toggle-password i');
    
    if (field && button) {
        if (field.type === 'password') {
            field.type = 'text';
            button.className = 'fas fa-eye-slash';
        } else {
            field.type = 'password';
            button.className = 'fas fa-eye';
        }
    }
}

function showToast(message, type = 'success') {
    // Simple toast notification
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.textContent = message;
    toast.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#28a745' : '#dc3545'};
        color: white;
        padding: 12px 20px;
        border-radius: 6px;
        z-index: 10000;
        animation: slideInRight 0.3s ease;
    `;
    
    document.body.appendChild(toast);
    
    setTimeout(() => {
        toast.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

function showLoading(show) {
    console.log(show ? 'Loading...' : 'Loading complete');
}

function showError(message) {
    alert('Error: ' + message);
}

function showSuccess(message) {
    alert('Éxito: ' + message);
}

function closeModal() {
    const modal = document.querySelector('.modal');
    if (modal) {
        modal.remove();
    }
}

// Eventos globales
window.togglePasswordVisibility = togglePasswordVisibility;
window.closeModal = closeModal;
window.submitPasswordChange = submitPasswordChange;
