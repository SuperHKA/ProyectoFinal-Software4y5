document.addEventListener('DOMContentLoaded', function() {
    // Variables globales
    let currentPage = 1;
    const totalPages = 5;
    let selectedTransactions = new Set();
    
    // Elementos DOM
    const selectAllCheckbox = document.getElementById('select-all');
    const transactionCheckboxes = document.querySelectorAll('.transactions-table tbody input[type="checkbox"]');
    const paginationButtons = document.querySelectorAll('.pagination-btn');
    const selectedCountElement = document.querySelector('.selected-actions span');
    const downloadSelectedButton = document.querySelector('.selected-actions .btn-outline');
    const exportButton = document.querySelector('.btn-primary');
    
    // Inicialización
    initTransactionsPage();
    
    function initTransactionsPage() {
        setupEventListeners();
        updateSelectedCount();
        updatePaginationUI();
    }
    
    function setupEventListeners() {
        // Checkbox "Seleccionar todo"
        if (selectAllCheckbox) {
            selectAllCheckbox.addEventListener('change', toggleSelectAll);
        }
        
        // Checkboxes individuales
        transactionCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', updateSelectedTransactions);
        });
        
        // Botones de paginación
        paginationButtons.forEach(button => {
            button.addEventListener('click', handlePagination);
        });
        
        // Botón de descargar selección
        if (downloadSelectedButton) {
            downloadSelectedButton.addEventListener('click', downloadSelectedTransactions);
        }
        
        // Botón de exportar
        if (exportButton) {
            exportButton.addEventListener('click', exportAllTransactions);
        }
        
        // Botones de acción (ver detalles, descargar, etc.)
        setupActionButtons();
    }
    
    function toggleSelectAll() {
        const isChecked = selectAllCheckbox.checked;
        
        transactionCheckboxes.forEach(checkbox => {
            checkbox.checked = isChecked;
            
            // Actualizar el conjunto de transacciones seleccionadas
            const transactionId = getTransactionId(checkbox);
            if (isChecked) {
                selectedTransactions.add(transactionId);
            } else {
                selectedTransactions.delete(transactionId);
            }
        });
        
        updateSelectedCount();
    }
    
    function updateSelectedTransactions(event) {
        const checkbox = event.target;
        const transactionId = getTransactionId(checkbox);
        
        if (checkbox.checked) {
            selectedTransactions.add(transactionId);
        } else {
            selectedTransactions.delete(transactionId);
        }
        
        // Actualizar el estado del checkbox "Seleccionar todo"
        updateSelectAllState();
        updateSelectedCount();
    }
    
    function getTransactionId(checkbox) {
        const row = checkbox.closest('tr');
        const description = row.querySelector('.transaction-info strong').textContent;
        const date = row.querySelector('.transaction-date .date').textContent;
        const amount = row.querySelector('.transaction-amount').textContent;
        
        return `${description}-${date}-${amount}`;
    }
    
    function updateSelectAllState() {
        if (!selectAllCheckbox) return;
        
        const allChecked = Array.from(transactionCheckboxes).every(checkbox => checkbox.checked);
        const someChecked = Array.from(transactionCheckboxes).some(checkbox => checkbox.checked);
        
        selectAllCheckbox.checked = allChecked;
        selectAllCheckbox.indeterminate = someChecked && !allChecked;
    }
    
    function updateSelectedCount() {
        if (!selectedCountElement) return;
        
        const count = selectedTransactions.size;
        selectedCountElement.textContent = `${count} seleccionado${count !== 1 ? 's' : ''}`;
        
        // Mostrar u ocultar las acciones de selección
        const selectedActions = document.querySelector('.selected-actions');
        if (selectedActions) {
            selectedActions.style.display = count > 0 ? 'flex' : 'none';
        }
    }
    
    function handlePagination(event) {
        const button = event.currentTarget;
        const isNext = button.querySelector('.fa-chevron-right');
        
        if (isNext && !button.disabled) {
            currentPage++;
        } else if (!isNext && !button.disabled) {
            currentPage = Math.max(1, currentPage - 1);
        }
        
        updatePaginationUI();
        
        // En una implementación real, aquí cargaríamos los datos de la página
        console.log(`Cambiando a página ${currentPage}`);
        
        // Simular cambio de página (en una implementación real, esto se haría con una solicitud al servidor)
        simulatePageChange();
    }
    
    function updatePaginationUI() {
        const prevButton = document.querySelector('.pagination-btn:first-child');
        const nextButton = document.querySelector('.pagination-btn:last-child');
        const pageInfo = document.querySelector('.pagination-info');
        
        if (prevButton) {
            prevButton.disabled = currentPage === 1;
        }
        
        if (nextButton) {
            nextButton.disabled = currentPage === totalPages;
        }
        
        if (pageInfo) {
            pageInfo.textContent = `Página ${currentPage} de ${totalPages}`;
        }
    }
    
    function simulatePageChange() {
        // En una implementación real, aquí se cargarían los datos de la nueva página
        // Por ahora, solo mostramos un mensaje
        const rows = document.querySelectorAll('.transactions-table tbody tr');
        
        // Simular que solo se muestran las transacciones de la página actual
        rows.forEach((row, index) => {
            const startIndex = (currentPage - 1) * 5;
            const endIndex = currentPage * 5;
            
            if (index >= startIndex && index < endIndex) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
        
        // Reiniciar selecciones al cambiar de página
        selectedTransactions.clear();
        transactionCheckboxes.forEach(checkbox => {
            checkbox.checked = false;
        });
        updateSelectedCount();
        updateSelectAllState();
    }
    
    function downloadSelectedTransactions() {
        if (selectedTransactions.size === 0) {
            showNotification('Por favor, selecciona al menos una transacción para descargar.', 'warning');
            return;
        }
        
        // En una implementación real, aquí enviaríamos una solicitud al servidor
        console.log('Descargando transacciones seleccionadas:', Array.from(selectedTransactions));
        
        // Simular descarga
        simulateDownload('transacciones_seleccionadas.csv');
        showNotification(`Se descargaron ${selectedTransactions.size} transacciones seleccionadas.`, 'success');
    }
    
    function exportAllTransactions() {
        // En una implementación real, aquí enviaríamos una solicitud al servidor
        console.log('Exportando todas las transacciones');
        
        // Simular descarga
        simulateDownload('todas_las_transacciones.csv');
        showNotification('Se descargó un archivo con todas las transacciones.', 'success');
    }
    
    function simulateDownload(filename) {
        // Crear un enlace temporal para simular la descarga
        const link = document.createElement('a');
        link.href = '#'; // En una implementación real, sería la URL del archivo
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    
    function setupActionButtons() {
        const actionButtons = document.querySelectorAll('.action-btn');
        
        actionButtons.forEach(button => {
            button.addEventListener('click', function() {
                const title = this.getAttribute('title');
                const row = this.closest('tr');
                const description = row.querySelector('.transaction-info strong').textContent;
                
                switch(title) {
                    case 'Ver detalles':
                        showTransactionDetails(description);
                        break;
                    case 'Descargar comprobante':
                        downloadReceipt(description);
                        break;
                    case 'Reportar problema':
                        reportProblem(description);
                        break;
                    case 'Cancelar':
                        cancelTransaction(description);
                        break;
                }
            });
        });
    }
    
    function showTransactionDetails(description) {
        // En una implementación real, aquí mostraríamos un modal con los detalles
        console.log(`Mostrando detalles de: ${description}`);
        showNotification(`Mostrando detalles de: ${description}`, 'info');
    }
    
    function downloadReceipt(description) {
        // En una implementación real, aquí descargaríamos el comprobante
        console.log(`Descargando comprobante de: ${description}`);
        simulateDownload(`comprobante_${description.replace(/\s+/g, '_')}.pdf`);
        showNotification(`Comprobante de ${description} descargado.`, 'success');
    }
    
    function reportProblem(description) {
        // En una implementación real, aquí abriríamos un formulario para reportar problemas
        console.log(`Reportando problema con: ${description}`);
        showNotification(`Problema reportado con: ${description}`, 'info');
    }
    
    function cancelTransaction(description) {
        if (confirm(`¿Estás seguro de que quieres cancelar: ${description}?`)) {
            // En una implementación real, aquí enviaríamos una solicitud al servidor
            console.log(`Transacción cancelada: ${description}`);
            showNotification(`Transacción cancelada: ${description}`, 'success');
        }
    }
    
    function showNotification(message, type = 'info') {
        // Crear notificación temporal
        const notification = document.createElement('div');
        notification.textContent = message;
        notification.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 15px 20px;
            border-radius: 10px;
            color: white;
            font-weight: 600;
            z-index: 10000;
            box-shadow: 0 5px 15px rgba(0,0,0,0.2);
            transition: all 0.3s ease;
        `;
        
        // Estilo según el tipo
        switch(type) {
            case 'success':
                notification.style.background = '#28a745';
                break;
            case 'warning':
                notification.style.background = '#ffc107';
                notification.style.color = '#212529';
                break;
            case 'error':
                notification.style.background = '#dc3545';
                break;
            default:
                notification.style.background = '#6c5bcd';
        }
        
        document.body.appendChild(notification);
        
        // Remover después de 3 segundos
        setTimeout(() => {
            notification.style.opacity = '0';
            notification.style.transform = 'translateX(100%)';
            setTimeout(() => {
                if (notification.parentNode) {
                    notification.parentNode.removeChild(notification);
                }
            }, 300);
        }, 3000);
    }
    
    // Inicializar la primera página
    simulatePageChange();
});