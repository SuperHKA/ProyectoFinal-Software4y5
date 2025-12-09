// Manejador del dashboard
document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    // Actualizar información del usuario en el header
    await updateUserInfo();

    // Cargar datos del dashboard
    await loadDashboardData();
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

// Normalizar datos de cuenta desde la API
function normalizeCuentaData(cuenta) {
    if (!cuenta) return null;
    
    return {
        cuentaId: cuenta['ID Cuenta'] || cuenta.cuentaId || cuenta.cuenta_id,
        numeroCuenta: cuenta['Número de Cuenta'] || cuenta.numeroCuenta || cuenta.numero_cuenta,
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
        transaccionId: transaccion['ID Transacción'] || transaccion.transaccionId || transaccion.transaccion_id,
        fecha: transaccion['Fecha'] || transaccion.fecha,
        tipo: transaccion['Tipo de Transacción'] || transaccion['Tipo Transacción'] || transaccion.tipo_transaccion || transaccion.tipo,
        tipoTransaccion: transaccion['Tipo de Transacción'] || transaccion['Tipo Transacción'] || transaccion.tipo_transaccion || transaccion.tipo,
        estado: transaccion['Estado'] || transaccion.estado,
        monto: parseFloat(transaccion['Monto'] || transaccion.monto || 0),
        descripcion: transaccion['Descripción'] || transaccion.descripcion,
        cuentaOrigen: transaccion['Cuenta Origen'] || transaccion.cuentaOrigen || transaccion.cuenta_origen,
        cuentaDestino: transaccion['Cuenta Destino'] || transaccion.cuentaDestino || transaccion.cuenta_destino,
        clienteDestino: transaccion['Cliente Destino'] || transaccion.clienteDestino || transaccion.cliente_destino,
        ...transaccion
    };
}

async function updateUserInfo() {
    try {
        const data = await authService.getClienteInfo();
        // Nueva estructura: { cliente: {...}, cuentas: [...] }
        const clienteRaw = data.cliente || data;
        const cliente = normalizeClienteData(clienteRaw);
        const displayName = (cliente && (cliente.nombreCompleto || cliente.nombre || cliente.username)) || (authService.getCurrentUser() && authService.getCurrentUser().username) || 'Usuario';

        const userNameElements = document.querySelectorAll('.user-info span, h1');
        userNameElements.forEach(el => {
            if (el.classList.contains('profile-name')) return; // evitar sobreescribir título del perfil
            // Si el elemento tiene texto de bienvenida, mantenerlo y añadir nombre
            if (el.textContent && el.textContent.toLowerCase().includes('bienvenido')) {
                el.textContent = `👋 Bienvenido, ${displayName}`;
            } else {
                // Para otros elementos como el header
                if (el.matches('.user-info span')) el.textContent = displayName;
            }
        });

        const avatar = document.querySelector('.user-avatar');
        if (avatar && cliente) {
            const fullName = cliente.nombreCompleto || cliente.nombre || displayName;
            const initials = fullName.split(' ').map(n => n[0]).slice(0,2).join('').toUpperCase();
            avatar.textContent = initials;
        }
    } catch (error) {
        console.warn('No se pudo obtener información extendida del usuario:', error);
    }
}

async function loadDashboardData() {
    try {
        // Mostrar indicador de carga
        showLoading(true);

        // Obtener datos del cliente (incluye cliente + cuentas)
        const data = await authService.getClienteInfo();
        const cuentasRaw = data.cuentas || data; // Compatibilidad con respuesta vieja
        const cuentas = Array.isArray(cuentasRaw) ? cuentasRaw.map(normalizeCuentaData) : [];
        
        // Guardar info del cliente en sessionStorage para usarla después
        sessionStorage.setItem('clienteInfo', JSON.stringify(data));
        
        if (Array.isArray(cuentas) && cuentas.length > 0) {
            displayCuentas(cuentas);
        } else {
            displayNoCuentas();
        }

        // Cargar transacciones recientes y calcular resumen mensual
        await loadTransaccionesRecientes();
        await loadMonthlySummary();

    } catch (error) {
        console.error('Error loading dashboard data:', error);
        showError('Error al cargar datos del dashboard');
    } finally {
        showLoading(false);
    }
}

function displayCuentas(cuentas) {
    const accountsGrid = document.querySelector('.accounts-grid');
    if (!accountsGrid) return;

    accountsGrid.innerHTML = '';

    cuentas.forEach(cuenta => {
        const card = document.createElement('div');
        card.className = 'account-card';
        
        const tipoCuenta = cuenta.tipoCuenta || cuenta['Tipo de Cuenta'] || 'Cuenta';
        const icon = getTipoCuentaIcon(tipoCuenta);
        const numeroCuenta = cuenta.numeroCuenta || cuenta['Número de Cuenta'] || '';
        const last4 = numeroCuenta ? String(numeroCuenta).slice(-4) : '----';

        const saldo = cuenta.saldo || cuenta['Saldo'] || 0;
        
        card.innerHTML = `
            <div class="account-header">
                <span class="account-type">
                    <i class="${icon}"></i>
                    ${tipoCuenta}
                </span>
                <i class="fas fa-ellipsis-h"></i>
            </div>
            <div class="account-balance">$${formatMoney(saldo)}</div>
            <div class="account-number">** ** ** ${last4}</div>
        `;
        
        accountsGrid.appendChild(card);
    });
}

function displayNoCuentas() {
    const accountsGrid = document.querySelector('.accounts-grid');
    if (!accountsGrid) return;

    accountsGrid.innerHTML = `
        <div class="no-data">
            <p>No tienes cuentas bancarias aún.</p>
            <button class="btn-primary" onclick="window.location.href='Profile.html'">
                Abrir una cuenta
            </button>
        </div>
    `;
}

async function loadTransaccionesRecientes() {
    try {
        const user = authService.getCurrentUser();
        if (!user || !user.username) {
            console.warn('No user logged in');
            displayTransacciones([]);
            return;
        }

        console.log('Cargando transacciones para usuario:', user.username);

        // Usar endpoint específico del cliente
        const response = await fetch(
            `http://localhost:5202/api/banco/transacciones/usuario?username=${encodeURIComponent(user.username)}&limit=5`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            console.log('Transacciones recibidas:', data);
            // data puede ser array directo o tener items
            const transacciones = Array.isArray(data) ? data : (data.items || []);
            displayTransacciones(transacciones);
        } else {
            console.warn('Failed to load transactions, status:', response.status);
            displayTransacciones([]);
        }
    } catch (error) {
        console.error('Error loading transactions:', error);
        displayTransacciones([]);
    }
}

function displayTransacciones(transacciones) {
    const transactionsList = document.querySelector('.transactions-list');
    if (!transactionsList) return;

    if (!transacciones || transacciones.length === 0) {
        transactionsList.innerHTML = '<div class="no-data">No hay transacciones recientes</div>';
        return;
    }

    transactionsList.innerHTML = '';
    
    transacciones.forEach(transRaw => {
        const trans = normalizeTransaccionData(transRaw);
        const item = document.createElement('div');
        item.className = 'transaction-item';
        
        // Determinar si es ingreso o egreso basándose en las cuentas del usuario
        const isIngreso = esIngresoParaCliente(trans);
        
        const icon = isIngreso ? 'fa-arrow-down' : 'fa-arrow-up';
        const amountClass = isIngreso ? 'positive' : 'negative';
        const sign = isIngreso ? '+' : '-';
        
        item.innerHTML = `
            <div class="transaction-icon">
                <i class="fas ${icon}"></i>
            </div>
            <div class="transaction-details">
                <div class="transaction-name">${trans.descripcion || trans.tipo || 'Transacción'}</div>
                <div class="transaction-date">${formatDate(trans.fecha)}</div>
            </div>
            <div class="transaction-amount ${amountClass}">${sign}$${formatMoney(trans.monto)}</div>
        `;
        
        transactionsList.appendChild(item);
    });
}

function getTipoCuentaIcon(tipo) {
    const iconMap = {
        'Corriente': 'fas fa-university',
        'Ahorro': 'fas fa-piggy-bank',
        'Dolares': 'fas fa-dollar-sign',
        'Dólares': 'fas fa-dollar-sign'
    };
    return iconMap[tipo] || 'fas fa-university';
}

function formatMoney(amount) {
    if (amount === null || amount === undefined) return '0.00';
    return parseFloat(amount).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    const options = { 
        year: 'numeric', 
        month: '2-digit', 
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    };
    return date.toLocaleDateString('es-MX', options);
}

function showLoading(show) {
    // Implementar indicador de carga
    const loader = document.getElementById('loader');
    if (loader) {
        loader.style.display = show ? 'block' : 'none';
    }
}

function showError(message) {
    alert(message); // Mejorar con un sistema de notificaciones más elegante
}

// Determinar si una transacción es ingreso para el cliente actual
function esIngresoParaCliente(trans) {
    const user = authService.getCurrentUser();
    if (!user) return false;
    
    // Obtener las cuentas del usuario del cache
    const userAccounts = getUserAccountNumbers();
    
    const tipo = (trans.tipoTransaccion || trans.tipo || '').toLowerCase();
    const descripcion = (trans.descripcion || '').toLowerCase();
    const cuentaOrigen = trans.cuentaOrigen;
    const cuentaDestino = trans.cuentaDestino;
    
    // Si es un depósito directo o nómina, siempre es ingreso
    if (tipo.includes('deposito') || descripcion.includes('nomina') || descripcion.includes('sueldo')) {
        return true;
    }
    
    // Para transferencias, verificar si la cuenta destino es del cliente
    if (cuentaDestino && userAccounts.includes(cuentaDestino)) {
        return true; // El cliente recibe dinero
    }
    
    // Si la cuenta origen es del cliente y destino no, es egreso
    if (cuentaOrigen && userAccounts.includes(cuentaOrigen)) {
        return false; // El cliente envía dinero
    }
    
    // Por defecto, usar lógica antigua
    return tipo.includes('ingreso') || descripcion.includes('recibida');
}

// Obtener números de cuenta del cliente actual
function getUserAccountNumbers() {
    try {
        const cachedData = sessionStorage.getItem('clienteInfo');
        if (cachedData) {
            const data = JSON.parse(cachedData);
            const cuentasRaw = data.cuentas || [];
            const cuentas = cuentasRaw.map(normalizeCuentaData);
            return cuentas.map(c => c.numeroCuenta).filter(Boolean);
        }
    } catch (e) {
        console.warn('Error obteniendo cuentas del cache:', e);
    }
    return [];
}

async function loadMonthlySummary() {
    try {
        const user = authService.getCurrentUser();
        if (!user || !user.username) return;

        // Obtener transacciones del mes actual usando el endpoint del cliente
        const now = new Date();
        const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
        const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
        
        const response = await fetch(
            `http://localhost:5202/api/banco/transacciones/usuario?username=${encodeURIComponent(user.username)}&limit=1000`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            const transaccionesRaw = Array.isArray(data) ? data : (data.items || []);
            const transacciones = transaccionesRaw.map(normalizeTransaccionData);
            
            // Calcular ingresos y gastos del mes actual
            let totalIngresos = 0;
            let totalGastos = 0;
            
            transacciones.forEach(trans => {
                const transDate = new Date(trans.fecha);
                // Solo contar transacciones del mes actual
                if (transDate >= firstDay && transDate <= lastDay) {
                    const esIngreso = esIngresoParaCliente(trans);
                    
                    if (esIngreso) {
                        totalIngresos += parseFloat(trans.monto || 0);
                    } else {
                        totalGastos += parseFloat(trans.monto || 0);
                    }
                }
            });
            
            const balance = totalIngresos - totalGastos;
            
            // Actualizar las tarjetas de resumen
            updateSummaryCards(totalIngresos, totalGastos, balance);
            // Actualizar las tarjetas de resumen
            updateSummaryCards(totalIngresos, totalGastos, balance);
            
            // Actualizar gráfica si existe
            updateMonthlyChart(transacciones);
        }
    } catch (error) {
        console.error('Error loading monthly summary:', error);
    }
}

// Actualizar las tarjetas de resumen mensual
function updateSummaryCards(ingresos, gastos, balance) {
    // Buscar las tarjetas por diferentes selectores posibles
    const summarySection = document.querySelector('.monthly-summary') || document.querySelector('.summary-cards');
    
    if (summarySection) {
        // Buscar dentro de la sección de resumen
        const cards = summarySection.querySelectorAll('.stat-card, .summary-card, .card');
        
        cards.forEach(card => {
            const label = card.querySelector('.stat-label, .card-label, h3, .label');
            const value = card.querySelector('.stat-value, .card-value, .value, .amount');
            
            if (label && value) {
                const labelText = label.textContent.toLowerCase();
                
                if (labelText.includes('ingreso') || labelText.includes('income')) {
                    value.textContent = `$${formatMoney(ingresos)}`;
                    value.style.color = '#10b981'; // Verde
                } else if (labelText.includes('gasto') || labelText.includes('expense')) {
                    value.textContent = `$${formatMoney(gastos)}`;
                    value.style.color = '#ef4444'; // Rojo
                } else if (labelText.includes('balance') || labelText.includes('saldo')) {
                    value.textContent = `$${formatMoney(balance)}`;
                    value.style.color = balance >= 0 ? '#10b981' : '#ef4444';
                }
            }
        });
    }
    
    // También actualizar si hay elementos con IDs específicos
    const ingresosEl = document.getElementById('total-ingresos');
    const gastosEl = document.getElementById('total-gastos');
    const balanceEl = document.getElementById('balance-mensual');
    
    if (ingresosEl) ingresosEl.textContent = `$${formatMoney(ingresos)}`;
    if (gastosEl) gastosEl.textContent = `$${formatMoney(gastos)}`;
    if (balanceEl) {
        balanceEl.textContent = `$${formatMoney(balance)}`;
        balanceEl.style.color = balance >= 0 ? '#10b981' : '#ef4444';
    }
}

function updateMonthlyChart(transacciones) {
    const canvas = document.getElementById('monthlyChart');
    if (!canvas) return;
    
    try {
        // Destruir gráfica existente si hay una
        if (window.monthlyChartInstance) {
            window.monthlyChartInstance.destroy();
            window.monthlyChartInstance = null;
        }
        
        // También destruir cualquier instancia de Chart.js asociada al canvas
        const existingChart = Chart.getChart(canvas);
        if (existingChart) {
            existingChart.destroy();
        }
        
        // Agrupar por mes (últimos 12 meses)
        const monthlyData = {};
        const now = new Date();
        
        for (let i = 11; i >= 0; i--) {
            const date = new Date(now.getFullYear(), now.getMonth() - i, 1);
            const key = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}`;
            monthlyData[key] = { ingresos: 0, gastos: 0 };
        }
        
        transacciones.forEach(trans => {
            const transDate = new Date(trans.fecha);
            const key = `${transDate.getFullYear()}-${String(transDate.getMonth() + 1).padStart(2, '0')}`;
            
            if (monthlyData[key]) {
                const esIngreso = esIngresoParaCliente(trans);
                
                if (esIngreso) {
                    monthlyData[key].ingresos += parseFloat(trans.monto || 0);
                } else {
                    monthlyData[key].gastos += parseFloat(trans.monto || 0);
                }
            }
        });
        
        const labels = [];
        const ingresosData = [];
        const gastosData = [];
        
        const meses = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 
                       'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
        
        Object.keys(monthlyData).sort().forEach(key => {
            const [year, month] = key.split('-');
            labels.push(meses[parseInt(month) - 1]);
            ingresosData.push(monthlyData[key].ingresos);
            gastosData.push(monthlyData[key].gastos);
        });
        
        // Crear nueva gráfica
        window.monthlyChartInstance = new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [
                    {
                        label: 'Ingresos',
                        data: ingresosData,
                        backgroundColor: 'rgba(52, 211, 153, 0.8)',
                        borderColor: 'rgba(52, 211, 153, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Gastos',
                        data: gastosData,
                        backgroundColor: 'rgba(248, 113, 113, 0.8)',
                        borderColor: 'rgba(248, 113, 113, 1)',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: false
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            callback: function(value) {
                                return '$' + value.toFixed(0);
                            }
                        }
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error updating chart:', error);
    }
}
