using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMercado.Models;

public partial class Producto
{
    [Key] public int IdProducto { get; set; }

    [MaxLength(50, ErrorMessage = "El código del producto no puede exceder los 50 caracteres.")]
    public string? CodigoDeBarra { get; set; }

    public bool EsPrecioManual { get; set; } = false;

    public string Nombre { get; set; } = null!;

    public string? Categoria { get; set; }

    public decimal? PrecioUnitario { get; set; }

    public int? Stock { get; set; }

    public int? Proveedor { get; set; }

    public bool? Estado { get; set; }

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();

    [ForeignKey(nameof(Proveedor))]
    public virtual Proveedor? ProveedorNavigation { get; set; }
}
