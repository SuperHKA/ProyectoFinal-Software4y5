// Manejador de transferencias
document.addEventListener('DOMContentLoaded', async function() {
    // Verificar autenticación
    if (!authService.isLoggedIn()) {
        window.location.href = 'Login.html';
        return;
    }

    await loadCuentasUsuario();
    setupTransferForm();
});

async function loadCuentasUsuario() {
    try {
        const cuentas = await authService.getCuentas();
        
        const selectOrigen = document.getElementById('cuenta-origen');
        const selectDestino = document.getElementById('cuenta-destino');
        
        if (selectOrigen && cuentas && cuentas.length > 0) {
            selectOrigen.innerHTML = '<option value="">Selecciona una cuenta</option>';
            cuentas.forEach(cuenta => {
                const option = document.createElement('option');
                option.value = cuenta.numeroCuenta;
                option.textContent = `${cuenta.tipoCuenta} - ${cuenta.numeroCuenta} ($${formatMoney(cuenta.saldo)})`;
                selectOrigen.appendChild(option);
            });
        }
    } catch (error) {
        console.error('Error loading cuentas:', error);
        showError('Error al cargar las cuentas');
    }
}

function setupTransferForm() {
    const form = document.querySelector('form');
    if (!form) return;

    form.addEventListener('submit', async function(e) {
        e.preventDefault();

        const cuentaOrigen = document.getElementById('cuenta-origen').value;
        const cuentaDestino = document.getElementById('cuenta-destino').value;
        const monto = parseFloat(document.getElementById('monto').value);
        const descripcion = document.getElementById('descripcion').value;

        // Validaciones
        if (!cuentaOrigen) {
            showError('Selecciona una cuenta de origen');
            return;
        }

        if (!cuentaDestino) {
            showError('Ingresa la cuenta de destino');
            return;
        }

        if (cuentaOrigen === cuentaDestino) {
            showError('Las cuentas de origen y destino deben ser diferentes');
            return;
        }

        if (!monto || monto <= 0) {
            showError('Ingresa un monto válido');
            return;
        }

        if (!descripcion) {
            showError('Ingresa una descripción');
            return;
        }

        showLoading(true);

        try {
            const transferData = {
                cuentaOrigen: cuentaOrigen,
                cuentaDestino: cuentaDestino,
                monto: monto,
                descripcion: descripcion
            };

            const result = await authService.transferir(transferData);

            if (result.success) {
                showSuccess('Transferencia realizada exitosamente');
                form.reset();
                setTimeout(() => {
                    window.location.href = 'dashboard.html';
                }, 2000);
            } else {
                showError(result.message);
            }
        } catch (error) {
            console.error('Transfer error:', error);
            showError('Error al realizar la transferencia');
        } finally {
            showLoading(false);
        }
    });
}

function formatMoney(amount) {
    if (amount === null || amount === undefined) return '0.00';
    return parseFloat(amount).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

function showLoading(show) {
    const button = document.querySelector('button[type="submit"]');
    if (button) {
        button.disabled = show;
        button.textContent = show ? 'Procesando...' : 'Transferir';
    }
}

function showError(message) {
    alert('Error: ' + message);
}

function showSuccess(message) {
    alert('Éxito: ' + message);
}
