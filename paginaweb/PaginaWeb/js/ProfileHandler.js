document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    try {
        const data = await authService.getClienteInfo();
        // Nueva estructura: { cliente: {...}, cuentas: [...] }
        const cliente = data.cliente || data;
        const cuentas = data.cuentas || [];

        if (!cliente) {
            console.warn('No se encontró información del cliente en la respuesta:', data);
            return;
        }

        // Actualizar cabecera (user-info)
        const avatarEl = document.querySelector('.user-avatar');
        const userNameSpan = document.querySelector('.user-info span');
        if (cliente.nombreCompleto) {
            if (userNameSpan) userNameSpan.textContent = cliente.nombreCompleto;
            if (avatarEl) {
                const initials = cliente.nombreCompleto.split(' ').map(n => n[0]).slice(0,2).join('').toUpperCase();
                avatarEl.textContent = initials;
            }
        } else if (cliente.username) {
            if (userNameSpan) userNameSpan.textContent = cliente.username;
            if (avatarEl) avatarEl.textContent = cliente.username.slice(0,2).toUpperCase();
        }

        // Profile header
        const profileName = document.querySelector('.profile-name');
        const profileId = document.querySelector('.profile-id');
        const profileAvatar = document.querySelector('.profile-avatar');

        if (profileName) profileName.textContent = cliente.nombreCompleto || cliente.username || profileName.textContent;
        if (profileId) profileId.textContent = `ID de Cliente: #${cliente.id || cliente.clienteId || (cliente.idCliente || '')}`;
        if (profileAvatar) profileAvatar.textContent = (cliente.nombreCompleto || cliente.username || 'U').split(' ').map(n=>n[0]).slice(0,2).join('').toUpperCase();

        // Llenar la sección de información personal (basado en campos comunes)
        const infoValues = document.querySelectorAll('.section-card .info-value');
        // Asumimos orden: Nombre completo, Correo, Teléfono, Fecha de Nacimiento, Dirección
        if (infoValues && infoValues.length >= 5) {
            infoValues[0].textContent = cliente.nombreCompleto || '';
            infoValues[1].textContent = cliente.email || cliente.correo || '';
            infoValues[2].textContent = cliente.telefono || '';
            infoValues[3].textContent = cliente.fechaNacimiento ? new Date(cliente.fechaNacimiento).toLocaleDateString('es-MX') : '';
            const direccion = cliente.direccion || (cliente.calle ? `${cliente.calle} ${cliente.numero || ''}, ${cliente.ciudad || ''}` : '');
            infoValues[4].textContent = direccion;
        } else {
            // Si no existen .info-value predefinidos, buscar elementos por label
            const labels = document.querySelectorAll('.info-row');
            labels.forEach(row => {
                const label = row.querySelector('.info-label');
                const value = row.querySelector('.info-value');
                if (!label || !value) return;
                const key = label.textContent.trim().toLowerCase();
                if (key.includes('nombre')) value.textContent = cliente.nombreCompleto || '';
                if (key.includes('correo')) value.textContent = cliente.email || cliente.correo || '';
                if (key.includes('teléfono') || key.includes('telefono')) value.textContent = cliente.telefono || '';
                if (key.includes('dirección')) value.textContent = cliente.direccion || direccion || '';
            });
        }

    } catch (error) {
        console.error('Error al cargar perfil:', error);
        // Si falla, redirigir a login por seguridad
        // window.location.href = 'Login.html';
    }
});
