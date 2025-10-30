using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization; // ¡NUEVO! Para la serialización SOAP
using Microsoft.EntityFrameworkCore;

namespace inventario_ferreteria.Models;

[Table("articulos")]
[DataContract] // ¡CRÍTICO! Marca esta clase como parte del contrato de datos SOAP
public partial class Articulo
{
    [Key]
    [Column("codigo")]
    [StringLength(20)]
    [DataMember] // ¡CRÍTICO! Permite que esta propiedad se transfiera por SOAP
    public string Codigo { get; set; } = null!;

    [Column("nombre")]
    [StringLength(100)]
    [DataMember]
    public string Nombre { get; set; } = null!;

    [Column("categoria")]
    [StringLength(50)]
    [DataMember]
    public string Categoria { get; set; } = null!;

    [Column("preciocompra")]
    [Precision(10, 2)]
    [DataMember]
    public decimal Preciocompra { get; set; }

    [Column("precioventa")]
    [Precision(10, 2)]
    [DataMember]
    public decimal Precioventa { get; set; }

    [Column("stock")]
    [DataMember]
    public int Stock { get; set; }

    [Column("proveedor")]
    [StringLength(100)]
    [DataMember]
    public string? Proveedor { get; set; }

    [Column("stockminimo")]
    [DataMember]
    public int? Stockminimo { get; set; }
}