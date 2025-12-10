// ========================================
// Manejador de Transacciones
// ========================================

// Configuración de la API
const API_BASE_URL = 'http://localhost:5202/api';

// Variables globales
let userAccounts = [];

// ========================================
// Inicialización
// ========================================

document.addEventListener('DOMContentLoaded', async function() {
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }
    await initTransaccionesPage();
});

async function initTransaccionesPage() {
    await cargarCuentasUsuario();
    configurarFormulario();
    await cargarUltimasTransacciones();
}

// ========================================
// Gestión de Cuentas
// ========================================

// Cargar las cuentas del usuario desde la base de datos
async function cargarCuentasUsuario() {
    try {
        const token = localStorage.getItem('authToken');
        const user = JSON.parse(localStorage.getItem('user'));

        console.log('Usuario completo desde localStorage:', user);

        if (!user) {
            throw new Error('Usuario no encontrado');
        }

        const username = user.username || user.nombreUsuario;
        if (!username) {
            throw new Error('Usuario sin nombre de usuario');
        }

        const url = `${API_BASE_URL}/Banco/clientes/por-usuario?username=${encodeURIComponent(username)}`;
        console.log('URL completa de la petición:', url);

        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        console.log('Status de respuesta del servidor:', response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('❌ Error del servidor:', errorText);
            throw new Error('Error al cargar las cuentas');
        }

        const data = await response.json();
        userAccounts = data.cuentas || [];
        
        // Normalizar los datos de las cuentas
        userAccounts = userAccounts.map(normalizeCuentaData);
        
        console.log('✅ Cuentas recibidas del servidor:', userAccounts);
        console.log('✅ Total de cuentas:', userAccounts.length);
        
        renderizarCuentasEnSelect();

    } catch (error) {
        console.error('Error al cargar cuentas:', error);
        mostrarMensaje('Error al cargar las cuentas: ' + error.message, 'error');
    }
}

// Normalizar datos de cuenta desde la API
function normalizeCuentaData(cuenta) {
    if (!cuenta) return null;
    
    return {
        cuentaId: cuenta['ID Cuenta'] || cuenta.cuentaId || cuenta.cuenta_id,
        numeroCuenta: cuenta['Numero de Cuenta'] || cuenta.numeroCuenta || cuenta.numero_cuenta,
        tipoCuenta: cuenta['Tipo de Cuenta'] || cuenta.tipoCuenta || cuenta.tipo_cuenta,
        saldo: cuenta['Saldo'] || cuenta.saldo || 0,
        activa: cuenta['Activa'] !== false && cuenta.activa !== false,
        ...cuenta
    };
}

// Normalizar datos de transacción desde la API
function normalizeTransaccionData(transaccion) {
    if (!transaccion) return null;
    
    return {
        transaccionId: transaccion['ID Transaccion'] || transaccion.transaccionId || transaccion.transaccion_id,
        tipo_transaccion: transaccion['Tipo Transaccion'] || transaccion['Tipo de Transaccion'] || transaccion.tipo_transaccion || transaccion.tipoTransaccion,
        tipoTransaccion: transaccion['Tipo Transaccion'] || transaccion['Tipo de Transaccion'] || transaccion.tipo_transaccion || transaccion.tipoTransaccion,
        monto: parseFloat(transaccion['Monto'] || transaccion.monto || 0),
        fecha: transaccion['Fecha'] || transaccion.fecha || transaccion.fechaTransaccion,
        fechaTransaccion: transaccion['Fecha'] || transaccion.fecha || transaccion.fechaTransaccion,
        descripcion: transaccion['Descripcion'] || transaccion.descripcion,
        estado: transaccion['Estado'] || transaccion.estado || 'Completado',
        cuenta_origen: transaccion['Cuenta Origen'] || transaccion.cuenta_origen || transaccion.cuentaOrigen,
        cuentaOrigen: transaccion['Cuenta Origen'] || transaccion.cuenta_origen || transaccion.cuentaOrigen,
        cuenta_destino: transaccion['Cuenta Destino'] || transaccion.cuenta_destino || transaccion.cuentaDestino,
        cuentaDestino: transaccion['Cuenta Destino'] || transaccion.cuenta_destino || transaccion.cuentaDestino,
        cuentaDestinoNumero: transaccion['Cuenta Destino'] || transaccion.cuentaDestinoNumero || transaccion.numero_cuenta_destino,
        clienteDestino: transaccion['Cliente Destino'] || transaccion.clienteDestino || transaccion.cliente_destino,
        ...transaccion
    };
}

// Renderizar las cuentas en el select
function renderizarCuentasEnSelect() {
    const selectCuenta = document.getElementById('from-account');
    if (!selectCuenta) return;

    selectCuenta.innerHTML = '<option value="">Seleccione una cuenta</option>';

    if (userAccounts && userAccounts.length > 0) {
        userAccounts.forEach(cuenta => {
            const option = crearOpcionCuenta(cuenta);
            selectCuenta.appendChild(option);
        });
    } else {
        selectCuenta.innerHTML = '<option value="">No tiene cuentas disponibles</option>';
    }
}

// Crear opción de cuenta para el select
function crearOpcionCuenta(cuenta) {
    const option = document.createElement('option');
    const cuentaId = cuenta.cuentaId;
    const numeroCuenta = cuenta.numeroCuenta || '';
    const tipoCuenta = cuenta.tipoCuenta || 'Cuenta';
    const saldo = cuenta.saldo || 0;

    option.value = cuentaId;
    option.textContent = `${tipoCuenta} ****${numeroCuenta.toString().slice(-4)} - $${saldo.toFixed(2)}`;
    option.dataset.saldo = saldo;
    option.dataset.numero = numeroCuenta;
    option.dataset.cuentaId = cuentaId;

    console.log('Cuenta cargada:', { cuentaId, numeroCuenta, tipoCuenta, saldo });
    return option;
}

// ========================================
// Procesamiento de Transacciones
// ========================================

// Configurar el formulario de transacción
function configurarFormulario() {
    const form = document.getElementById('transaction-form');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        await procesarTransaccion();
    });
}

// Procesar la transacción
async function procesarTransaccion() {
    const formData = obtenerDatosFormulario();
    
    if (!validarDatosTransaccion(formData)) {
        return;
    }

    try {
        const token = localStorage.getItem('authToken');
        const user = JSON.parse(localStorage.getItem('user'));

        mostrarMensaje('🔍 Buscando cuenta de destino...', 'info');
        const cuentaDestinoId = await buscarCuentaIdPorNumero(formData.cuentaDestino);

        if (!cuentaDestinoId) {
            mostrarMensaje('❌ La cuenta de destino no existe o no está activa', 'error');
            return;
        }

        console.log('Cuenta Destino - ID encontrado:', cuentaDestinoId);

        const transferencia = {
            cuentaOrigen: formData.cuentaOrigenId.toString(),
            cuentaDestino: cuentaDestinoId.toString(),
            monto: formData.monto,
            descripcion: formData.descripcion || formData.tipo,
            usuarioId: user.id || user.clienteId,
            username: user.username
        };

        console.log('Enviando transferencia:', transferencia);

        const response = await fetch(`${API_BASE_URL}/Banco/transfer/internal`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(transferencia)
        });

        const resultado = await response.json();
        console.log('Respuesta del servidor:', resultado);

        if (!response.ok || !resultado.success) {
            throw new Error(resultado.message || 'Error al procesar la transacción');
        }

        const mensajeExito = resultado.transaccionId
            ? `✅ Transferencia exitosa! ID: ${resultado.transaccionId}`
            : '✅ Transferencia realizada exitosamente';

        mostrarMensaje(mensajeExito, 'success');

        limpiarFormulario();
        await cargarCuentasUsuario();
        await cargarUltimasTransacciones();

    } catch (error) {
        console.error('❌ Error en la transacción:', error);
        mostrarMensaje('❌ ' + (error.message || 'Error al realizar la transacción'), 'error');
    }
}

// Obtener datos del formulario
function obtenerDatosFormulario() {
    const fromAccount = document.getElementById('from-account');
    const toAccountNumero = document.getElementById('to-account').value.trim();
    const amount = parseFloat(document.getElementById('amount').value);
    const type = document.getElementById('transaction-type').value;
    const description = document.getElementById('description').value.trim();

    const cuentaOrigenId = fromAccount.selectedOptions[0]?.dataset.cuentaId || fromAccount.value;
    const numeroCuentaOrigen = fromAccount.selectedOptions[0]?.dataset.numero;
    const saldoDisponible = parseFloat(fromAccount.selectedOptions[0]?.dataset.saldo || 0);

    return {
        cuentaOrigenId,
        numeroCuentaOrigen,
        cuentaDestino: toAccountNumero,
        monto: amount,
        tipo: type,
        descripcion: description,
        saldoDisponible
    };
}

// Validar datos de la transacción
function validarDatosTransaccion(data) {
    if (!data.cuentaOrigenId) {
        mostrarMensaje('Seleccione una cuenta de origen', 'error');
        return false;
    }

    if (!data.cuentaDestino) {
        mostrarMensaje('Ingrese una cuenta de destino', 'error');
        return false;
    }

    if (!data.tipo) {
        mostrarMensaje('Seleccione un tipo de transacción', 'error');
        return false;
    }

    if (data.monto <= 0 || isNaN(data.monto)) {
        mostrarMensaje('El monto debe ser mayor a 0', 'error');
        return false;
    }

    if (data.monto > data.saldoDisponible) {
        mostrarMensaje(`Saldo insuficiente. Disponible: $${data.saldoDisponible.toFixed(2)}`, 'error');
        return false;
    }

    if (data.numeroCuentaOrigen === data.cuentaDestino) {
        mostrarMensaje('La cuenta de destino no puede ser la misma que la de origen', 'error');
        return false;
    }

    return true;
}

// Limpiar el formulario
function limpiarFormulario() {
    document.getElementById('transaction-form').reset();
}

// Buscar el ID de una cuenta por su número
async function buscarCuentaIdPorNumero(numeroCuenta) {
    try {
        const token = localStorage.getItem('authToken');
        console.log('Buscando cuenta con número:', numeroCuenta);

        const response = await fetch(`${API_BASE_URL}/Banco/cuentas`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error('Error al buscar cuenta de destino');
        }

        const todasLasCuentasRaw = await response.json();
        const todasLasCuentas = todasLasCuentasRaw.map(normalizeCuentaData);
        console.log('Total de cuentas en el sistema:', todasLasCuentas.length);

        const cuentaEncontrada = todasLasCuentas.find(c =>
            c.cuentaId === numeroCuenta || c.numeroCuenta === numeroCuenta
        );

        if (cuentaEncontrada) {
            console.log('✅ Cuenta destino encontrada:', { cuentaId: cuentaEncontrada.cuentaId, numero: numeroCuenta });
            return cuentaEncontrada.cuentaId;
        }

        console.log('❌ Cuenta destino NO encontrada con número:', numeroCuenta);
        return null;

    } catch (error) {
        console.error('❌ Error al buscar cuenta:', error);
        return null;
    }
}

// Cargar las últimas transacciones del usuario
async function cargarUltimasTransacciones() {
    try {
        const token = localStorage.getItem('authToken');
        const user = JSON.parse(localStorage.getItem('user'));

        if (!user) return;

        const username = user.username || user.nombreUsuario;
        const response = await fetch(`${API_BASE_URL}/Banco/transacciones/usuario?username=${encodeURIComponent(username)}&limit=5`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            mostrarMensajeTabla('No se pudieron cargar las transacciones');
            console.error('Error al cargar transacciones:', response.status);
            return;
        }

        const transaccionesRaw = await response.json();
        const transacciones = Array.isArray(transaccionesRaw) 
            ? transaccionesRaw.map(normalizeTransaccionData) 
            : [];
        console.log('✅ Transacciones cargadas y normalizadas:', transacciones);
        actualizarTablaTransacciones(transacciones);

    } catch (error) {
        console.error('Error al cargar transacciones:', error);
        mostrarMensajeTabla('Error al cargar transacciones');
    }
}

// Mostrar mensaje en la tabla
function mostrarMensajeTabla(mensaje) {
    const tbody = document.getElementById('transactions-tbody');
    if (tbody) {
        tbody.innerHTML = `<tr><td colspan="5" style="text-align: center; padding: 20px; color: #666;">${mensaje}</td></tr>`;
    }
}

// Actualizar la tabla de transacciones
function actualizarTablaTransacciones(transacciones) {
    const tbody = document.getElementById('transactions-tbody');
    if (!tbody) return;

    if (!transacciones || transacciones.length === 0) {
        tbody.innerHTML = '<tr><td colspan="5" style="text-align: center; padding: 20px; color: #666;">No hay transacciones recientes</td></tr>';
        return;
    }

    tbody.innerHTML = '';

    transacciones.forEach(trans => {
        const row = crearFilaTransaccion(trans);
        tbody.appendChild(row);
    });
}

// Crear fila de transacción
function crearFilaTransaccion(trans) {
    const fecha = new Date(trans.fecha);
    
    // Determinar si es ingreso o egreso basándose en las cuentas del usuario
    const esIngreso = esIngresoParaUsuario(trans);

    const row = document.createElement('tr');
    row.innerHTML = `
        <td class="transaction-date">
            <span class="date">${fecha.toLocaleDateString('es-ES')}</span>
            <span class="time">${fecha.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' })}</span>
        </td>
        <td class="transaction-description">
            <div class="transaction-icon">
                <i class="fas fa-${obtenerIconoTransaccion(trans.tipoTransaccion)}"></i>
            </div>
            <div class="transaction-info">
                <strong>${trans.descripcion || trans.tipoTransaccion}</strong>
                <span>Cuenta ****${(trans.cuentaDestinoNumero || trans.cuentaDestino || '0000').toString().slice(-4)}</span>
            </div>
        </td>
        <td><span class="category-tag ${obtenerClaseCategoria(trans.tipoTransaccion)}">${trans.tipoTransaccion}</span></td>
        <td class="transaction-amount ${esIngreso ? 'positive' : 'negative'}">${esIngreso ? '+' : '-'}$${trans.monto.toFixed(2)}</td>
        <td><span class="status-badge ${obtenerEstadoBadge(trans.estado)}">${trans.estado}</span></td>
    `;
    return row;
}

// Determinar si una transacción es ingreso para el usuario actual
function esIngresoParaUsuario(trans) {
    const userAccountsNumbers = userAccounts.map(c => c.numeroCuenta).filter(Boolean);
    
    const tipo = (trans.tipoTransaccion || '').toLowerCase();
    const descripcion = (trans.descripcion || '').toLowerCase();
    const cuentaOrigen = trans.cuentaOrigen;
    const cuentaDestino = trans.cuentaDestino;
    
    // Si es un depósito directo o nómina, siempre es ingreso
    if (tipo.includes('deposito') || descripcion.includes('nomina') || descripcion.includes('sueldo')) {
        return true;
    }
    
    // Para transferencias, verificar si la cuenta destino es del usuario
    if (cuentaDestino && userAccountsNumbers.includes(cuentaDestino)) {
        return true; // El usuario recibe dinero
    }
    
    // Si la cuenta origen es del usuario y destino no, es egreso
    if (cuentaOrigen && userAccountsNumbers.includes(cuentaOrigen)) {
        return false; // El usuario envía dinero
    }
    
    // Por defecto, considerar como egreso si no coincide
    return tipo.includes('ingreso') || descripcion.includes('recibida');
}

// Obtener clase de badge según estado
function obtenerEstadoBadge(estado) {
    return (estado === 'Completado' || estado === 'Completada') ? 'completed' : 'pending';
}

// ========================================
// Utilidades y Helpers
// ========================================

// Obtener icono según tipo de transacción
function obtenerIconoTransaccion(tipo) {
    const iconos = {
        'Transferencia': 'exchange-alt',
        'Pago de Servicio': 'bolt',
        'Pago a Tarjeta': 'credit-card',
        'Deposito': 'arrow-down',
        'Retiro': 'arrow-up',
        'Compra': 'shopping-cart'
    };
    return iconos[tipo] || 'exchange-alt';
}

// Obtener clase CSS según tipo de transacción
function obtenerClaseCategoria(tipo) {
    const clases = {
        'Transferencia': 'transfer',
        'Pago de Servicio': 'payment',
        'Pago a Tarjeta': 'payment',
        'Deposito': 'deposit',
        'Retiro': 'withdrawal',
        'Compra': 'purchase'
    };
    return clases[tipo] || 'transfer';
}

// ========================================
// Sistema de Notificaciones
// ========================================
function mostrarMensaje(mensaje, tipo) {
    const colores = {
        'success': '#4CAF50',
        'error': '#f44336',
        'info': '#2196F3',
        'warning': '#ff9800'
    };

    const iconos = {
        'success': '✓',
        'error': '✕',
        'info': 'ℹ',
        'warning': '⚠'
    };

    const tiempoVisible = tipo === 'error' ? 8000 : tipo === 'success' ? 6000 : 5000;

    // Crear contenedor si no existe
    let container = document.getElementById('notifications-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'notifications-container';
        container.style.cssText = `
            position: fixed;
            top: 80px;
            right: 20px;
            z-index: 10000;
            display: flex;
            flex-direction: column;
            gap: 12px;
            pointer-events: none;
        `;
        document.body.appendChild(container);
    }

    const notification = document.createElement('div');
    notification.className = `notification-toast ${tipo}`;
    notification.innerHTML = `
        <div class="notification-icon">${iconos[tipo] || 'ℹ'}</div>
        <div class="notification-content">
            <div class="notification-message">${mensaje}</div>
        </div>
        <button class="notification-close" onclick="this.parentElement.remove()">×</button>
    `;
    
    notification.style.cssText = `
        min-width: 300px;
        max-width: 450px;
        background: white;
        border-left: 4px solid ${colores[tipo] || colores.info};
        border-radius: 8px;
        box-shadow: 0 6px 20px rgba(0,0,0,0.2);
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 16px;
        opacity: 0;
        transform: translateX(400px);
        transition: all 0.4s cubic-bezier(0.68, -0.55, 0.265, 1.55);
        pointer-events: auto;
    `;

    const style = document.createElement('style');
    style.textContent = `
        .notification-toast .notification-icon {
            width: 32px;
            height: 32px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            font-weight: bold;
            color: white;
            flex-shrink: 0;
        }
        .notification-toast.success .notification-icon { background: #4CAF50; }
        .notification-toast.error .notification-icon { background: #f44336; }
        .notification-toast.info .notification-icon { background: #2196F3; }
        .notification-toast.warning .notification-icon { background: #ff9800; }
        .notification-toast .notification-content {
            flex: 1;
            color: #333;
        }
        .notification-toast .notification-message {
            font-size: 14px;
            line-height: 1.4;
        }
        .notification-toast .notification-close {
            background: none;
            border: none;
            font-size: 24px;
            color: #999;
            cursor: pointer;
            padding: 0;
            width: 24px;
            height: 24px;
            line-height: 1;
            flex-shrink: 0;
            transition: color 0.2s;
        }
        .notification-toast .notification-close:hover {
            color: #333;
        }
    `;

    if (!document.querySelector('style[data-toast-styles]')) {
        style.setAttribute('data-toast-styles', 'true');
        document.head.appendChild(style);
    }

    container.appendChild(notification);

    requestAnimationFrame(() => {
        notification.style.opacity = '1';
        notification.style.transform = 'translateX(0)';
    });

    const hideTimeout = setTimeout(() => {
        notification.style.opacity = '0';
        notification.style.transform = 'translateX(400px)';
        setTimeout(() => {
            notification.remove();
            if (container.children.length === 0) {
                container.remove();
            }
        }, 400);
    }, tiempoVisible);

    notification.addEventListener('mouseenter', () => clearTimeout(hideTimeout));
    notification.addEventListener('mouseleave', () => {
        setTimeout(() => {
            notification.style.opacity = '0';
            notification.style.transform = 'translateX(400px)';
            setTimeout(() => {
                notification.remove();
                if (container.children.length === 0) {
                    container.remove();
                }
            }, 400);
        }, 1000);
    });
}