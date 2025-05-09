using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniMercado.Models;

public partial class DetalleFactura
{
    [Key] public int IdDetalleFactura { get; set; }

    public int? IdFactura { get; set; }

    public int? IdProducto { get; set; }

    public int? Cantidad { get; set; }

    public decimal? PrecioUnitario { get; set; }

    [ForeignKey(nameof(IdFactura))]
    public virtual Factura? IdFacturaNavigation { get; set; }

    [ForeignKey(nameof(IdProducto))]
    public virtual Producto? IdProductoNavigation { get; set; }

}
