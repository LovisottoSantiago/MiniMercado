document.addEventListener("DOMContentLoaded", function () {

    const searchInput = document.getElementById("busquedaProductos");
    const rows = document.querySelectorAll("#productTable tbody tr");

    searchInput.addEventListener("keyup", function () {

        const filter = this.value.toLowerCase();
        rows.forEach(row => {

            const id = row.cells[0].textContent.toLowerCase();
            const name = row.cells[1].textContent.toLowerCase();

            if (id.includes(filter) || name.includes(filter)) {
                row.style.display = "";
            } else {
                row.style.display = "none";
            }

        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
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


    