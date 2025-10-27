using inventario_ferreteria.Models;
using inventario_ferreteria.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Diagnostics;

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
        var stopwatch = Stopwatch.StartNew(); // Inicia medición de tiempo

        // Validaciones
        if (string.IsNullOrWhiteSpace(articulo.Codigo))
            return new ArticuloRegistrarResult { Success = false, Message = "El código es obligatorio." };

        if (_context.Articulos.Any(a => a.Codigo == articulo.Codigo))
            return new ArticuloRegistrarResult { Success = false, Message = "Código ya existe." };

        if (articulo.Preciocompra < 0 || articulo.Precioventa < 0)
            return new ArticuloRegistrarResult { Success = false, Message = "Los precios deben ser positivos." };

        if (articulo.Precioventa < articulo.Preciocompra)
            return new ArticuloRegistrarResult { Success = false, Message = "El precio de venta no puede ser menor al precio de compra." };

        if (articulo.Stockminimo < 0)
            return new ArticuloRegistrarResult { Success = false, Message = "No vale ingresar números menores que 0 en el stock mínimo." };

        try
        {
            _context.Articulos.Add(articulo);
            _context.SaveChanges();

            // Alertar si stock menor al mínimo
            if (articulo.Stock < articulo.Stockminimo)
            {
                return new ArticuloRegistrarResult
                {
                    Success = true,
                    Message = "Registrado correctamente, pero atención: el stock está por debajo del mínimo."
                };
            }

            return new ArticuloRegistrarResult { Success = true, Message = "Registrado correctamente." };
        }
        catch (Exception ex)
        {
            return new ArticuloRegistrarResult { Success = false, Message = "Error al guardar: " + ex.Message };
        }
        finally
        {
            stopwatch.Stop(); // Detener medición
            Console.WriteLine($"Tiempo de ejecución RegistrarArticulo: {stopwatch.ElapsedMilliseconds} ms");
        }
    }


    public ArticuloActualizarResult ActualizarArticulo(Articulo articulo)
        {
            if (string.IsNullOrWhiteSpace(articulo.Codigo))
                return new ArticuloActualizarResult { Success = false, Message = "El código es obligatorio." };

            var existente = _context.Articulos.Find(articulo.Codigo);
            if (existente == null)
                return new ArticuloActualizarResult { Success = false, Message = "Artículo no encontrado." };

            if (articulo.Preciocompra < 0 || articulo.Precioventa < 0)
                return new ArticuloActualizarResult { Success = false, Message = "Los precios deben ser positivos." };

            if (articulo.Precioventa < articulo.Preciocompra)
                return new ArticuloActualizarResult { Success = false, Message = "El precio de venta no puede ser menor al precio de compra." };

            if (articulo.Stockminimo < 0)
                return new ArticuloActualizarResult { Success = false, Message = "No vale ingresar números menores que 0 en el stock mínimo." };

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

                if (existente.Stock < existente.Stockminimo)
                {
                    return new ArticuloActualizarResult
                    {
                        Success = true,
                        Message = "Actualizado correctamente, pero atención: el stock está por debajo del mínimo."
                    };
                }

                return new ArticuloActualizarResult { Success = true, Message = "Actualizado correctamente." };
            }
            catch (Exception ex)
            {
                return new ArticuloActualizarResult { Success = false, Message = "Error al actualizar: " + ex.Message };
            }
        }
        public Articulo? ObtenerPorCodigo(string codigo)
        {
            var stopwatch = Stopwatch.StartNew(); // Inicia medición de tiempo

            // Busca un artículo por su código en la base de datos
            var articulo = _context.Articulos.Find(codigo);

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecución ObtenerPorCodigo: {stopwatch.ElapsedMilliseconds} ms");

            return articulo;
        }

        public IEnumerable<Articulo> BuscarPorNombre(string nombre)
        {
            var stopwatch = Stopwatch.StartNew(); // Inicia medición de tiempo
            IEnumerable<Articulo> resultados;

            // Si no se proporciona nombre, retorna todos los artículos
            if (string.IsNullOrWhiteSpace(nombre))
                resultados = _context.Articulos.ToList();
            else
                // Retorna los artículos cuyo nombre contiene la cadena proporcionada
                resultados = _context.Articulos
                    .Where(a => a.Nombre.Contains(nombre))
                    .AsNoTracking() // Mejora rendimiento en consultas de solo lectura
                    .ToList();

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecución BuscarPorNombre: {stopwatch.ElapsedMilliseconds} ms");

            return resultados;
        }



        public bool EliminarArticulo(string codigo)
        {
            var stopwatch = Stopwatch.StartNew(); // Inicia medición de tiempo

            var articulo = _context.Articulos.Find(codigo);
            if (articulo == null)
            {
                stopwatch.Stop();
                Console.WriteLine($"Tiempo de ejecución EliminarArticulo: {stopwatch.ElapsedMilliseconds} ms");
                return false;
            }

            _context.Articulos.Remove(articulo);
            _context.SaveChanges();

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecución EliminarArticulo: {stopwatch.ElapsedMilliseconds} ms");

            return true;
        }

        // SOAP wrappers (ya reutilizan lógica, así que RNF2 se cumple indirectamente)
        public ArticuloRegistrarResult InsertarArticuloSoap(Articulo articulo)
        {
            return RegistrarArticulo(articulo); // RNF2 medido en RegistrarArticulo
        }

        public Articulo? ConsultarArticuloPorCodigoSoap(string codigo)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = ObtenerPorCodigo(codigo);

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecución ConsultarArticuloPorCodigoSoap: {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
    }
}
