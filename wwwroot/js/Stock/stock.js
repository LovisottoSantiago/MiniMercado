//FUNCION BARRA BÚSQUEDA PRODUCTOS
document.addEventListener("DOMContentLoaded", function () {

    //FUNCION SELECCIÓN FILA PRODUCTO
    const filas = document.querySelectorAll(".fila-producto");

    filas.forEach(fila => {
        fila.addEventListener("click", function () {
            document.querySelectorAll(".acciones").forEach(acc => acc.style.visibility = "hidden");
            filas.forEach(f => f.classList.remove("selected"));

            const acciones = this.querySelector(".acciones");
            if (acciones) {
                acciones.style.visibility = "visible";
                acciones.style.width = "auto";
                acciones.style.margin = "auto";
                this.classList.add("selected");
            }
        });
    });

    

});