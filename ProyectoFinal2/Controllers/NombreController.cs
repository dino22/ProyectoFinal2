using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class NombreController : ControllerBase
    {
        [HttpGet]
        public string TraerNombre()
        {
            return "SISTEMA DE GESTIÓN";
        }
    }
}
