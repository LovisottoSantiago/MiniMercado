using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;

namespace MiniMercado.Controllers
{
    [Authorize(Roles = "Administrador")]
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
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre");
            return View();
        }

        // POST: Producto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProducto,Nombre,PrecioUnitario,Stock,Proveedor,Estado,EsPrecioManual")] Producto producto)
        {
            // Si es precio manual, ignorar precio y stock (o validarlos según lógica)
            if (producto.EsPrecioManual)
            {
                producto.PrecioUnitario = null;
                producto.Stock = null;
            }

            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", producto.Proveedor);
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
        public async Task<IActionResult> Edit(int id, [Bind("IdProducto,Nombre,PrecioUnitario,Stock,Proveedor,Estado,EsPrecioManual")] Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (producto.EsPrecioManual)
                    {
                        producto.PrecioUnitario = null;
                        producto.Stock = null;
                    }

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


        // GET: Producto/Delete/5
        public async Task<IActionResult> DeleteParcial(int? id, string origen)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Producto
                .Include(p => p.ProveedorNavigation)
                .FirstOrDefaultAsync(m => m.IdProducto == id);
            ViewData["Origen"] = origen; // Para saber de dónde se llama
            if (producto == null)
            {
                return NotFound();
            }

            return PartialView(producto);
        }

        [HttpPost, ActionName("DeleteParcial")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedParcial(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                _context.Producto.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "StockScreen");
        }


        private bool ProductoExists(int id)
        {
            return _context.Producto.Any(e => e.IdProducto == id);
        }


        // Controladores parciales nuevos
        public IActionResult CreateParcial(string origen)
        {
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre");
            ViewData["Origen"] = origen; // Para saber de dónde se llama
            var producto = new Producto
            {
                Estado = true // Valor por defecto
            };

            return PartialView(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateParcial([Bind("IdProducto,Nombre,PrecioUnitario,Stock,Proveedor,Estado,EsPrecioManual")] Producto producto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "StockScreen");
            }
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", producto.Proveedor);

            return View(producto);
        }


        public async Task<IActionResult> EditParcial(int? id, string origen)
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
            ViewData["Origen"] = origen; //
            return PartialView(producto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditParcial(int id, [Bind("IdProducto,Nombre,PrecioUnitario,Stock,Proveedor,Estado,EsPrecioManual")] Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (producto.EsPrecioManual)
                    {
                        producto.PrecioUnitario = null;
                        producto.Stock = null;
                    }

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
                return RedirectToAction("Index", "StockScreen");
            }
            ViewData["Proveedor"] = new SelectList(_context.Proveedor, "IdProveedor", "Nombre", producto.Proveedor);

            return View(producto);
        }









    }
}
