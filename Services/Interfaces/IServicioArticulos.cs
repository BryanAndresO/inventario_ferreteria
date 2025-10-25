using System.ServiceModel;
using inventario_ferreteria.Models;
using System.Collections.Generic;

namespace inventario_ferreteria.Services.Interfaces
{
 [ServiceContract]
 public interface IServicioArticulos
 {
 // Operaciones usadas por la aplicación
 ArticuloRegistrarResult RegistrarArticulo(Articulo articulo);
 ArticuloActualizarResult ActualizarArticulo(Articulo articulo);
 Articulo? ObtenerPorCodigo(string codigo);
 IEnumerable<Articulo> BuscarPorNombre(string nombre);
 bool EliminarArticulo(string codigo);

 // Operaciones expuestas por SOAP
 [OperationContract]
 ArticuloRegistrarResult InsertarArticuloSoap(Articulo articulo);

 [OperationContract]
 Articulo? ConsultarArticuloPorCodigoSoap(string codigo);
 }

 // Resultados para manejo de errores/validaciones
 public class ArticuloRegistrarResult
 {
 public bool Success { get; set; }
 public string Message { get; set; } = string.Empty;
 }

 public class ArticuloActualizarResult
 {
 public bool Success { get; set; }
 public string Message { get; set; } = string.Empty;
 }
}
