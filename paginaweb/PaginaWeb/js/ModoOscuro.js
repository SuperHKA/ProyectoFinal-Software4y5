/**
 * Dark Mode Toggle - Global Implementation
 * Applies dark mode across all pages when activated from profile
 */

function toggleDarkMode() {
    document.body.classList.toggle('dark-mode');
    document.documentElement.classList.toggle('dark-mode');
    
    // Actualizar botón si existe
    const toggleBtn = document.getElementById('darkModeToggle');
    if (toggleBtn) {
        toggleBtn.classList.toggle('active');
    }

    // Guardar preferencia en localStorage
    const isDarkMode = document.body.classList.contains('dark-mode');
    if (isDarkMode) {
        localStorage.setItem('darkMode', 'enabled');
    } else {
        localStorage.setItem('darkMode', 'disabled');
    }
    
    // Dispatch custom event for other components to react
    window.dispatchEvent(new CustomEvent('darkModeToggle', { 
        detail: { enabled: isDarkMode } 
    }));
}

/**
 * Apply dark mode immediately on page load
 * This runs before DOMContentLoaded to prevent flash of light mode
 */
(function() {
    const darkModeEnabled = localStorage.getItem('darkMode') === 'enabled';
    if (darkModeEnabled) {
        document.documentElement.classList.add('dark-mode');
        if (document.body) {
            document.body.classList.add('dark-mode');
        }
    }
})();

/**
 * Ensure dark mode is applied after DOM is loaded and sync toggle button
 */
window.addEventListener('DOMContentLoaded', function() {
    const darkModeEnabled = localStorage.getItem('darkMode') === 'enabled';
    
    if (darkModeEnabled) {
        document.body.classList.add('dark-mode');
        document.documentElement.classList.add('dark-mode');
        
        // Sync toggle button state
        const toggleBtn = document.getElementById('darkModeToggle');
        if (toggleBtn) {
            toggleBtn.classList.add('active');
        }
    }
});

// Make functions globally available
window.toggleDarkMode = toggleDarkMode;

// Expose dark mode state getter
window.isDarkMode = function() {
    return localStorage.getItem('darkMode') === 'enabled';
};
