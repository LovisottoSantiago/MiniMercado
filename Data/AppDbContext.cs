using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MiniMercado.Models;

namespace MiniMercado.Data
{
    public class AppDbContext : DbContext
    {
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
    public virtual DbSet<DetalleFactura> DetalleFactura { get; set; }

    public virtual DbSet<Factura> Factura { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<Proveedor> Proveedor { get; set; }

    }
}
