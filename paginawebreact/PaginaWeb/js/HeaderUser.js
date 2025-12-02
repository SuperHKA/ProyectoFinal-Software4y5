document.addEventListener('DOMContentLoaded', async function() {
    // Actualiza el header en todas las páginas para mostrar nombre e iniciales si hay sesión
    const userSpan = document.querySelector('.user-info span');
    const avatar = document.querySelector('.user-avatar');

    if (!userSpan && !avatar) return; // nada que actualizar

    if (!authService || !authService.isLoggedIn()) {
        // Mostrar opciones de login/registro si existe
        if (userSpan) userSpan.textContent = 'Invitado';
        if (avatar) avatar.textContent = 'GT';
        return;
    }

    try {
        const data = await authService.getClienteInfo();
        const cliente = data.cliente || data || (Array.isArray(data) ? data[0] : null);
        const displayName = (cliente && (cliente.nombreCompleto || cliente.username)) || (authService.getCurrentUser() && authService.getCurrentUser().username) || 'Usuario';

        if (userSpan) userSpan.textContent = displayName;
        if (avatar) {
            const initials = (cliente && cliente.nombreCompleto) ? cliente.nombreCompleto.split(' ').map(n=>n[0]).slice(0,2).join('').toUpperCase() : (displayName.slice(0,2).toUpperCase());
            avatar.textContent = initials;
        }
    } catch (err) {
        console.warn('No se pudo actualizar header desde API:', err);
    }
});
