using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Controllers.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductoController : ControllerBase
    {
        [HttpGet(Name = "Traer Productos")]
        public List<Producto> TraerProductos()
        {
            return ProductoHandler.TraerProductos();
        }

        [HttpPost(Name = "Post Producto")]
        public List<PostProducto> AgregarProducto([FromBody] List<PostProducto> ListadoProductos)
        {
            return ProductoHandler.AgregarProducto(ListadoProductos);
        }

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

        [HttpDelete(Name = "Delete Producto")]
        public string EliminarProducto([FromHeader] int producto)
        {
            return ProductoHandler.EliminarProducto(producto);
        }
    }

}
