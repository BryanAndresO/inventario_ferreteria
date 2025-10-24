using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace inventario_ferreteria.Models;

[Table("articulos")]
public partial class Articulo
{
    [Key]
    [Column("codigo")]
    [StringLength(20)]
    public string Codigo { get; set; } = null!;

    [Column("nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Column("categoria")]
    [StringLength(50)]
    public string Categoria { get; set; } = null!;

    [Column("preciocompra")]
    [Precision(10, 2)]
    public decimal Preciocompra { get; set; }

    [Column("precioventa")]
    [Precision(10, 2)]
    public decimal Precioventa { get; set; }

    [Column("stock")]
    public int Stock { get; set; }

    [Column("proveedor")]
    [StringLength(100)]
    public string? Proveedor { get; set; }

    [Column("stockminimo")]
    public int? Stockminimo { get; set; }
}
