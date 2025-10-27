using Microsoft.AspNetCore.Mvc;
using inventario_ferreteria.Models;
using inventario_ferreteria.Services.Interfaces;

namespace inventario_ferreteria.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly IServicioArticulos _repo;

        public ArticuloController(IServicioArticulos repo)
        {
            _repo = repo;
        }

        public IActionResult Index()
        {
            var lista = _repo.BuscarPorNombre(string.Empty);
            return View(lista);
        }
        // ✅ Buscar por nombre
        [HttpGet]
        public IActionResult BuscarPorNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return RedirectToAction(nameof(Index));

            var lista = _repo.BuscarPorNombre(nombre);

            if (!lista.Any())
                TempData["Mensaje"] = "No se encontraron artículos con ese nombre.";

            return View("Index", lista);
        }

        // Buscar por código
        [HttpGet]
        public IActionResult BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return RedirectToAction(nameof(Index));

            var articulo = _repo.ObtenerPorCodigo(codigo);

            if (articulo == null)
            {
                TempData["Mensaje"] = "No se encontró un artículo con ese código.";
                return RedirectToAction(nameof(Index));
            }

            // Para mantener compatibilidad con la vista Index (que espera una lista)
            return View("Index", new List<Articulo> { articulo });
        }


        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();
            var articulo = _repo.ObtenerPorCodigo(id);
            if (articulo == null) return NotFound();
            return View(articulo);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Articulo articulo)
        {
            if (!ModelState.IsValid) return View(articulo);

            var result = _repo.RegistrarArticulo(articulo);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(articulo);
            }

            TempData["Mensaje"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var articulo = _repo.ObtenerPorCodigo(id);
            if (articulo == null) return NotFound();
            return View(articulo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, Articulo articulo)
        {
            if (id != articulo.Codigo) return NotFound();
            if (!ModelState.IsValid) return View(articulo);

            var result = _repo.ActualizarArticulo(articulo);

            TempData["Mensaje"] = result.Message;

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message);
                return View(articulo);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var articulo = _repo.ObtenerPorCodigo(id);
            if (articulo == null) return NotFound();
            return View(articulo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            _repo.EliminarArticulo(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
