using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Controllers.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class VentaController : ControllerBase
    {
        //Método API para traer Ventas por idUsuario
        [HttpGet(Name = "GetVenta")]
        public List<Controllers.DTO.GetVenta> TraerVentas([FromHeader] int idUsuario)
        {
            return VentaHandler.TraerVentas(idUsuario);
        }
        //Método API para modificar Ventas
        [HttpPost(Name = "PostVenta")]
        public List<PostVenta> AgregarVentas([FromBody] List<PostVenta> ListVenta)
        {
            return VentaHandler.AgregarVentas(ListVenta);
        }
        //Método API para eliminar Ventas
        [HttpDelete(Name = "DeleteVenta")]
        public string EliminarVenta([FromHeader] int venta)
        {
            return VentaHandler.EliminarVenta(venta);
        }
    }
}
