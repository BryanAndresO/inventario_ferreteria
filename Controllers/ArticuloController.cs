using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using inventario_ferreteria.Models;
using inventario_ferreteria.Services.Interfaces;

namespace inventario_ferreteria.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly IServicioArticulos _servicio;

        public ArticuloController(IServicioArticulos servicio)
        {
            _servicio = servicio;
        }

        // GET: Articulo
        public IActionResult Index()
        {
            var lista = _servicio.BuscarPorNombre(string.Empty);
            return View(lista);
        }

        // GET: Articulo/Details/5
        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();

            var articulo = _servicio.ObtenerPorCodigo(id);
            if (articulo == null) return NotFound();

            return View(articulo);
        }

        // GET: Articulo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Articulo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Codigo,Nombre,Categoria,Preciocompra,Precioventa,Stock,Proveedor,Stockminimo")] Articulo articulo)
        {
            if (!ModelState.IsValid)
                return View(articulo);

            var result = _servicio.RegistrarArticulo(articulo);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(articulo);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Articulo/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var articulo = _servicio.ObtenerPorCodigo(id);
            if (articulo == null) return NotFound();
            return View(articulo);
        }

        // POST: Articulo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, [Bind("Codigo,Nombre,Categoria,Preciocompra,Precioventa,Stock,Proveedor,Stockminimo")] Articulo articulo)
        {
            if (id != articulo.Codigo) return NotFound();
            if (!ModelState.IsValid) return View(articulo);
            var result = _servicio.ActualizarArticulo(articulo);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(articulo);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Articulo/Delete/5
        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var articulo = _servicio.ObtenerPorCodigo(id);
            if (articulo == null) return NotFound();
            return View(articulo);
        }

        // POST: Articulo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var deleted = _servicio.EliminarArticulo(id);
            if (!deleted)
            {
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el artículo (no encontrado).");
            }
            return RedirectToAction(nameof(Index));
        }

        // NUEVAS ACCIONES PARA BÚSQUEDA

        // Buscar artículos por nombre
        [HttpGet]
        public IActionResult BuscarPorNombre(string nombre)
        {
            var lista = _servicio.BuscarPorNombre(nombre ?? string.Empty);
            return View("Index", lista); // Reutiliza la vista Index
        }

        // Buscar artículo por código
        [HttpGet]
        public IActionResult BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrEmpty(codigo))
                return RedirectToAction(nameof(Index));

            var articulo = _servicio.ObtenerPorCodigo(codigo);
            if (articulo == null)
                return RedirectToAction(nameof(Index));

            return View("Details", articulo); // Reutiliza la vista Details
        }
    }
}
