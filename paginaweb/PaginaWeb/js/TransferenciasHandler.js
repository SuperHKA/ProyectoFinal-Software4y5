// Manejador de transferencias mejorado con transferencias externas
document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    // Configurar formulario
    await loadCuentasUsuario();
    setupTransferForm();
    setupExternalTransfers();
    loadContactosFrecuentes();
    setupLimitesTransferencia();
    
    // Pre-seleccionar cuenta si viene de otra página
    checkPreselectedAccount();
    
    // Cargar transferencias recientes
    await loadTransferenciasRecientes();
});

let cuentasUsuario = [];
let limitesTransferencia = {
    diario: 5000,
    usado_hoy: 0,
    semanal: 15000,
    usado_semanal: 0,
    mensual: 50000,
    usado_mensual: 0
};

async function loadCuentasUsuario() {
    try {
        const data = await authService.getClienteInfo();
        const cuentasRaw = data.cuentas || data;
        cuentasUsuario = Array.isArray(cuentasRaw) ? cuentasRaw.map(normalizeCuentaData) : [];
        
        const selectOrigen = document.getElementById('fromAccount');
        if (selectOrigen && cuentasUsuario && cuentasUsuario.length > 0) {
            selectOrigen.innerHTML = '<option value="">Selecciona una cuenta</option>';
            cuentasUsuario.forEach(cuenta => {
                const option = document.createElement('option');
                const cuentaId = cuenta.cuentaId;
                option.value = cuentaId; // Usar el ID, no el número
                const tipoCuenta = cuenta.tipoCuenta;
                const numeroCuenta = cuenta.numeroCuenta;
                const saldo = cuenta.saldo;
                option.textContent = `${tipoCuenta} - ${numeroCuenta} ($${formatMoney(saldo)})`;
                option.dataset.saldo = saldo;
                option.dataset.numeroCuenta = numeroCuenta;
                option.dataset.cuentaId = cuentaId;
                selectOrigen.appendChild(option);
            });
        }
        
        // Actualizar información de límites
        updateLimitInfo();
        
    } catch (error) {
        console.error('Error loading cuentas:', error);
        showError('Error al cargar las cuentas');
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
        tipo_transaccion: transaccion['Tipo Transaccion'] || transaccion.tipo_transaccion || transaccion.tipoTransaccion,
        tipoTransaccion: transaccion['Tipo Transaccion'] || transaccion.tipo_transaccion || transaccion.tipoTransaccion,
        monto: parseFloat(transaccion['Monto'] || transaccion.monto || 0),
        fecha: transaccion['Fecha'] || transaccion.fecha,
        descripcion: transaccion['Descripcion'] || transaccion.descripcion,
        estado: transaccion['Estado'] || transaccion.estado || 'Completado',
        cuenta_origen: transaccion['Cuenta Origen'] || transaccion.cuenta_origen || transaccion.cuentaOrigen,
        cuentaOrigen: transaccion['Cuenta Origen'] || transaccion.cuenta_origen || transaccion.cuentaOrigen,
        cuenta_destino: transaccion['Cuenta Destino'] || transaccion.cuenta_destino || transaccion.cuentaDestino,
        cuentaDestino: transaccion['Cuenta Destino'] || transaccion.cuenta_destino || transaccion.cuentaDestino,
        clienteDestino: transaccion['Cliente Destino'] || transaccion.clienteDestino || transaccion.cliente_destino,
        ...transaccion
    };
}

function setupTransferForm() {
    const form = document.getElementById('transferForm');
    const transferTypeSelect = document.getElementById('transferType');
    const fromAccountSelect = document.getElementById('fromAccount');
    const toAccountInput = document.getElementById('toAccount');
    const amountInput = document.getElementById('amount');
    
    if (!form) return;

    // Manejar cambio de tipo de transferencia
    if (transferTypeSelect) {
        transferTypeSelect.addEventListener('change', function() {
            handleTransferTypeChange(this.value);
        });
    }
    
    // Manejar cambio de cuenta origen
    if (fromAccountSelect) {
        fromAccountSelect.addEventListener('change', function() {
            updateAvailableBalance(this.value);
        });
    }
    
    // Validación en tiempo real del monto
    if (amountInput) {
        amountInput.addEventListener('input', function() {
            validateTransferAmount(this.value);
        });
    }
    
    // Validación de cuenta destino
    if (toAccountInput) {
        toAccountInput.addEventListener('blur', function() {
            validateDestinationAccount(this.value);
        });
    }

    // Configurar eventos de pasos del formulario
    setupFormSteps();
    
    // Prevenir envío del formulario (lo manejaremos con el botón confirmar)
    form.addEventListener('submit', function(e) {
        e.preventDefault();
    });
}

function setupExternalTransfers() {
    // Configurar validación especial para transferencias externas
    const transferTypeSelect = document.getElementById('transferType');
    
    if (transferTypeSelect) {
        transferTypeSelect.addEventListener('change', function() {
            const isExternal = this.value === 'other-bank' || this.value === 'international';
            // TODO: Implementar showExternalTransferInfo cuando sea necesario
            // showExternalTransferInfo(isExternal);
        });
    }
}

function handleTransferTypeChange(tipoTransferencia) {
    const toAccountInput = document.getElementById('toAccount');
    const toAccountLabel = document.querySelector('label[for="toAccount"]');
    
    if (!toAccountInput || !toAccountLabel) return;
    
    switch(tipoTransferencia) {
        case 'same-bank':
            toAccountLabel.textContent = 'Cuenta de Destino (Mismo Banco)';
            toAccountInput.placeholder = 'Número de cuenta (8-12 dígitos)';
            toAccountInput.setAttribute('pattern', '[0-9]{8,12}');
            toAccountInput.setAttribute('maxlength', '12');
            showTransferInfo('internal');
            break;
            
        case 'other-bank':
            toAccountLabel.textContent = 'Cuenta de Destino (Otro Banco)';
            toAccountInput.placeholder = 'CBU o número de cuenta (20-22 dígitos)';
            toAccountInput.setAttribute('pattern', '[0-9]{20,22}');
            toAccountInput.setAttribute('maxlength', '22');
            showTransferInfo('external');
            break;
            
        case 'international':
            toAccountLabel.textContent = 'Cuenta Internacional';
            toAccountInput.placeholder = 'IBAN o número de cuenta internacional';
            toAccountInput.removeAttribute('pattern');
            toAccountInput.setAttribute('maxlength', '34');
            showTransferInfo('international');
            break;
    }
}

function showTransferInfo(tipo) {
    // Crear o actualizar panel de información
    let infoPanel = document.querySelector('.transfer-info-panel');
    
    if (!infoPanel) {
        infoPanel = document.createElement('div');
        infoPanel.className = 'transfer-info-panel';
        
        const step1 = document.getElementById('step1');
        if (step1) {
            step1.appendChild(infoPanel);
        }
    }
    
    const infoContent = getTransferInfoContent(tipo);
    infoPanel.innerHTML = infoContent;
}

function getTransferInfoContent(tipo) {
    switch(tipo) {
        case 'internal':
            return `
                <div class="info-content">
                    <div class="info-icon"><i class="fas fa-university"></i></div>
                    <div class="info-text">
                        <h4>Transferencia Interna</h4>
                        <ul>
                            <li>✓ Procesamiento inmediato</li>
                            <li>✓ Sin comisiones</li>
                            <li>✓ Disponible 24/7</li>
                            <li>✓ Límite diario: $${formatMoney(limitesTransferencia.diario)}</li>
                        </ul>
                    </div>
                </div>
            `;
            
        case 'external':
            return `
                <div class="info-content">
                    <div class="info-icon"><i class="fas fa-exchange-alt"></i></div>
                    <div class="info-text">
                        <h4>Transferencia Externa</h4>
                        <ul>
                            <li>⚡ Procesamiento: 1-24 horas</li>
                            <li>💰 Comisión: $2.50 + 0.5%</li>
                            <li>📅 Lunes a viernes 8:00-18:00</li>
                            <li>📊 Límite diario: $${formatMoney(limitesTransferencia.diario * 0.8)}</li>
                        </ul>
                    </div>
                </div>
            `;
            
        case 'international':
            return `
                <div class="info-content">
                    <div class="info-icon"><i class="fas fa-globe"></i></div>
                    <div class="info-text">
                        <h4>Transferencia Internacional</h4>
                        <ul>
                            <li>🌍 Procesamiento: 3-5 días hábiles</li>
                            <li>💰 Comisión: $15.00 + 2%</li>
                            <li>📋 Documentación adicional requerida</li>
                            <li>📊 Límite diario: $${formatMoney(limitesTransferencia.diario * 0.5)}</li>
                        </ul>
                    </div>
                </div>
            `;
            
        default:
            return '';
    }
}

function validateTransferAmount(amount) {
    const amountValue = parseFloat(amount);
    const fromAccount = document.getElementById('fromAccount').value;
    const transferType = document.getElementById('transferType').value;
    const amountInput = document.getElementById('amount');
    
    if (!amountInput) return;
    
    // Limpiar clases de validación anteriores
    amountInput.classList.remove('valid', 'invalid');
    
    if (isNaN(amountValue) || amountValue <= 0) {
        showAmountValidation('Ingresa un monto válido', 'error');
        amountInput.classList.add('invalid');
        return false;
    }
    
    // Obtener saldo de la cuenta seleccionada
    const selectedAccountRaw = cuentasUsuario.find(c => {
        const id = c['ID Cuenta'] || c.cuentaId || c.cuenta_id || c.cuentaID;
        return id == fromAccount;
    });
    const selectedAccount = selectedAccountRaw ? normalizeCuentaData(selectedAccountRaw) : null;
    const saldo = selectedAccount ? selectedAccount.saldo : 0;
    if (selectedAccount && amountValue > saldo) {
        showAmountValidation('Saldo insuficiente', 'error');
        amountInput.classList.add('invalid');
        return false;
    }
    
    // Verificar límites según tipo de transferencia
    const limite = getLimiteByType(transferType);
    if (amountValue > limite) {
        showAmountValidation(`Excede el límite de $${formatMoney(limite)}`, 'error');
        amountInput.classList.add('invalid');
        return false;
    }
    
    // Verificar límite diario usado
    if (limitesTransferencia.usado_hoy + amountValue > limitesTransferencia.diario) {
        const disponible = limitesTransferencia.diario - limitesTransferencia.usado_hoy;
        showAmountValidation(`Límite diario excedido. Disponible: $${formatMoney(disponible)}`, 'error');
        amountInput.classList.add('invalid');
        return false;
    }
    
    // Todo válido
    amountInput.classList.add('valid');
    showAmountValidation('Monto válido', 'success');
    return true;
}

function getLimiteByType(transferType) {
    switch(transferType) {
        case 'same-bank':
            return limitesTransferencia.diario;
        case 'other-bank':
            return limitesTransferencia.diario * 0.8;
        case 'international':
            return limitesTransferencia.diario * 0.5;
        default:
            return limitesTransferencia.diario;
    }
}

function showAmountValidation(message, type) {
    let validationMessage = document.querySelector('.amount-validation');
    
    if (!validationMessage) {
        validationMessage = document.createElement('div');
        validationMessage.className = 'amount-validation';
        
        const amountInput = document.getElementById('amount');
        if (amountInput && amountInput.parentNode) {
            amountInput.parentNode.appendChild(validationMessage);
        }
    }
    
    validationMessage.textContent = message;
    validationMessage.className = `amount-validation ${type}`;
}

function validateDestinationAccount(accountNumber) {
    const transferType = document.getElementById('transferType').value;
    const toAccountInput = document.getElementById('toAccount');
    
    if (!toAccountInput || !accountNumber) return false;
    
    // Limpiar validaciones anteriores
    toAccountInput.classList.remove('valid', 'invalid');
    
    let isValid = false;
    let message = '';
    
    switch(transferType) {
        case 'same-bank':
            isValid = /^[0-9]{8,12}$/.test(accountNumber);
            message = isValid ? 'Número de cuenta válido' : 'Formato de cuenta inválido (8-12 dígitos)';
            break;
            
        case 'other-bank':
            isValid = /^[0-9]{12,22}$/.test(accountNumber);
            message = isValid ? 'CBU válido' : 'Formato de CBU inválido (12-22 dígitos)';
            break;
            
        case 'international':
            // Validación básica para IBAN (simplificada)
            isValid = /^[A-Z]{2}[0-9]{2}[A-Z0-9]{4,30}$/.test(accountNumber.toUpperCase());
            message = isValid ? 'IBAN válido' : 'Formato de IBAN inválido';
            break;
            
        default:
            isValid = true; // Si no hay tipo específico, aceptar cualquier formato
            break;
    }
    
    toAccountInput.classList.add(isValid ? 'valid' : 'invalid');
    showAccountValidation(message, isValid ? 'success' : 'error');
    
    return isValid;
}

function showAccountValidation(message, type) {
    let validationMessage = document.querySelector('.account-validation');
    
    if (!validationMessage) {
        validationMessage = document.createElement('div');
        validationMessage.className = 'account-validation';
        
        const toAccountInput = document.getElementById('toAccount');
        if (toAccountInput && toAccountInput.parentNode) {
            toAccountInput.parentNode.appendChild(validationMessage);
        }
    }
    
    validationMessage.textContent = message;
    validationMessage.className = `account-validation ${type}`;
}

// Buscar el ID de una cuenta por su número (igual que en transacciones.js)
async function buscarCuentaIdPorNumero(numeroCuenta) {
    try {
        const token = authService.getAuthToken();
        console.log('Buscando cuenta con número:', numeroCuenta);

        const response = await fetch(`${authService.baseURL}/banco/cuentas`, {
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
            c.numeroCuenta === numeroCuenta
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

async function realizarTransferenciaInterna(formData) {
    try {
        const user = authService.getCurrentUser();
        if (!user) {
            throw new Error('Usuario no autenticado');
        }

        console.log('Buscando cuenta de destino...');
        const cuentaDestinoId = await buscarCuentaIdPorNumero(formData.toAccount);
        
        if (!cuentaDestinoId) {
            return {
                success: false,
                message: 'La cuenta de destino no existe o no está activa',
                data: null
            };
        }

        console.log('✅ Cuenta destino encontrada ID:', cuentaDestinoId);

        const transferData = {
            cuentaOrigen: formData.fromAccount.toString(),
            cuentaDestino: cuentaDestinoId.toString(),
            monto: parseFloat(formData.amount),
            descripcion: formData.description,
            usuarioId: user.id || user.clienteId,
            username: user.username
        };

        console.log('Enviando transferencia interna a API:', transferData);
        console.log('URL:', `${authService.baseURL}/banco/transfer/internal`);

        const response = await fetch(`${authService.baseURL}/banco/transfer/internal`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authService.getAuthToken()}`
            },
            body: JSON.stringify(transferData),
        });

        console.log('Status de respuesta:', response.status, response.statusText);
        
        let data;
        try {
            data = await response.json();
            console.log('Respuesta de la API:', data);
        } catch (jsonError) {
            console.error('❌ Error parseando JSON:', jsonError);
            data = { message: 'Error al procesar la respuesta del servidor' };
        }
        
        console.log('✅ response.ok:', response.ok);
        
        return {
            success: response.ok,
            message: data.message || (response.ok ? 'Transferencia realizada exitosamente' : 'Error en la transferencia'),
            data: data
        };
        
    } catch (error) {
        console.error('❌ Error en realizarTransferenciaInterna:', error);
        return {
            success: false,
            message: error.message || 'Error de conexión con el servidor',
            data: null
        };
    }
}

async function realizarTransferenciaExterna(formData) {
    try {
        const user = authService.getCurrentUser();
        if (!user) {
            throw new Error('Usuario no autenticado');
        }

        const transferData = {
            cuentaOrigen: formData.fromAccount.toString(),
            cuentaDestino: formData.toAccount.toString(),
            monto: parseFloat(formData.amount),
            descripcion: formData.description,
            tipoTransferencia: formData.transferType,
            usuarioId: user.id || user.clienteId,
            username: user.username
        };

        const response = await fetch(`${authService.baseURL}/banco/transfer/external`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${authService.getAuthToken()}`
            },
            body: JSON.stringify(transferData),
        });

        const data = await response.json();
        
        return {
            success: response.ok,
            message: data.message || 'Transferencia externa enviada exitosamente',
            data: data
        };
        
    } catch (error) {
        throw error;
    }
}

function getFormData() {
    return {
        fromAccount: document.getElementById('fromAccount').value,
        toAccount: document.getElementById('toAccount').value,
        amount: document.getElementById('amount').value,
        transferType: document.getElementById('transferType').value,
        description: document.getElementById('description').value,
        reference: document.getElementById('reference')?.value || '',
        scheduleDate: document.getElementById('scheduleDate')?.value || null
    };
}

function validateFormData(formData) {
    // Validaciones básicas
    if (!formData.fromAccount) {
        showError('Selecciona una cuenta de origen');
        return false;
    }

    if (!formData.toAccount) {
        showError('Ingresa la cuenta de destino');
        return false;
    }

    if (formData.fromAccount === formData.toAccount && formData.transferType === 'same-bank') {
        showError('Las cuentas de origen y destino deben ser diferentes');
        return false;
    }

    if (!formData.amount || parseFloat(formData.amount) <= 0) {
        showError('Ingresa un monto válido');
        return false;
    }

    if (!formData.transferType) {
        showError('Selecciona el tipo de transferencia');
        return false;
    }

    if (!formData.description) {
        showError('Ingresa una descripción');
        return false;
    }

    // Validaciones específicas según tipo
    if (!validateTransferAmount(formData.amount)) {
        return false;
    }

    if (!validateDestinationAccount(formData.toAccount)) {
        return false;
    }

    return true;
}

function validateCurrentStep(stepNumber) {
    if (stepNumber === 1) {
        const fromAccount = document.getElementById('fromAccount').value;
        const toAccount = document.getElementById('toAccount').value;
        const amount = document.getElementById('amount').value;
        const transferType = document.getElementById('transferType').value;
        
        if (!fromAccount) {
            showError('Selecciona una cuenta de origen');
            return false;
        }
        
        if (!toAccount) {
            showError('Ingresa la cuenta de destino');
            return false;
        }
        
        if (!transferType) {
            showError('Selecciona el tipo de transferencia');
            return false;
        }
        
        if (!amount || parseFloat(amount) <= 0) {
            showError('Ingresa un monto válido');
            return false;
        }
        
        const selectedAccountRaw = cuentasUsuario.find(c => {
            const id = c['ID Cuenta'] || c.cuentaId || c.cuenta_id || c.cuentaID;
            return id == fromAccount;
        });
        if (selectedAccountRaw) {
            const selectedAccount = normalizeCuentaData(selectedAccountRaw);
            const saldo = selectedAccount.saldo;
            if (parseFloat(amount) > parseFloat(saldo)) {
                showError(`Saldo insuficiente. Disponible: $${formatMoney(saldo)}`);
                return false;
            }
        }
        
        if (transferType === 'same-bank' && fromAccount === toAccount) {
            showError('Las cuentas de origen y destino deben ser diferentes');
            return false;
        }
        
        return true;
    }
    
    if (stepNumber === 2) {
        const description = document.getElementById('description').value;
        if (!description || description.trim() === '') {
            showError('Ingresa una descripción');
            return false;
        }
        return true;
    }
    
    return true;
}

function setupFormSteps() {
    // Configurar navegación entre pasos del formulario
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const confirmBtn = document.getElementById('confirmBtn');
    
    console.log('Configurando eventos de formulario...');
    console.log('  nextBtn:', nextBtn);
    console.log('  prevBtn:', prevBtn);
    console.log('  confirmBtn:', confirmBtn);
    
    if (nextBtn) {
        nextBtn.addEventListener('click', goToNextStep);
        console.log('✅ Evento click agregado a nextBtn');
    }
    
    if (prevBtn) {
        prevBtn.addEventListener('click', goToPrevStep);
        console.log('✅ Evento click agregado a prevBtn');
    }
    
    if (confirmBtn) {
        confirmBtn.addEventListener('click', confirmTransfer);
        console.log('✅ Evento click agregado a confirmBtn');
    }
}

function goToNextStep() {
    // Lógica para avanzar al siguiente paso
    const currentStep = document.querySelector('.form-step.active');
    const stepNumber = parseInt(currentStep.id.replace('step', ''));
    
    if (!validateCurrentStep(stepNumber)) {
        return;
    }
    
    // Si estamos en el paso 2, antes de ir al paso 3 llenar el resumen
    if (stepNumber === 2) {
        populateConfirmationData();
    }
    
    showStep(stepNumber + 1);
}

function goToPrevStep() {
    const currentStep = document.querySelector('.form-step.active');
    const stepNumber = parseInt(currentStep.id.replace('step', ''));
    showStep(stepNumber - 1);
}

function showStep(stepNumber) {
    // Ocultar todos los pasos
    document.querySelectorAll('.form-step').forEach(step => {
        step.classList.remove('active');
    });
    
    // Mostrar paso actual
    const targetStep = document.getElementById(`step${stepNumber}`);
    if (targetStep) {
        targetStep.classList.add('active');
    }
    
    // Actualizar botones de navegación
    updateNavigationButtons(stepNumber);
}

function updateNavigationButtons(stepNumber) {
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const confirmBtn = document.getElementById('confirmBtn');
    
    if (prevBtn) {
        prevBtn.style.display = stepNumber > 1 ? 'block' : 'none';
    }
    
    if (nextBtn) {
        nextBtn.style.display = (stepNumber === 1 || stepNumber === 2) ? 'block' : 'none';
    }
    
    if (confirmBtn) {
        confirmBtn.style.display = stepNumber === 3 ? 'block' : 'none';
    }
}

function populateConfirmationData() {
    const formData = getFormData();
    
    // Llenar resumen de confirmación
    document.getElementById('summaryFrom').textContent = getAccountDisplayName(formData.fromAccount);
    document.getElementById('summaryTo').textContent = formData.toAccount;
    document.getElementById('summaryAmount').textContent = `$${formatMoney(formData.amount)}`;
    document.getElementById('summaryDescription').textContent = formData.description;
    document.getElementById('summaryType').textContent = getTransferTypeDisplayName(formData.transferType);
    
    // Calcular comisión
    const comision = calculateCommission(formData.transferType, parseFloat(formData.amount));
    document.getElementById('summaryFee').textContent = `$${formatMoney(comision)}`;
    
    const total = parseFloat(formData.amount) + comision;
    document.getElementById('summaryTotal').textContent = `$${formatMoney(total)}`;
}

function calculateCommission(transferType, amount) {
    switch(transferType) {
        case 'same-bank':
            return 0;
        case 'other-bank':
            return 2.50 + (amount * 0.005); // $2.50 + 0.5%
        case 'international':
            return 15.00 + (amount * 0.02); // $15.00 + 2%
        default:
            return 0;
    }
}

function getAccountDisplayName(accountNumber) {
    const accountRaw = cuentasUsuario.find(c => {
        const num = c['Numero de Cuenta'] || c.numeroCuenta || c.numero_cuenta;
        return num === accountNumber;
    });
    if (accountRaw) {
        const account = normalizeCuentaData(accountRaw);
        return `${account.tipoCuenta} - ${accountNumber}`;
    }
    return accountNumber;
}

function getTransferTypeDisplayName(transferType) {
    const typeMap = {
        'same-bank': 'Mismo Banco',
        'other-bank': 'Otro Banco',
        'international': 'Internacional'
    };
    return typeMap[transferType] || transferType;
}

async function loadContactosFrecuentes() {
    try {
        const user = authService.getCurrentUser();
        if (!user) return;

        const username = user.username || user.nombreUsuario;
        const response = await fetch(`${authService.baseURL}/banco/transacciones/usuario?username=${encodeURIComponent(username)}&limit=50`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${authService.getAuthToken()}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            console.error('Error al cargar transacciones para contactos frecuentes');
            return;
        }

        const transaccionesRaw = await response.json();
        const transacciones = transaccionesRaw.map(normalizeTransaccionData);
        
        // Obtener cuentas del usuario
        const userAccountNumbers = getUserAccountNumbers();
        
        // Filtrar solo transferencias salientes y agrupar por cuenta destino
        const contactosMap = new Map();
        
        transacciones.forEach(trans => {
            // Solo transferencias salientes del usuario
            if (userAccountNumbers.includes(trans.cuentaOrigen) && trans.cuentaDestino) {
                const cuentaDestino = trans.cuentaDestino;
                
                if (!contactosMap.has(cuentaDestino)) {
                    contactosMap.set(cuentaDestino, {
                        cuenta: cuentaDestino,
                        nombre: trans.clienteDestino || trans.descripcion || 'Contacto',
                        cantidad: 0,
                        ultimaFecha: new Date(trans.fecha)
                    });
                }
                
                const contacto = contactosMap.get(cuentaDestino);
                contacto.cantidad++;
                
                // Actualizar con la fecha más reciente
                const fechaTrans = new Date(trans.fecha);
                if (fechaTrans > contacto.ultimaFecha) {
                    contacto.ultimaFecha = fechaTrans;
                    // Actualizar nombre con el cliente destino más reciente
                    if (trans.clienteDestino) {
                        contacto.nombre = trans.clienteDestino;
                    }
                }
            }
        });
        
        // Convertir a array y ordenar por frecuencia y recencia
        const contactos = Array.from(contactosMap.values())
            .sort((a, b) => {
                // Primero por cantidad de transferencias
                if (b.cantidad !== a.cantidad) {
                    return b.cantidad - a.cantidad;
                }
                // Luego por fecha más reciente
                return b.ultimaFecha - a.ultimaFecha;
            })
            .slice(0, 4); // Top 4 contactos más frecuentes
        
        // Mejorar nombres de contactos (remover "Transferencia a" si existe)
        contactos.forEach(contacto => {
            contacto.nombre = contacto.nombre
                .replace(/^Transferencia (a|para) /i, '')
                .replace(/^Pago (a|para) /i, '')
                .trim();
            
            // Si el nombre queda vacío o es muy genérico, usar formato de cuenta
            if (!contacto.nombre || contacto.nombre.toLowerCase() === 'contacto') {
                contacto.nombre = `Cuenta ${contacto.cuenta.slice(-4)}`;
            }
        });
        
        displayContactosFrecuentes(contactos);
        
    } catch (error) {
        console.error('Error al cargar contactos frecuentes:', error);
    }
}

function displayContactosFrecuentes(contactos) {
    const contactsContainer = document.querySelector('.quick-contacts');
    
    if (!contactsContainer) return;
    
    if (!contactos || contactos.length === 0) {
        contactsContainer.innerHTML = '<div style="text-align: center; padding: 20px; color: #666; grid-column: 1/-1;">No hay contactos frecuentes aún</div>';
        return;
    }
    
    contactsContainer.innerHTML = '';
    contactos.forEach(contacto => {
        const contactCard = createContactCard(contacto);
        contactsContainer.appendChild(contactCard);
    });
}

function createContactCard(contacto) {
    const card = document.createElement('div');
    card.className = 'contact-card quick-contact-btn';
    card.dataset.account = contacto.cuenta;
    card.dataset.name = contacto.nombre;
    
    const initials = contacto.nombre.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
    
    card.innerHTML = `
        <div class="contact-avatar">${initials}</div>
        <span class="contact-name">${contacto.nombre}</span>
        <span class="contact-account">**** ${String(contacto.cuenta).slice(-4)}</span>
    `;
    
    card.addEventListener('click', function() {
        selectContact(contacto.cuenta, contacto.nombre);
    });
    
    return card;
}

function selectContact(accountNumber, name) {
    const toAccountInput = document.getElementById('toAccount');
    const descriptionInput = document.getElementById('description');
    
    if (toAccountInput) {
        toAccountInput.value = accountNumber;
        validateDestinationAccount(accountNumber);
    }
    
    if (descriptionInput && !descriptionInput.value) {
        descriptionInput.value = `Transferencia a ${name}`;
    }
    
    // Auto-seleccionar tipo de transferencia basado en longitud de cuenta
    const transferTypeSelect = document.getElementById('transferType');
    if (transferTypeSelect && accountNumber.length <= 12) {
        transferTypeSelect.value = 'same-bank';
        handleTransferTypeChange('same-bank');
    }
}

async function setupLimitesTransferencia() {
    try {
        // Cargar transacciones del usuario para calcular uso
        const user = authService.getCurrentUser();
        if (!user) return;

        const username = user.username || user.nombreUsuario;
        const response = await fetch(`${authService.baseURL}/banco/transacciones/usuario?username=${encodeURIComponent(username)}&limit=100`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${authService.getAuthToken()}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            console.error('Error al cargar transacciones para límites');
            return;
        }

        const transaccionesRaw = await response.json();
        const transacciones = transaccionesRaw.map(normalizeTransaccionData);
        
        // Calcular uso basado en transacciones
        const ahora = new Date();
        const hoy = new Date(ahora.getFullYear(), ahora.getMonth(), ahora.getDate());
        const inicioSemana = new Date(hoy);
        inicioSemana.setDate(hoy.getDate() - hoy.getDay());
        const inicioMes = new Date(ahora.getFullYear(), ahora.getMonth(), 1);
        
        const userAccountNumbers = getUserAccountNumbers();
        
        let usoDiario = 0;
        let usoSemanal = 0;
        let usoMensual = 0;
        
        transacciones.forEach(trans => {
            const fechaTrans = new Date(trans.fecha);
            // Solo contar egresos (transferencias salientes)
            if (userAccountNumbers.includes(trans.cuentaOrigen)) {
                const monto = trans.monto;
                
                if (fechaTrans >= hoy) {
                    usoDiario += monto;
                }
                if (fechaTrans >= inicioSemana) {
                    usoSemanal += monto;
                }
                if (fechaTrans >= inicioMes) {
                    usoMensual += monto;
                }
            }
        });
        
        limitesTransferencia = {
            diario: 5000,
            usado_hoy: usoDiario,
            semanal: 15000,
            usado_semanal: usoSemanal,
            mensual: 50000,
            usado_mensual: usoMensual
        };
        
        updateLimitInfo();
    } catch (error) {
        console.error('Error al configurar límites:', error);
        // Fallback a valores por defecto
        limitesTransferencia = {
            diario: 5000,
            usado_hoy: 0,
            semanal: 15000,
            usado_semanal: 0,
            mensual: 50000,
            usado_mensual: 0
        };
        updateLimitInfo();
    }
}

function updateLimitInfo() {
    // Actualizar todas las tarjetas de límites
    const limitCards = document.querySelectorAll('.limit-card');
    
    if (limitCards.length >= 3) {
        // Límite Diario
        updateLimitCard(limitCards[0], limitesTransferencia.diario, limitesTransferencia.usado_hoy);
        
        // Límite Semanal
        updateLimitCard(limitCards[1], limitesTransferencia.semanal, limitesTransferencia.usado_semanal);
        
        // Límite Mensual
        updateLimitCard(limitCards[2], limitesTransferencia.mensual, limitesTransferencia.usado_mensual);
    }
}

function updateLimitCard(card, limite, usado) {
    const limitValue = card.querySelector('.limit-value');
    const limitUsed = card.querySelector('.limit-used');
    const progressFill = card.querySelector('.progress-fill');
    
    const porcentajeUsado = (usado / limite) * 100;
    
    if (limitValue) limitValue.textContent = `$${formatMoney(limite)}`;
    if (limitUsed) limitUsed.textContent = `Usado: $${formatMoney(usado)}`;
    if (progressFill) {
        progressFill.style.width = `${Math.min(porcentajeUsado, 100)}%`;
        
        // Cambiar color según el porcentaje
        if (porcentajeUsado >= 90) {
            progressFill.style.backgroundColor = '#f44336'; // Rojo
        } else if (porcentajeUsado >= 70) {
            progressFill.style.backgroundColor = '#ff9800'; // Naranja
        } else {
            progressFill.style.backgroundColor = '#4CAF50'; // Verde
        }
    }
}

function checkPreselectedAccount() {
    const preselectedAccount = sessionStorage.getItem('preselectedAccount');
    if (preselectedAccount) {
        const fromAccountSelect = document.getElementById('fromAccount');
        if (fromAccountSelect) {
            fromAccountSelect.value = preselectedAccount;
            updateAvailableBalance(preselectedAccount);
        }
        sessionStorage.removeItem('preselectedAccount');
    }
}

function updateAvailableBalance(cuentaId) {
    const accountRaw = cuentasUsuario.find(c => {
        const id = c.cuentaId || c.cuenta_id || c.cuentaID;
        return id == cuentaId;
    });
    const account = accountRaw ? normalizeCuentaData(accountRaw) : null;
    
    // Mostrar saldo disponible
    let balanceInfo = document.querySelector('.balance-info');
    if (!balanceInfo) {
        balanceInfo = document.createElement('div');
        balanceInfo.className = 'balance-info';
        
        const amountGroup = document.querySelector('#amount').closest('.form-group');
        if (amountGroup) {
            amountGroup.appendChild(balanceInfo);
        }
    }
    
    if (account) {
        balanceInfo.innerHTML = `
            <small class="balance-text">
                <i class="fas fa-wallet"></i>
                Saldo disponible: <strong>$${formatMoney(account.saldo)}</strong>
            </small>
        `;
    } else {
        balanceInfo.innerHTML = '';
    }
}

// Funciones auxiliares existentes
function formatMoney(amount) {
    if (amount === null || amount === undefined) return '0.00';
    return parseFloat(amount).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

function showLoading(show) {
    const submitBtn = document.querySelector('button[type="submit"]') || document.getElementById('confirmBtn');
    if (submitBtn) {
        submitBtn.disabled = show;
        submitBtn.innerHTML = show ? 
            '<i class="fas fa-spinner fa-spin"></i> Procesando...' : 
            '<i class="fas fa-check"></i> Confirmar Transferencia';
    }
}

function showError(message) {
    alert('Error: ' + message);
    // TODO: Implementar sistema de notificaciones toast
}

function showSuccess(message) {
    alert('Éxito: ' + message);
    // TODO: Implementar sistema de notificaciones toast
}

function showTransferSuccess(result) {
    console.log('Mostrando mensaje de éxito y redirigiendo...');
    const message = `¡Transferencia realizada exitosamente!\n\nLa transferencia se ha procesado correctamente.\nPuedes ver el detalle en tu historial de transacciones.`;
    alert(message);
    
    // Redirigir a transacciones después de un momento
    console.log('⏱️ Redirigiendo a transacciones.html en 1.5 segundos...');
    setTimeout(() => {
        console.log('Redirigiendo ahora...');
        window.location.href = 'transacciones.html';
    }, 1500);
}

function resetForm() {
    const form = document.getElementById('transferForm');
    if (form) {
        form.reset();
        showStep(1);
        
        // Limpiar validaciones
        document.querySelectorAll('.valid, .invalid').forEach(el => {
            el.classList.remove('valid', 'invalid');
        });
        
        document.querySelectorAll('.amount-validation, .account-validation').forEach(el => {
            el.remove();
        });
    }
}

async function confirmTransfer() {
    console.log('confirmTransfer() llamada');
    
    // Ejecutar la transferencia cuando se confirme en el paso 3
    const formData = getFormData();
    console.log('Datos del formulario:', formData);
    
    // Validación final
    if (!validateFormData(formData)) {
        console.log('❌ Validación falló');
        return;
    }

    console.log('✅ Validación exitosa, ejecutando transferencia...');
    try {
        showLoading(true);
        
        // Determinar si es transferencia interna o externa
        const isExternal = formData.transferType === 'other-bank' || formData.transferType === 'international';
        
        let result;
        if (isExternal) {
            result = await realizarTransferenciaExterna(formData);
        } else {
            result = await realizarTransferenciaInterna(formData);
        }

        if (result.success) {
            console.log('✅ Transferencia exitosa!');
            showTransferSuccess(result);
            
            // Actualizar límites
            limitesTransferencia.usado_hoy += parseFloat(formData.amount);
            updateLimitInfo();
            
            // Recargar cuentas y transferencias recientes
            console.log('Recargando cuentas y transferencias...');
            await loadCuentasUsuario();
            await loadTransferenciasRecientes();
            
            // Resetear formulario y volver al paso 1
            resetForm();
        } else {
            console.log('❌ Transferencia falló:', result.message);
            showError(result.message || 'Error al realizar la transferencia');
        }
        
    } catch (error) {
        console.error('Transfer error:', error);
        showError('Error al realizar la transferencia. Por favor, intenta de nuevo.');
    } finally {
        showLoading(false);
    }
}

// Cargar transferencias recientes
async function loadTransferenciasRecientes() {
    try {
        const user = authService.getCurrentUser();
        if (!user) return;

        const username = user.username || user.nombreUsuario;
        const response = await fetch(`${authService.baseURL}/banco/transacciones/usuario?username=${encodeURIComponent(username)}&limit=3`, {
            method: 'GET',
            headers: {
                'Authorization': `Bearer ${authService.getAuthToken()}`,
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            console.error('Error al cargar transferencias:', response.status);
            return;
        }

        const transferencias = await response.json();
        console.log('✅ Transferencias recientes cargadas:', transferencias);
        displayTransferenciasRecientes(transferencias);
    } catch (error) {
        console.error('Error al cargar transferencias:', error);
    }
}

function displayTransferenciasRecientes(transferencias) {
    const transfersList = document.querySelector('.transfers-list');
    if (!transfersList) return;

    if (!transferencias || transferencias.length === 0) {
        transfersList.innerHTML = '<div style="text-align: center; padding: 20px; color: #666;">No hay transferencias recientes</div>';
        return;
    }

    const userAccountNumbers = getUserAccountNumbers();
    transfersList.innerHTML = '';
    
    transferencias.forEach(transRaw => {
        const trans = normalizeTransaccionData(transRaw);
        const transferItem = createTransferItem(trans, userAccountNumbers);
        transfersList.appendChild(transferItem);
    });
}

function createTransferItem(trans, userAccountNumbers) {
    const div = document.createElement('div');
    div.className = 'transfer-item';
    
    const esIngreso = esIngresoParaCliente(trans, userAccountNumbers);
    const icon = esIngreso ? 'arrow-down' : 'arrow-up';
    const amountClass = esIngreso ? 'positive' : 'negative';
    const signo = esIngreso ? '+' : '-';
    
    const descripcion = trans.descripcion || trans.tipoTransaccion || 'Transferencia';
    const fecha = formatearFechaTransfer(trans.fecha);

    div.innerHTML = `
        <div class="transfer-icon">
            <i class="fas fa-${icon}"></i>
        </div>
        <div class="transfer-details">
            <span class="transfer-description">${descripcion}</span>
            <span class="transfer-date">${fecha}</span>
        </div>
        <div class="transfer-amount ${amountClass}">${signo}$${formatMoney(trans.monto)}</div>
    `;

    return div;
}

function esIngresoParaCliente(trans, userAccountNumbers) {
    if (userAccountNumbers.includes(trans.cuentaDestino)) return true;
    if (userAccountNumbers.includes(trans.cuentaOrigen)) return false;
    const tipo = (trans.tipoTransaccion || '').toLowerCase();
    return tipo.includes('ingreso') || tipo.includes('depósito') || tipo.includes('deposito');
}

function getUserAccountNumbers() {
    try {
        const clienteInfo = sessionStorage.getItem('clienteInfo');
        if (clienteInfo) {
            const data = JSON.parse(clienteInfo);
            if (data.cuentas && Array.isArray(data.cuentas)) {
                return data.cuentas.map(c => normalizeCuentaData(c)).map(c => c.numeroCuenta).filter(Boolean);
            }
        }
        
        if (cuentasUsuario && cuentasUsuario.length > 0) {
            return cuentasUsuario.map(c => c.numeroCuenta).filter(Boolean);
        }
        
        return [];
    } catch (error) {
        console.error('Error obteniendo números de cuenta:', error);
        return [];
    }
}

function formatearFechaTransfer(fecha) {
    if (!fecha) return 'Fecha no disponible';
    
    const date = new Date(fecha);
    const hoy = new Date();
    const ayer = new Date(hoy);
    ayer.setDate(ayer.getDate() - 1);
    
    hoy.setHours(0, 0, 0, 0);
    ayer.setHours(0, 0, 0, 0);
    const fechaDate = new Date(date);
    fechaDate.setHours(0, 0, 0, 0);
    
    const hora = date.toLocaleTimeString('es-ES', { hour: '2-digit', minute: '2-digit' });
    
    if (fechaDate.getTime() === hoy.getTime()) return `Hoy, ${hora}`;
    if (fechaDate.getTime() === ayer.getTime()) return `Ayer, ${hora}`;
    
    const dia = date.getDate();
    const mes = date.toLocaleString('es-ES', { month: 'short' });
    return `${dia} ${mes}, ${hora}`;
}

// Eventos globales necesarios para el HTML existente
window.confirmTransfer = confirmTransfer;
