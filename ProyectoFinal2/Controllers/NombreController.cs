using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class NombreController : ControllerBase
    {
        //Método HTTP que devuelve el nombre del Sistema
        [HttpGet]
        public string TraerNombre()
        {
            return "SISTEMA DE GESTIÓN";
        }
    }
}
