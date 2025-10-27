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
        var stopwatch = Stopwatch.StartNew(); // Inicia medici�n de tiempo

        // Validaciones
        if (string.IsNullOrWhiteSpace(articulo.Codigo))
            return new ArticuloRegistrarResult { Success = false, Message = "El c�digo es obligatorio." };

        if (_context.Articulos.Any(a => a.Codigo == articulo.Codigo))
            return new ArticuloRegistrarResult { Success = false, Message = "C�digo ya existe." };

        if (articulo.Preciocompra < 0 || articulo.Precioventa < 0)
            return new ArticuloRegistrarResult { Success = false, Message = "Los precios deben ser positivos." };

        if (articulo.Precioventa < articulo.Preciocompra)
            return new ArticuloRegistrarResult { Success = false, Message = "El precio de venta no puede ser menor al precio de compra." };

        if (articulo.Stockminimo < 0)
            return new ArticuloRegistrarResult { Success = false, Message = "No vale ingresar n�meros menores que 0 en el stock m�nimo." };

        try
        {
            _context.Articulos.Add(articulo);
            _context.SaveChanges();

            // Alertar si stock menor al m�nimo
            if (articulo.Stock < articulo.Stockminimo)
            {
                return new ArticuloRegistrarResult
                {
                    Success = true,
                    Message = "Registrado correctamente, pero atenci�n: el stock est� por debajo del m�nimo."
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
            stopwatch.Stop(); // Detener medici�n
            Console.WriteLine($"Tiempo de ejecuci�n RegistrarArticulo: {stopwatch.ElapsedMilliseconds} ms");
        }
    }


    public ArticuloActualizarResult ActualizarArticulo(Articulo articulo)
        {
            if (string.IsNullOrWhiteSpace(articulo.Codigo))
                return new ArticuloActualizarResult { Success = false, Message = "El c�digo es obligatorio." };

            var existente = _context.Articulos.Find(articulo.Codigo);
            if (existente == null)
                return new ArticuloActualizarResult { Success = false, Message = "Art�culo no encontrado." };

            if (articulo.Preciocompra < 0 || articulo.Precioventa < 0)
                return new ArticuloActualizarResult { Success = false, Message = "Los precios deben ser positivos." };

            if (articulo.Precioventa < articulo.Preciocompra)
                return new ArticuloActualizarResult { Success = false, Message = "El precio de venta no puede ser menor al precio de compra." };

            if (articulo.Stockminimo < 0)
                return new ArticuloActualizarResult { Success = false, Message = "No vale ingresar n�meros menores que 0 en el stock m�nimo." };

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
                        Message = "Actualizado correctamente, pero atenci�n: el stock est� por debajo del m�nimo."
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
            var stopwatch = Stopwatch.StartNew(); // Inicia medici�n de tiempo

            // Busca un art�culo por su c�digo en la base de datos
            var articulo = _context.Articulos.Find(codigo);

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecuci�n ObtenerPorCodigo: {stopwatch.ElapsedMilliseconds} ms");

            return articulo;
        }

        public IEnumerable<Articulo> BuscarPorNombre(string nombre)
        {
            var stopwatch = Stopwatch.StartNew(); // Inicia medici�n de tiempo
            IEnumerable<Articulo> resultados;

            // Si no se proporciona nombre, retorna todos los art�culos
            if (string.IsNullOrWhiteSpace(nombre))
                resultados = _context.Articulos.ToList();
            else
                // Retorna los art�culos cuyo nombre contiene la cadena proporcionada
                resultados = _context.Articulos
                    .Where(a => a.Nombre.Contains(nombre))
                    .AsNoTracking() // Mejora rendimiento en consultas de solo lectura
                    .ToList();

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecuci�n BuscarPorNombre: {stopwatch.ElapsedMilliseconds} ms");

            return resultados;
        }



        public bool EliminarArticulo(string codigo)
        {
            var stopwatch = Stopwatch.StartNew(); // Inicia medici�n de tiempo

            var articulo = _context.Articulos.Find(codigo);
            if (articulo == null)
            {
                stopwatch.Stop();
                Console.WriteLine($"Tiempo de ejecuci�n EliminarArticulo: {stopwatch.ElapsedMilliseconds} ms");
                return false;
            }

            _context.Articulos.Remove(articulo);
            _context.SaveChanges();

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecuci�n EliminarArticulo: {stopwatch.ElapsedMilliseconds} ms");

            return true;
        }

        // SOAP wrappers (ya reutilizan l�gica, as� que RNF2 se cumple indirectamente)
        public ArticuloRegistrarResult InsertarArticuloSoap(Articulo articulo)
        {
            return RegistrarArticulo(articulo); // RNF2 medido en RegistrarArticulo
        }

        public Articulo? ConsultarArticuloPorCodigoSoap(string codigo)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = ObtenerPorCodigo(codigo);

            stopwatch.Stop();
            Console.WriteLine($"Tiempo de ejecuci�n ConsultarArticuloPorCodigoSoap: {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }
    }
}
