// Manejador de notificaciones del cliente
document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticaci�n
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    // Actualizar informaci�n del usuario en el header
    await updateUserInfo();

    // Cargar notificaciones del cliente
    await loadNotificaciones();

    // Configurar actualizaci�n peri�dica
    setupPeriodicUpdate();
});

async function updateUserInfo() {
    try {
        const data = await authService.getClienteInfo();
        const cliente = data.cliente || data;
        const displayName = (cliente && (cliente.nombreCompleto || cliente.nombre || cliente.username)) || 
                          (authService.getCurrentUser() && authService.getCurrentUser().username) || 'Usuario';

        const userNameElements = document.querySelectorAll('.user-info span');
        userNameElements.forEach(el => {
            el.textContent = displayName;
        });

        const avatar = document.querySelector('.user-avatar');
        if (avatar && cliente) {
            const fullName = cliente.nombreCompleto || cliente.nombre || displayName;
            const initials = fullName.split(' ').map(n => n[0]).slice(0,2).join('').toUpperCase();
            avatar.textContent = initials;
        }
    } catch (error) {
        console.warn('No se pudo obtener informaci�n del usuario:', error);
    }
}

async function loadNotificaciones() {
    try {
        showLoading(true);

        const user = authService.getCurrentUser();
        if (!user) {
            throw new Error('No se pudo obtener información del usuario');
        }

        // Obtener información del cliente
        const clienteInfo = await authService.getClienteInfo();
        const clienteId = clienteInfo.cliente?.clienteId || clienteInfo.clienteId || user.clienteId;
        
        if (!clienteId) {
            console.warn('No se pudo obtener el clienteId, usando transacciones');
            await loadNotificacionesFromTransacciones();
            return;
        }

        console.log('📌 Cargando notificaciones para clienteId:', clienteId);

        // Obtener notificaciones del cliente desde la API
        const response = await fetch(
            `${authService.baseURL}/banco/clientes/${clienteId}/notificaciones?dias=30`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`,
                    'Content-Type': 'application/json'
                }
            }
        );

        if (response.ok) {
            const notificaciones = await response.json();
            console.log('✅ Notificaciones cargadas:', notificaciones);
            
            // Si no hay notificaciones de la BD, generar desde transacciones
            if (!notificaciones || notificaciones.length === 0) {
                console.log('📌 No hay notificaciones en BD, generando desde transacciones...');
                await loadNotificacionesFromTransacciones();
            } else {
                displayNotificaciones(notificaciones);
            }
        } else {
            console.error('Error al obtener notificaciones:', response.statusText);
            await loadNotificacionesFromTransacciones();
        }
    } catch (error) {
        console.error('Error loading notificaciones:', error);
        await loadNotificacionesFromTransacciones();
    } finally {
        showLoading(false);
    }
}

// Nueva función para generar notificaciones desde transacciones
async function loadNotificacionesFromTransacciones() {
    try {
        const user = authService.getCurrentUser();
        if (!user) return;

        const username = user.username || user.nombreUsuario;
        
        // Obtener transacciones recientes
        const response = await fetch(
            `${authService.baseURL}/banco/transacciones/usuario?username=${encodeURIComponent(username)}&limit=10`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`,
                    'Content-Type': 'application/json'
                }
            }
        );

        if (!response.ok) {
            console.error('Error al obtener transacciones');
            displayFallbackNotificaciones();
            return;
        }

        const transacciones = await response.json();
        console.log('✅ Transacciones obtenidas para notificaciones:', transacciones);
        
        // Convertir transacciones a notificaciones
        const notificaciones = transacciones.map((trans, index) => {
            const esIngreso = determinarSiEsIngreso(trans);
            const monto = trans.monto || 0;
            const fecha = trans.fecha || trans.fechaTransaccion;
            
            // Determinar tipo según contexto
            let tipo;
            let asunto;
            let contenido;
            
            if (esIngreso) {
                // Ingresos = Verde (Éxito)
                tipo = 'Confirmación';
                asunto = 'Ingreso Recibido';
                contenido = `Has recibido $${monto.toFixed(2)}. ${trans.descripcion || trans.tipo_transaccion || ''}`;
            } else {
                // Egresos - verificar si puede causar problemas
                const saldoActual = trans.saldoActual || trans.saldo_actual || 0;
                const esCritico = saldoActual < 100 || monto > saldoActual * 0.5;
                
                if (esCritico) {
                    // Saldo bajo o transferencia grande = Rojo (Alerta)
                    tipo = 'Alerta';
                    asunto = 'Transferencia Importante';
                    contenido = `Has enviado $${monto.toFixed(2)}. ${saldoActual < 100 ? 'Saldo bajo.' : 'Monto significativo.'} ${trans.descripcion || trans.tipo_transaccion || ''}`;
                } else {
                    // Transferencia normal = Amarillo (Información)
                    tipo = 'Información';
                    asunto = 'Transferencia Realizada';
                    contenido = `Has enviado $${monto.toFixed(2)}. ${trans.descripcion || trans.tipo_transaccion || ''}`;
                }
            }
            
            return {
                notificacionId: index + 1,
                tipo: tipo,
                asunto: asunto,
                contenido: contenido,
                fechaEnvio: fecha,
                estado: index < 3 ? 'No leída' : 'Leída' // Marcar las 3 más recientes como no leídas
            };
        });

        displayNotificaciones(notificaciones);
        
    } catch (error) {
        console.error('Error generando notificaciones desde transacciones:', error);
        displayFallbackNotificaciones();
    }
}

// Función auxiliar para determinar si es ingreso
function determinarSiEsIngreso(trans) {
    const user = authService.getCurrentUser();
    const clienteInfo = JSON.parse(sessionStorage.getItem('clienteInfo') || '{}');
    const cuentasUsuario = clienteInfo.cuentas || [];
    const numerosCuenta = cuentasUsuario.map(c => c.numeroCuenta || c.numero_cuenta);
    
    const cuentaDestino = trans.cuenta_destino || trans.cuentaDestino;
    
    // Si la cuenta destino es del usuario, es un ingreso
    if (cuentaDestino && numerosCuenta.includes(cuentaDestino)) {
        return true;
    }
    
    return false;
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
                <p>Todas tus notificaciones aparecer�n aqu�</p>
            </div>
        `;
        return;
    }

    container.innerHTML = '';
    
    notificaciones.forEach(notif => {
        const notificacionElement = createNotificacionElement(notif);
        container.appendChild(notificacionElement);
    });

    // Actualizar el badge de notificaciones en el men�
    updateNotificationBadge(getUnreadCount(notificaciones));
}

function createNotificacionElement(notificacion) {
    const div = document.createElement('div');
    const typeClass = getNotificacionTypeClass(notificacion.tipo);
    const isRead = notificacion.estado === 'Le�da' || notificacion.estado === 'leida';
    
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
                ${!isRead ? `<button class="mark-read-btn" onclick="markAsRead(${notificacion.notificacionId})" title="Marcar como le�da">
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
        
        // Marcar como le�da autom�ticamente al hacer clic
        if (!isRead) {
            markAsRead(notificacion.notificacionId);
        }
    });
    
    return div;
}

function getNotificacionTypeClass(tipo) {
    const typeMap = {
        'Alerta': 'notificacion2', // Rojo - Alertas, deudas, problemas críticos
        'Información': 'notificacion1', // Amarillo - Información general, pendientes
        'Confirmación': 'notificacion3', // Verde - Confirmaciones, éxitos, ingresos
        'Transaccion': 'notificacion3', // Verde - Transacciones exitosas
        'Seguridad': 'notificacion2', // Rojo - Seguridad crítica
        'Promocion': 'notificacion1', // Amarillo - Promociones informativas
        'Deuda': 'notificacion2', // Rojo - Deudas
        'Pendiente': 'notificacion1', // Amarillo - Acciones pendientes
        'Exito': 'notificacion3' // Verde - Operaciones exitosas
    };
    return typeMap[tipo] || 'notificacion1';
}

function getNotificacionIcon(tipo) {
    const iconMap = {
        'Alerta': 'fas fa-exclamation-triangle',
        'Información': 'fas fa-info-circle',
        'Confirmación': 'fas fa-check-circle',
        'Transaccion': 'fas fa-exchange-alt',
        'Seguridad': 'fas fa-shield-alt',
        'Promocion': 'fas fa-gift',
        'Deuda': 'fas fa-exclamation-circle',
        'Pendiente': 'fas fa-clock',
        'Exito': 'fas fa-check-double'
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
        return `Hace ${diffDays} d�as`;
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
    return notificaciones.filter(n => n.estado !== 'Le�da' && n.estado !== 'leida').length;
}

function updateNotificationBadge(count) {
    // Buscar el enlace de notificaciones en el men�
    const notifLink = document.querySelector('a[href*="notificaciones"]');
    if (!notifLink) return;
    
    // Remover badge existente
    const existingBadge = notifLink.querySelector('.notification-badge');
    if (existingBadge) {
        existingBadge.remove();
    }
    
    // Agregar nuevo badge si hay notificaciones no le�das
    if (count > 0) {
        const badge = document.createElement('span');
        badge.className = 'notification-badge';
        badge.textContent = count > 99 ? '99+' : count;
        notifLink.appendChild(badge);
    }
}

async function markAsRead(notificacionId) {
    try {
        // Por ahora simulamos la actualizaci�n (la API no tiene endpoint espec�fico)
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
    if (!confirm('�Est�s seguro de que deseas eliminar esta notificaci�n?')) {
        return;
    }
    
    try {
        // Por ahora solo removemos del DOM (la API no tiene endpoint de eliminaci�n)
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
        alert('Error al eliminar la notificaci�n');
    }
}

function displayFallbackNotificaciones() {
    // Mostrar mensaje de que no hay notificaciones disponibles
    const container = document.querySelector('.notificaciones-container');
    if (!container) return;
    
    container.innerHTML = `
        <div class="no-notificaciones">
            <div class="no-data-icon">
                <i class="fas fa-bell-slash"></i>
            </div>
            <h3>No hay notificaciones disponibles</h3>
            <p>Cuando realices transacciones, aparecerán notificaciones aquí</p>
        </div>
    `;
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

// Funci�n para marcar todas como le�das
function markAllAsRead() {
    const unreadNotifications = document.querySelectorAll('.notificacion.unread');
    unreadNotifications.forEach(notif => {
        const id = notif.getAttribute('data-id');
        markAsRead(parseInt(id));
    });
}

// Funci�n para limpiar todas las notificaciones le�das
function clearReadNotifications() {
    if (!confirm('�Est�s seguro de que deseas eliminar todas las notificaciones le�das?')) {
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