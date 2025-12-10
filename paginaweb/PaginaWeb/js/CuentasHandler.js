// Manejador mejorado de cuentas bancarias para clientes
document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    // Actualizar información del usuario
    await updateUserInfo();

    // Cargar cuentas del cliente
    await loadCuentasCliente();
});

let cuentasData = [];
let showSaldos = true;

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

async function updateUserInfo() {
    try {
        const data = await authService.getClienteInfo();
        const clienteRaw = data.cliente || data;
        const cliente = normalizeClienteData(clienteRaw);
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
            const initials = fullName ? fullName.split(' ').map(n => n[0]).slice(0,2).join('').toUpperCase() : 'U';
            avatar.textContent = initials;
        }
        
    } catch (error) {
        console.warn('No se pudo obtener información del usuario:', error);
    }
}

async function loadCuentasCliente() {
    try {
        showLoading(true);

        const data = await authService.getClienteInfo();
        const cuentasRaw = data.cuentas || data;
        cuentasData = Array.isArray(cuentasRaw) ? cuentasRaw.map(normalizeCuentaData) : [];
        
        if (cuentasData.length > 0) {
            displayCuentas(cuentasData);
            updateCuentasStats(cuentasData);
        } else {
            displayNoCuentas();
        }
        
    } catch (error) {
        console.error('Error loading cuentas:', error);
        showError('Error al cargar las cuentas');
        displayFallbackCuentas();
    } finally {
        showLoading(false);
    }
}

function displayCuentas(cuentas) {
    const container = document.querySelector('.cuentas-grid') || createCuentasGrid();
    if (!container) return;

    container.innerHTML = '';

    cuentas.forEach((cuenta, index) => {
        const card = createCuentaCard(cuenta, index);
        container.appendChild(card);
    });

    // Crear botón para solicitar nueva cuenta
    const newAccountCard = createNewAccountCard();
    container.appendChild(newAccountCard);
}

function createCuentasGrid() {
    const mainContent = document.querySelector('.accounts-content') || 
                       document.querySelector('.main-content') || 
                       document.querySelector('.content-area');
    
    if (!mainContent) return null;

    const gridContainer = document.createElement('div');
    gridContainer.className = 'cuentas-grid';
    
    const section = document.createElement('section');
    section.className = 'cuentas-section';
    section.innerHTML = `
        <div class="section-header">
            <h2><i class="fas fa-university"></i> Mis Cuentas Bancarias</h2>
            <div class="header-actions">
                <button class="btn-toggle-saldos" onclick="toggleSaldosVisibility()">
                    <i class="fas fa-eye"></i> Ocultar Saldos
                </button>
            </div>
        </div>
    `;
    
    section.appendChild(gridContainer);
    mainContent.appendChild(section);
    
    return gridContainer;
}

function createCuentaCard(cuenta, index) {
    const card = document.createElement('div');
    card.className = `cuenta-card ${getAccountTypeClass(cuenta.tipoCuenta)}`;
    card.setAttribute('data-cuenta-id', cuenta.cuentaId || index);
    
    const isActive = cuenta.activa !== false;
    const saldo = parseFloat(cuenta.saldo || 0);
    const tipoCuenta = cuenta.tipoCuenta || 'Cuenta';
    const numeroCuenta = cuenta.numeroCuenta || '****';
    const last4 = String(numeroCuenta).slice(-4);
    
    card.innerHTML = `
        <div class="cuenta-header">
            <div class="cuenta-type">
                <div class="type-icon">
                    <i class="${getAccountIcon(tipoCuenta)}"></i>
                </div>
                <div class="type-info">
                    <h3>${tipoCuenta}</h3>
                    <p>**** **** **** ${last4}</p>
                </div>
            </div>
            <div class="cuenta-status">
                <span class="status-badge ${isActive ? 'active' : 'inactive'}">
                    <i class="fas ${isActive ? 'fa-check-circle' : 'fa-times-circle'}"></i>
                    ${isActive ? 'Activa' : 'Inactiva'}
                </span>
            </div>
        </div>
        
        <div class="cuenta-balance">
            <div class="balance-label">Saldo Disponible</div>
            <div class="balance-amount ${showSaldos ? '' : 'hidden-balance'}">
                ${showSaldos ? `$${formatMoney(saldo)}` : '••••••'}
            </div>
        </div>
        
        <div class="cuenta-actions">
            <button class="btn-action primary" onclick="realizarTransferencia('${numeroCuenta}')">
                <i class="fas fa-paper-plane"></i>
                Transferir
            </button>
            <button class="btn-action secondary" onclick="verMovimientos('${numeroCuenta}')">
                <i class="fas fa-list"></i>
                Movimientos
            </button>
            <button class="btn-action tertiary" onclick="verDetalles(${cuenta.cuentaId || index})">
                <i class="fas fa-info-circle"></i>
                Detalles
            </button>
        </div>
        
        <div class="cuenta-info-extra">
            <div class="info-item">
                <span class="info-label">Tipo de Cuenta:</span>
                <span class="info-value">${tipoCuenta}</span>
            </div>
            <div class="info-item">
                <span class="info-label">Número Completo:</span>
                <span class="info-value">${numeroCuenta}</span>
            </div>
            ${cuenta.limite ? `
                <div class="info-item">
                    <span class="info-label">Límite Diario:</span>
                    <span class="info-value">$${formatMoney(cuenta.limite)}</span>
                </div>
            ` : ''}
        </div>
    `;
    
    return card;
}

function createNewAccountCard() {
    const card = document.createElement('div');
    card.className = 'cuenta-card new-account-card';
    
    card.innerHTML = `
        <div class="new-account-content">
            <div class="new-account-icon">
                <i class="fas fa-plus-circle"></i>
            </div>
            <h3>Solicitar Nueva Cuenta</h3>
            <p>Abre una nueva cuenta bancaria y accede a más beneficios</p>
            <button class="btn-new-account" onclick="solicitarNuevaCuenta()">
                <i class="fas fa-plus"></i>
                Solicitar Cuenta
            </button>
        </div>
    `;
    
    return card;
}

function updateCuentasStats(cuentas) {
    const totalCuentas = cuentas.length;
    const cuentasActivas = cuentas.filter(c => c.activa !== false).length;
    const saldoTotal = cuentas.reduce((sum, c) => sum + (parseFloat(c.saldo || 0)), 0);
    
    // Actualizar stats en el header si existe
    updateStatsDisplay(totalCuentas, cuentasActivas, saldoTotal);
}

function updateStatsDisplay(total, activas, saldoTotal) {
    // Crear o actualizar panel de estadísticas
    let statsContainer = document.querySelector('.cuentas-stats');
    
    if (!statsContainer) {
        statsContainer = document.createElement('div');
        statsContainer.className = 'cuentas-stats';
        
        const section = document.querySelector('.cuentas-section');
        if (section) {
            section.insertBefore(statsContainer, section.querySelector('.cuentas-grid'));
        }
    }
    
    statsContainer.innerHTML = `
        <div class="stats-grid">
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-university"></i>
                </div>
                <div class="stat-info">
                    <div class="stat-value">${total}</div>
                    <div class="stat-label">Total Cuentas</div>
                </div>
            </div>
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-check-circle"></i>
                </div>
                <div class="stat-info">
                    <div class="stat-value">${activas}</div>
                    <div class="stat-label">Cuentas Activas</div>
                </div>
            </div>
            <div class="stat-item">
                <div class="stat-icon">
                    <i class="fas fa-dollar-sign"></i>
                </div>
                <div class="stat-info">
                    <div class="stat-value ${showSaldos ? '' : 'hidden-balance'}">
                        ${showSaldos ? `$${formatMoney(saldoTotal)}` : '••••••'}
                    </div>
                    <div class="stat-label">Saldo Total</div>
                </div>
            </div>
        </div>
    `;
}

function toggleSaldosVisibility() {
    showSaldos = !showSaldos;
    
    // Actualizar todas las tarjetas
    const balanceAmounts = document.querySelectorAll('.balance-amount');
    const toggleBtn = document.querySelector('.btn-toggle-saldos');
    
    balanceAmounts.forEach(amount => {
        if (showSaldos) {
            amount.classList.remove('hidden-balance');
            const cuentaId = amount.closest('.cuenta-card').getAttribute('data-cuenta-id');
            const cuenta = cuentasData[parseInt(cuentaId)] || cuentasData.find(c => (c['ID Cuenta'] || c.cuentaId) == cuentaId);
            if (cuenta) {
                amount.textContent = `$${formatMoney(parseFloat(cuenta.saldo || 0))}`;
            }
        } else {
            amount.classList.add('hidden-balance');
            amount.textContent = '••••••';
        }
    });
    
    // Actualizar stats
    if (cuentasData.length > 0) {
        updateCuentasStats(cuentasData);
    }
    
    // Actualizar botón
    if (toggleBtn) {
        toggleBtn.innerHTML = showSaldos ? 
            '<i class="fas fa-eye-slash"></i> Ocultar Saldos' : 
            '<i class="fas fa-eye"></i> Mostrar Saldos';
    }
}

async function solicitarNuevaCuenta() {
    // Crear modal para solicitar nueva cuenta
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.innerHTML = `
        <div class="modal-content">
            <div class="modal-header">
                <h3>Solicitar Nueva Cuenta</h3>
                <button class="close-modal" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <form id="nuevaCuentaForm">
                    <div class="form-group">
                        <label for="tipoCuenta">Tipo de Cuenta:</label>
                        <select id="tipoCuenta" required>
                            <option value="">Seleccionar tipo</option>
                            <option value="Corriente">Cuenta Corriente</option>
                            <option value="Ahorro">Cuenta de Ahorro</option>
                            <option value="Dolares">Cuenta en Dólares</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label for="saldoInicial">Depósito Inicial (Opcional):</label>
                        <input type="number" id="saldoInicial" min="0" step="0.01" placeholder="0.00">
                    </div>
                    <div class="form-group">
                        <label for="motivoSolicitud">Motivo de la Solicitud:</label>
                        <textarea id="motivoSolicitud" rows="3" placeholder="Explique brevemente por qué necesita esta cuenta..."></textarea>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn-secondary" onclick="closeModal()">Cancelar</button>
                <button type="button" class="btn-primary" onclick="submitSolicitudCuenta()">
                    <i class="fas fa-paper-plane"></i> Enviar Solicitud
                </button>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    modal.style.display = 'flex';
}

async function submitSolicitudCuenta() {
    const form = document.getElementById('nuevaCuentaForm');
    const formData = new FormData(form);
    const tipoCuenta = document.getElementById('tipoCuenta').value;
    const saldoInicial = parseFloat(document.getElementById('saldoInicial').value) || 0;
    const motivo = document.getElementById('motivoSolicitud').value;
    
    if (!tipoCuenta) {
        alert('Por favor seleccione un tipo de cuenta');
        return;
    }
    
    try {
        // Por ahora solo simular la solicitud (la API no tiene endpoint específico)
        showLoading(true);
        
        // Simular delay
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        closeModal();
        showSuccess(`Solicitud de ${tipoCuenta} enviada exitosamente. Te contactaremos pronto.`);
        
        // TODO: Implementar cuando esté el endpoint real
        // const result = await apiClient.post('/banco/solicitar-cuenta', {
        //     tipoCuenta, saldoInicial, motivo
        // });
        
    } catch (error) {
        console.error('Error submitting account request:', error);
        showError('Error al enviar la solicitud');
    } finally {
        showLoading(false);
    }
}

function realizarTransferencia(numeroCuenta) {
    // Redirigir a página de transferencias con cuenta pre-seleccionada
    sessionStorage.setItem('preselectedAccount', numeroCuenta);
    window.location.href = 'transferencias.html';
}

function verMovimientos(numeroCuenta) {
    // Redirigir a historial con filtro de cuenta
    sessionStorage.setItem('filterAccount', numeroCuenta);
    window.location.href = 'Historial.html';
}

function verDetalles(cuentaId) {
    const cuenta = cuentasData.find(c => (c['ID Cuenta'] || c.cuentaId) == cuentaId) || cuentasData[parseInt(cuentaId)];
    if (!cuenta) return;
    
    // Extraer valores con fallback a nuevos aliases primero
    const tipoCuenta = cuenta['Tipo de Cuenta'] || cuenta.tipoCuenta || 'Cuenta';
    const numeroCuenta = cuenta['Número de Cuenta'] || cuenta.numeroCuenta || '****';
    const saldo = cuenta.Saldo || cuenta.saldo || 0;
    const activa = cuenta.Activa !== undefined ? cuenta.Activa : (cuenta.activa !== false);
    
    // Crear modal con detalles completos
    const modal = document.createElement('div');
    modal.className = 'modal';
    modal.innerHTML = `
        <div class="modal-content large">
            <div class="modal-header">
                <h3>Detalles de Cuenta - ${tipoCuenta}</h3>
                <button class="close-modal" onclick="closeModal()">&times;</button>
            </div>
            <div class="modal-body">
                <div class="cuenta-detalles">
                    <div class="detalle-section">
                        <h4>Información General</h4>
                        <div class="detalle-grid">
                            <div class="detalle-item">
                                <span class="detalle-label">Tipo de Cuenta:</span>
                                <span class="detalle-value">${tipoCuenta}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Número de Cuenta:</span>
                                <span class="detalle-value">${numeroCuenta}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Estado:</span>
                                <span class="detalle-value">
                                    <span class="status-badge ${activa ? 'active' : 'inactive'}">
                                        ${activa ? 'Activa' : 'Inactiva'}
                                    </span>
                                </span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Saldo Actual:</span>
                                <span class="detalle-value balance-highlight">$${formatMoney(parseFloat(saldo))}</span>
                            </div>
                        </div>
                    </div>
                    
                    <div class="detalle-section">
                        <h4>Límites y Restricciones</h4>
                        <div class="detalle-grid">
                            <div class="detalle-item">
                                <span class="detalle-label">Límite Diario:</span>
                                <span class="detalle-value">$${formatMoney(cuenta.limite || 5000)}</span>
                            </div>
                            <div class="detalle-item">
                                <span class="detalle-label">Límite Mensual:</span>
                                <span class="detalle-value">$${formatMoney((cuenta.limite || 5000) * 30)}</span>
                            </div>
                        </div>
                    </div>
                    
                    <div class="detalle-section">
                        <h4>Acciones Disponibles</h4>
                        <div class="acciones-grid">
                            <button class="btn-accion" onclick="realizarTransferencia('${numeroCuenta}'); closeModal();">
                                <i class="fas fa-paper-plane"></i>
                                Realizar Transferencia
                            </button>
                            <button class="btn-accion" onclick="verMovimientos('${numeroCuenta}'); closeModal();">
                                <i class="fas fa-list"></i>
                                Ver Movimientos
                            </button>
                            <button class="btn-accion" onclick="descargarEstadoCuenta('${numeroCuenta}');">
                                <i class="fas fa-file-pdf"></i>
                                Descargar Estado
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    document.body.appendChild(modal);
    modal.style.display = 'flex';
}

async function descargarEstadoCuenta(numeroCuenta) {
    try {
        // Simular descarga de estado de cuenta
        showLoading(true);
        
        // TODO: Implementar descarga real desde la API
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // Simular descarga
        const link = document.createElement('a');
        link.href = 'data:application/pdf;base64,JVBERi0xLjQ...'; // Base64 del PDF
        link.download = `estado_cuenta_${numeroCuenta}_${new Date().toISOString().split('T')[0]}.pdf`;
        link.click();
        
        showSuccess('Estado de cuenta descargado exitosamente');
        
    } catch (error) {
        console.error('Error downloading account statement:', error);
        showError('Error al descargar el estado de cuenta');
    } finally {
        showLoading(false);
    }
}

// Funciones auxiliares
function getAccountTypeClass(tipoCuenta) {
    const typeMap = {
        'Corriente': 'corriente',
        'Ahorro': 'ahorro', 
        'Dolares': 'dolares',
        'Dólares': 'dolares'
    };
    return typeMap[tipoCuenta] || 'default';
}

function getAccountIcon(tipoCuenta) {
    const iconMap = {
        'Corriente': 'fas fa-university',
        'Ahorro': 'fas fa-piggy-bank',
        'Dolares': 'fas fa-dollar-sign',
        'Dólares': 'fas fa-dollar-sign'
    };
    return iconMap[tipoCuenta] || 'fas fa-credit-card';
}

function formatMoney(amount) {
    if (amount === null || amount === undefined) return '0.00';
    return parseFloat(amount).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

function displayNoCuentas() {
    const container = document.querySelector('.cuentas-grid') || createCuentasGrid();
    if (!container) return;
    
    container.innerHTML = `
        <div class="no-cuentas">
            <div class="no-data-icon">
                <i class="fas fa-university"></i>
            </div>
            <h3>No tienes cuentas bancarias</h3>
            <p>Solicita tu primera cuenta para comenzar a usar nuestros servicios</p>
            <button class="btn-primary" onclick="solicitarNuevaCuenta()">
                <i class="fas fa-plus"></i>
                Solicitar Primera Cuenta
            </button>
        </div>
    `;
}

function displayFallbackCuentas() {
    // Mostrar cuentas de ejemplo si la API falla
    const cuentasEjemplo = [
        {
            cuentaId: 1,
            numeroCuenta: '1234567890',
            tipoCuenta: 'Corriente',
            saldo: 1500.75,
            activa: true,
            limite: 5000
        },
        {
            cuentaId: 2,
            numeroCuenta: '0987654321',
            tipoCuenta: 'Ahorro',
            saldo: 2500.00,
            activa: true,
            limite: 3000
        }
    ];
    
    cuentasData = cuentasEjemplo;
    displayCuentas(cuentasEjemplo);
    updateCuentasStats(cuentasEjemplo);
}

function showLoading(show) {
    // TODO: Implementar indicador de carga
    if (show) {
        console.log('Loading...');
    }
}

function showError(message) {
    alert('Error: ' + message);
}

function showSuccess(message) {
    alert('Éxito: ' + message);
}

function closeModal() {
    const modal = document.querySelector('.modal');
    if (modal) {
        modal.remove();
    }
}

// Eventos globales
window.toggleSaldosVisibility = toggleSaldosVisibility;
window.solicitarNuevaCuenta = solicitarNuevaCuenta;
window.submitSolicitudCuenta = submitSolicitudCuenta;
window.realizarTransferencia = realizarTransferencia;
window.verMovimientos = verMovimientos;
window.verDetalles = verDetalles;
window.descargarEstadoCuenta = descargarEstadoCuenta;
window.closeModal = closeModal;