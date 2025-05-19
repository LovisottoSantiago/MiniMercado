using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniMercado.Models;

public partial class Factura
{
    [Key] public int IdFactura { get; set; }

    public DateTime? Fecha { get; set; }

    public string? FormaPago { get; set; }

    public decimal? Total { get; set; }

    public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; } = new List<DetalleFactura>();
}
