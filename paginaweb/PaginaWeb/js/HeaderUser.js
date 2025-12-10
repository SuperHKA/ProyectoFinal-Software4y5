// Normalizar datos del cliente desde la API
function normalizeClienteData(cliente) {
    if (!cliente) return null;
    
    // Extraer campos de dirección
    const calle = cliente['Calle'] || cliente.calle || '';
    const numero = cliente['Numero'] || cliente.numero || '';
    const ciudad = cliente['Ciudad'] || cliente.ciudad || '';
    const estado = cliente['Estado/Provincia'] || cliente.estado || cliente.estado_provincia || '';
    const pais = cliente['Pais'] || cliente.pais || '';
    
    // Concatenar dirección completa
    const direccionPartes = [];
    if (pais) direccionPartes.push(pais);
    if (estado) direccionPartes.push(estado);
    if (ciudad) direccionPartes.push(ciudad);
    if (calle) direccionPartes.push(calle);
    if (numero) direccionPartes.push('#' + numero);
    const direccionCompleta = direccionPartes.length > 0 ? direccionPartes.join(', ') : 'N/A';
    
    return {
        clienteId: cliente['ID Cliente'] || cliente.clienteId || cliente.cliente_id,
        nombre: cliente['Nombre Completo'] || cliente['Nombre'] || cliente.nombre,
        nombreCompleto: cliente['Nombre Completo'] || cliente['Nombre'] || cliente.nombre,
        email: cliente['Email'] || cliente.email,
        telefono: cliente['Telefono'] || cliente.telefono,
        username: cliente['Usuario'] || cliente.username,
        direccion: direccionCompleta,
        ...cliente
    };
}

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
        // Nueva estructura: { cliente: {...}, cuentas: [...] }
        const clienteRaw = data.cliente || data;
        const cliente = normalizeClienteData(clienteRaw);
        const displayName = (cliente && (cliente.nombreCompleto || cliente.nombre || cliente.username)) || (authService.getCurrentUser() && authService.getCurrentUser().username) || 'Usuario';

        if (userSpan) userSpan.textContent = displayName;
        if (avatar) {
            const fullName = cliente ? (cliente.nombreCompleto || cliente.nombre || displayName) : displayName;
            const initials = fullName.split(' ').map(n=>n[0]).slice(0,2).join('').toUpperCase();
            avatar.textContent = initials;
        }
    } catch (err) {
        console.warn('No se pudo actualizar header desde API:', err);
    }
});
