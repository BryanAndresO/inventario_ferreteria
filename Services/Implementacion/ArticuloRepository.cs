using inventario_ferreteria.Data;
using inventario_ferreteria.Models;
using inventario_ferreteria.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace inventario_ferreteria.Services.Implementacion
{
    public class ArticuloRepository : IServicioArticulos
    {
        private readonly InventarioContext _context;

        public ArticuloRepository(InventarioContext context)
        {
            _context = context;
        }

        public ArticuloRegistrarResult RegistrarArticulo(Articulo articulo)
        {
            if (_context.Articulos.Any(a => a.Codigo == articulo.Codigo))
                return new ArticuloRegistrarResult { Success = false, Message = "El código ya existe." };

            _context.Articulos.Add(articulo);
            _context.SaveChanges();

            return new ArticuloRegistrarResult { Success = true, Message = "Registrado correctamente." };
        }

        public ArticuloActualizarResult ActualizarArticulo(Articulo articulo)
        {
            var existente = _context.Articulos.Find(articulo.Codigo);
            if (existente == null)
                return new ArticuloActualizarResult { Success = false, Message = "No encontrado." };

            _context.Entry(existente).CurrentValues.SetValues(articulo);
            _context.SaveChanges();

            return new ArticuloActualizarResult { Success = true, Message = "Actualizado correctamente." };
        }

        public Articulo? ObtenerPorCodigo(string codigo)
        {
            return _context.Articulos.Find(codigo);
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
