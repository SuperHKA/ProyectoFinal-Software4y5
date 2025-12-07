// Sistema de verificación de estado de la API
class ApiStatusChecker {
    constructor() {
        this.apiURL = 'http://localhost:5202/api/banco/health';
        this.checkInterval = 5000; // 5 segundos
        this.isConnected = null;
        this.statusElement = null;
        this.intervalId = null;
        this.hideTimeout = null;
        
        this.init();
    }

    init() {
        // Crear el elemento de estado
        this.createStatusElement();
        
        // Iniciar verificación
        this.startChecking();
    }

    createStatusElement() {
        // Crear el elemento de estado si no existe
        if (document.getElementById('api-status-indicator')) {
            this.statusElement = document.getElementById('api-status-indicator');
            return;
        }

        const statusDiv = document.createElement('div');
        statusDiv.id = 'api-status-indicator';
        statusDiv.className = 'api-status-indicator';
        statusDiv.innerHTML = `
            <div class="api-status-content">
                <i class="fas fa-circle api-status-icon"></i>
                <span class="api-status-text">Verificando conexión...</span>
            </div>
        `;
        
        document.body.appendChild(statusDiv);
        this.statusElement = statusDiv;
    }

    async checkConnection() {
        try {
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), 3000); // 3 segundos timeout
            
            const response = await fetch(this.apiURL, {
                method: 'GET',
                signal: controller.signal
            });
            
            clearTimeout(timeoutId);
            
            if (response.ok) {
                this.handleConnectionSuccess();
                return true;
            } else {
                this.handleConnectionError('API no disponible');
                return false;
            }
        } catch (error) {
            if (error.name === 'AbortError') {
                this.handleConnectionError('Tiempo de espera agotado');
            } else {
                this.handleConnectionError('No se puede conectar con la API');
            }
            return false;
        }
    }

    handleConnectionSuccess() {
        if (this.isConnected === true) {
            // Ya estaba conectado, no hacer nada
            return;
        }

        this.isConnected = true;
        
        // Actualizar UI a estado conectado
        this.statusElement.className = 'api-status-indicator connected';
        this.statusElement.querySelector('.api-status-text').textContent = 'API Conectada';
        
        // Mostrar brevemente y luego ocultar
        this.statusElement.style.display = 'flex';
        
        // Limpiar timeout anterior si existe
        if (this.hideTimeout) {
            clearTimeout(this.hideTimeout);
        }
        
        // Ocultar después de 3 segundos
        this.hideTimeout = setTimeout(() => {
            this.statusElement.style.display = 'none';
        }, 3000);
    }

    handleConnectionError(message) {
        if (this.isConnected === false) {
            // Ya estaba desconectado, solo actualizar mensaje si cambió
            const currentText = this.statusElement.querySelector('.api-status-text').textContent;
            if (currentText !== message) {
                this.statusElement.querySelector('.api-status-text').textContent = message;
            }
            return;
        }

        this.isConnected = false;
        
        // Limpiar timeout de ocultación si existe
        if (this.hideTimeout) {
            clearTimeout(this.hideTimeout);
            this.hideTimeout = null;
        }
        
        // Actualizar UI a estado desconectado
        this.statusElement.className = 'api-status-indicator disconnected';
        this.statusElement.querySelector('.api-status-text').textContent = message;
        
        // Mostrar permanentemente
        this.statusElement.style.display = 'flex';
    }

    startChecking() {
        // Hacer primera verificación inmediatamente
        this.checkConnection();
        
        // Configurar verificación periódica
        this.intervalId = setInterval(() => {
            this.checkConnection();
        }, this.checkInterval);
    }

    stopChecking() {
        if (this.intervalId) {
            clearInterval(this.intervalId);
            this.intervalId = null;
        }
        
        if (this.hideTimeout) {
            clearTimeout(this.hideTimeout);
            this.hideTimeout = null;
        }
    }

    destroy() {
        this.stopChecking();
        
        if (this.statusElement && this.statusElement.parentNode) {
            this.statusElement.parentNode.removeChild(this.statusElement);
        }
    }
}

// Inicializar el verificador cuando el DOM esté listo
let apiStatusChecker = null;

document.addEventListener('DOMContentLoaded', function() {
    apiStatusChecker = new ApiStatusChecker();
});

// Limpiar al cerrar la página
window.addEventListener('beforeunload', function() {
    if (apiStatusChecker) {
        apiStatusChecker.destroy();
    }
});
