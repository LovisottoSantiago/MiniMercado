using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;

namespace MiniMercado.Controllers
{
    public class ProductoController : Controller
    {
        private readonly AppDbContext _context;

        public ProductoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Producto
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Producto.Include(p => p.ProveedorNavigation);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Producto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.ProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Producto/Create
        public IActionResult Create()
        {
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor");
            return View();
        }

        // POST: Producto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProducto,Nombre,PrecioUnitario,Stock,Proveedor,Estado")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "IdProveedor", producto.Proveedor);
            return View(producto);
        }

        // GET: Producto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", producto.Proveedor);
            return View(producto);
        }

        // POST: Producto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,Nombre,PrecioUnitario,Stock,Proveedor,Estado")] Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdProducto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", producto.Proveedor);
            return View(producto);
        }

        // GET: Producto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.ProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Producto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                _context.Producto.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult EditarParcial(Producto producto)
        {
            Console.WriteLine($"EditarParcial - Id: {producto.IdProducto}, Nombre: {producto.Nombre}, Precio: {producto.PrecioUnitario}, Stock: {producto.Stock}, Estado: {producto.Estado}, Proveedor: {producto.Proveedor}");
            
            if (!ModelState.IsValid) //Verifica que el modelo cumpla las reglas de validación (atributos [Required], rangos, etc).
            {
                var errores = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, errors = errores }); //Si hay errores, devuelve JSON con success: false y el listado de errores para mostrar en la UI.
            }

            var productoExistente = _context.Producto.FirstOrDefault(p => p.IdProducto == producto.IdProducto); //_context es el contexto de la base de datos, y se busca el producto por su IdProducto.
            if (productoExistente == null)
            {
                return Json(new { success = false, error = "Producto no encontrado" });
            }

            try
            {
                productoExistente.Nombre = producto.Nombre;
                productoExistente.PrecioUnitario = producto.PrecioUnitario;
                productoExistente.Stock = producto.Stock;
                productoExistente.Estado = producto.Estado;
                productoExistente.Proveedor = producto.Proveedor;

                _context.Entry(productoExistente).State = EntityState.Modified; //Acá le estás diciendo a Entity Framework que el objeto productoExistente ha sido modificado y que debe preparar una actualización (UPDATE) en la base de datos para ese registro.
                _context.SaveChanges(); // Guarda los cambios en la base de datos.

                return Json(new { success = true }); // Si todo sale bien, devuelve JSON con success: true.
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }); // Si ocurre un error, devuelve JSON con success: false y el mensaje de error.
            }
        }
                [HttpGet] // Método para cargar el formulario de edición parcial
public IActionResult EditarParcial(int id)
{
    var producto = _context.Producto.FirstOrDefault(p => p.IdProducto == id); //instancia en producto el producto de la base de datos que tiene ese id
    if (producto == null)
        return NotFound();

    ViewBag.Proveedor = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", producto.Proveedor);

    return PartialView("_EditarProductoPartial", producto);
}


        [HttpPost]

               private bool ProductoExists(int id)
        {
            return _context.Producto.Any(e => e.IdProducto == id);
        }
    }
}
