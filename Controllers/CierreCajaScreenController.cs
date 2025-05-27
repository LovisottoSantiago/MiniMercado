using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;

namespace MiniMercado.Controllers
{
    public class CierreCajaScreenController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public CierreCajaScreenController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

         public IActionResult Index(bool mostrarTodas = false)
        {
            var hoy = DateTime.Today;

            var facturas = _context.Factura
                .Where(f => mostrarTodas || (f.Fecha.HasValue && f.Fecha.Value.Date == hoy))
                .OrderByDescending(f => f.Fecha)
                .ToList();

            ViewData["MostrarTodas"] = mostrarTodas;

            return View(facturas);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
