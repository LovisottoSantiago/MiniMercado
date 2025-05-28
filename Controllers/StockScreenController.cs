using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;
using Microsoft.AspNetCore.Authorization;

namespace MiniMercado.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class StockScreen : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public StockScreen(ILogger<HomeController> logger, AppDbContext context)
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
    }
}
