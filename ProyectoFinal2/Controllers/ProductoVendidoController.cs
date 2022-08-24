using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ProductoVendidoController
    {
        [HttpGet(Name = "Producto Vendido")]
        public List<ProductoVendido> TraerProductoVendido([FromHeader] int IDUsuario)
        {
            return ProductoVendidoHandler.TraerProductosVendidos(IDUsuario);
        }
    }
}
