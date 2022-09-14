using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductoVendidoController
    {
        //Método HTTP para traer ProductoVendido por idUsuario
        [HttpGet(Name = "Producto Vendido")]
        public List<GetProductoVendido> TraerProductoVendido([FromHeader] int idUsuario)
        {
            return ProductoVendidoHandler.TraerProductosVendidos(idUsuario);
        }
    }
}
