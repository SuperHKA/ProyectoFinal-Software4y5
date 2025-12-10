function toggleDarkMode() {
    document.body.classList.toggle('dark-mode');
    
    // Actualizar botón si existe
    const toggleBtn = document.getElementById('darkModeToggle');
    if (toggleBtn) {
        toggleBtn.classList.toggle('active');
    }

    // Guardar preferencia en localStorage
    if (document.body.classList.contains('dark-mode')) {
        localStorage.setItem('darkMode', 'enabled');
    } else {
        localStorage.setItem('darkMode', 'disabled');
    }
}

// Aplicar modo oscuro inmediatamente (antes de DOMContentLoaded para evitar flash)
if (localStorage.getItem('darkMode') === 'enabled') {
    document.documentElement.classList.add('dark-mode');
    if (document.body) {
        document.body.classList.add('dark-mode');
    }
}

// Cargar preferencia al iniciar y sincronizar botón
window.addEventListener('DOMContentLoaded', (event) => {
    if (localStorage.getItem('darkMode') === 'enabled') {
        document.body.classList.add('dark-mode');
        const toggleBtn = document.getElementById('darkModeToggle');
        if (toggleBtn) {
            toggleBtn.classList.add('active');
        }
    }
});

// Hacer la función global
window.toggleDarkMode = toggleDarkMode;