using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MiniMercado.Models;

public partial class Proveedor
{
    [Key] public int IdProveedor { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Direccion { get; set; }

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
