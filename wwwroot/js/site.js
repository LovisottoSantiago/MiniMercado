document.addEventListener("DOMContentLoaded", function () { //Funcion para la barra de busqueda.

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
//Funcion para mostrar los botones de acciones al hacer click en la fila de la tabla.
document.addEventListener("DOMContentLoaded", function () {   //sirve para asegurarte de que todo el contenido HTML de la página esté completamente cargado y listo antes de ejecutar tu código JavaScript.


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

    //Selecciona la accion que elegimos
    document.querySelectorAll(".acciones a").forEach(action => { //Por cada accion que tenemos
        action.addEventListener("click", function (e) {  //Agrega un eventlistener a cada accion
            e.preventDefault(); //Previene el comportamiento por defecto del enlace(navegar a otra pagina)
            const actionType = this.textContent.trim(); //Captura el texto del enlace
            const row = this.closest("tr"); //Captura la fila mas cercana al enlace
            const id = row.cells[0].textContent.trim(); //Captura el id de la fila(se asume que el id esta en la primera celda de la fila)
            console.log("Action Type:", actionType); 
            console.log("Product ID:", id);

            showActionContainer(actionType, id);
        });
    });
    document.body.addEventListener("submit", function (e) { // Captura el evento de submit en el body
        const form = e.target;  // Captura el formulario que se esta enviando
        if (form.id === "editForm") { 
            e.preventDefault(); // Previene el comportamiento por defecto del formulario
            console.log("Submit capturado desde script global");

            const formData = new FormData(form); //Se crea un objeto FormData con los campos del formulario.
                                                //Esto es ideal para enviar los datos vía fetch como si fueran de un formulario real.

            fetch("/Producto/EditarParcial", { //Se hace una petición POST a la URL /Producto/EditarParcial.  Se manda el formData como cuerpo, simulando el envío del formulario.
                
                method: "POST",
                body: formData
            })
            .then(res => res.json()) //se espera una respuesta en formato JSON.
            .then(data => { //data tendra la respuesta del servidor.
                if (data.success) { //si la respuesta es exitosa
                    alert("Producto editado correctamente.");
                    document.getElementById("actionContainer").style.display = "none";
                    document.getElementById("actionContainerBackdrop").style.display = "none";
                    location.reload(); //Recarga la página para ver los cambios.
                } else {
                    alert("Hubo un error al guardar.");
                    console.error(data.errors || data.error); // Si hay errores, los muestra en la consola.
                }
            })
            .catch(err => { //Si hay un error en el fetch se ejecuta este bloque.
                console.error("Error en el envío:", err); 
            });
        }
    });


});

function showActionContainer(actionType, id) {
    const actionContainer = document.getElementById("actionContainer");
    const actionContainerTitle = document.querySelector(".actionContainer-title");
    const actionContainerBody = document.querySelector(".actionContainer-body");

    actionContainerTitle.textContent = `${actionType} Producto`;
    if(actionType === "Editar"){
        fetch(`/Producto/EditarParcial/${id}`) //obtiene el formulario de edición parcial
        .then(res => res.text())
        .then(html => {
            actionContainerBody.innerHTML = html;
            actionContainer.style.display = "block";
            document.getElementById("actionContainerBackdrop").style.display = "block";
        })
    }
    else if(actionType === "Eliminar"){
        actionContainerBody.innerHTML = `
            <p>¿Estás seguro de que deseas eliminar el producto con ID ${id}?</p>
            <button id="confirmDelete" class="btn btn-danger">Eliminar</button>
            <button id="cancelDelete" class="btn btn-secondary">Cancelar</button>
        `;
        actionContainer.style.display = "block";
        document.getElementById("actionContainerBackdrop").style.display = "block";

        document.getElementById("confirmDelete").addEventListener("click", function () {
            fetch(`/Producto/Delete/${id}`, { method: "DELETE" })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        alert("Producto eliminado correctamente.");
                        location.reload();
                    } else {
                        alert("Hubo un error al eliminar el producto.");
                    }
                });
        });

        document.getElementById("cancelDelete").addEventListener("click", function () {
            actionContainer.style.display = "none";
            document.getElementById("actionContainerBackdrop").style.display = "none";
        });
    }

    // Mostrar el modal
    
    

    // Cerrar el modal al hacer clic en el botón de cerrar
    actionContainer.querySelector(".close-actionContainer").addEventListener("click", function () {
        actionContainer.style.display = "none";
        document.getElementById("actionContainerBackdrop").style.display = "none";

    });
}



    