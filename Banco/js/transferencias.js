// Funcionalidad para el formulario de transferencias
document.addEventListener('DOMContentLoaded', function() {
    let currentStep = 1;
    const totalSteps = 3;
    
    // Elementos del DOM
    const step1 = document.getElementById('step1');
    const step2 = document.getElementById('step2');
    const step3 = document.getElementById('step3');
    const nextBtn = document.getElementById('nextBtn');
    const prevBtn = document.getElementById('prevBtn');
    const confirmBtn = document.getElementById('confirmBtn');
    const transferForm = document.getElementById('transferForm');
    const confirmationModal = document.getElementById('confirmationModal');
    
    // Inicializar formulario
    showStep(1);
    
    // Botón Siguiente
    nextBtn.addEventListener('click', function() {
        if (validateStep(currentStep)) {
            currentStep++;
            showStep(currentStep);
        }
    });
    
    // Botón Anterior
    prevBtn.addEventListener('click', function() {
        currentStep--;
        showStep(currentStep);
    });
    
    // Botón Confirmar
    confirmBtn.addEventListener('click', function() {
        if (validateStep(currentStep)) {
            showConfirmationModal();
        }
    });
    
    // Mostrar paso actual
    function showStep(step) {
        // Ocultar todos los pasos
        step1.classList.remove('active');
        step2.classList.remove('active');
        step3.classList.remove('active');
        
        // Mostrar paso actual
        document.getElementById(`step${step}`).classList.add('active');
        
        // Actualizar botones de navegación
        prevBtn.style.display = step === 1 ? 'none' : 'block';
        nextBtn.style.display = step === totalSteps ? 'none' : 'block';
        confirmBtn.style.display = step === totalSteps ? 'block' : 'none';
        
        // Actualizar resumen en el paso 3
        if (step === 3) {
            updateTransferSummary();
        }
    }
    
    // Validar paso actual
    function validateStep(step) {
        let isValid = true;
        
        switch(step) {
            case 1:
                const fromAccount = document.getElementById('fromAccount').value;
                const toAccount = document.getElementById('toAccount').value;
                const amount = document.getElementById('amount').value;
                
                if (!fromAccount) {
                    showError('fromAccount', 'Selecciona una cuenta de origen');
                    isValid = false;
                }
                
                if (!toAccount) {
                    showError('toAccount', 'Ingresa la cuenta de destino');
                    isValid = false;
                }
                
                if (!amount || amount <= 0) {
                    showError('amount', 'Ingresa un monto válido');
                    isValid = false;
                }
                break;
                
            case 2:
                const description = document.getElementById('description').value;
                if (!description) {
                    showError('description', 'Ingresa una descripción');
                    isValid = false;
                }
                break;
        }
        
        return isValid;
    }
    
    // Mostrar error
    function showError(fieldId, message) {
        const field = document.getElementById(fieldId);
        const errorDiv = field.parentNode.querySelector('.error-message') || document.createElement('div');
        
        errorDiv.className = 'error-message';
        errorDiv.style.color = '#dc3545';
        errorDiv.style.fontSize = '12px';
        errorDiv.style.marginTop = '5px';
        errorDiv.textContent = message;
        
        if (!field.parentNode.querySelector('.error-message')) {
            field.parentNode.appendChild(errorDiv);
        }
        
        field.style.borderColor = '#dc3545';
        
        // Remover error después de 3 segundos
        setTimeout(() => {
            errorDiv.remove();
            field.style.borderColor = '#ddd';
        }, 3000);
    }
    
    // Actualizar resumen de transferencia
    function updateTransferSummary() {
        const fromAccount = document.getElementById('fromAccount');
        const toAccount = document.getElementById('toAccount');
        const amount = document.getElementById('amount');
        const description = document.getElementById('description');
        const transferType = document.getElementById('transferType');
        
        document.getElementById('summaryFrom').textContent = fromAccount.options[fromAccount.selectedIndex].text;
        document.getElementById('summaryTo').textContent = toAccount.value;
        document.getElementById('summaryAmount').textContent = `$${parseFloat(amount.value).toLocaleString('en-US', {minimumFractionDigits: 2})}`;
        document.getElementById('summaryDescription').textContent = description.value;
        document.getElementById('summaryType').textContent = transferType.options[transferType.selectedIndex].text;
        document.getElementById('summaryFee').textContent = '$0.50';
        document.getElementById('summaryTotal').textContent = `$${(parseFloat(amount.value) + 0.50).toLocaleString('en-US', {minimumFractionDigits: 2})}`;
    }
    
    // Mostrar modal de confirmación
    function showConfirmationModal() {
        confirmationModal.style.display = 'block';
        
        // Actualizar detalles en el modal
        document.getElementById('modalFrom').textContent = document.getElementById('summaryFrom').textContent;
        document.getElementById('modalTo').textContent = document.getElementById('summaryTo').textContent;
        document.getElementById('modalAmount').textContent = document.getElementById('summaryAmount').textContent;
        document.getElementById('modalDescription').textContent = document.getElementById('summaryDescription').textContent;
    }
    
    // Cerrar modal
    window.closeConfirmationModal = function() {
        confirmationModal.style.display = 'none';
    }
    
    // Confirmar transferencia
    window.confirmTransfer = function() {
        const fromAccount = document.getElementById('fromAccount').value;
        const toAccount = document.getElementById('toAccount').value;
        const amount = document.getElementById('amount').value;
        const description = document.getElementById('description').value;
        
        // Simular envío de transferencia
        const transferData = {
            fromAccount: fromAccount,
            toAccount: toAccount,
            amount: amount,
            description: description,
            timestamp: new Date().toISOString()
        };
        
        // Mostrar mensaje de éxito
        alert('¡Transferencia realizada exitosamente!');
        
        // Cerrar modal y resetear formulario
        closeConfirmationModal();
        transferForm.reset();
        currentStep = 1;
        showStep(1);
        
        // Aquí normalmente enviarías los datos al servidor
        console.log('Datos de transferencia:', transferData);
    }
    
    // Contactos rápidos
    const quickContactButtons = document.querySelectorAll('.quick-contact-btn');
    quickContactButtons.forEach(button => {
        button.addEventListener('click', function() {
            const account = this.getAttribute('data-account');
            const name = this.getAttribute('data-name');
            
            document.getElementById('toAccount').value = account;
            
            // Avanzar al siguiente paso si es necesario
            if (currentStep === 1) {
                nextBtn.click();
            }
        });
    });
    
    // Acciones rápidas
    const quickActionCards = document.querySelectorAll('.quick-action-card');
    quickActionCards.forEach(card => {
        card.addEventListener('click', function() {
            const action = this.getAttribute('data-action');
            
            switch(action) {
                case 'same-bank':
                    document.getElementById('transferType').value = 'same-bank';
                    break;
                case 'other-bank':
                    document.getElementById('transferType').value = 'other-bank';
                    break;
                case 'schedule':
                    // Aquí podrías abrir un modal para programar transferencia
                    alert('Funcionalidad de programación de transferencias próximamente');
                    break;
            }
        });
    });
    
    // Cerrar modal al hacer clic fuera
    window.onclick = function(event) {
        if (event.target === confirmationModal) {
            closeConfirmationModal();
        }
    }
});