// Para evitar el input auto
$('input').attr('autocomplete','off');


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


function acortarNombre(nombre, maxLength = 17) {
    if (nombre.length > maxLength) {
        return nombre.substring(0, maxLength - 3) + "...";
    }
    return nombre;
}



// Carrito de compras
function guardarCarritoEnLocalStorage() {
    localStorage.setItem("carrito", JSON.stringify(carrito));
}

function cargarCarritoDesdeLocalStorage() {
    const data = localStorage.getItem("carrito");
    if (data) {
        try {
            const items = JSON.parse(data);
            if (Array.isArray(items)) {
                carrito.length = 0;
                items.forEach(p => carrito.push(p));
            }
        } catch (e) {
            console.error("Error al cargar el carrito:", e);
        }
    }
}

// Cargar el carrito al iniciar la página
const carrito = [];
cargarCarritoDesdeLocalStorage();

let totalCarrito = 0;

function agregarAlCarrito(producto) {
    const existente = carrito.find(p => p.id === producto.id);
    if (existente) {
        existente.cantidad += producto.cantidad;
    } else {
        carrito.push(producto);
    }
    guardarCarritoEnLocalStorage();
    renderizarCarrito();
}

function vaciarCarrito() {
    carrito.length = 0; 
    document.querySelector(".sub-total-txt h2").textContent = `Total: $0.00`;
    guardarCarritoEnLocalStorage();
    renderizarCarrito();
}

function renderizarCarrito() {
    const tbody = document.querySelector("#carritoTable tbody");
    tbody.innerHTML = "";
    let totalCarrito = 0;

    // Si el carrito está vacío, mostrar una fila de placeholder
    if (carrito.length === 0) {
            const filaVacia = document.createElement("tr");
            filaVacia.classList.add("fila-producto", "placeholder-row");
            filaVacia.innerHTML = `
                <td style="min-width: 5rem;">&nbsp;</td>
                <td style="min-width: 8rem;">&nbsp;</td>
                <td style="min-width: 5rem;">&nbsp;</td>
                <td style="min-width: 5rem;">&nbsp;</td>
            `;
            tbody.appendChild(filaVacia);
            return;
        }


    carrito.forEach(p => {
        const fila = document.createElement("tr");
        const subtotal = p.precio * p.cantidad;
        totalCarrito += subtotal;

        fila.innerHTML = `
                <td>${p.cantidad}</td>
                <td>${acortarNombre(p.nombre)}</td>
                <td>${p.precio.toFixed(2)}</td>
                <td>${subtotal.toFixed(2)}</td>
            `;
        tbody.appendChild(fila);
    });

    document.querySelector(".sub-total-txt h2").textContent = `Total: $${totalCarrito.toFixed(2)}`;
}


document.getElementById("vaciarCarritoBtn").addEventListener("click", function () {
    if (carrito.length === 0) {
        // alert("El carrito ya está vacío.");
        showAlert("El carrito ya está vacío.");
        return;
    }

    showConfirm("¿Estás seguro de que deseas vaciar el carrito?", () => {
        vaciarCarrito();
        renderizarCarrito();
        // alert("Carrito vaciado.");
    }
    , () => {
        // Acción cancelada
    });
});


// ---------------------- Modal cantidad ----------------------

let productoTemporal = null;
let modoModal = null;  // "id" o "cantidad"

function abrirModalCantidad(producto = null, modo = "cantidad") {
    modoModal = modo;
    const modal = document.getElementById("modalCantidad");
    const inputCantidad = document.getElementById("inputCantidad");
    const inputId = document.getElementById("inputId");
    const inputPrecioManual = document.getElementById("inputPrecioManual");
    const titulo = document.getElementById("modalTitulo");

    productoTemporal = producto;

    inputCantidad.style.display = "none";
    inputId.style.display = "none";
    inputPrecioManual.style.display = "none";


    if (modo === "id") {
        titulo.textContent = "Ingrese ID del producto";
        inputId.style.display = "block";
        inputId.style.autocomplete = "off";
        inputCantidad.style.display = "none";
        inputId.value = "";
        inputId.focus();
    } else if (modo === "cantidad") {
        titulo.textContent = `Ingrese cantidad para "${productoTemporal.nombre}"`;
        inputCantidad.style.display = "block";
        inputCantidad.value = 1;
        inputCantidad.focus();
    } else if (modo === "precioManual") {
        titulo.textContent = `Ingrese precio para "${productoTemporal.nombre}"`;
        inputPrecioManual.style.display = "block";
        inputPrecioManual.value = "";
        inputPrecioManual.focus();
    }

    modal.style.display = "flex";
}


function cerrarModalCantidad() {
    document.getElementById("modalCantidad").style.display = "none";
    productoTemporal = null;
    modoModal = null;
}


document.getElementById("btnConfirmarCantidad").addEventListener("click", () => {
    if (modoModal === "id") {
        const idIngresado = document.getElementById("inputId").value.trim();
        if (!idIngresado) {
            showAlert("Ingrese un ID válido.");
            return;
        }

        // Buscar producto por ID
        fetch(`/VentaScreen/ObtenerProducto?id=${idIngresado}`)
            .then(response => {
                if (!response.ok) throw new Error("Producto no encontrado");
                return response.json();
            })
            .then(producto => {
                cerrarModalCantidad();

                // Si es un producto de precio manual
                if (producto.esPrecioManual) {
                    abrirModalCantidad({
                        id: producto.idProducto,
                        nombre: producto.nombre,
                        precio: 0
                    }, "precioManual");
                } else {
                    abrirModalCantidad({
                        id: producto.idProducto,
                        nombre: producto.nombre,
                        precio: producto.precioUnitario
                    }, "cantidad");
                }
            })
            .catch(error => alert(error.message));
    } else if (modoModal === "cantidad") {
        const cantidad = parseInt(document.getElementById("inputCantidad").value, 10);
        if (isNaN(cantidad) || cantidad <= 0) {
            showAlert("Ingrese una cantidad válida.");
            return;
        }
        agregarAlCarrito({ ...productoTemporal, cantidad });
        cerrarModalCantidad();
    } else if (modoModal === "precioManual") {
        const precioManual = parseFloat(document.getElementById("inputPrecioManual").value);
        if (isNaN(precioManual) || precioManual <= 0) {
            showAlert("Ingrese un precio válido.");
            return;
        }

        agregarAlCarrito({ ...productoTemporal, precio: precioManual, cantidad: 1 });
        cerrarModalCantidad();
    }
});


document.getElementById("btnCancelarCantidad").addEventListener("click", () => {
    cerrarModalCantidad();
});

// Cambiá el evento del botón para abrir el modal en modo ID:
document.getElementById("btnAgregarId").addEventListener("click", () => {
    abrirModalCantidad(null, "id");
});


let ultimaFilaSeleccionada = null;

// Cargar  por doble click en la tabla de productos
function asignarEventosDobleClick() {
    const filas = document.querySelectorAll("#productTable tbody tr");

    filas.forEach(fila => {
        fila.addEventListener("click", function () {
            filas.forEach(f => f.classList.remove("selected"));
            this.classList.add("selected");

            const celdas = this.querySelectorAll("td");

            if (this === ultimaFilaSeleccionada) {
                const id = celdas[0].textContent.trim();

                fetch(`/VentaScreen/ObtenerProducto?id=${id}`)
                    .then(response => {
                        if (!response.ok) throw new Error("Producto no encontrado");
                        return response.json();
                    })
                    .then(producto => {
                        if (producto.esPrecioManual) {
                            abrirModalCantidad({
                                id: producto.idProducto,
                                nombre: producto.nombre,
                                precio: 0
                            }, "precioManual");
                        } else {
                            abrirModalCantidad({
                                id: producto.idProducto,
                                nombre: producto.nombre,
                                precio: producto.precioUnitario
                            }, "cantidad");
                        }
                    })
                    .catch(error => alert(error.message));
            }

            ultimaFilaSeleccionada = this;
        });
    });
}







// ---------------------- Modal Genérico para alertas/confirmaciones ----------------------
function showAlert(message, onClose) {
  const modal = document.getElementById("customModal");
  const msg = document.getElementById("customModalMessage");
  const btnOk = document.getElementById("customModalBtnOk");
  const btnCancel = document.getElementById("customModalBtnCancel");

  msg.textContent = message;
  btnOk.style.display = "inline-block";
  btnCancel.style.display = "none";

  btnOk.onclick = () => {
    modal.style.display = "none";
    if (onClose) onClose();
  };

  modal.style.display = "flex";
}

function showConfirm(message, onConfirm, onCancel) {
  const modal = document.getElementById("customModal");
  const msg = document.getElementById("customModalMessage");
  const btnOk = document.getElementById("customModalBtnOk");
  const btnCancel = document.getElementById("customModalBtnCancel");

  msg.textContent = message;
  btnOk.style.display = "inline-block";
  btnCancel.style.display = "inline-block";

  btnOk.onclick = () => {
    modal.style.display = "none";
    if (onConfirm) onConfirm();
  };

  btnCancel.onclick = () => {
    modal.style.display = "none";
    if (onCancel) onCancel();
  };

  modal.style.display = "flex";
}





// Cuando la página cargue, renderizar carrito desde localStorage
document.addEventListener("DOMContentLoaded", function () {
    renderizarCarrito();
    asignarEventosDobleClick();
});




// Realizar venta
const modalPago = document.getElementById("modalFormaPago");
const formasPagoButtons = document.getElementById("formasPagoButtons");
const btnCancelarPago = document.getElementById("btnCancelarPago");

document.getElementById("btnConfirmarVenta").addEventListener("click", function () {
    if (carrito.length === 0) {
        showAlert("El carrito está vacío.");
        return;
    }
    modalPago.style.display = "flex"; // Mostrar modal
});

// Cerrar modal al cancelar
btnCancelarPago.addEventListener("click", function () {
    modalPago.style.display = "none";
});

// Evento para todos los botones de forma de pago
formasPagoButtons.addEventListener("click", function(event) {
    const button = event.target;
    if (button.tagName !== "BUTTON") return; // Ignorar clicks fuera de botones

    const formaPago = button.getAttribute("data-valor");
    if (!formaPago) return;

    modalPago.style.display = "none";

    const ventaData = carrito.map(p => ({
        idProducto: p.id,
        cantidad: p.cantidad,
        precioUnitario: p.precio
    }));

    // Mostrar loader
    document.getElementById("loaderOverlay").style.display = "flex";

    fetch(`/VentaScreen/RealizarVenta?formaPago=${encodeURIComponent(formaPago)}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(ventaData)
    })
    .then(response => {
        if (!response.ok) throw new Error("Error al realizar la venta.");
        return response.json();
    })
    .then(data => {
        showAlert("Venta realizada con éxito. Nro Factura: " + data.facturaId);
        vaciarCarrito();
        renderizarCarrito();
        recargarTablaProductos();
    })
    .catch(error => {
        console.error(error);
        showAlert("Hubo un error al procesar la venta.");
    })
    .finally(() => {
        document.getElementById("loaderOverlay").style.display = "none";
    });
});



// Recargar tabla de productos despues de vender
function recargarTablaProductos() {
    fetch('/VentaScreen/ObtenerProductosTabla')
        .then(response => {
            if (!response.ok) throw new Error("Error al obtener productos.");
            return response.json();
        })
        .then(productos => {
            const tbody = document.querySelector('#productTable tbody');
            tbody.innerHTML = ''; // limpiar contenido actual

            productos.forEach(p => {
                let nombre = p.nombre.length > 25 ? p.nombre.substring(0, 22) + "..." : p.nombre;

                const tr = document.createElement('tr');
                tr.classList.add('fila-producto');

                tr.innerHTML = `
                    <td>${p.idProducto}</td>
                    <td>${nombre}</td>
                    <td>${p.stock}</td>
                    <td>${p.precioUnitario.toFixed(2)}</td>
                    <td>${p.proveedor}</td>
                `;

                tbody.appendChild(tr);
            });
            
            asignarEventosDobleClick();
        })
        .catch(error => {
            console.error(error);
            showAlert("Error al recargar los productos.");
        });
}
