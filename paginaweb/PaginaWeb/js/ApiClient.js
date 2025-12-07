// Cliente API mejorado para comunicación con el backend
class ApiClient {
    constructor() {
        this.baseURL = 'http://localhost:5202/api';
    }

    // Método auxiliar para hacer peticiones
    async request(endpoint, options = {}) {
        const url = `${this.baseURL}${endpoint}`;
        const token = localStorage.getItem('authToken');

        const config = {
            ...options,
            headers: {
                'Content-Type': 'application/json',
                ...options.headers,
            },
        };

        // Agregar token si existe
        if (token && !options.skipAuth) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }

        try {
            const response = await fetch(url, config);
            const data = await response.json();

            return {
                ok: response.ok,
                status: response.status,
                data: data
            };
        } catch (error) {
            console.error('API Request Error:', error);
            throw error;
        }
    }

    // GET request
    async get(endpoint, options = {}) {
        return this.request(endpoint, {
            ...options,
            method: 'GET',
        });
    }

    // POST request
    async post(endpoint, body, options = {}) {
        return this.request(endpoint, {
            ...options,
            method: 'POST',
            body: JSON.stringify(body),
        });
    }

    // PUT request
    async put(endpoint, body, options = {}) {
        return this.request(endpoint, {
            ...options,
            method: 'PUT',
            body: JSON.stringify(body),
        });
    }

    // DELETE request
    async delete(endpoint, options = {}) {
        return this.request(endpoint, {
            ...options,
            method: 'DELETE',
        });
    }

    // Health check
    async checkHealth() {
        try {
            const response = await this.get('/banco/health', { skipAuth: true });
            return response.ok;
        } catch (error) {
            return false;
        }
    }
}

// Instancia global del cliente API
const apiClient = new ApiClient();
