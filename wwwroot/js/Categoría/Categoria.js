document.addEventListener("DOMContentLoaded", function () {

    // --- Modal: Cargar formulario dinámico y categorías ---
    $(document).on("click", ".open-modal", function (e) {
        e.preventDefault();
        var url = $(this).data("url");

        $.get(url, function (data) {
            $("#productoModalContent").html(data);

            const modal = new bootstrap.Modal(document.getElementById('productoModal'), {
                backdrop: 'static',
                keyboard: false
            });

            modal.show();

            cargarCategoriasEnModal();
        });
    });

    // --- Categorías generales ---
    const categoriaSelect = document.getElementById("categoriaSelect");
    const btnCrearCategoria = document.getElementById("btnCrearCategoria");

    let categorias = [];

    if (categoriaSelect) {
        fetch('/StockScreen/GetCategorias?t=' + new Date().getTime())
            .then(response => response.json())
            .then(data => {
                categorias = data;
                renderizarCategorias();
            })
            .catch(error => console.error("Error al cargar categorías:", error));
    }

    if (btnCrearCategoria) {
        btnCrearCategoria.addEventListener("click", () => {
            const nuevaCategoria = prompt("Ingrese el nombre de la nueva categoría:");
            if (!nuevaCategoria) return;

            fetch("/StockScreen/AgregarCategoria", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(nuevaCategoria)
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(text); });
                    }
                    return fetch('/StockScreen/GetCategorias?t=' + new Date().getTime());
                })
                .then(response => response.json())
                .then(data => {
                    categorias = data;
                    renderizarCategorias();
                    alert("Categoría agregada correctamente.");
                })
                .catch(error => alert("Error al agregar la categoría: " + error.message));
        });
    }

    function renderizarCategorias() {
        categoriaSelect.innerHTML = "";
        categorias.forEach(categoria => {
            const option = document.createElement("option");
            option.value = categoria.nombre;
            option.textContent = categoria.nombre;
            categoriaSelect.appendChild(option);
        });
    }

    // --- Función para cuando se carga un modal dinámico ---
    function cargarCategoriasEnModal() {
        const categoriaSelect = document.getElementById("categoriaSelect");
        const btnCrearCategoria = document.getElementById("btnCrearCategoria");
        if (!categoriaSelect) return;

        fetch('/StockScreen/GetCategorias?t=' + new Date().getTime())
            .then(response => response.json())
            .then(data => {
                categoriaSelect.innerHTML = "";
                data.forEach(categoria => {
                    const option = document.createElement("option");
                    option.value = categoria.nombre;
                    option.textContent = categoria.nombre;
                    categoriaSelect.appendChild(option);
                });
            })
            .catch(error => console.error("Error al cargar categorías:", error));

        if (btnCrearCategoria) {
            btnCrearCategoria.removeEventListener("click", crearCategoriaHandler);
            btnCrearCategoria.addEventListener("click", crearCategoriaHandler);
        }

        function crearCategoriaHandler() {
            const nuevaCategoria = prompt("Ingrese el nombre de la nueva categoría:");
            if (!nuevaCategoria) return;

            fetch("/StockScreen/AgregarCategoria", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(nuevaCategoria)
            })
                .then(response => {
                    if (!response.ok) {
                        return response.text().then(text => { throw new Error(text); });
                    }
                    return fetch('/StockScreen/GetCategorias?t=' + new Date().getTime());
                })
                .then(response => response.json())
                .then(data => {
                    categoriaSelect.innerHTML = "";
                    data.forEach(categoria => {
                        const option = document.createElement("option");
                        option.value = categoria.nombre;
                        option.textContent = categoria.nombre;
                        categoriaSelect.appendChild(option);
                    });
                    alert("Categoría agregada correctamente.");
                })
                .catch(error => alert("Error al agregar la categoría: " + error.message));
        }
    }
});
