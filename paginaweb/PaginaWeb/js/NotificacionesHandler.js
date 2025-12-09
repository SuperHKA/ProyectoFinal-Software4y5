// Manejador de notificaciones del cliente
document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    // Actualizar información del usuario en el header
    await updateUserInfo();

    // Cargar notificaciones del cliente
    await loadNotificaciones();

    // Configurar actualización periódica
    setupPeriodicUpdate();
});

// Normalizar datos del cliente desde la API
function normalizeClienteData(cliente) {
    if (!cliente) return null;
    
    return {
        clienteId: cliente['ID Cliente'] || cliente.clienteId || cliente.cliente_id,
        nombre: cliente['Nombre'] || cliente.nombre || cliente['Nombre Completo'],
        nombreCompleto: cliente['Nombre'] || cliente['Nombre Completo'] || cliente.nombre,
        email: cliente['Email'] || cliente.email,
        telefono: cliente['Teléfono'] || cliente.telefono,
        username: cliente['Usuario'] || cliente.username || cliente['Nombre de Usuario'],
        ...cliente
    };
}

// Normalizar datos de notificación desde la API
function normalizeNotificacionData(notif) {
    if (!notif) return null;
    
    return {
        notificacionId: notif['ID Notificación'] || notif.notificacionId || notif.notificacion_id,
        tipo: notif['Tipo'] || notif.tipo,
        asunto: notif['Asunto'] || notif.asunto,
        contenido: notif['Contenido'] || notif.contenido,
        fechaEnvio: notif['Fecha de Envío'] || notif['Fecha Envío'] || notif.fechaEnvio || notif.fecha_envio,
        estado: notif['Estado'] || notif.estado,
        ...notif
    };
}

async function updateUserInfo() {
    try {
        const data = await authService.getClienteInfo();
        const clienteRaw = data.cliente || data;
        const cliente = normalizeClienteData(clienteRaw);
        const displayName = (cliente && (cliente.nombreCompleto || cliente.nombre || cliente.username)) || 
                          (authService.getCurrentUser() && authService.getCurrentUser().username) || 'Usuario';

        const userNameElements = document.querySelectorAll('.user-info span');
        userNameElements.forEach(el => {
            el.textContent = displayName;
        });

        const avatar = document.querySelector('.user-avatar');
        if (avatar && cliente) {
            const fullName = cliente.nombreCompleto || cliente.nombre || displayName;
            const initials = fullName ? fullName.split(' ').map(n => n[0]).slice(0,2).join('').toUpperCase() : 'U';
            avatar.textContent = initials;
        }
    } catch (error) {
        console.warn('No se pudo obtener información del usuario:', error);
    }
}

async function loadNotificaciones() {
    try {
        showLoading(true);

        const user = authService.getCurrentUser();
        if (!user) {
            throw new Error('Usuario no autenticado');
        }

        // Obtener el clienteId desde la API
        let clienteId = user.clienteId;
        if (!clienteId) {
            const data = await authService.getClienteInfo();
            const cliente = normalizeClienteData(data.cliente || data);
            clienteId = cliente.clienteId;
        }

        if (!clienteId) {
            console.warn('No se pudo obtener el ID del cliente');
            displayFallbackNotificaciones();
            return;
        }

        // Obtener notificaciones del cliente desde la API
        const response = await fetch(
            `http://localhost:5202/api/banco/clientes/${clienteId}/notificaciones?dias=30`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const notificaciones = await response.json();
            displayNotificaciones(notificaciones);
        } else {
            console.error('Error al obtener notificaciones:', response.statusText);
            displayFallbackNotificaciones();
        }
    } catch (error) {
        console.error('Error loading notificaciones:', error);
        displayFallbackNotificaciones();
    } finally {
        showLoading(false);
    }
}

function displayNotificaciones(notificaciones) {
    const container = document.querySelector('.notificaciones-container');
    if (!container) return;

    if (!notificaciones || notificaciones.length === 0) {
        container.innerHTML = `
            <div class="no-notificaciones">
                <div class="no-data-icon">
                    <i class="fas fa-bell-slash"></i>
                </div>
                <h3>No tienes notificaciones</h3>
                <p>Todas tus notificaciones aparecerán aquí</p>
            </div>
        `;
        return;
    }

    container.innerHTML = '';
    
    notificaciones.forEach(notifRaw => {
        const notif = normalizeNotificacionData(notifRaw);
        const notificacionElement = createNotificacionElement(notif);
        container.appendChild(notificacionElement);
    });

    // Actualizar el badge de notificaciones en el menú
    updateNotificationBadge(getUnreadCount(notificaciones));
}

function createNotificacionElement(notificacion) {
    const div = document.createElement('div');
    const typeClass = getNotificacionTypeClass(notificacion.tipo);
    const isRead = notificacion.estado === 'Leída' || notificacion.estado === 'leida';
    
    div.className = `notificacion ${typeClass} ${isRead ? 'read' : 'unread'}`;
    div.setAttribute('data-id', notificacion.notificacionId);
    
    const fechaFormatted = formatFecha(notificacion.fechaEnvio);
    const icon = getNotificacionIcon(notificacion.tipo);
    
    div.innerHTML = `
        <div class="notificacion-header">
            <div class="notificacion-icon">
                <i class="${icon}"></i>
            </div>
            <div class="notificacion-info">
                <h4 class="notificacion-titulo">${notificacion.asunto}</h4>
                <span class="notificacion-fecha">${fechaFormatted}</span>
            </div>
            <div class="notificacion-actions">
                ${!isRead ? `<button class="mark-read-btn" onclick="markAsRead(${notificacion.notificacionId})" title="Marcar como leída">
                    <i class="fas fa-check"></i>
                </button>` : ''}
                <button class="delete-notif-btn" onclick="deleteNotificacion(${notificacion.notificacionId})" title="Eliminar">
                    <i class="fas fa-trash"></i>
                </button>
            </div>
        </div>
        <div class="notificacion-content">
            <p class="notificacion-mensaje">${notificacion.contenido || notificacion.asunto}</p>
        </div>
        ${!isRead ? '<div class="unread-indicator"></div>' : ''}
    `;
    
    // Agregar evento de clic para expandir/contraer
    div.addEventListener('click', function(e) {
        if (e.target.closest('.notificacion-actions')) return;
        
        const content = div.querySelector('.notificacion-content');
        const isExpanded = div.classList.contains('expanded');
        
        if (isExpanded) {
            div.classList.remove('expanded');
            content.style.maxHeight = '0';
        } else {
            div.classList.add('expanded');
            content.style.maxHeight = content.scrollHeight + 'px';
        }
        
        // Marcar como leída automáticamente al hacer clic
        if (!isRead) {
            markAsRead(notificacion.notificacionId);
        }
    });
    
    return div;
}

function getNotificacionTypeClass(tipo) {
    const typeMap = {
        'Alerta': 'notificacion1', // Rojo - Alertas importantes
        'Información': 'notificacion2', // Amarillo - Información general
        'Confirmación': 'notificacion3', // Verde - Confirmaciones
        'Transaccion': 'notificacion3', // Verde - Transacciones
        'Seguridad': 'notificacion1', // Rojo - Seguridad
        'Promoción': 'notificacion2' // Amarillo - Promociones
    };
    return typeMap[tipo] || 'notificacion2';
}

function getNotificacionIcon(tipo) {
    const iconMap = {
        'Alerta': 'fas fa-exclamation-triangle',
        'Información': 'fas fa-info-circle',
        'Confirmación': 'fas fa-check-circle',
        'Transaccion': 'fas fa-exchange-alt',
        'Seguridad': 'fas fa-shield-alt',
        'Promoción': 'fas fa-gift'
    };
    return iconMap[tipo] || 'fas fa-bell';
}

function formatFecha(fechaString) {
    if (!fechaString) return 'Sin fecha';
    
    const fecha = new Date(fechaString);
    const now = new Date();
    const diffTime = Math.abs(now - fecha);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    if (diffDays === 1) {
        return 'Ayer';
    } else if (diffDays < 7) {
        return `Hace ${diffDays} días`;
    } else {
        return fecha.toLocaleDateString('es-ES', {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric'
        });
    }
}

function getUnreadCount(notificaciones) {
    if (!notificaciones) return 0;
    return notificaciones.filter(n => n.estado !== 'Leída' && n.estado !== 'leida').length;
}

function updateNotificationBadge(count) {
    // Buscar el enlace de notificaciones en el menú
    const notifLink = document.querySelector('a[href*="notificaciones"]');
    if (!notifLink) return;
    
    // Remover badge existente
    const existingBadge = notifLink.querySelector('.notification-badge');
    if (existingBadge) {
        existingBadge.remove();
    }
    
    // Agregar nuevo badge si hay notificaciones no leídas
    if (count > 0) {
        const badge = document.createElement('span');
        badge.className = 'notification-badge';
        badge.textContent = count > 99 ? '99+' : count;
        notifLink.appendChild(badge);
    }
}

async function markAsRead(notificacionId) {
    try {
        // Por ahora simulamos la actualización (la API no tiene endpoint específico)
        const notifElement = document.querySelector(`[data-id="${notificacionId}"]`);
        if (notifElement) {
            notifElement.classList.remove('unread');
            notifElement.classList.add('read');
            
            const unreadIndicator = notifElement.querySelector('.unread-indicator');
            if (unreadIndicator) {
                unreadIndicator.remove();
            }
            
            const markReadBtn = notifElement.querySelector('.mark-read-btn');
            if (markReadBtn) {
                markReadBtn.remove();
            }
        }
        
        // Actualizar badge
        const allUnread = document.querySelectorAll('.notificacion.unread');
        updateNotificationBadge(allUnread.length);
        
    } catch (error) {
        console.error('Error marking notification as read:', error);
    }
}

async function deleteNotificacion(notificacionId) {
    if (!confirm('¿Estás seguro de que deseas eliminar esta notificación?')) {
        return;
    }
    
    try {
        // Por ahora solo removemos del DOM (la API no tiene endpoint de eliminación)
        const notifElement = document.querySelector(`[data-id="${notificacionId}"]`);
        if (notifElement) {
            notifElement.style.animation = 'slideOut 0.3s ease-in-out';
            setTimeout(() => {
                notifElement.remove();
                
                // Verificar si no quedan notificaciones
                const remainingNotifs = document.querySelectorAll('.notificacion');
                if (remainingNotifs.length === 0) {
                    displayNotificaciones([]);
                }
                
                // Actualizar badge
                const allUnread = document.querySelectorAll('.notificacion.unread');
                updateNotificationBadge(allUnread.length);
            }, 300);
        }
        
    } catch (error) {
        console.error('Error deleting notification:', error);
        alert('Error al eliminar la notificación');
    }
}

function displayFallbackNotificaciones() {
    // Mostrar notificaciones de ejemplo si la API falla
    const notificacionesEjemplo = [
        {
            notificacionId: 1,
            tipo: 'Confirmación',
            asunto: 'Transferencia Exitosa',
            contenido: 'Tu transferencia de $250.00 a María González ha sido procesada exitosamente.',
            fechaEnvio: new Date().toISOString(),
            estado: 'No leída'
        },
        {
            notificacionId: 2,
            tipo: 'Información',
            asunto: 'Estado de Cuenta Disponible',
            contenido: 'Tu estado de cuenta del mes de octubre ya está disponible para descarga.',
            fechaEnvio: new Date(Date.now() - 86400000).toISOString(),
            estado: 'No leída'
        },
        {
            notificacionId: 3,
            tipo: 'Alerta',
            asunto: 'Saldo Bajo',
            contenido: 'Tu cuenta corriente tiene un saldo de $50.00. Considera realizar un depósito.',
            fechaEnvio: new Date(Date.now() - 172800000).toISOString(),
            estado: 'Leída'
        }
    ];
    
    displayNotificaciones(notificacionesEjemplo);
}

function setupPeriodicUpdate() {
    // Actualizar notificaciones cada 5 minutos
    setInterval(async () => {
        try {
            await loadNotificaciones();
        } catch (error) {
            console.error('Error in periodic notification update:', error);
        }
    }, 300000); // 5 minutos
}

function showLoading(show) {
    const container = document.querySelector('.notificaciones-container');
    if (!container) return;
    
    if (show) {
        container.innerHTML = `
            <div class="loading-notificaciones">
                <div class="loading-spinner"></div>
                <p>Cargando notificaciones...</p>
            </div>
        `;
    }
}

// Función para marcar todas como leídas
function markAllAsRead() {
    const unreadNotifications = document.querySelectorAll('.notificacion.unread');
    unreadNotifications.forEach(notif => {
        const id = notif.getAttribute('data-id');
        markAsRead(parseInt(id));
    });
}

// Función para limpiar todas las notificaciones leídas
function clearReadNotifications() {
    if (!confirm('¿Estás seguro de que deseas eliminar todas las notificaciones leídas?')) {
        return;
    }
    
    const readNotifications = document.querySelectorAll('.notificacion.read');
    readNotifications.forEach(notif => {
        notif.style.animation = 'slideOut 0.3s ease-in-out';
        setTimeout(() => {
            notif.remove();
        }, 300);
    });
    
    setTimeout(() => {
        const remainingNotifs = document.querySelectorAll('.notificacion');
        if (remainingNotifs.length === 0) {
            displayNotificaciones([]);
        }
    }, 350);
}