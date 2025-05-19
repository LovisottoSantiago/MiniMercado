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


    document.querySelectorAll(".acciones a").forEach(action => {
        action.addEventListener("click", function (e) {
            e.preventDefault();
            const actionType = this.textContent.trim();
            const row = this.closest("tr");
            const id = row.cells[0].textContent.trim();
            console.log("Action Type:", actionType);
            console.log("Product ID:", id);

            showActionContainer(actionType, id);
        });
    });
});

function showActionContainer(actionType, id) {
    const actionContainer = document.getElementById("actionContainer");
    const actionContainerTitle = document.querySelector(".actionContainer-title");
    const actionContainerBody = document.querySelector(".actionContainer-body");

    actionContainerTitle.textContent = `${actionType} Producto`;
    if(actionType === "Editar"){
        fetch(`/Producto/EditarParcial/${id}`)
        .then(res => res.text())
        .then(html => {
            actionContainerBody.innerHTML = html;
            actionContainer.style.display = "block";
            document.getElementById("actionContainerBackdrop").style.display = "block";
        })
    }

    // Mostrar el modal
    
    

    // Cerrar el modal al hacer clic en el botón de cerrar
    actionContainer.querySelector(".close-actionContainer").addEventListener("click", function () {
        actionContainer.style.display = "none";
        document.getElementById("actionContainerBackdrop").style.display = "none";

    });
}


    