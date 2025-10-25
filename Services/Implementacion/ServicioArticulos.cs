using System;
using System.Linq;
using System.Collections.Generic;
using inventario_ferreteria.Models;
using inventario_ferreteria.Services.Interfaces;

namespace inventario_ferreteria.Services.Implementacion
{
 public class ServicioArticulos : IServicioArticulos
 {
 private readonly InventarioContext _context;

 public ServicioArticulos(InventarioContext context)
 {
 _context = context;
 }

 public ArticuloRegistrarResult RegistrarArticulo(Articulo articulo)
 {
 // Validaciones
 if (string.IsNullOrWhiteSpace(articulo.Codigo))
 return new ArticuloRegistrarResult { Success = false, Message = "El código es obligatorio." };

 if (_context.Articulos.Any(a => a.Codigo == articulo.Codigo))
 return new ArticuloRegistrarResult { Success = false, Message = "Código ya existe." };

 if (articulo.Preciocompra <0 || articulo.Precioventa <0)
 return new ArticuloRegistrarResult { Success = false, Message = "Los precios deben ser positivos." };

 if (articulo.Precioventa < articulo.Preciocompra)
 return new ArticuloRegistrarResult { Success = false, Message = "El precio de venta no puede ser menor al precio de compra." };

 try
 {
 _context.Articulos.Add(articulo);
 _context.SaveChanges();

 // Alertar si stock menor al mínimo
 if (articulo.Stockminimo.HasValue && articulo.Stock < articulo.Stockminimo.Value)
 {
 // en una aplicación real podríamos usar notificaciones; aquí devolvemos mensaje
 return new ArticuloRegistrarResult { Success = true, Message = "Registrado. Atención: stock por debajo del mínimo." };
 }

 return new ArticuloRegistrarResult { Success = true, Message = "Registrado correctamente." };
 }
 catch (Exception ex)
 {
 return new ArticuloRegistrarResult { Success = false, Message = "Error al guardar: " + ex.Message };
 }
 }

 public ArticuloActualizarResult ActualizarArticulo(Articulo articulo)
 {
 if (string.IsNullOrWhiteSpace(articulo.Codigo))
 return new ArticuloActualizarResult { Success = false, Message = "El código es obligatorio." };

 var existente = _context.Articulos.Find(articulo.Codigo);
 if (existente == null) return new ArticuloActualizarResult { Success = false, Message = "Artículo no encontrado." };

 if (articulo.Preciocompra <0 || articulo.Precioventa <0)
 return new ArticuloActualizarResult { Success = false, Message = "Los precios deben ser positivos." };

 if (articulo.Precioventa < articulo.Preciocompra)
 return new ArticuloActualizarResult { Success = false, Message = "El precio de venta no puede ser menor al precio de compra." };

 try
 {
 existente.Nombre = articulo.Nombre;
 existente.Categoria = articulo.Categoria;
 existente.Preciocompra = articulo.Preciocompra;
 existente.Precioventa = articulo.Precioventa;
 existente.Stock = articulo.Stock;
 existente.Proveedor = articulo.Proveedor;
 existente.Stockminimo = articulo.Stockminimo;

 _context.Articulos.Update(existente);
 _context.SaveChanges();

 // alertar
 if (existente.Stockminimo.HasValue && existente.Stock < existente.Stockminimo.Value)
 return new ArticuloActualizarResult { Success = true, Message = "Actualizado. Atención: stock por debajo del mínimo." };

 return new ArticuloActualizarResult { Success = true, Message = "Actualizado correctamente." };
 }
 catch (Exception ex)
 {
 return new ArticuloActualizarResult { Success = false, Message = "Error al actualizar: " + ex.Message };
 }
 }

 public Articulo? ObtenerPorCodigo(string codigo)
 {
 return _context.Articulos.Find(codigo);
 }

 public IEnumerable<Articulo> BuscarPorNombre(string nombre)
 {
 if (string.IsNullOrWhiteSpace(nombre)) return _context.Articulos.ToList();
 return _context.Articulos.Where(a => a.Nombre.Contains(nombre)).ToList();
 }

 public bool EliminarArticulo(string codigo)
 {
 var articulo = _context.Articulos.Find(codigo);
 if (articulo == null) return false;
 _context.Articulos.Remove(articulo);
 _context.SaveChanges();
 return true;
 }

 // SOAP wrappers
 public ArticuloRegistrarResult InsertarArticuloSoap(Articulo articulo)
 {
 // Reusar lógica
 return RegistrarArticulo(articulo);
 }

 public Articulo? ConsultarArticuloPorCodigoSoap(string codigo)
 {
 return ObtenerPorCodigo(codigo);
 }
 }
}
