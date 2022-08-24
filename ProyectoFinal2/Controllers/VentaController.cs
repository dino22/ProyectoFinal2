using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Controllers.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class VentaController : ControllerBase
    {
        [HttpGet(Name = "GetVenta")]
        public List<Venta> TraerVentas([FromHeader] int IDUsuario)
        {
            return VentaHandler.TraerVentas(IDUsuario);
        }

        [HttpPost(Name = "PostVenta")]
        public List<PostVenta> AgregarVentas([FromBody] List<PostVenta> ListVenta)
        {
            return VentaHandler.AgregarVentas(ListVenta);
        }

        [HttpDelete(Name = "DeleteVenta")]
        public string EliminarVenta([FromHeader] int venta)
        {
            return VentaHandler.EliminarVenta(venta);
        }
    }
}
