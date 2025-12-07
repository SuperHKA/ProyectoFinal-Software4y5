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

async function updateUserInfo() {
    try {
        const data = await authService.getClienteInfo();
        // Nueva estructura: { cliente: {...}, cuentas: [...] }
        const cliente = data.cliente || data;
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
        const cuentas = data.cuentas || data; // Compatibilidad con respuesta vieja
        
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
        
        const tipoCuenta = cuenta.tipoCuenta || 'Cuenta';
        const icon = getTipoCuentaIcon(tipoCuenta);
        const last4 = cuenta && cuenta.numeroCuenta ? String(cuenta.numeroCuenta).slice(-4) : '----';

        card.innerHTML = `
            <div class="account-header">
                <span class="account-type">
                    <i class="${icon}"></i>
                    ${tipoCuenta}
                </span>
                <i class="fas fa-ellipsis-h"></i>
            </div>
            <div class="account-balance">$${formatMoney(cuenta.saldo)}</div>
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
        const response = await fetch(
            `http://localhost:5202/api/banco/transacciones-recientes?page=1&pageSize=5`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            displayTransacciones(data.items || []);
        }
    } catch (error) {
        console.error('Error loading transactions:', error);
    }
}

function displayTransacciones(transacciones) {
    const transactionsList = document.querySelector('.transactions-list');
    if (!transactionsList) return;

    if (transacciones.length === 0) {
        transactionsList.innerHTML = '<div class="no-data">No hay transacciones recientes</div>';
        return;
    }

    transactionsList.innerHTML = '';
    
    transacciones.forEach(trans => {
        const item = document.createElement('div');
        item.className = 'transaction-item';
        
        const isIngreso = trans.tipo === 'Deposito' || trans.tipo === 'Transferencia Recibida';
        const icon = isIngreso ? 'fa-arrow-down' : 'fa-arrow-up';
        const amountClass = isIngreso ? 'positive' : 'negative';
        const sign = isIngreso ? '+' : '-';
        
        item.innerHTML = `
            <div class="transaction-icon">
                <i class="fas ${icon}"></i>
            </div>
            <div class="transaction-details">
                <div class="transaction-name">${trans.descripcion || trans.tipo}</div>
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

async function loadMonthlySummary() {
    try {
        const user = authService.getCurrentUser();
        if (!user) return;

        // Obtener transacciones del mes actual
        const now = new Date();
        const firstDay = new Date(now.getFullYear(), now.getMonth(), 1);
        const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0);
        
        const response = await fetch(
            `${authService.baseURL}/banco/transacciones-recientes?page=1&pageSize=1000`,
            {
                headers: {
                    'Authorization': `Bearer ${authService.getAuthToken()}`
                }
            }
        );

        if (response.ok) {
            const data = await response.json();
            const transacciones = data.items || [];
            
            // Calcular ingresos y gastos del mes actual
            let totalIngresos = 0;
            let totalGastos = 0;
            
            transacciones.forEach(trans => {
                const transDate = new Date(trans.fecha);
                // Solo contar transacciones del mes actual
                if (transDate >= firstDay && transDate <= lastDay) {
                    const esIngreso = trans.tipo === 'Deposito' || trans.tipo === 'Transferencia Recibida' || 
                                     trans.descripcion?.toLowerCase().includes('depósito') || 
                                     trans.descripcion?.toLowerCase().includes('recibida');
                    
                    if (esIngreso) {
                        totalIngresos += parseFloat(trans.monto || 0);
                    } else {
                        totalGastos += parseFloat(trans.monto || 0);
                    }
                }
            });
            
            const balance = totalIngresos - totalGastos;
            
            // Actualizar las tarjetas de resumen
            const incomeCard = document.querySelector('.stat-card.income .stat-value');
            const expensesCard = document.querySelector('.stat-card.expenses .stat-value');
            const balanceCard = document.querySelector('.stat-card:not(.income):not(.expenses) .stat-value');
            
            if (incomeCard) incomeCard.textContent = `$${formatMoney(totalIngresos)}`;
            if (expensesCard) expensesCard.textContent = `$${formatMoney(totalGastos)}`;
            if (balanceCard) {
                balanceCard.textContent = `$${formatMoney(balance)}`;
                balanceCard.className = 'stat-value ' + (balance >= 0 ? 'income' : 'expenses');
            }
            
            // Actualizar gráfica si existe
            updateMonthlyChart(transacciones);
        }
    } catch (error) {
        console.error('Error loading monthly summary:', error);
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
                const esIngreso = trans.tipo === 'Deposito' || trans.tipo === 'Transferencia Recibida' || 
                                 trans.descripcion?.toLowerCase().includes('depósito') || 
                                 trans.descripcion?.toLowerCase().includes('recibida');
                
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
