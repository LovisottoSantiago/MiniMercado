using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;

namespace MiniMercado.Controllers
{
    public class VentaScreenController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public VentaScreenController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var productos = _context.Producto
                .Include(p => p.ProveedorNavigation)
                .Where(p => !p.EsPrecioManual) // Solo productos no precio manual
                .ToList();
            ViewData["ModoPrecioManual"] = false;
            return View(productos);
        }

        public IActionResult PrecioManual()
        {
            var productosPrecioManual = _context.Producto
                .Include(p => p.ProveedorNavigation)
                .Where(p => p.EsPrecioManual)
                .ToList();
            ViewData["ModoPrecioManual"] = true;
            return View("Index", productosPrecioManual);  // Reusa la vista Index pero con otro set de datos
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        [HttpGet]
        public IActionResult ObtenerProducto(int id)
        {
            var producto = _context.Producto
                .Include(p => p.ProveedorNavigation)
                .FirstOrDefault(p => p.IdProducto == id);

            if (producto == null)
                return NotFound();

            return Json(new
            {
                idProducto = producto.IdProducto,
                nombre = producto.Nombre,
                precioUnitario = producto.PrecioUnitario,
                stock = producto.Stock,
                esPrecioManual = producto.EsPrecioManual
            });
        }











    }
}
