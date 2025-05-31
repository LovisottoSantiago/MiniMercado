using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Data;
using MiniMercado.Models;
using Microsoft.AspNetCore.Authorization;

namespace MiniMercado.Controllers
{
    [Authorize(Roles = "Administrador")]
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
        public IActionResult ObtenerProducto(string CodigoDeBarra)
        {
            var producto = _context.Producto
                .Include(p => p.ProveedorNavigation)
                .FirstOrDefault(p => p.CodigoDeBarra == CodigoDeBarra);

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


        [HttpPost]
        public async Task<IActionResult> RealizarVenta([FromQuery] string formaPago, [FromBody] List<DetalleFacturaDTO> productos)
        {
            if (productos == null || !productos.Any())
                return BadRequest("El carrito está vacío.");

            // Si no vino nada usamos “Efectivo”
            formaPago ??= "Efectivo";

            var factura = new Factura
            {
                Fecha = DateTime.Now,
                FormaPago = formaPago,
                Total = productos.Sum(p => p.PrecioUnitario * p.Cantidad),
                DetalleFacturas = new List<DetalleFactura>()
            };
            foreach (var item in productos)
            {
                factura.DetalleFacturas.Add(new DetalleFactura
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                });

                // Actualizar stock del producto
                var productoDb = await _context.Producto.FindAsync(item.IdProducto);
                if (productoDb != null && productoDb.Stock.HasValue)
                    productoDb.Stock -= item.Cantidad;
            }

            _context.Factura.Add(factura);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Venta realizada correctamente", facturaId = factura.IdFactura });
        }



        [HttpGet]
        public IActionResult ObtenerProductosTabla()
        {
            var productos = _context.Producto
                .Include(p => p.ProveedorNavigation)
                .Where(p => !p.EsPrecioManual)
                .Select(p => new
                {
                    idProducto = p.IdProducto,
                    nombre = p.Nombre,
                    stock = p.Stock,
                    precioUnitario = p.PrecioUnitario,
                    proveedor = p.ProveedorNavigation.Nombre
                })
                .ToList();

            return Json(productos);
        }






    }
}



    public class DetalleFacturaDTO
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }