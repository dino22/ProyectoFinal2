using Microsoft.AspNetCore.Mvc;
using ProyectoFinal2.Model;
using ProyectoFinal2.Repository;

namespace ProyectoFinal2.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class LoginController
    {
        [HttpGet("{user}/{psw}")]
        public Usuario ValidarUsuaruio(string user, string psw)
        {
            return UsuarioHandler.ValidarUsuario(user, psw);
        }
    }
}
