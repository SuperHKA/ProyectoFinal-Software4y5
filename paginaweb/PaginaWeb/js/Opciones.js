function aparecerCuadro() {
    var cuadro = document.getElementById("cuadro-opciones");
    if (cuadro.style.display === "none" || cuadro.style.display === "") {
        cuadro.style.display = "block";
    } else {
        cuadro.style.display = "none";
    }
}

// Actualizar opciones según estado de autenticación cuando se carga la página
document.addEventListener('DOMContentLoaded', function() {
    // Esperar a que los otros scripts se carguen
    setTimeout(function() {
        if (typeof router !== 'undefined') {
            router.updateNavigation();
        }
    }, 100);
});

// Cerrar cuadro al hacer clic fuera
document.addEventListener('click', function(event) {
    const cuadro = document.getElementById("cuadro-opciones");
    const botonCuenta = document.querySelector('.cuenta');
    
    if (cuadro && cuadro.style.display === "block") {
        if (!cuadro.contains(event.target) && !botonCuenta.contains(event.target)) {
            cuadro.style.display = "none";
        }
    }
});