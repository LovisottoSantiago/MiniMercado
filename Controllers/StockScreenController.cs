using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace MiniMercado.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class StockScreenController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public StockScreenController(ILogger<HomeController> logger, AppDbContext context)
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


        [AllowAnonymous]
        [HttpPost]
        public IActionResult AgregarCategoria([FromBody] string nuevaCategoria)
        {
            var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data/categorias.json");

            var json = System.IO.File.ReadAllText(rutaArchivo);
            var lista = JsonSerializer.Deserialize<List<Categoria>>(json) ?? new List<Categoria>();

            if (lista.Any(c => c.nombre.Equals(nuevaCategoria, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("La categoría ya existe.");
            }

            lista.Add(new Categoria { nombre = nuevaCategoria });
            var nuevoJson = JsonSerializer.Serialize(lista, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(rutaArchivo, nuevoJson);

            return Ok();
        }


        [HttpGet]
        public IActionResult GetCategorias()
        {
            var rutaArchivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data/categorias.json");

            if (!System.IO.File.Exists(rutaArchivo))
            {
                return NotFound(new { error = "Archivo de categorías no encontrado" });
            }

            var json = System.IO.File.ReadAllText(rutaArchivo);
            return Content(json, "application/json");
        }



    }
}

        public class Categoria
        {
            public string nombre { get; set; }
        }