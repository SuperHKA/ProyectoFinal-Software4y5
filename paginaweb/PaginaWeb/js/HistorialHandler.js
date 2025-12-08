document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    // Configurar elementos del DOM
    setupFilters();
    setupDatePickers();
    setupPagination();

    // Actualizar información del usuario
    await updateUserInfo();

    // Cargar datos iniciales
    await loadInitialData();
});

let currentPage = 1;
let pageSize = 10;
let totalPages = 1;
let currentFilters = {
    fechaDesde: null,
    fechaHasta: null,
    tipoTransaccion: 'todos',
    searchTerm: '',
    ordenarPor: 'fecha',
    orden: 'desc'
};

async function updateUserInfo() {
    try {
        const data = await authService.getClienteInfo();
        const cliente = data.cliente || data;
        const displayName = (cliente && (cliente.nombreCompleto || cliente.nombre || cliente.username)) || 
                          (authService.getCurrentUser() && authService.getCurrentUser().username) || 'Usuario';

        // Actualizar header de usuario
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

        // Actualizar información de la cuenta
        updateAccountCard(cliente, data.cuentas);
        
    } catch (error) {
        console.warn('No se pudo obtener información del usuario:', error);
    }
}

function updateAccountCard(cliente, cuentas) {
    const accountNameEl = document.querySelector('.nom-cuenta');
    const accountNumberEl = document.querySelector('.num-cuenta');
    
    if (accountNameEl && cliente) {
        accountNameEl.textContent = `Cuenta de ${cliente.nombreCompleto || cliente.nombre || cliente.username}`;
    }
    
    if (accountNumberEl && cuentas && cuentas.length > 0) {
        const primaryAccount = cuentas[0];
        accountNumberEl.textContent = primaryAccount.numeroCuenta;
    }
}

async function loadInitialData() {
    try {
        showLoading(true);

        // Cargar transacciones del cliente
        await loadTransactionsData();
        
        // Cargar estadísticas del mes
        await loadMonthlyStats();
        
        // Generar gráfico si existe
        generateTransactionChart();
        
    } catch (error) {
        console.error('Error loading initial data:', error);
        showError('Error al cargar el historial');
    } finally {
        showLoading(false);
    }
}

async function loadTransactionsData() {
    try {
        const user = authService.getCurrentUser();
        if (!user || !user.clienteId) {
            throw new Error('Cliente ID no disponible');
        }

        // Construir parámetros de consulta
        const queryParams = new URLSearchParams({
            page: currentPage,
            pageSize: pageSize
        });

        // Agregar filtros si están definidos
        if (currentFilters.fechaDesde) {
            queryParams.append('desde', currentFilters.fechaDesde);
        }
        if (currentFilters.fechaHasta) {
            queryParams.append('hasta', currentFilters.fechaHasta);
        }
        if (currentFilters.searchTerm) {
            queryParams.append('q', currentFilters.searchTerm);
        }

        const response = await fetch(
            `http://localhost:5202/api/banco/transacciones-cliente?clienteId=${user.clienteId}&${queryParams}`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            displayTransactions(data.items || data);
            updatePagination(data.total || data.length);
        } else {
            // Fallback a endpoint general si el específico no existe
            await loadFallbackTransactions();
        }
    } catch (error) {
        console.error('Error loading transactions:', error);
        await loadFallbackTransactions();
    }
}

async function loadFallbackTransactions() {
    try {
        const response = await fetch(
            `http://localhost:5202/api/banco/transacciones-recientes?page=${currentPage}&pageSize=${pageSize}`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            displayTransactions(data.items || data);
            updatePagination(data.total || data.length);
        } else {
            displayFallbackTransactions();
        }
    } catch (error) {
        displayFallbackTransactions();
    }
}

function displayTransactions(transactions) {
    const tableBody = document.querySelector('.historial-tabla tbody');
    const recentList = document.querySelector('.transacciones-lista');
    
    if (!transactions || transactions.length === 0) {
        if (tableBody) {
            tableBody.innerHTML = `
                <tr>
                    <td colspan="4" class="no-data">
                        <div class="no-transactions">
                            <i class="fas fa-receipt"></i>
                            <p>No se encontraron transacciones</p>
                        </div>
                    </td>
                </tr>
            `;
        }
        return;
    }

    // Actualizar tabla de historial completo
    if (tableBody) {
        tableBody.innerHTML = '';
        let runningBalance = 0;
        
        transactions.forEach(trans => {
            const row = document.createElement('tr');
            const isIngreso = isIngresoTransaction(trans);
            const monto = parseFloat(trans.monto || 0);
            
            if (isIngreso) {
                runningBalance += monto;
            } else {
                runningBalance -= monto;
            }
            
            row.innerHTML = `
                <td>${formatDate(trans.fecha)}</td>
                <td>
                    <div class="transaction-description">
                        <span class="description-text">${trans.descripcion || trans.tipo_transaccion}</span>
                        <span class="transaction-type">${trans.tipo_transaccion || 'Transacción'}</span>
                    </div>
                </td>
                <td class="${isIngreso ? 'monto-positivo' : 'monto-negativo'}">
                    ${isIngreso ? '+' : '-'}$${formatMoney(monto)}
                </td>
                <td>$${formatMoney(Math.abs(runningBalance))}</td>
            `;
            
            tableBody.appendChild(row);
        });
    }

    // Actualizar lista de transacciones recientes (primeras 4)
    if (recentList) {
        recentList.innerHTML = '';
        const recentTransactions = transactions.slice(0, 4);
        
        recentTransactions.forEach(trans => {
            const item = document.createElement('div');
            item.className = 'transaccion-item';
            
            const isIngreso = isIngresoTransaction(trans);
            const monto = parseFloat(trans.monto || 0);
            
            item.innerHTML = `
                <div class="transaccion-info">
                    <div class="merchant">${trans.descripcion || trans.tipo_transaccion}</div>
                    <div class="fecha">${formatDate(trans.fecha)}</div>
                </div>
                <div class="monto ${isIngreso ? 'positivo' : 'negativo'}">
                    ${isIngreso ? '+' : '-'}$${formatMoney(monto)}
                </div>
            `;
            
            recentList.appendChild(item);
        });
    }
}

async function loadMonthlyStats() {
    try {
        const user = authService.getCurrentUser();
        if (!user) return;

        // Obtener estadísticas del mes actual
        const now = new Date();
        const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
        const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);

        // Cargar cuentas para obtener saldo actual
        const accountData = await authService.getClienteInfo();
        const cuentas = accountData.cuentas || [];
        
        // Calcular saldo total
        const saldoTotal = cuentas.reduce((sum, cuenta) => sum + (parseFloat(cuenta.saldo) || 0), 0);
        const saldoDisponible = saldoTotal * 0.8; // Simular 80% disponible

        // Cargar transacciones del mes para estadísticas
        const response = await fetch(
            `http://localhost:5202/api/banco/transacciones-recientes?page=1&pageSize=1000`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            const transacciones = data.items || data;
            
            const statsDelMes = calculateMonthlyStats(transacciones, firstDay, lastDay);
            updateStatsDisplay(saldoTotal, saldoDisponible, statsDelMes);
        }
        
    } catch (error) {
        console.error('Error loading monthly stats:', error);
        // Mostrar estadísticas por defecto
        updateStatsDisplay(1500, 1000, { ingresos: 2300, egresos: 800, transacciones: 15 });
    }
}

function calculateMonthlyStats(transacciones, fechaDesde, fechaHasta) {
    let ingresos = 0;
    let egresos = 0;
    let totalTransacciones = 0;

    transacciones.forEach(trans => {
        const transDate = new Date(trans.fecha);
        if (transDate >= fechaDesde && transDate <= fechaHasta) {
            totalTransacciones++;
            const monto = parseFloat(trans.monto || 0);
            
            if (isIngresoTransaction(trans)) {
                ingresos += monto;
            } else {
                egresos += monto;
            }
        }
    });

    return { ingresos, egresos, transacciones: totalTransacciones };
}

function updateStatsDisplay(saldoTotal, saldoDisponible, statsDelMes) {
    // Actualizar tarjeta de saldo
    const saldoTotalEl = document.querySelector('.saldo-item .saldo');
    const saldoDispEl = document.querySelectorAll('.saldo-item .saldo')[1];
    
    if (saldoTotalEl) {
        saldoTotalEl.textContent = `${formatMoney(saldoTotal)} USD`;
    }
    if (saldoDispEl) {
        saldoDispEl.textContent = `${formatMoney(saldoDisponible)} USD`;
    }

    // Actualizar resumen del mes
    const statCards = document.querySelectorAll('.stat-card .stat-value');
    if (statCards.length >= 4) {
        statCards[0].textContent = `$${formatMoney(saldoTotal)}`;
        statCards[1].textContent = `+$${formatMoney(statsDelMes.ingresos)}`;
        statCards[2].textContent = `-$${formatMoney(statsDelMes.egresos)}`;
        statCards[3].textContent = statsDelMes.transacciones;
    }
}

function setupFilters() {
    // Crear panel de filtros si no existe
    const filtersContainer = document.querySelector('.filtros-container') || createFiltersPanel();
    
    // Configurar eventos de filtros
    const searchInput = document.getElementById('searchTransactions');
    const tipoSelect = document.getElementById('tipoTransaccion');
    const fechaDesde = document.getElementById('fechaDesde');
    const fechaHasta = document.getElementById('fechaHasta');
    
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                currentFilters.searchTerm = this.value;
                applyFilters();
            }, 500);
        });
    }
    
    if (tipoSelect) {
        tipoSelect.addEventListener('change', function() {
            currentFilters.tipoTransaccion = this.value;
            applyFilters();
        });
    }
    
    if (fechaDesde) {
        fechaDesde.addEventListener('change', function() {
            currentFilters.fechaDesde = this.value;
            applyFilters();
        });
    }
    
    if (fechaHasta) {
        fechaHasta.addEventListener('change', function() {
            currentFilters.fechaHasta = this.value;
            applyFilters();
        });
    }
}

function createFiltersPanel() {
    const historialSection = document.querySelector('.seccion-historial');
    if (!historialSection) return null;
    
    const filtersHTML = `
        <div class="filtros-container">
            <div class="filtros-header">
                <h3><i class="fas fa-filter"></i> Filtros</h3>
                <button class="btn-reset-filters" onclick="resetFilters()">
                    <i class="fas fa-undo"></i> Limpiar
                </button>
            </div>
            <div class="filtros-grid">
                <div class="filter-group">
                    <label for="searchTransactions">Buscar:</label>
                    <input type="text" id="searchTransactions" placeholder="Descripción o tipo...">
                </div>
                <div class="filter-group">
                    <label for="tipoTransaccion">Tipo:</label>
                    <select id="tipoTransaccion">
                        <option value="todos">Todos</option>
                        <option value="transferencia">Transferencias</option>
                        <option value="deposito">Depósitos</option>
                        <option value="retiro">Retiros</option>
                        <option value="pago">Pagos</option>
                    </select>
                </div>
                <div class="filter-group">
                    <label for="fechaDesde">Desde:</label>
                    <input type="date" id="fechaDesde">
                </div>
                <div class="filter-group">
                    <label for="fechaHasta">Hasta:</label>
                    <input type="date" id="fechaHasta">
                </div>
            </div>
        </div>
    `;
    
    historialSection.insertAdjacentHTML('afterbegin', filtersHTML);
    return historialSection.querySelector('.filtros-container');
}

function setupDatePickers() {
    // Configurar fechas por defecto (último mes)
    const fechaHasta = document.getElementById('fechaHasta');
    const fechaDesde = document.getElementById('fechaDesde');
    
    if (fechaHasta) {
        fechaHasta.value = new Date().toISOString().split('T')[0];
    }
    
    if (fechaDesde) {
        const lastMonth = new Date();
        lastMonth.setMonth(lastMonth.getMonth() - 1);
        fechaDesde.value = lastMonth.toISOString().split('T')[0];
    }
}

function setupPagination() {
    const verMasBtn = document.querySelector('.btn-ver-mas');
    if (verMasBtn) {
        verMasBtn.addEventListener('click', function() {
            currentPage++;
            loadTransactionsData();
        });
    }
}

function updatePagination(totalItems) {
    totalPages = Math.ceil(totalItems / pageSize);
    
    const verMasBtn = document.querySelector('.btn-ver-mas');
    if (verMasBtn) {
        if (currentPage >= totalPages) {
            verMasBtn.style.display = 'none';
        } else {
            verMasBtn.style.display = 'block';
            verMasBtn.textContent = `Ver más (${totalItems - (currentPage * pageSize)} restantes)`;
        }
    }
}

async function applyFilters() {
    currentPage = 1; // Reset a primera página
    await loadTransactionsData();
}

function resetFilters() {
    currentFilters = {
        fechaDesde: null,
        fechaHasta: null,
        tipoTransaccion: 'todos',
        searchTerm: '',
        ordenarPor: 'fecha',
        orden: 'desc'
    };
    
    // Limpiar inputs
    const searchInput = document.getElementById('searchTransactions');
    const tipoSelect = document.getElementById('tipoTransaccion');
    const fechaDesde = document.getElementById('fechaDesde');
    const fechaHasta = document.getElementById('fechaHasta');
    
    if (searchInput) searchInput.value = '';
    if (tipoSelect) tipoSelect.value = 'todos';
    if (fechaDesde) fechaDesde.value = '';
    if (fechaHasta) fechaHasta.value = '';
    
    applyFilters();
}

// Función para exportar a CSV
async function exportToCSV() {
    try {
        const user = authService.getCurrentUser();
        if (!user) return;

        // Obtener todas las transacciones (sin paginación)
        const response = await fetch(
            `http://localhost:5202/api/banco/transacciones-recientes?page=1&pageSize=10000`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            const transacciones = data.items || data;
            
            // Crear CSV
            const csvContent = generateCSV(transacciones);
            downloadCSV(csvContent, `historial_transacciones_${new Date().toISOString().split('T')[0]}.csv`);
        }
    } catch (error) {
        console.error('Error exporting to CSV:', error);
        showError('Error al exportar datos');
    }
}

function generateCSV(transacciones) {
    const headers = ['Fecha', 'Descripción', 'Tipo', 'Monto', 'Estado'];
    const csvRows = [headers.join(',')];
    
    transacciones.forEach(trans => {
        const row = [
            formatDate(trans.fecha),
            `"${trans.descripcion || trans.tipo_transaccion || ''}"`,
            `"${trans.tipo_transaccion || ''}"`,
            trans.monto || 0,
            `"${trans.estado || 'Completado'}"`
        ];
        csvRows.push(row.join(','));
    });
    
    return csvRows.join('\n');
}

function downloadCSV(csvContent, filename) {
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    
    if (link.download !== undefined) {
        const url = URL.createObjectURL(blob);
        link.setAttribute('href', url);
        link.setAttribute('download', filename);
        link.style.visibility = 'hidden';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
}

// Funciones auxiliares
function isIngresoTransaction(trans) {
    const tipo = (trans.tipo_transaccion || '').toLowerCase();
    const descripcion = (trans.descripcion || '').toLowerCase();
    
    return tipo.includes('deposito') || tipo.includes('ingreso') || 
           descripcion.includes('deposito') || descripcion.includes('recibida') ||
           descripcion.includes('nomina') || descripcion.includes('sueldo');
}

function formatMoney(amount) {
    if (amount === null || amount === undefined) return '0.00';
    return parseFloat(amount).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

function formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('es-ES', {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    });
}

function showLoading(show) {
    const tableBody = document.querySelector('.historial-tabla tbody');
    if (!tableBody) return;
    
    if (show) {
        tableBody.innerHTML = `
            <tr>
                <td colspan="4" class="loading-cell">
                    <div class="loading-transaction">
                        <i class="fas fa-spinner fa-spin"></i>
                        <span>Cargando transacciones...</span>
                    </div>
                </td>
            </tr>
        `;
    }
}

function showError(message) {
    console.error(message);
    // TODO: Implementar sistema de notificaciones toast
    alert('Error: ' + message);
}

function displayFallbackTransactions() {
    // Datos de ejemplo si la API falla
    const transaccionesEjemplo = [
        {
            fecha: new Date().toISOString(),
            descripcion: 'Depósito de nómina',
            tipo_transaccion: 'Depósito',
            monto: 1500,
            estado: 'Completado'
        },
        {
            fecha: new Date(Date.now() - 86400000).toISOString(),
            descripcion: 'Compra en tienda',
            tipo_transaccion: 'Compra',
            monto: 200,
            estado: 'Completado'
        },
        {
            fecha: new Date(Date.now() - 172800000).toISOString(),
            descripcion: 'Transferencia enviada',
            tipo_transaccion: 'Transferencia',
            monto: 500,
            estado: 'Completado'
        }
    ];
    
    displayTransactions(transaccionesEjemplo);
}

function generateTransactionChart() {
    const chartCanvas = document.getElementById('transactionChart');
    if (!chartCanvas) return;

    // TODO: Implementar gráfico con Chart.js basado en las transacciones reales
    console.log('Chart generation would be implemented here');
}

// Eventos globales
window.resetFilters = resetFilters;
window.exportToCSV = exportToCSV;
