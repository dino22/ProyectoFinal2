using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class LoginController
    {
        [HttpGet(Name = "Inicio Sesión")]
        public Usuario ValidarUsuaruio([FromHeader] string user, string psw)
        {
            return UsuarioHandler.ValidarUsuario(user, psw);
        }
    }
}
