        using inventario_ferreteria.Data;
        using inventario_ferreteria.Models;
        using inventario_ferreteria.Services.Interfaces;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.Extensions.Logging;
        using System.Collections.Generic;
        using System.Linq;

        namespace inventario_ferreteria.Services.Implementacion
        {
            public class ArticuloRepository : IServicioArticulos
            {
                private readonly InventarioContext _context;
                private readonly ILogger<ArticuloRepository> _logger;

                public ArticuloRepository(InventarioContext context, ILogger<ArticuloRepository> logger)
                {
                    _context = context;
                    _logger = logger;
                }

                private (bool IsValid, string ErrorMessage) ValidarArticulo(Articulo articulo)
                {
                    if (articulo.Preciocompra <= 0)
                        return (false, "El precio de compra debe ser mayor a cero.");

                    if (articulo.Precioventa <= 0)
                        return (false, "El precio de venta debe ser mayor a cero.");

                    if (articulo.Precioventa <= articulo.Preciocompra)
                        return (false, "El precio de venta debe ser mayor que el precio de compra.");

                    if (articulo.Stockminimo.HasValue && articulo.Stockminimo < 0)
                        return (false, "El stock mínimo no puede ser un valor negativo.");

                    if (articulo.Stockminimo.HasValue && articulo.Stock < articulo.Stockminimo.Value)
                        return (false, "El stock no puede ser menor que el stock mínimo permitido.");

                    return (true, string.Empty);
                }

                public ArticuloRegistrarResult RegistrarArticulo(Articulo articulo)
                {
                    if (_context.Articulos.Any(a => a.Codigo == articulo.Codigo))
                        return new ArticuloRegistrarResult { Success = false, Message = "El código ya existe." };

                    var (isValid, errorMessage) = ValidarArticulo(articulo);
                    if (!isValid)
                        return new ArticuloRegistrarResult { Success = false, Message = errorMessage };

                    _context.Articulos.Add(articulo);
                    _context.SaveChanges();

                    return new ArticuloRegistrarResult { Success = true, Message = "Registrado correctamente." };
                }

                public ArticuloActualizarResult ActualizarArticulo(Articulo articulo)
                {
                    var existente = _context.Articulos.AsNoTracking().FirstOrDefault(a => a.Codigo == articulo.Codigo);
                    if (existente == null)
                        return new ArticuloActualizarResult { Success = false, Message = "No encontrado." };

                    var (isValid, errorMessage) = ValidarArticulo(articulo);
                    if (!isValid)
                        return new ArticuloActualizarResult { Success = false, Message = errorMessage };

                    _context.Articulos.Update(articulo);
                    _context.SaveChanges();

                    return new ArticuloActualizarResult { Success = true, Message = "Actualizado correctamente." };
                }

                public Articulo? ObtenerPorCodigo(string codigo)
                {
                    _logger.LogInformation("Iniciando la búsqueda del artículo con código: {Codigo}", codigo);
                    try
                    {
                        var articulo = _context.Articulos.Find(codigo);
                        if (articulo != null)
                        {
                            _logger.LogInformation("Artículo con código {Codigo} encontrado.", codigo);
                        }
                        else
                        {
                            _logger.LogWarning("No se encontró ningún artículo con el código: {Codigo}", codigo);
                        }
                        return articulo;
                    }
                    catch (System.Exception ex)
                    {
                        _logger.LogError(ex, "Error al obtener el artículo con código: {Codigo}", codigo);
                        throw;
                    }
                }

                public IEnumerable<Articulo> BuscarPorNombre(string nombre)
                {
                    return _context.Articulos
                        .Where(x => x.Nombre.Contains(nombre))
                        .ToList();
                }

                public bool EliminarArticulo(string codigo)
                {
                    var articulo = _context.Articulos.Find(codigo);
                    if (articulo == null) return false;
                    _context.Articulos.Remove(articulo);
                    _context.SaveChanges();
                    return true;
                }

                public IEnumerable<Articulo> ObtenerArticulosConStockBajo()
                {
                    return _context.Articulos
                        .Where(a => a.Stockminimo.HasValue && a.Stock <= a.Stockminimo.Value)
                        .ToList();
                }

                public int ContarTotalArticulos()
                {
                    return _context.Articulos.Count();
                }

                public decimal CalcularValorTotalInventario()
                {
                    if (!_context.Articulos.Any())
                        return 0;

                    return _context.Articulos.Sum(a => a.Preciocompra * a.Stock);
                }

                // Métodos expuestos por SOAP
                public ArticuloRegistrarResult InsertarArticuloSoap(Articulo articulo)
                {
                    return RegistrarArticulo(articulo);
                }

                public Articulo? ConsultarArticuloPorCodigoSoap(string codigo)
                {
                    return ObtenerPorCodigo(codigo);
                }
            }
        }
