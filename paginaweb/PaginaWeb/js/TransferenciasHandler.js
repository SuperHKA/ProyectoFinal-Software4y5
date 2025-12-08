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
});

let cuentasUsuario = [];
let limitesTransferencia = {
    diario: 5000,
    usado_hoy: 0
};

async function loadCuentasUsuario() {
    try {
        const data = await authService.getClienteInfo();
        cuentasUsuario = data.cuentas || data;
        
        const selectOrigen = document.getElementById('fromAccount');
        if (selectOrigen && cuentasUsuario && cuentasUsuario.length > 0) {
            selectOrigen.innerHTML = '<option value="">Selecciona una cuenta</option>';
            cuentasUsuario.forEach(cuenta => {
                const option = document.createElement('option');
                option.value = cuenta.numeroCuenta;
                option.textContent = `${cuenta.tipoCuenta} - ${cuenta.numeroCuenta} ($${formatMoney(cuenta.saldo)})`;
                option.dataset.saldo = cuenta.saldo;
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
    
    // Manejar envío del formulario
    form.addEventListener('submit', handleTransferSubmit);
}

function setupExternalTransfers() {
    // Configurar validación especial para transferencias externas
    const transferTypeSelect = document.getElementById('transferType');
    
    if (transferTypeSelect) {
        transferTypeSelect.addEventListener('change', function() {
            const isExternal = this.value === 'other-bank' || this.value === 'international';
            showExternalTransferInfo(isExternal);
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
    const selectedAccount = cuentasUsuario.find(c => c.numeroCuenta === fromAccount);
    if (selectedAccount && amountValue > selectedAccount.saldo) {
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
    
    if (!toAccountInput || !accountNumber) return;
    
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
            isValid = /^[0-9]{20,22}$/.test(accountNumber);
            message = isValid ? 'CBU válido' : 'Formato de CBU inválido (20-22 dígitos)';
            break;
            
        case 'international':
            // Validación básica para IBAN (simplificada)
            isValid = /^[A-Z]{2}[0-9]{2}[A-Z0-9]{4,30}$/.test(accountNumber.toUpperCase());
            message = isValid ? 'IBAN válido' : 'Formato de IBAN inválido';
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

async function handleTransferSubmit(e) {
    e.preventDefault();

    const formData = getFormData();
    if (!validateFormData(formData)) {
        return;
    }

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
            showTransferSuccess(result);
            resetForm();
            
            // Actualizar límites
            limitesTransferencia.usado_hoy += parseFloat(formData.amount);
            updateLimitInfo();
        } else {
            showError(result.message);
        }
        
    } catch (error) {
        console.error('Transfer error:', error);
        showError('Error al realizar la transferencia');
    } finally {
        showLoading(false);
    }
}

async function realizarTransferenciaInterna(formData) {
    try {
        const transferData = {
            numeroCuentaOrigen: formData.fromAccount,
            numeroCuentaDestino: formData.toAccount,
            monto: parseFloat(formData.amount),
            descripcion: formData.description,
            username: authService.getCurrentUser()?.username
        };

        const response = await fetch(`${authService.baseURL}/banco/transfer/internal`, {
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
            message: data.message || 'Transferencia realizada exitosamente',
            data: data
        };
        
    } catch (error) {
        throw error;
    }
}

async function realizarTransferenciaExterna(formData) {
    try {
        const transferData = {
            numeroCuentaOrigen: formData.fromAccount,
            numeroCuentaDestino: formData.toAccount,
            monto: parseFloat(formData.amount),
            descripcion: formData.description,
            tipoTransferencia: formData.transferType,
            username: authService.getCurrentUser()?.username
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

function setupFormSteps() {
    // Configurar navegación entre pasos del formulario
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const confirmBtn = document.getElementById('confirmBtn');
    
    if (nextBtn) {
        nextBtn.addEventListener('click', goToNextStep);
    }
    
    if (prevBtn) {
        prevBtn.addEventListener('click', goToPrevStep);
    }
    
    if (confirmBtn) {
        confirmBtn.addEventListener('click', confirmTransfer);
    }
}

function goToNextStep() {
    // Lógica para avanzar al siguiente paso
    const currentStep = document.querySelector('.form-step.active');
    const stepNumber = parseInt(currentStep.id.replace('step', ''));
    
    if (validateCurrentStep(stepNumber)) {
        showStep(stepNumber + 1);
        
        if (stepNumber + 1 === 3) {
            populateConfirmationData();
        }
    }
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
        nextBtn.style.display = stepNumber < 3 ? 'block' : 'none';
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
    const account = cuentasUsuario.find(c => c.numeroCuenta === accountNumber);
    return account ? `${account.tipoCuenta} - ${accountNumber}` : accountNumber;
}

function getTransferTypeDisplayName(transferType) {
    const typeMap = {
        'same-bank': 'Mismo Banco',
        'other-bank': 'Otro Banco',
        'international': 'Internacional'
    };
    return typeMap[transferType] || transferType;
}

function loadContactosFrecuentes() {
    // Cargar contactos frecuentes desde localStorage
    const contactos = getContactosFrecuentes();
    const contactsContainer = document.querySelector('.quick-contacts');
    
    if (contactsContainer && contactos.length > 0) {
        contactsContainer.innerHTML = '';
        contactos.forEach(contacto => {
            const contactCard = createContactCard(contacto);
            contactsContainer.appendChild(contactCard);
        });
    }
}

function getContactosFrecuentes() {
    try {
        const stored = localStorage.getItem('contactos_frecuentes');
        return stored ? JSON.parse(stored) : [];
    } catch (error) {
        return [];
    }
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

function setupLimitesTransferencia() {
    // Simular carga de límites desde API
    limitesTransferencia = {
        diario: 5000,
        usado_hoy: Math.random() * 1000 // Simular uso del día
    };
    
    updateLimitInfo();
}

function updateLimitInfo() {
    const disponible = limitesTransferencia.diario - limitesTransferencia.usado_hoy;
    const porcentajeUsado = (limitesTransferencia.usado_hoy / limitesTransferencia.diario) * 100;
    
    // Buscar tarjeta de límite diario y actualizar
    const limitCard = document.querySelector('.limit-card');
    if (limitCard) {
        const limitValue = limitCard.querySelector('.limit-value');
        const limitUsed = limitCard.querySelector('.limit-used');
        const progressFill = limitCard.querySelector('.progress-fill');
        
        if (limitValue) limitValue.textContent = `$${formatMoney(limitesTransferencia.diario)}`;
        if (limitUsed) limitUsed.textContent = `Usado: $${formatMoney(limitesTransferencia.usado_hoy)}`;
        if (progressFill) progressFill.style.width = `${porcentajeUsado}%`;
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

function updateAvailableBalance(accountNumber) {
    const account = cuentasUsuario.find(c => c.numeroCuenta === accountNumber);
    
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
    const message = `Transferencia realizada exitosamente.\n\nReferencia: ${result.data?.transaccionId || 'N/A'}`;
    showSuccess(message);
    
    // Redirigir al dashboard después de un delay
    setTimeout(() => {
        window.location.href = 'dashboard.html';
    }, 2000);
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

function confirmTransfer() {
    // Enviar formulario cuando se confirme en el paso 3
    document.getElementById('transferForm').dispatchEvent(new Event('submit'));
}

// Eventos globales necesarios para el HTML existente
window.confirmTransfer = confirmTransfer;
