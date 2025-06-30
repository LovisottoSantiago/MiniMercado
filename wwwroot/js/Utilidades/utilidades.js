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