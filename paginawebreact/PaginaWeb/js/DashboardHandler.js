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
        const cliente = data.cliente || data || (Array.isArray(data) ? data[0] : null);
        const displayName = (cliente && (cliente.nombreCompleto || cliente.username)) || (authService.getCurrentUser() && authService.getCurrentUser().username) || 'Usuario';

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
        if (avatar && cliente && cliente.nombreCompleto) {
            const initials = cliente.nombreCompleto.split(' ').map(n => n[0]).slice(0,2).join('').toUpperCase();
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

        // Obtener cuentas del usuario
        const cuentas = await authService.getCuentas();
        
        if (cuentas && cuentas.length > 0) {
            displayCuentas(cuentas);
        } else {
            displayNoCuentas();
        }

        // Cargar transacciones recientes
        await loadTransaccionesRecientes();

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
