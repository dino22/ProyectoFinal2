using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Controllers.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class ProductoVendidoController
    {
        [HttpGet(Name = "Producto Vendido")]
        public List<GetProductoVendido> TraerProductoVendido([FromHeader] int idUsuario)
        {
            return ProductoVendidoHandler.TraerProductosVendidos(idUsuario);
        }
    }
}
