using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;

namespace MiniMercado.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CierreCajaScreenController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public CierreCajaScreenController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(bool mostrarTodas = false, DateTime? fechaFiltro = null)
        {
            var facturas = _context.Factura.AsQueryable(); // Obtener todas las facturas

            if (fechaFiltro.HasValue)
            {
                var fecha = fechaFiltro.Value.Date;
                facturas = facturas.Where(f => f.Fecha.HasValue && f.Fecha.Value.Date == fecha);
            }
            else if (!mostrarTodas)
            {
                var hoy = DateTime.Today;
                facturas = facturas.Where(f => f.Fecha.HasValue && f.Fecha.Value.Date == hoy);
            }

            var lista = facturas.OrderByDescending(f => f.Fecha).ToList();

            ViewData["MostrarTodas"] = mostrarTodas;
            ViewData["FechaFiltro"] = fechaFiltro?.ToString("yyyy-MM-dd");

            return View(lista);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}