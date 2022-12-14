using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductoController : ControllerBase
    {
        //Método HTTP trae todos los Productos
        [HttpGet(Name = "Traer Productos")]
        public List<Producto> TraerProductos()
        {
            return ProductoHandler.TraerProductos();
        }
        //Método HTTP para agregar Producto por idUsuario si no existe
        [HttpPost(Name = "Post Producto")]
        public List<PostProducto> AgregarProducto([FromBody] List<PostProducto> ListadoProductos)
        {
            return ProductoHandler.AgregarProducto(ListadoProductos);
        }
        //Método HTTP para modificar un Producto
        [HttpPut(Name = "Put Producto")]
        public string ModificarProducto([FromBody] PutProducto producto)
        {
            return ProductoHandler.ModificarProducto(new Producto
            {
                Id = producto.Id,
                Descripciones = producto.Descripciones,
                Costo = producto.Costo,
                PrecioVenta = producto.PrecioVenta,
                Stock = producto.Stock,
                IdUsuario = producto.IdUsuario
            });
        }
        //Método HTTP para eliminar un Producto
        [HttpDelete(Name = "Delete Producto")]
        public string EliminarProducto([FromHeader] int producto)
        {
            return ProductoHandler.EliminarProducto(producto);
        }
    }

}
