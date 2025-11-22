function aparecerCuadro(){
   var cuadro= document.getElementById("cuadro-opciones");
    if (cuadro.style.display=== "none"){
        cuadro.style.display = "block";
    }
    else {
        cuadro.style.display= "none";
    }
}