using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.DTO;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class VentaController : ControllerBase
    {
        //Método HTTP para traer Ventas por idUsuario
        [HttpGet(Name = "GetVenta")]
        public List<DTO.GetVenta> TraerVentas([FromHeader] int idUsuario)
        {
            return VentaHandler.TraerVentas(idUsuario);
        }
        //Método HTTP para modificar Ventas
        [HttpPost(Name = "PostVenta")]
        public List<PostVenta> AgregarVentas([FromBody] List<PostVenta> ListVenta)
        {
            return VentaHandler.AgregarVentas(ListVenta);
        }
        //Método HTTP para eliminar Ventas
        [HttpDelete(Name = "DeleteVenta")]
        public string EliminarVenta([FromHeader] int venta)
        {
            return VentaHandler.EliminarVenta(venta);
        }
    }
}
